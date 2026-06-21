# Guía de estudio — Cambios finales API y Comunicación

Documento detallado de los **3 cambios obligatorios** que cierran tu parte del proyecto OrbitNet.  
Incluye archivo, líneas, qué hace cada bloque y cómo se conecta todo.

---

## Resumen de los 3 cambios

| # | Cambio | Archivo principal | Para qué sirve |
|---|--------|-------------------|----------------|
| 1 | Cola real en RAM | `OrbitNetStore.cs` | Guardar paquetes y calcular `%` de ocupación real |
| 2 | Relay cross-port conectado | `SpaceController.cs` + `TickProcessor.cs` | Enviar paquetes entre puerto 5000 y 5001 |
| 3 | Validar config cargada | `SpaceController.cs` | Bloquear relay/simulación si no hubo POST `/config` |

---

## Flujo completo después de los cambios

```
                    POST /config (XML)
                          │
                          ▼
                 XmlIngestService ──► OrbitNetStore.ConfigLoaded = true
                          │
        ┌─────────────────┴─────────────────┐
        │                                   │
   POST /relay                          POST /simulation/step
        │                                   │
   ¿ConfigLoaded?                      ¿ConfigLoaded?
   NO → 400                            NO → 400
   SÍ → continúa                       SÍ → avanza ticks
        │
   ¿destino_ip es del otro hemisferio?
        │
   SÍ → RelayHttpService ──HTTP+BasicAuth──► puerto hermano (5000↔5001)
        │                                      status: "Forwarded"
   NO → OrbitNetStore.EncolarPaquete()
        │                                      status: "Routed"
        └─► queue_occupancy_percentage = (paquetes/10)*100
```

---

## Archivo 1: `Orbinet.Web/Models/Entities/OrbitNetStore.cs`

Este archivo es la **memoria RAM** del sistema. Antes solo guardaba contadores; ahora también guarda la cola de paquetes.

### Líneas 1-5 — Imports y clase

```csharp
using Orbinet.Web.Models.Entities;

public class OrbitNetStore
{
    public const int CapacidadMaximaCola = 10;
```

- **Línea 1:** Importa `MessagePacket` para poder guardar paquetes en una lista.
- **Línea 5:** `CapacidadMaximaCola = 10` define el tamaño máximo de la cola. Se usa para calcular el porcentaje de ocupación: 1 paquete = 10%, 5 paquetes = 50%, etc.

### Líneas 7-14 — Propiedades de estado

```csharp
    public int NodosProcesados { get; set; }
    public int CurrentTick { get; set; }
    public int EventsProcessed { get; set; }
    public int LogicalJumps { get; set; }
    public double QueueOccupancyPercentage { get; set; }
    public string ReceptorSatelliteId { get; set; }
    public bool ConfigLoaded { get; set; }
    public List<MessagePacket> PaquetesEnCola { get; } = new();
```

| Propiedad | Qué guarda |
|-----------|------------|
| `NodosProcesados` | Cuántos nodos XML pasaron validación RegEx |
| `CurrentTick` | Tick actual de simulación |
| `EventsProcessed` | Eventos procesados en simulación |
| `LogicalJumps` | Saltos lógicos de órbita |
| `QueueOccupancyPercentage` | % de llenado de la cola (calculado, ya no es 40.0 fijo) |
| `ReceptorSatelliteId` | ID del satélite receptor (ej. `SAT-POL-1001`) |
| `ConfigLoaded` | `true` solo si POST `/config` fue exitoso |
| `PaquetesEnCola` | **NUEVO** — Lista de paquetes recibidos localmente |

### Líneas 16-19 — Constructor

```csharp
    public OrbitNetStore()
    {
        ReceptorSatelliteId = "SAT-POL-1001";
    }
```

Inicializa el ID del receptor. **Ya no** pone `QueueOccupancyPercentage = 40.0` porque ese valor era fake.

### Líneas 21-25 — Encolar paquete

```csharp
    public void EncolarPaquete(MessagePacket paquete)
    {
        PaquetesEnCola.Add(paquete);
        QueueOccupancyPercentage = CalcularOcupacionCola();
    }
```

- **Línea 23:** Agrega el paquete JSON recibido a la lista en RAM.
- **Línea 24:** Recalcula el porcentaje automáticamente cada vez que entra un paquete.

### Líneas 27-30 — Calcular ocupación

```csharp
    public double CalcularOcupacionCola()
    {
        return Math.Round((PaquetesEnCola.Count / (double)CapacidadMaximaCola) * 100.0, 1);
    }
```

**Fórmula:** `(cantidad en cola / capacidad máxima) × 100`

