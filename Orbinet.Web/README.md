OrbitNet/
│
├── OrbitNet.Web/
│
│   ├── Controllers/
│   │
│   │   ├── HomeController.cs
│   │   ├── UploadController.cs
│   │   ├── SimulationController.cs
│   │   ├── MatrixController.cs
│   │   ├── SatellitesController.cs
│   │   ├── ReportController.cs
│   │   └── LogController.cs
│   │
│   │   └── Api/
│   │       ├── ConfigurationController.cs
│   │       ├── RelayController.cs
│   │       └── SimulationApiController.cs
│
│   ├── Models/
│   │
│   │   ├── Entities/
│   │   │   ├── Satellite.cs
│   │   │   ├── PolarOrbit.cs
│   │   │   ├── GroundAntenna.cs
│   │   │   ├── MessagePacket.cs
│   │   │   ├── AuditLogEntry.cs
│   │   │   └── SimulationState.cs
│   │   │
│   │   ├── DTOs/
│   │   │   ├── UploadConfigurationRequest.cs
│   │   │   ├── UploadConfigurationResponse.cs
│   │   │   ├── RelayRequestDto.cs
│   │   │   ├── RelayResponseDto.cs
│   │   │   ├── SimulationStepRequest.cs
│   │   │   ├── SimulationStepResponse.cs
│   │   │   └── ErrorResponseDto.cs
│   │   │
│   │   ├── ViewModels/
│   │   │   ├── DashboardViewModel.cs
│   │   │   ├── UploadViewModel.cs
│   │   │   ├── MatrixViewModel.cs
│   │   │   ├── SatelliteViewModel.cs
│   │   │   ├── ReportViewModel.cs
│   │   │   └── LogViewModel.cs
│   │   │
│   │   └── Enums/
│   │       ├── PriorityLevel.cs
│   │       ├── MessageStatus.cs
│   │       ├── OrbitType.cs
│   │       └── LogSeverity.cs
│
│   ├── Views/
│   │
│   │   ├── Home/
│   │   │   └── Index.cshtml
│   │   │
│   │   ├── Upload/
│   │   │   ├── Index.cshtml
│   │   │   └── Result.cshtml
│   │   │
│   │   ├── Simulation/
│   │   │   ├── Dashboard.cshtml
│   │   │   └── TickResult.cshtml
│   │   │
│   │   ├── Matrix/
│   │   │   └── Index.cshtml
│   │   │
│   │   ├── Satellites/
│   │   │   └── Index.cshtml
│   │   │
│   │   ├── Reports/
│   │   │   ├── MemoryLayout.cshtml
│   │   │   ├── Routing.cshtml
│   │   │   └── Buffers.cshtml
│   │   │
│   │   ├── Logs/
│   │   │   └── Index.cshtml
│   │   │
│   │   └── Shared/
│   │       ├── _Layout.cshtml
│   │       ├── _ViewImports.cshtml
│   │       ├── _ViewStart.cshtml
│   │       ├── Error.cshtml
│   │       └── _LanguageSelector.cshtml
│
│   ├── Services/
│   │
│   │   ├── SimulationEngine/
│   │   │   ├── TickProcessor.cs
│   │   │   ├── OrbitalRotator.cs
│   │   │   ├── RoutingService.cs
│   │   │   ├── PriorityDispatcher.cs
│   │   │   └── SimulationCoordinator.cs
│   │   │
│   │   ├── Validation/
│   │   │   ├── XmlIngestService.cs
│   │   │   ├── RegexValidator.cs
│   │   │   ├── XmlValidator.cs
│   │   │   └── XmlErrorFormatter.cs
│   │   │
│   │   ├── Logging/
│   │   │   └── AuditLogger.cs
│   │   │
│   │   ├── Reports/
│   │   │   ├── MemoryLayoutReport.cs
│   │   │   ├── RelayRouteReport.cs
│   │   │   └── BufferCapacityReport.cs
│   │   │
│   │   └── Communication/
│   │       ├── BasicAuthService.cs
│   │       └── RelayHttpClient.cs
│
│   ├── DataStructures/
│   │
│   │   ├── Interfaces/
│   │   │   ├── IMatrix.cs
│   │   │   ├── IAVLTree.cs
│   │   │   ├── IMessageBuffer.cs
│   │   │   └── IAuditLog.cs
│   │   │
│   │   ├── Matrix/
│   │   │   ├── HeaderNode.cs
│   │   │   ├── MatrixNode.cs
│   │   │   └── RedSatelitalPlano.cs
│   │   │
│   │   ├── AVL/
│   │   │   ├── AvlNode.cs
│   │   │   └── RegistroSatelites.cs
│   │   │
│   │   ├── Buffer/
│   │   │   ├── AbbNode.cs
│   │   │   └── BufferMensajes.cs
│   │   │
│   │   └── Logs/
│   │       ├── LogNode.cs
│   │       └── LogAuditoria.cs
│
│   ├── Graphviz/
│   │
│   │   ├── DotCompiler.cs
│   │   ├── SvgBuilder.cs
│   │   ├── DotTemplates.cs
│   │   ├── MatrixGraphGenerator.cs
│   │   ├── AvlGraphGenerator.cs
│   │   ├── BufferGraphGenerator.cs
│   │   └── RouteGraphGenerator.cs
│
│   ├── Middleware/
│   │
│   │   ├── ExceptionMiddleware.cs
│   │   ├── BasicAuthMiddleware.cs
│   │   └── RequestLoggingMiddleware.cs
│
│   ├── Configuration/
│   │
│   │   ├── AppInstanceSettings.cs
│   │   ├── AuthenticationConfiguration.cs
│   │   ├── HttpClientConfiguration.cs
│   │   ├── LocalizationConfiguration.cs
│   │   └── GraphvizConfiguration.cs
│
│   ├── Utils/
│   │
│   │   ├── Constants.cs
│   │   ├── RegexPatterns.cs
│   │   ├── CoordinateMapper.cs
│   │   ├── BasicAuthEncoder.cs
│   │   └── XmlNamespaces.cs
│
│   ├── Resources/
│   │
│   │   ├── SharedResource.es.resx
│   │   └── SharedResource.en.resx
│
│   ├── wwwroot/
│   │
│   │   ├── css/
│   │   │   ├── site.css
│   │   │   ├── dashboard.css
│   │   │   └── reports.css
│   │   │
│   │   ├── js/
│   │   │   ├── dashboard.js
│   │   │   ├── simulation.js
│   │   │   └── reports.js
│   │   │
│   │   ├── images/
│   │   │
│   │   └── lib/
│
│   ├── appsettings.json
│   ├── appsettings.Development.json
│   ├── appsettings.North.json
│   ├── appsettings.South.json
│   └── Program.cs
│
├── OrbitNet.Tests/
│
│   ├── AVLTests/
│   │   └── RegistroSatelitesTests.cs
│   │
│   ├── MatrixTests/
│   │   └── RedSatelitalPlanoTests.cs
│   │
│   ├── BufferTests/
│   │   └── BufferMensajesTests.cs
│   │
│   ├── LogTests/
│   │   └── LogAuditoriaTests.cs
│   │
│   ├── GraphvizTests/
│   │   └── GraphvizGenerationTests.cs
│   │
│   └── IntegrationTests/
│       ├── RelayTests.cs
│       ├── BasicAuthTests.cs
│       ├── XmlUploadTests.cs
│       └── SimulationTests.cs
│
├── documentacion/
│   ├── MANUAL_TECNICO.md
│   ├── MANUAL_USUARIO.md
│   ├── UML/
│   └── DiagramaFlujo/
│
└── archivodeprueba/
    ├── norte.xml
    ├── sur.xml
    └── errores.xml




