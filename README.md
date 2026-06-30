# OrbitNet 🛰️ — Simulador de Red de Satélites

**OrbitNet** es un simulador de constelaciones de satélites de órbita baja y media (LEO/MEO) que modela la interacción entre satélites y estaciones terrestres. El sistema representa el tránsito de paquetes de datos entre satélites utilizando **estructuras de datos personalizadas** (TDAs) implementadas manualmente con punteros.

---

## 📋 Tabla de Contenido

- [Arquitectura](#-arquitectura)
- [Estructuras de Datos (TDAs)](#-estructuras-de-datos-tdas)
- [Tecnologías](#️-tecnologías)
- [Requisitos](#-requisitos)
- [Ejecución](#-ejecución)
- [Endpoints API REST](#-endpoints-api-rest)
- [Vistas del Frontend](#-vistas-del-frontend)
- [Motor de Simulación](#-motor-de-simulación)
- [Internacionalización](#️-internacionalización)
- [Estructura del Proyecto](#-estructura-del-proyecto)
- [Pruebas](#-pruebas)

---

## 🏗️ Arquitectura

El proyecto utiliza una **arquitectura distribuida con dos instancias simultáneas**:

| Instancia | Hemisferio | Puerto | Acceso |
|-----------|-----------|--------|--------|
| **Norte** | `North` | `5000` | http://localhost:5000 |
| **Sur** | `South` | `5001` | http://localhost:5001 |

Ambas instancias se comunican entre sí mediante **HTTP Basic Auth** para el reenvío de paquetes entre hemisferios (cross-port relay).

### Flujo de comunicación cross-port

```
Instancia Norte (5000)  ──POST /api/v1/space/relay + Auth──►  Instancia Sur (5001)
Instancia Sur (5001)    ──POST /api/v1/space/relay + Auth──►  Instancia Norte (5000)
```

---

## 🧱 Estructuras de Datos (TDAs)

Todas las estructuras están implementadas **manualmente con punteros autorreferenciados** (sin usar `List<T>`, `Dictionary<T>`, etc. de .NET).

| TDA | Ubicación | Descripción |
|-----|-----------|-------------|
| **ListaEnlazada<T>** | `DataStructures/Listas/` | Lista doblemente enlazada con cabeza y cola |
| **NodoLista<T>** | `DataStructures/Listas/` | Nodo genérico con puntero `Siguiente` |
| **ListaAntenas** | `DataStructures/Antenas/` | Lista enlazada especializada para antenas terrestres |
| **RegistroSatelites (AVL)** | `DataStructures/AVL/` | Árbol AVL para índice de satélites por ID |
| **AvlNode** | `DataStructures/AVL/` | Nodo AVL con punteros `Left`, `Right` y altura |
| **BufferMensajes (ABB)** | `DataStructures/Buffer/` | Árbol Binario de Búsqueda para cola de mensajes por prioridad |
| **RedSatelitalPlano (Matriz Dispersa)** | `DataStructures/Matrix/` | Matriz dispersa con nodos fila/columna y 4 punteros |
| **HeaderNode** | `DataStructures/Matrix/` | Nodo cabecera de fila/columna |
| **MatrixNode** | `DataStructures/Matrix/` | Nodo interno con posición (fila, columna) |
| **LogAuditoria** | `DataStructures/Logs/` | Bitácora tipo lista para registro de eventos |

### Interfaces

| Interfaz | Propósito |
|----------|-----------|
| `IAbstractCollection` | Operaciones básicas: `Add`, `Remove`, `Clear`, `Count` |
| `IMatrix` | Operaciones de matriz dispersa: `Insert`, `Remove`, `Search`, `GetNeighbors` |
| `IMessageBuffer` | Operaciones de buffer: `Enqueue`, `Dequeue`, `Peek`, `Clear` |

---

## ☁️ Tecnologías

- **.NET 10.0** (ASP.NET Core MVC + API)
- **C#** con `Nullable` habilitado e `ImplicitUsings`
- **Razor** para vistas del frontend
- **Chart.js** para gráficas en el dashboard Relay
- **Graphviz (DOT)** para visualización de estructuras de datos
- **SVG inline** para mapas y reportes visuales
- **HTTP Basic Auth** para comunicación entre instancias

---

## 📦 Requisitos

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- Dos terminales (PowerShell, Bash, CMD)
- Navegador web moderno (Chrome, Firefox, Edge)

---

## 🚀 Ejecución

### 1. Compilar el proyecto

```bash
cd V4.3\IPC2_Proyecto_2026_GrupoUno-develop\OrbitNet.Web
dotnet build
```

### 2. Iniciar las dos instancias

**Terminal 1 — Hemisferio Norte (puerto 5000):**

```bash
set ASPNETCORE_PORT=5000
set HEMISPHERE=North
dotnet run
```

O usando PowerShell:

```powershell
$env:ASPNETCORE_PORT=5000
$env:HEMISPHERE="North"
dotnet run
```

**Terminal 2 — Hemisferio Sur (puerto 5001):**

```bash
set ASPNETCORE_PORT=5001
set HEMISPHERE=South
dotnet run
```

### 3. Abrir en el navegador

| Instancia | URL |
|-----------|-----|
| Dashboard Norte | http://localhost:5000 |
| Dashboard Sur | http://localhost:5001 |

### Variables de entorno

| Variable | Valores | Default | Descripción |
|----------|---------|---------|-------------|
| `ASPNETCORE_PORT` | `5000`, `5001` | `5000` | Puerto del servidor |
| `HEMISPHERE` | `North`, `South` | Según puerto | Hemisferio de la instancia |

---

## 🔌 Endpoints API REST

### Configuración

| Método | Ruta | Descripción |
|--------|------|-------------|
| `POST` | `/api/v1/space/config` | Carga configuración XML al sistema |

### Relay (comunicación entre hemisferios)

| Método | Ruta | Descripción |
|--------|------|-------------|
| `POST` | `/api/v1/space/relay` | Envía un paquete (requiere Basic Auth) |

### Simulación

| Método | Ruta | Descripción |
|--------|------|-------------|
| `POST` | `/api/v1/space/simulation/step` | Avanza la simulación N ticks |
| `POST` | `/Simulation/TickResult` | Ejecuta 1 tick (vista) |
| `POST` | `/Simulation/ExecuteTicks` | Ejecuta N ticks (10, 100) |
| `POST` | `/Simulation/StopSimulation` | Detiene la simulación |

### Datos

| Método | Ruta | Descripción |
|--------|------|-------------|
| `GET` | `/api/v1/space/data/matrix` | Datos de la matriz dispersa |
| `GET` | `/api/v1/space/data/memory-report` | Reporte de memoria (AVL) |
| `GET` | `/api/v1/space/data/live-simulation` | Datos en vivo de simulación |

### Relay Dashboard

| Método | Ruta | Descripción |
|--------|------|-------------|
| `GET` | `/Relay` | Dashboard de relays |
| `GET` | `/Relay/Refresh` | Datos actualizados (JSON) |
| `POST` | `/Relay/ForceSend` | Reenviar paquete manualmente |
| `POST` | `/Relay/ClearBuffer` | Vaciar un buffer |
| `GET` | `/Relay/TestConnectivity` | Probar conexión con hemisferio hermano |
| `GET` | `/Relay/ExportBuffersCsv` | Exportar buffers a CSV |

### Credenciales HTTP Basic

| Campo | Valor |
|-------|-------|
| Usuario | `orbitnet_admin` |
| Contraseña | `USAC_ECYS_2026` |

---

## 🖥️ Vistas del Frontend

| Ruta | Vista | Descripción |
|------|-------|-------------|
| `/` | `Home/Index` | Dashboard principal con métricas, mapa del planeta y acciones rápidas |
| `/Satellites` | `Satellites/Index` | Lista de satélites con detalle |
| `/Satellites/Details/{id}` | `Satellites/Details` | Detalle individual de satélite |
| `/Matrix` | `Matrix/Index` | Visualización de la matriz dispersa en SVG |
| `/Reports` | `Reports/Index` | Reportes del sistema (memoria, rutas, buffers, matriz) |
| `/Simulation/Dashboard` | `Simulation/Dashboard` | Panel de control de simulación |
| `/Upload` | `Upload/Index` | Carga de archivos XML de configuración |
| `/Logs` | `Logs/Index` | Bitácora de eventos del sistema |
| `/Relay` | `Relay/Index` | Dashboard de relays con gráficas y auto-refresh |
| `/Configuration` | `Configuration/Index` | Configuración del sistema |

### Componentes CSS disponibles

| Archivo | Propósito |
|---------|-----------|
| `site.css` | Tokens de diseño (colores, sombras, radii) |
| `layout.css` | Layout app-shell con sidebar |
| `sidebar.css` | Barra lateral de navegación |
| `topbar.css` | Barra superior |
| `cards.css` | Tarjetas métricas, stats, reportes, mapa del planeta |
| `buttons.css` | Botones y badges |
| `badges.css` | Badges de estado |
| `tables.css` | Tablas de datos |
| `relay.css` | Dashboard de relays |
| `reports.css` | Reportes del sistema |
| `upload.css` | Página de carga XML |

---

## ⚙️ Motor de Simulación

El motor de simulación se encuentra en `Services/SimulationEngine/`:

| Componente | Descripción |
|------------|-------------|
| `TickProcessor` | Procesa cada tick: rotación orbital, enrutamiento, despacho |
| `OrbitalRotator` | Calcula la rotación de satélites en sus órbitas |
| `RoutingService` | Encuentra rutas óptimas entre satélites y antenas |
| `PriorityDispatcher` | Despacha mensajes según prioridad |
| `SimulationCoordinator` | Coordina todos los componentes del motor |
| `State/SatelliteStateIndex` | Índice de estados de satélites |
| `State/SatelliteRuntimeIndex` | Índice runtime de satélites |

### Generación de SVGs y Graphviz

Los reportes visuales se generan en `Graphviz/`:

| Generador | Descripción |
|-----------|-------------|
| `MatrixGraphGenerator` | Genera DOT de la matriz dispersa |
| `AvlGraphGenerator` | Genera DOT del árbol AVL |
| `BufferGraphGenerator` | Genera DOT del buffer ABB |
| `RouteGraphGenerator` | Genera DOT de rutas |
| `SvgBuilder` | Construye SVGs a partir de datos |

---

## 🌐 Internacionalización

El proyecto soporta **español** e **inglés** usando `IStringLocalizer<T>`.

- Archivos de recursos: `Resources/OrbitNet/Web/Controllers/`
- `SharedResource.resx` — Textos en inglés (default)
- `SharedResource.es.resx` — Textos en español

**Cambio de idioma:**
```
GET /Home/SetLanguage?culture=es&returnUrl=/ 
GET /Home/SetLanguage?culture=en&returnUrl=/
```

---

## 📁 Estructura del Proyecto

```
OrbitNet.Web/
├── ArchivosPrueba/           # XML de prueba para carga
├── Configuration/            # Configuración de la instancia
├── Controllers/
│   ├── API/                  # Controladores REST
│   │   ├── ConfigController.cs
│   │   ├── DataController.cs
│   │   ├── RelayController.cs
│   │   └── SimulationApiController.cs
│   ├── HomeController.cs
│   ├── SatelliteController.cs
│   ├── SimulationController.cs
│   ├── RelayDashboardController.cs
│   ├── ReportsController.cs
│   ├── MatrixController.cs
│   ├── LogsController.cs
│   ├── UploadController.cs
│   ├── ConfigurationController.cs
│   ├── SpaceController.cs
│   └── SharedResource.cs
├── DataStructures/           # TDAs personalizados
│   ├── Antenas/
│   ├── AVL/
│   ├── Buffer/
│   ├── Interfaces/
│   ├── Listas/
│   ├── Logs/
│   └── Matrix/
├── Graphviz/                 # Generadores DOT/SVG
├── Middleware/                # Filtros globales
├── Models/                   # Entidades, DTOs, ViewModels, Enums
│   ├── DTOs/
│   ├── Entities/
│   ├── Enums/
│   ├── Mappers/
│   └── ViewModels/
├── Resources/                # Archivos de localización
├── Services/                 # Lógica de negocio
│   ├── Communication/        # HTTP Basic Auth, Relay HTTP
│   ├── SimulationEngine/     # Motor de simulación
│   │   └── State/            # Índices de estado runtime
│   └── Validation/           # Validación XML
├── Utils/                    # Constantes
├── Views/                    # Vistas Razor
│   ├── Home/
│   ├── Satellites/
│   ├── Simulation/
│   ├── Relay/
│   ├── Reports/
│   ├── Matrix/
│   ├── Logs/
│   ├── Upload/
│   ├── Configuration/
│   └── Shared/
├── wwwroot/                  # Archivos estáticos (CSS)
│   ├── cards.css
│   ├── site.css
│   ├── relay.css
│   └── ... (10 archivos CSS)
├── Program.cs                # Punto de entrada
└── appsettings*.json         # Configuración
```

---

## 🧪 Pruebas

El proyecto incluye pruebas unitarias en `OrbitNet.Tests/`:

| Categoría | Archivo | Descripción |
|-----------|---------|-------------|
| AVL | `AVLTests/AVLTest.cs` | Pruebas del árbol AVL |
| Antenas | `AntenasTests/ListaAntenasTests.cs` | Pruebas de lista de antenas |
| Buffer | `BufferTests/BufferMensajesTests.cs` | Pruebas del buffer ABB |
| Matriz | `MatrixTests/HeaderNodeTests.cs` | Pruebas de nodos cabecera |
| Matriz | `MatrixTests/MatrixNodeTests.cs` | Pruebas de nodos de matriz |
| Matriz | `MatrixTests/RedSatelitalPlanoTests.cs` | Pruebas de la matriz dispersa |
| Integración | `IntegrationTests/BasicAuthTests.cs` | Pruebas de autenticación |
| Integración | `IntegrationTests/RelayTests.cs` | Pruebas de relay |
| Integración | `IntegrationTests/XmlIngestTests.cs` | Pruebas de carga XML |
| Logs | `LogTests/LogAuditoriaTests.cs` | Pruebas de bitácora |

```bash
# Ejecutar todas las pruebas
dotnet test

# Ejecutar pruebas de una categoría específica
dotnet test --filter "FullyQualifiedName~AVLTests"
```

---

## 🗺️ Mapa del Planeta

El dashboard principal (`Home/Index`) muestra un **SVG inline** interactivo del planeta Tierra con:
- Océanos con gradiente y estrellas de fondo
- Continentes estilizados (Norteamérica, Sudamérica, Europa, África, Asia, Australia)
- Línea del ecuador punteada
- Resaltado del hemisferio activo (Norte o Sur)
- Satélites animados con efecto pulso
- Badge de estado de la simulación
- Tick actual de la simulación

---

## 🤝 Contribución

1. Haz fork del repositorio
2. Crea una rama desde `develop`: `git checkout -b feature/nueva-funcionalidad`
3. Haz tus cambios
4. Ejecuta `dotnet build` y `dotnet test` para verificar
5. Envía un Pull Request a `develop`

---

## 📄 Licencia

Proyecto académico — **Universidad de San Carlos de Guatemala (USAC)**  
Facultad de Ingeniería — Escuela de Ciencias y Sistemas  
*IPC2 — Proyecto 2026 — Grupo Uno*