Ejemplos:
- 0 paquetes → 0.0%
- 1 paquete → 10.0%
- 3 paquetes → 30.0%
- 10 paquetes → 100.0%

`Math.Round(..., 1)` redondea a 1 decimal para el JSON de respuesta.

---

## Archivo 2: `Orbinet.Web/Services/SimulationEngine/TickProcessor.cs`

Contiene la lógica de **decidir si un paquete debe salir al otro hemisferio**.

### Líneas 44-51 — Método nuevo `RequiereRelayCrossPort`

```csharp
        public bool RequiereRelayCrossPort(MessagePacket paquete)
        {
            bool esDestinoSur = paquete.DestinationIp == "10.0.0.90";
            bool esDestinoNorte = paquete.DestinationIp == "10.0.0.50";

            return (_settings.Hemisphere == "North" && esDestinoSur)
                || (_settings.Hemisphere == "South" && esDestinoNorte);
        }
```

**Reglas del PDF del proyecto:**

| Instancia | Hemisferio | IP local | IP del hermano |
|-----------|------------|----------|----------------|
| Puerto 5000 | North | 10.0.0.50 | 10.0.0.90 (Sur) |
| Puerto 5001 | South | 10.0.0.90 | 10.0.0.50 (Norte) |

**Cuándo retorna `true` (hay que reenviar):**
- Estás en **Norte** y el paquete va a `10.0.0.90` → reenviar a Sur (5001)
- Estás en **Sur** y el paquete va a `10.0.0.50` → reenviar a Norte (5000)

**Cuándo retorna `false` (paquete local):**
- Norte + destino `10.0.0.50` → se queda en Norte
- Sur + destino `10.0.0.90` → se queda en Sur

### Líneas 53-69 — `IntentarRelayCrossPortAsync` (ya existía, ahora SÍ se usa)

```csharp
        public async Task<bool> IntentarRelayCrossPortAsync(MessagePacket paquete)
        {
            bool esDestinoSur = paquete.DestinationIp == "10.0.0.90";
            bool esDestinoNorte = paquete.DestinationIp == "10.0.0.50";

            if (_settings.Hemisphere == "North" && esDestinoSur)
            {
                return await _relayHttpService.EnviarPaqueteAlHemisferioHermanoAsync(paquete);
            }

            if (_settings.Hemisphere == "South" && esDestinoNorte)
            {
                return await _relayHttpService.EnviarPaqueteAlHemisferioHermanoAsync(paquete);
            }

            return false;
        }
```

- Llama a `RelayHttpService` que hace el POST HTTP real al puerto hermano.
- Retorna `true` si el otro servidor respondió 2xx.
- Retorna `false` si no aplica cross-port o si falló la conexión.

---

## Archivo 3: `Orbinet.Web/Services/Communication/RelayHttpService.cs`

Servicio que **envía HTTP** al otro puerto. No se modificó en este cambio, pero es parte del flujo cross-port.

### Líneas 24-35 — Envío HTTP

```csharp
    public async Task<bool> EnviarPaqueteAlHemisferioHermanoAsync(MessagePacket paquete)
    {
        HttpClient client = _httpClientFactory.CreateClient();
        string url = "http://localhost:" + _settings.SiblingPort + "/api/v1/space/relay";

        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Headers.Add("Authorization", _basicAuthService.CrearCabeceraAuthorization());
        request.Content = JsonContent.Create(paquete);

        HttpResponseMessage response = await client.SendAsync(request);
        return response.IsSuccessStatusCode;
    }
```

| Línea | Qué hace |
|-------|----------|
| 26 | Crea un `HttpClient` desde la fábrica (patrón recomendado en .NET) |
| 27 | Arma la URL: Norte envía a `localhost:5001`, Sur a `localhost:5000` |
| 29-30 | Crea POST con cabecera `Authorization: Basic ...` |
| 31 | Serializa el paquete a JSON en el body |
| 33-34 | Envía y retorna si fue exitoso (201 Created del otro servidor) |

---

## Archivo 4: `Orbinet.Web/Controllers/SpaceController.cs` (cambio principal)

El controlador conecta todo: auth, validación de config, cross-port y cola.

### Endpoint `POST /relay` — Líneas 51-105

#### Paso 1: Validar Basic Auth (líneas 54-63)

```csharp
            string? authHeader = Request.Headers.Authorization.FirstOrDefault();

            if (!_basicAuthService.EsCabeceraValida(authHeader))
            {
                return Unauthorized(new RelayErrorResponse { ... });
            }
```

- Lee la cabecera `Authorization` del request HTTP.
- Si falta o es incorrecta → **401 Unauthorized**.
- Credenciales válidas: `orbitnet_admin:USAC_ECYS_2026` en Base64.