QUE HARA CADA INTEGRANTE
Integrante 1: Estructuras de Datos

Responsable de todo:

DataStructures/

Archivos:

Interfaces/
Matrix/
AVL/
Buffer/
Logs/

Y sus pruebas:

OrbitNet.Tests/
├── AVLTests/
├── MatrixTests/
├── BufferTests/
└── LogTests/

Este integrante prácticamente no debería tocar nada de Controllers, Views o Services.

Integrante 2: Motor de Simulación

Responsable de:

Services/SimulationEngine/

Archivos:

TickProcessor.cs
OrbitalRotator.cs
RoutingService.cs
PriorityDispatcher.cs
SimulationCoordinator.cs

Y:

Models/Entities/

Porque necesita trabajar con:

Satellite.cs
PolarOrbit.cs
GroundAntenna.cs
MessagePacket.cs
SimulationState.cs

También:

SimulationTests.cs

Integrante 3: API y Comunicación

Responsable de:

Controllers/Api/
ConfigurationController.cs
RelayController.cs
SimulationApiController.cs

Además:

Services/Communication/
BasicAuthService.cs
RelayHttpClient.cs

Y:

Middleware/
BasicAuthMiddleware.cs
RequestLoggingMiddleware.cs

Pruebas:

RelayTests.cs
BasicAuthTests.cs

Integrante 4: Frontend y Reportes

Responsable de:

Views/
wwwroot/
Graphviz/
Controllers/
HomeController.cs
UploadController.cs
SimulationController.cs
MatrixController.cs
SatelliteController.cs
ReportController.cs
LogController.cs

Y:

Models/ViewModels/

Pruebas:

GraphvizGenerationTests.cs




ARCHIVOS QUE NO SE DEBEN DE TOCAR LIBREMENTE

Estos son los peligrosos:

Program.cs
appsettings.json
appsettings.North.json
appsettings.South.json
Models/Entities/*
Models/DTOs/*

Porque son archivos compartidos por casi todo el sistema.


Entidades
Satellite.cs
PolarOrbit.cs
GroundAntenna.cs
MessagePacket.cs
SimulationState.cs

DTOs
RelayRequestDto.cs
RelayResponseDto.cs
SimulationStepRequest.cs
SimulationStepResponse.cs

Interfaces
IMatrix.cs
IAVLTree.cs
IMessageBuffer.cs
IAuditLog.cs

archivos con más conflicto de integración, aquí definiremos el día de integración y pruebas
Program.cs
SimulationCoordinator.cs
Satellite.cs
SimulationState.cs
RoutingService.cs