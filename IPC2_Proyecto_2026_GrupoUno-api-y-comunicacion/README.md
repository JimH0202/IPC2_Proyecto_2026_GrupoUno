# Rama `api-y-comunicacion` — API REST y Comunicación Multi-Puerto

Documentación de los cambios realizados en esta rama para la **Fase 3** del Proyecto OrbitNet-NetCore.

**Responsable:** API y comunicación  
**Alcance:** Solo endpoints REST, configuración de puertos 5000/5001, HttpClient y HTTP Basic Authentication.

> No se modificaron TDAs, Graphviz, vistas Razor, pruebas unitarias ni el motor de simulación completo.  
> El único ajuste fuera del alcance estricto fue reparar `Resources/SharedResource.es.resx` (archivo vacío que impedía compilar el proyecto).

---

## Resumen de la rama

| Bloque | Estado |
|--------|--------|
| Modelos / entidades base | Commits previos en la rama |
| Configuración puertos 5000 y 5001 | Implementado |
| `SpaceController` con 3 endpoints | Implementado |
| `IHttpClientFactory` + Basic Auth | Implementado |
| Relay cross-port Norte ↔ Sur | Implementado (servicio listo) |

---

## Commits ya existentes en la rama (modelos base)

Estos archivos ya estaban en la rama antes de la infraestructura de la API:

| Archivo | Descripción |
|---------|-------------|
| `Models/Entities/Satellite.cs` | Entidad satélite (`Id`, `Name`, `Ip`) |
| `Models/Entities/GroundAntenna.cs` | Entidad antena terrestre |
| `Models/Entities/MessagePacket` | Paquete JSON de relay (ahora renombrado a `.cs`) |
| `Models/Entities/PolarOrbit.cs` | Entidad órbita polar |
| `Models/Entities/SimulationState.cs` | Estado simple de simulación (`TickNow`) |
| `Models/DTOs/RelayRequestDto.cs` | DTO renombrado desde `UploadConfigurationRequest.cs` |

### Corrección menor en entidades

`PolarOrbit.cs` y `SimulationState.cs` tenían por error el mismo contenido que `Satellite.cs` (copia duplicada). Se corrigió para que cada clase tenga su definición real.

---

## Cambios nuevos — Infraestructura API y comunicación

### 1. Configuración de puertos concurrentes

| Archivo | Qué hace |
|---------|----------|
| `Properties/launchSettings.json` | Perfil **OrbitNet-North** → puerto **5000** / Perfil **OrbitNet-South** → puerto **5001** |
| `appsettings.json` | Configuración base de logging |
| `appsettings.North.json` | Hemisferio Norte: puerto 5000, hermano 5001 |
| `appsettings.South.json` | Hemisferio Sur: puerto 5001, hermano 5000 |
| `Configuration/AppInstanceSettings.cs` | Clase POCO: `Hemisphere`, `Port`, `SiblingPort` |
| `Utils/Constants.cs` | Usuario, contraseña y puertos fijos del protocolo |
| `OrbitNet.Web.csproj` | Proyecto ASP.NET Core 8.0 (necesario para compilar y correr la API) |

**Cómo levantar las dos instancias:**

```powershell
# Terminal 1 — Hemisferio Norte
dotnet run --launch-profile OrbitNet-North

# Terminal 2 — Hemisferio Sur
dotnet run --launch-profile OrbitNet-South
```

---

### 2. Controlador de la API

| Archivo | Qué hace |
|---------|----------|
| `Controllers/SpaceController.cs` | Expone los 3 endpoints REST bajo `/api/v1/space` |
| `Program.cs` | Registra controladores, servicios e `AddHttpClient()` |
| `Models/DTOs/ApiDtos.cs` | DTOs de request/response para config, relay y simulation |

#### Endpoints implementados