#### Paso 2: Validar que exista config (líneas 65-73) — **NUEVO**

```csharp
            if (!_store.ConfigLoaded)
            {
                return BadRequest(new ConfigErrorResponse
                {
                    Status = "Error",
                    ErrorCode = "CONFIG_NOT_LOADED",
                    Details = "Debe cargar la configuracion XML con POST /api/v1/space/config antes de enviar relay."
                });
            }
```

- Si nadie llamó antes a POST `/config` con XML válido → **400 Bad Request**.
- Esto garantiza el orden: primero config, luego relay.

#### Paso 3: Cross-port o cola local (líneas 75-104) — **NUEVO**

```csharp
            if (_tickProcessor.RequiereRelayCrossPort(paquete))
            {
                bool enviado = await _tickProcessor.IntentarRelayCrossPortAsync(paquete);

                if (!enviado)
                {
                    return StatusCode(502, new ConfigErrorResponse
                    {
                        ErrorCode = "RELAY_FORWARD_FAILED",
                        ...
                    });
                }

                return StatusCode(201, new RelaySuccessResponse
                {
                    Status = "Forwarded",
                    Message = "Paquete reenviado al hemisferio hermano via HTTP ...",
                    QueueOccupancyPercentage = _store.CalcularOcupacionCola()
                });
            }

            _store.EncolarPaquete(paquete);

            return StatusCode(201, new RelaySuccessResponse
            {
                Status = "Routed",
                Message = "Mensaje insertado con exito en el buffer ...",
                QueueOccupancyPercentage = _store.QueueOccupancyPercentage
            });
```

**Dos caminos posibles:**

| Camino | Condición | HTTP | status JSON |
|--------|-----------|------|-------------|
| Reenvío | Destino es el otro hemisferio | 201 o 502 | `"Forwarded"` |
| Local | Destino es este hemisferio | 201 | `"Routed"` |

- **502:** El otro servidor (5000 o 5001) no está corriendo.
- **201 Routed:** Paquete guardado en `PaquetesEnCola` con % real.
- **201 Forwarded:** Paquete enviado al hermano; no se encola localmente.

#### Nota: método ahora es `async`

```csharp
        public async Task<IActionResult> RecibirRelay([FromBody] MessagePacket paquete)
```

Cambió de `IActionResult` a `async Task<IActionResult>` porque hay que `await` la llamada HTTP al puerto hermano.

### Endpoint `POST /simulation/step` — Líneas 107-122

```csharp
        [HttpPost("simulation/step")]
        public IActionResult AvanzarSimulacion([FromBody] SimulationStepRequestDto request)
        {
            if (!_store.ConfigLoaded)
            {
                return BadRequest(new ConfigErrorResponse
                {
                    ErrorCode = "CONFIG_NOT_LOADED",
                    ...
                });
            }

            SimulationStepResponse response = _tickProcessor.AvanzarSimulacion(request.Ticks);
            return Ok(response);
        }
```

**NUEVO:** Misma validación que relay — no puedes simular sin haber cargado el XML antes.

---

## Tabla de códigos HTTP después de los cambios

| Endpoint | Condición | Código | error_code |
|----------|-----------|--------|------------|
| POST `/config` | XML inválido | 400 | `XML_SCHEMA_VIOLATION` |
| POST `/config` | XML OK | 200 | — |
| POST `/relay` | Sin Basic Auth | 401 | — |
| POST `/relay` | Sin config previa | 400 | `CONFIG_NOT_LOADED` |
| POST `/relay` | Cross-port falló | 502 | `RELAY_FORWARD_FAILED` |
| POST `/relay` | Reenviado OK | 201 | status: `Forwarded` |
| POST `/relay` | Encolado local | 201 | status: `Routed` |
| POST `/simulation/step` | Sin config previa | 400 | `CONFIG_NOT_LOADED` |
| POST `/simulation/step` | OK | 200 | status: `Simulated` |

---

## Cómo probar manualmente (2 terminales)

### Terminal 1 — Norte (5000)
```powershell
cd Orbinet.Web
dotnet run --launch-profile OrbitNet-North
```

### Terminal 2 — Sur (5001)
```powershell
cd Orbinet.Web
dotnet run --launch-profile OrbitNet-South
```

### Terminal 3 — Pruebas

**1. Cargar config en Norte:**
```powershell
$xml = Get-Content "Orbinet.Web/ArchivosPrueba/Cargaexitosa1_CNorte_5000.xml" -Raw
Invoke-RestMethod -Uri "http://localhost:5000/api/v1/space/config" -Method POST -ContentType "application/json" -Body (@{ xml_data = $xml } | ConvertTo-Json)
```

