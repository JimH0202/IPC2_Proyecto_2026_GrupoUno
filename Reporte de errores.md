# Reporte de Integración en Rama `develop` (Fase 2)

A continuación slos conflictos de control de versiones presentados durante la unificación de los módulos de Estructuras de Datos, Frontend/Reportes y API/Comunicación:

### 1. Archivos Core y de Arranque (`Program.cs` y `.csproj`)
* **`Program.cs`**: Se unificaron las inyecciones de dependencias. El archivo ahora inicializa los controladores MVC (`AddControllersWithViews`), la configuración de internacionalización (`AddLocalization`), el cliente HTTP de la API (`AddHttpClient`), así como los servicios y estructuras de datos en memoria (Singletons).
* **`Orbinet.Web.csproj`**: Se resolvieron los conflictos de dependencias conservando todas las referencias a paquetes NuGet (`<PackageReference>`), asegurando la compilación tanto de las herramientas gráficas (Graphviz) como del Backend.

### 2. Configuración de Instancias y Puertos
* **`launchSettings.json`**: Se eliminaron los puertos asignados por defecto por el entorno de desarrollo, forzando la configuración requerida: puerto 5000 para el perfil `OrbitNet-North` y puerto 5001 para `OrbitNet-South`.
* **`AppInstanceSettings.cs`**: Se conservaron las propiedades declaradas por ambos módulos (`SiblingPort` como entero y `RemoteHemisphereUrl` como cadena de texto) con sus respectivos valores por defecto, previniendo errores de ejecución en la comunicación REST y en la interfaz de usuario.
* **Archivos `appsettings.json` (Norte y Sur)**: Debido a diferencias en la nomenclatura de los nodos (`"AppInstance"` frente a `"SystemConfiguration"`), se conservaron ambas secciones para que cada módulo pueda consumir sus respectivas variables de entorno.

### 3. Vistas e Internacionalización
* **`HomeController.cs`**: Se priorizó el código proveniente de la rama de Frontend, responsable del manejo principal de las vistas. La lógica de la API permanece aislada en el `SpaceController`.
* **`SharedResource.es.resx` (Diccionarios)**: Se resolvieron los conflictos de formato conservando el esquema XML (`<xsd:schema>`) provisto por el Frontend y asegurando que las referencias de ensamblado apunten a la versión de .NET 8.0, garantizando el soporte del idioma.

### 4. Documentación
* **`README.md`**: Se integraron ambas documentaciones. La sección superior detalla la arquitectura general y la estructura de directorios, mientras que la sección inferior contiene la especificación técnica de la API, ejemplos de *payloads* JSON y la configuración de seguridad mediante *Basic Auth*.