| Método | Ruta | Descripción |
|--------|------|-------------|
| `POST` | `/api/v1/space/config` | Recibe JSON con `xml_data`, valida con RegEx y carga en RAM |
| `POST` | `/api/v1/space/relay` | Recibe paquete JSON; **exige Basic Auth** (401 si falla) |
| `POST` | `/api/v1/space/simulation/step` | Avanza la simulación según `ticks` |

#### Ejemplo de payload — config

```json
{
  "xml_data": "<?xml version=\"1.0\"?><orbitnet>...</orbitnet>"
}
```

#### Ejemplo de payload — relay

```json
{
  "codigo_hex": "A19F",
  "emisor_id": "SAT-ECU-0012",
  "destino_ip": "10.0.0.50",
  "prioridad": 5,
  "contenido": "Alerta de tsunami detectada."
}
```

#### Ejemplo de payload — simulation/step

```json
{
  "ticks": 1
}
```

---

### 3. Motor de comunicación (HttpClient + Basic Auth)

| Archivo | Qué hace |
|---------|----------|
| `Services/Communication/BasicAuthService.cs` | Crea y valida cabecera `Authorization: Basic Base64(...)` |
| `Services/Communication/RelayHttpService.cs` | Envía POST al puerto hermano usando `IHttpClientFactory` |
| `Services/Validation/XmlIngestService.cs` | Parseo XML con `XmlDocument` + validación RegEx |
| `Services/SimulationEngine/TickProcessor.cs` | Avance de ticks y lógica de relay cross-port |
| `Models/Entities/OrbitNetStore.cs` | Estado en memoria RAM (singleton simple) |
| `Models/Entities/MessagePacket.cs` | Entidad del paquete (renombrada desde archivo sin extensión) |

#### Credenciales HTTP Basic (según PDF del proyecto)

| Campo | Valor |
|-------|-------|
| Usuario | `orbitnet_admin` |
| Contraseña | `USAC_ECYS_2026` |
| Base64 | `b3JiaXRuZXRfYWRtaW46VVNBQ19FQ1lTXzIwMjY=` |

#### Flujo cross-port

```
Instancia Norte (5000)  ──POST /relay + Basic Auth──►  Instancia Sur (5001)
Instancia Sur (5001)    ──POST /relay + Basic Auth──►  Instancia Norte (5000)
```

El `RelayHttpService` apunta a `http://localhost:{SiblingPort}/api/v1/space/relay`.

---

## Archivos que NO se tocaron en esta rama

- `DataStructures/` (TDAs manuales)
- `Graphviz/`
- `Views/` y controladores MVC de UI (`HomeController`, etc.)
- `OrbitNet.Tests/`
- `Documentacion/`
- `Middleware/ExceptionMiddleware.cs`

---

## Estructura de carpetas relevante para API

```
OrbitNet.Web/
├── Controllers/
│   └── SpaceController.cs          ← Endpoints REST
├── Configuration/
│   └── AppInstanceSettings.cs      ← Config por hemisferio
├── Models/
│   ├── DTOs/
│   │   ├── ApiDtos.cs              ← Request/Response JSON
│   │   └── RelayRequestDto.cs
│   └── Entities/
│       ├── GroundAntenna.cs
│       ├── MessagePacket.cs
│       ├── OrbitNetStore.cs        ← Memoria RAM simple
│       ├── PolarOrbit.cs
│       ├── Satellite.cs
│       └── SimulationState.cs
├── Properties/
│   └── launchSettings.json         ← Perfiles 5000 / 5001
├── Services/
│   ├── Communication/
│   │   ├── BasicAuthService.cs
│   │   └── RelayHttpService.cs
│   ├── SimulationEngine/
│   │   └── TickProcessor.cs
│   └── Validation/
│       └── XmlIngestService.cs
├── Utils/
│   └── Constants.cs
├── Program.cs
├── appsettings.json
├── appsettings.North.json
├── appsettings.South.json
└── OrbitNet.Web.csproj
```