**2. Cargar config en Sur:**
```powershell
$xml = Get-Content "Orbinet.Web/ArchivosPrueba/Cargaexitosa2_CSur_5001.xml" -Raw
Invoke-RestMethod -Uri "http://localhost:5001/api/v1/space/config" -Method POST -ContentType "application/json" -Body (@{ xml_data = $xml } | ConvertTo-Json)
```

**3. Relay local en Norte (destino 10.0.0.50):**
```powershell
$h = @{ Authorization = "Basic b3JiaXRuZXRfYWRtaW46VVNBQ19FQ1lTXzIwMjY=" }
$b = @{ codigo_hex="A19F"; emisor_id="SAT-ECU-0012"; destino_ip="10.0.0.50"; prioridad=5; contenido="Local Norte" } | ConvertTo-Json
Invoke-RestMethod -Uri "http://localhost:5000/api/v1/space/relay" -Method POST -Headers $h -ContentType "application/json" -Body $b
```
Esperado: `"status":"Routed"`, `"queue_occupancy_percentage":10.0`

**4. Relay cross-port Norte → Sur (destino 10.0.0.90):**
```powershell
$b = @{ codigo_hex="A19F"; emisor_id="SAT-ECU-0012"; destino_ip="10.0.0.90"; prioridad=5; contenido="Cross-port a Sur" } | ConvertTo-Json
Invoke-RestMethod -Uri "http://localhost:5000/api/v1/space/relay" -Method POST -Headers $h -ContentType "application/json" -Body $b
```
Esperado: `"status":"Forwarded"` (Norte reenvía a Sur por HTTP)

**5. Relay sin config (debe fallar):**
```powershell
# En una instancia nueva sin haber cargado XML:
Invoke-WebRequest -Uri "http://localhost:5000/api/v1/space/relay" -Method POST -Headers $h -ContentType "application/json" -Body $b
```
Esperado: **400** con `CONFIG_NOT_LOADED`

---

## Pruebas automatizadas

Archivo: `OrbitNet.Tests/IntegrationTests/RelayTests.cs`

| Test | Qué verifica |
|------|--------------|
| `PostRelay_SinConfig_Retorna400` | Relay bloqueado sin config |
| `PostSimulationStep_SinConfig_Retorna400` | Simulación bloqueada sin config |
| `PostRelay_ConBasicAuth_Retorna201` | Relay local con cola real |
| `RelayHttpService_EnviaBasicAuthAlPuertoHermano` | Simulación 5000→5001 con mock HTTP |

Ejecutar:
```powershell
cd OrbitNet.Tests
dotnet test --filter "FullyQualifiedName~IntegrationTests"
```
Resultado: **15/15 pruebas OK**

---

## Qué NO tocamos (no es tu parte)

- `RedSatelitalPlano` — matriz dispersa (equipo TDAs)
- Buffer ABB real — `IMessageBuffer` está vacío a propósito
- `UploadController` — sigue con mock (UI de otro compañero)
- Motor orbital completo — solo contador de ticks

---

## Preguntas frecuentes para el examen/informe

**¿Por qué `OrbitNetStore` es Singleton?**  
Porque toda la app comparte el mismo estado en RAM (config cargada, cola, ticks). Un solo store para todos los requests.

**¿Por qué cross-port no encola localmente?**  
Si el destino es el otro hemisferio, el paquete debe llegar al servidor hermano. El hermano lo encola cuando lo recibe en su `/relay`.

**¿Qué pasa si Sur no está corriendo y Norte intenta cross-port?**  
Retorna **502** con `RELAY_FORWARD_FAILED`.

**¿Cuál es la diferencia entre `Routed` y `Forwarded`?**  
- `Routed` = paquete guardado en cola local  
- `Forwarded` = paquete enviado por HTTP al otro puerto

**¿Por qué capacidad 10?**  
Valor simple para calcular porcentajes legibles. Cuando el equipo de TDAs conecte el buffer ABB real, puede reemplazar esta lista.

---

## Archivos modificados en este cambio

```
Orbinet.Web/
├── Models/Entities/OrbitNetStore.cs      ← cola + cálculo %
├── Controllers/SpaceController.cs      ← relay cross-port + validación config
└── Services/SimulationEngine/TickProcessor.cs  ← RequiereRelayCrossPort()

OrbitNet.Tests/
└── IntegrationTests/RelayTests.cs      ← tests actualizados + 2 nuevos
```

---

*Documento generado para estudio — Área API y Comunicación, Proyecto OrbitNet.*
