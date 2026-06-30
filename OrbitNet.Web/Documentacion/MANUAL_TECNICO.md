# Manual Tecnico

**Universidad de San Carlos de Guatemala**  
**Facultad de Ingenieria**  
**Escuela de Ciencias y Sistemas**

**Curso:** Introduccion a la Programacion y Computacion 2  
**Proyecto:** OrbitNet - Sistema de Comunicacion Satelital  
**Lenguaje:** C# (.NET 10)  
**Framework:** ASP.NET Core MVC

---

# Introduccion

Proyecto **OrbitNet** desarrollado bajo la arquitectura ASP.NET Core MVC. El sistema simula una red de comunicacion satelital conformada por satelites, antenas terrestres y buffers de mensajes, permitiendo la administracion de informacion, la simulacion del intercambio de paquetes y la generacion de reportes mediante Graphviz

---

# Descripcion General del Proyecto

OrbitNet es una aplicacion web desarrollada utilizando ASP.NET Core MVC cuya finalidad es simular una red de comunicacion satelital entre dos hemisferios

El sistema administra satelites, antenas terrestres, buffers de mensajes y una matriz dispersa que representa la distribucion espacial de la red. Ademas, incorpora mecanismos para visualizar estructuras mediante Graphviz y establecer comunicacion entre dos instancias del sistema utilizando diferentes puertos

Entre las funcionalidades principales del sistema se encuentran:

- Administracion de satelites
- Administracion de antenas terrestres
- Simulacion del envio y recepcion de mensajes
- Gestion de buffers de prioridad
- Representacion de la red satelital mediante una matriz dispersa
- Generacion de reportes graficos mediante Graphviz
- Comunicacion entre hemisferios utilizando servicios HTTP
- Internacionalizacion de la interfaz en Espanol e Ingles
- Ejecucion de pruebas unitarias mediante xUnit
---

# Arquitectura del Sistema

El proyecto utiliza el patron de arquitectura **Modelo - Vista - Controlador (MVC)** proporcionado por ASP.NET Core

## Modelo (Models)

Contiene todas las clases que representan las entidades del sistema, los ViewModels utilizados por las vistas, los DTO empleados para la comunicacion entre servicios y las enumeraciones utilizadas por la logica del negocio

Entre los modelos principales se encuentran:

- Satellite
- GroundAntenna
- MessagePacket
- SimulationState
- DashboardViewModel
- RelayDashboardViewModel

---

## Vista (Views)

Las vistas implementan la interfaz grafica del sistema utilizando Razor (.cshtml)

Desde ellas el usuario puede:

- Visualizar satelites
- Administrar antenas
- Ejecutar simulaciones
- Consultar reportes
- Visualizar buffers
- Cambiar el idioma del sistema
- Administrar configuraciones

Todas las vistas utilizan recursos de internacionalizacion mediante archivos `.resx`

---

## Controlador (Controllers)

Los controladores reciben las solicitudes HTTP del usuario, procesan la informacion necesaria mediante los servicios correspondientes y envian los modelos adecuados hacia las vistas

Entre los principales controladores se encuentran:

- HomeController
- SimulationController
- SatelliteController
- MatrixController
- UploadController
- ReportsController
- LogsController
- ConfigurationController
- RelayController
- SpaceController

---

## Servicios (Services)

Los servicios contienen la logica de negocio del proyecto.

Dentro de ellos se implementan procesos como:

- Comunicacion entre hemisferios
- Procesamiento de mensajes
- Validacion de archivos XML
- Administracion de configuracion
- Simulacion del sistema
- Autenticacion
- Generacion de datos para las vistas

---

## Estructuras de Datos (DataStructures)

El sistema implementa estructuras de datos desarrolladas manualmente sin utilizar colecciones dinamicas del framework

Las principales estructuras implementadas son:

- Arbol AVL para el registro de satelites
- Arbol Binario de Busqueda para el Buffer de Mensajes
- Lista enlazada para antenas terrestres
- Matriz dispersa ortogonal para representar la red satelital
- Lista enlazada para la bitacora de eventos

---

### Controllers

Contiene todos los controladores MVC encargados de atender las solicitudes del usuario

### DataStructures

Implementa todas las estructuras de datos desarrolladas manualmente para el proyecto

### Models

Almacena las entidades, DTO, ViewModels, enumeraciones y mapeadores utilizados por la aplicacion

### Services

Implementa la logica principal del sistema y la comunicacion entre modulos

### Views

Contiene todas las interfaces desarrolladas utilizando Razor Pages

### Graphviz

Genera la representacion grafica de las estructuras de datos implementadas

### Resources

Almacena los archivos de internacionalizacion para soportar multiples idiomas

### OrbitNet.Tests

Contiene las pruebas unitarias e integracion desarrolladas utilizando xUnit para verificar el funcionamiento de las estructuras de datos y demas componentes del sistema
---

# Tecnologias Utilizadas

El desarrollo del proyecto se realizo utilizando las siguientes tecnologias:

| Tecnologia       | Descripcion                                         |
|------------------|-----------------------------------------------------|
| C#               | Lenguaje principal del proyecto                     |
| .NET 10          | Plataforma de desarrollo utilizada                  |
| ASP.NET Core MVC | Framework para la aplicacion web                    |
| Razor            | Motor de vistas del sistema                         |
| xUnit            | Framework para pruebas unitarias                    |
| Graphviz         | Generacion de reportes graficos                     |
| XML              | Formato utilizado para la carga de configuraciones  |
| HTTP             | Comunicacion entre las instancias del sistema       |

---

# Requisitos del Sistema

Para ejecutar correctamente el proyecto se requiere:

- .NET SDK 10.0 o superior
- Visual Studio Code o Visual Studio 2022
- Git
- Graphviz instalado y configurado
- Sistema Operativo Windows 10 o superior
- Navegador web moderno compatible con ASP.NET Core

---

# Compilacion del Proyecto

Para compilar el sistema desde la terminal se utiliza el siguiente comando:

```bash
dotnet build
```

# Ejecucion del Proyecto

Para ejecutar la aplicacion se utiliza:

```bash
dotnet run
```

Posteriormente el sistema estara disponible mediante el puerto configurado en el archivo `launchSettings.json` o `appsettings.json`.

---

# Ejecucion de Pruebas

Las pruebas unitarias pueden ejecutarse mediante:

```bash
dotnet test
```

Las pruebas verifican el correcto funcionamiento de las estructuras de datos, controladores y servicios implementados en el proyecto

# Controladores del Sistema

Los controladores constituyen la capa encargada de recibir las solicitudes realizadas por el usuario, procesar la informacion mediante los servicios correspondientes y devolver la respuesta adecuada a las vistas

Cada controlador tiene una responsabilidad especifica dentro del sistema y evita que la logica de negocio se encuentre directamente en las vistas

---

# HomeController

## Descripcion

Es el controlador principal del sistema
Se encarga de cargar la pagina inicial, mostrar la informacion general del sistema y administrar el cambio de idioma entre Espanol e Ingles
Ademas permite visualizar el tablero principal del sistema donde se muestran los datos generales de la simulacion

## Metodos principales

### Index()

Carga la vista principal del sistema mostrando el Dashboard con la informacion general

### SetLanguage()

Permite cambiar el idioma de toda la aplicacion utilizando los archivos de internacionalizacion

### Error()

Muestra la pagina de errores cuando ocurre una excepcion dentro del sistema

---

# SimulationController

## Descripcion

Controlador encargado de administrar la simulacion de la red satelital
Permite iniciar la simulacion, avanzar los ticks del sistema y detener la ejecucion cuando sea necesario

## Metodos principales

### Dashboard()

Carga el panel principal de la simulacion

### ExecuteTicks()

Ejecuta la cantidad de ticks indicada por el usuario

### TickResult()

Muestra el resultado obtenido despues de ejecutar uno o varios ticks

### StopSimulation()

Finaliza la simulacion y actualiza el estado del sistema

---

# UploadController

## Descripcion

Permite cargar archivos XML hacia el sistema.
Los archivos cargados contienen la informacion utilizada para crear satelites, antenas, orbitas y demas elementos necesarios para la simulacion.

## Metodos principales

### Index()

Carga la vista para seleccionar el archivo XML

### Upload()

Procesa el archivo recibido y ejecuta las validaciones correspondientes

### Result()

Muestra el resultado de la carga indicando si el proceso fue exitoso o si ocurrieron errores

---

# MatrixController

## Descripcion

Controlador encargado de administrar la visualizacion de la matriz dispersa utilizada para representar la red satelital
Permite mostrar graficamente la posicion de cada satelite dentro de la red

## Metodos principales

### Index()

Carga la vista principal de la matriz dispersa.

---

# SatellitesController

## Descripcion

Administra la informacion relacionada con los satelites registrados dentro del sistema
Permite consultar la informacion almacenada para cada satelite

## Metodos principales

### Index()

Muestra el listado de satelites disponibles

### Details()

Presenta toda la informacion correspondiente a un satelite especifico

---

# ReportsController

## Descripcion

Controlador encargado de generar los diferentes reportes del sistema
Estos reportes utilizan Graphviz para representar graficamente las estructuras de datos implementadas

## Metodos principales

### Index()

Carga la pagina principal de reportes

### MemoryLayout()

Genera el reporte relacionado con la distribucion de memoria

### Routing()

Genera el reporte del recorrido realizado por los mensajes

### Buffers()

Genera el reporte correspondiente al Buffer de Mensajes

---

# LogsController

## Descripcion

Permite visualizar la bitacora de eventos generados durante la ejecucion del sistema
Cada evento registrado contiene informacion acerca de las operaciones realizadas por la simulacion

## Metodos principales

### Index()

Carga la vista principal de la bitacora.

---

# ConfigurationController

## Descripcion

Controlador encargado de administrar la configuracion general del sistema
Permite visualizar y modificar determinados parametros utilizados durante la ejecucion

## Metodos principales

### Index()

Carga la pagina de configuracion

### Save()

Guarda los cambios realizados por el usuario

---

# RelayController

## Descripcion

Controlador encargado de administrar la comunicacion entre los dos hemisferios del sistema
Implementa las operaciones necesarias para enviar y recibir informacion utilizando peticiones HTTP entre diferentes puertos

## Metodos principales

### Index()

Carga el Dashboard de comunicacion

### Refresh()

Actualiza el estado de la conexion entre ambos hemisferios

### ForceSend()

Realiza el envio manual de un paquete hacia el hemisferio remoto

---

# SpaceController

## Descripcion

Controlador encargado de administrar la informacion relacionada con el espacio de simulacion
Sirve como punto de acceso para consultar el estado general de la red satelital y facilitar la comunicacion entre los diferentes componentes del sistema

## Metodos principales

### Index()

Carga la vista principal correspondiente al espacio de simulacion.

---

# Resumen de los Controladores

| Controlador             | Funcion Principal                        |
|-------------------------|------------------------------------------|
| HomeController          | Pagina principal e internacionalizacion  |
| SimulationController    | Administracion de la simulacion          |
| UploadController        | Carga y validacion de archivos XML       |
| MatrixController        | Visualizacion de la matriz dispersa      |
| SatellitesController    | Administracion de satelites              |
| ReportsController       | Generacion de reportes Graphviz          |
| LogsController          | Visualizacion de la bitacora             |
| ConfigurationController | Configuracion del sistema                |
| RelayController         | Comunicacion entre hemisferios           |
| SpaceController         | Administracion del espacio de simulacion |


# Estructuras de Datos

Las estructuras de datos constituyen el nucleo del proyecto OrbitNet
Todas fueron implementadas manualmente sin utilizar estructuras dinamicas del framework, cumpliendo con los requerimientos establecidos para el proyecto
Cada estructura fue desarrollada para resolver una necesidad especifica dentro de la simulacion de la red satelital
---

# RegistroSatelites

## Descripcion

RegistroSatelites implementa un Arbol AVL encargado de administrar todos los satelites registrados dentro del sistema
Su principal objetivo es mantener el arbol balanceado automaticamente para garantizar tiempos de busqueda, insercion y eliminacion eficientes aun cuando el numero de satelites aumente

## Atributos principales

### root

Almacena la referencia al nodo raiz del arbol AVL

### count

Guarda la cantidad total de satelites almacenados

## Metodos principales

### Insert()

Inserta un nuevo satelite dentro del arbol verificando que no existan identificadores duplicados

### Search()

Busca un satelite utilizando su identificador

### Delete()

Elimina un satelite del arbol manteniendo el balance del AVL

### TraverseInOrder()

Realiza un recorrido InOrder devolviendo los satelites ordenados por identificador

### BalanceNode()

Verifica el factor de balance del nodo y ejecuta las rotaciones necesarias

### RotateLeft()

Realiza una rotacion simple hacia la izquierda

### RotateRight()

Realiza una rotacion simple hacia la derecha

### UpdateHeight()

Actualiza la altura de cada nodo despues de una insercion o eliminacion

### GetRoot()

Devuelve la raiz del arbol para permitir la integracion con Graphviz

---

# AvlNode

## Descripcion

Representa cada nodo utilizado por el Arbol AVL
Cada nodo almacena un satelite y las referencias necesarias para formar la estructura del arbol

## Atributos principales

- Satellite
- Height
- Left
- Right

## Funcion

Permitir la construccion del Arbol AVL manteniendo la informacion necesaria para realizar balanceos y recorridos

---

# BufferMensajes

## Descripcion

Implementa un Arbol Binario de Busqueda encargado de almacenar los paquetes de comunicacion segun su prioridad
El mensaje con mayor prioridad siempre sera el primero en ser procesado por el motor de simulacion

## Atributos principales

### root

Referencia al nodo raiz del ABB

### count

Cantidad de mensajes almacenados

## Metodos principales

### Enqueue()

Inserta un nuevo paquete dentro del arbol

### Dequeue()

Obtiene y elimina el mensaje con mayor prioridad

### Peek()

Obtiene el siguiente mensaje sin eliminarlo

### SearchByHexCode()

Busca un mensaje utilizando su codigo hexadecimal

### TraverseInOrder()

Realiza un recorrido ordenado del arbol

### InsertRecursive()

Metodo privado encargado de insertar el nodo en la posicion correspondiente

### DeleteMax()

Elimina el nodo con mayor prioridad

---

# AbbNode

## Descripcion

Representa el nodo utilizado por el Buffer de Mensajes
Cada nodo almacena un objeto MessagePacket junto con las referencias hacia los hijos izquierdo y derecho

## Atributos principales

- Packet
- Left
- Right

## Funcion

Construir el Arbol Binario de Busqueda utilizado para administrar los mensajes

---

# ListaAntenas

## Descripcion

Implementa una lista enlazada encargada de almacenar todas las antenas terrestres registradas en el sistema
Esta estructura permite localizar antenas utilizando su identificador o direccion IP

## Metodos principales

### Add()

Agrega una nueva antena al final de la lista

### SearchById()

Busca una antena utilizando su identificador

### SearchByIp()

Busca una antena utilizando su direccion IP

### Clear()

Elimina todas las antenas almacenadas

## Funcion

Administrar las antenas terrestres utilizadas durante la comunicacion entre satelites

---

# AntenaNode

## Descripcion

Representa cada nodo utilizado por la lista enlazada de antenas

## Atributos principales

- Antenna
- Next

## Funcion

Almacenar una antena y mantener la referencia hacia el siguiente nodo de la lista

---

# RedSatelitalPlano

## Descripcion

Implementa una Matriz Dispersa Ortogonal utilizada para representar la posicion de cada satelite dentro del plano cartesiano
Cada satelite ocupa una posicion determinada mediante coordenadas de fila y columna

## Metodos principales

### Insert()

Inserta un nuevo nodo dentro de la matriz

### Delete()

Elimina un nodo de la matriz

### Search()

Busca un nodo utilizando sus coordenadas

### GetRowHeader()

Obtiene un encabezado de fila

### GetColumnHeader()

Obtiene un encabezado de columna

### GetFirstRowHeader()

Devuelve el primer encabezado de fila para permitir recorridos externos

### InsertInRow()

Inserta un nodo manteniendo el orden horizontal

### InsertInColumn()

Inserta un nodo manteniendo el orden vertical

## Funcion

Representar la distribucion espacial de la red satelital y servir como base para la generacion de reportes mediante Graphviz

---

# MatrixNode

## Descripcion

Representa cada nodo almacenado dentro de la matriz dispersa

## Atributos principales

- Row
- Column
- SatelliteId
- IpAddress
- Up
- Down
- Left
- Right

## Funcion

Mantener las conexiones horizontales y verticales propias de una matriz dispersa

---

# HeaderNode

## Descripcion

Representa los encabezados utilizados por la matriz dispersa
Cada encabezado permite acceder rapidamente a una fila o columna determinada

## Funcion

Facilitar la insercion, eliminacion y recorrido de la matriz

---

# LogAuditoria

## Descripcion

Implementa una lista enlazada utilizada para almacenar todos los eventos importantes generados durante la ejecucion del sistema
Permite mantener una bitacora cronologica de las operaciones realizadas

## Metodos principales

### EscribirEvento()

Agrega un nuevo registro a la bitacora

### BuscarPorRegex()

Busca eventos utilizando expresiones regulares

### Clear()

Elimina todos los registros almacenados

## Funcion

Registrar todas las operaciones importantes realizadas por el sistema durante la simulacion

---

# LogNode

## Descripcion

Representa cada nodo utilizado por la bitacora del sistema

## Atributos principales

- FechaHora
- Gravedad
- Mensaje
- Next

## Funcion

Almacenar la informacion correspondiente a un evento registrado dentro del sistema

---

# Interfaces

El proyecto implementa interfaces para estandarizar el comportamiento de las estructuras de datos

## IAbstractCollection

Define las operaciones comunes utilizadas por todas las estructuras

Entre ellas

- Count
- IsEmpty
- Clear()

---

## IMessageBuffer

Define el contrato que debe cumplir cualquier implementacion del Buffer de Mensajes

Entre los metodos definidos se encuentran

- Agregar()
- ObtenerSiguiente()
- VerSiguiente()
- BuscarPorCodigoHex()
- RecorrerInOrden()

---

## IMatrix

Define las operaciones necesarias para administrar la matriz dispersa
Permite mantener independencia entre la implementacion y el resto del sistema

---

# Integracion con Graphviz

Las estructuras principales fueron preparadas para integrarse con Graphviz mediante metodos que permiten recorrer internamente cada estructura

Entre ellas

- RegistroSatelites mediante GetRoot()
- RedSatelitalPlano mediante GetFirstRowHeader()
- ListaAntenas mediante la lista enlazada
- BufferMensajes mediante recorridos del ABB

Esta integracion permite generar reportes graficos sin modificar directamente las estructuras implementadas

# Models del Sistema

La carpeta Models contiene todas las clases utilizadas para representar la informacion manejada por el sistema

Dentro de esta carpeta se encuentran las entidades principales, los objetos utilizados para transportar informacion, los ViewModels empleados por las vistas y las enumeraciones utilizadas durante la simulacion

Su objetivo es centralizar toda la informacion necesaria para el funcionamiento de la aplicacion

---

# Entities

## Descripcion

Las entidades representan los objetos principales del dominio del sistema
Cada entidad almacena la informacion utilizada por la simulacion y posteriormente es administrada mediante las estructuras de datos desarrolladas para el proyecto
Las entidades son utilizadas por los controladores, servicios y estructuras de datos

---

# Satellite

## Descripcion

Representa un satelite registrado dentro de la red OrbitNet
Cada objeto contiene toda la informacion necesaria para identificar y administrar un satelite durante la simulacion

## Informacion almacenada

- Identificador
- Nombre
- Direccion IP
- Tipo de orbita
- Estado
- Posicion
- Demas propiedades utilizadas por la simulacion

## Funcion

Servir como objeto principal del Arbol AVL y representar cada satelite dentro del sistema

---

# GroundAntenna

## Descripcion

Representa una antena terrestre encargada de enviar y recibir mensajes desde los satelites

## Informacion almacenada

- Identificador
- Nombre
- Direccion IP
- Coordenadas
- Estado

## Funcion

Permitir la comunicacion entre la red terrestre y la red satelital

---

# MessagePacket

## Descripcion

Representa un paquete de informacion enviado entre satelites y antenas
Cada paquete contiene toda la informacion necesaria para ser procesado por el Buffer de Mensajes

## Informacion almacenada

- Codigo hexadecimal
- Prioridad
- Direccion origen
- Direccion destino
- Contenido del mensaje
- Estado

## Funcion

Representar cada mensaje administrado por el Buffer de Mensajes

---

# SimulationState

## Descripcion

Representa el estado general de la simulacion
Permite conocer si la simulacion se encuentra ejecutandose y almacenar informacion relacionada con el estado actual del sistema

---

# DTO

## Descripcion

Los DTO permiten transportar informacion entre los distintos servicios del sistema sin exponer directamente las entidades principales
Su uso facilita la comunicacion entre modulos y mantiene una separacion adecuada entre la logica del negocio y la presentacion
Entre los DTO utilizados se encuentran objetos relacionados con:

- Comunicacion entre puertos
- Informacion de Relay
- Respuestas HTTP
- Transferencia de informacion entre servicios

---

# ViewModels

## Descripcion

Los ViewModels contienen la informacion utilizada especificamente por las vistas Razor
Cada ViewModel prepara los datos necesarios para que la interfaz pueda mostrarlos sin depender directamente de las entidades del sistema

---

# DashboardViewModel

## Funcion

Almacena la informacion utilizada por la pagina principal del sistema
Contiene indicadores generales de la simulacion como:

- Tick actual
- Cantidad de satelites
- Mensajes pendientes
- Mensajes procesados
- Estado de la simulacion

---

# SimulationViewModel

## Funcion

Representa la informacion utilizada durante la simulacion
Permite mostrar el estado actualizado despues de ejecutar cada Tick

---

# SatelliteViewModel

## Funcion

Contiene la informacion necesaria para mostrar un satelite dentro de las vistas
Se utiliza tanto en los listados como en la vista de detalles

---

# UploadViewModel

## Funcion

Almacena el resultado obtenido despues de procesar un archivo XML

Permite informar al usuario:

- Archivo procesado
- Cantidad de satelites cargados
- Cantidad de antenas cargadas
- Errores encontrados

---

# MatrixViewModel

## Funcion

Representa la informacion necesaria para mostrar la matriz dispersa dentro de la interfaz grafica

---

# ReportViewModel

## Funcion

Contiene toda la informacion utilizada para generar los diferentes reportes del sistema

Entre ellos:

- Reportes de memoria
- Reportes de rutas
- Reportes del Buffer de Mensajes

---

# LogViewModel

## Funcion

Representa la informacion mostrada dentro de la bitacora del sistema

Cada registro contiene:

- Fecha
- Tipo de evento
- Descripcion
- Detalles

---

# Enums

## Descripcion

Las enumeraciones permiten representar conjuntos de valores constantes utilizados durante la simulacion
Su uso facilita la lectura del codigo y evita el uso de valores literales

Entre las enumeraciones utilizadas se encuentran:

- OrbitType
- SimulationStatus
- MessagePriority
- Otras utilizadas por los diferentes servicios

---

# Mappers

## Descripcion

Los mapeadores permiten convertir entidades en DTO o ViewModels y viceversa
De esta manera se mantiene separada la informacion utilizada internamente por el sistema de aquella presentada al usuario
Los mapeadores simplifican el intercambio de informacion entre los diferentes modulos del proyecto

---

# Organizacion de Models

La carpeta Models se encuentra organizada de la siguiente manera

Models
│
├── DTO
├── Entities
├── Enums
├── Mappers
└── ViewModels

Cada una de estas carpetas tiene una responsabilidad especifica dentro del sistema y permite mantener una arquitectura organizada y facil de mantener

---

# Importancia de Models

La carpeta Models constituye la base de toda la aplicacion
Toda la informacion utilizada por las estructuras de datos, los servicios, los controladores y las vistas se encuentra representada mediante las entidades y ViewModels definidos dentro de esta carpeta
Su correcta organizacion facilita el mantenimiento del proyecto y permite reutilizar los objetos en los diferentes modulos del sistema

# Services del Sistema

La carpeta Services contiene toda la logica de negocio del proyecto OrbitNet
Los servicios permiten separar la logica de procesamiento de los controladores, facilitando el mantenimiento del sistema y permitiendo reutilizar el codigo desde diferentes partes de la aplicacion
Cada servicio posee una responsabilidad especifica dentro de la simulacion

---

# BasicAuthService

## Descripcion

Implementa el mecanismo de autenticacion basica utilizado durante la comunicacion entre las dos instancias del sistema
Permite validar las credenciales antes de aceptar una solicitud HTTP proveniente del hemisferio remoto

## Funciones principales

- Validacion de credenciales
- Autorizacion de solicitudes
- Proteccion de los servicios internos

---

# RelayHttpService

## Descripcion

Implementa la comunicacion HTTP entre los dos hemisferios del sistema
Permite enviar paquetes de informacion desde una instancia hacia otra utilizando peticiones HTTP

## Funciones principales

### Envio de paquetes

Realiza solicitudes HTTP hacia el hemisferio remoto

### Recepcion de respuestas

Procesa la respuesta obtenida despues del envio

### Verificacion de conexion

Comprueba la disponibilidad del hemisferio remoto antes de realizar el envio

---

# TickProcessor

## Descripcion

Es el servicio encargado de ejecutar la simulacion mediante el avance de ticks
Durante cada tick se procesan los mensajes pendientes, se actualizan los estados de los satelites y se registran los eventos correspondientes

## Funciones principales

- Procesar mensajes pendientes
- Actualizar el estado de la simulacion
- Ejecutar operaciones programadas
- Registrar eventos en la bitacora

---

# XmlIngestService

## Descripcion

Procesa los archivos XML cargados por el usuario
Extrae la informacion contenida dentro del archivo y genera las entidades correspondientes para ser almacenadas dentro de las estructuras de datos

## Funciones principales

### Lectura del archivo XML

Obtiene toda la informacion contenida en el archivo

### Validacion

Verifica que la estructura del XML sea correcta

### Creacion de entidades

Genera objetos Satellite, GroundAntenna y demas elementos necesarios para la simulacion

### Registro de errores

Detecta inconsistencias durante la lectura del archivo

---

# OrbitNetStore

## Descripcion

Representa el almacenamiento central de informacion utilizado durante la ejecucion del sistema
Permite que los diferentes controladores y servicios compartan la misma informacion sin duplicar datos

## Funciones principales

- Almacenar satelites
- Almacenar antenas
- Administrar la simulacion
- Compartir informacion entre modulos

---

# MockDataService

## Descripcion

Servicio utilizado para generar informacion de prueba durante el desarrollo del proyecto
Permite mostrar informacion dentro de las vistas aun cuando la simulacion completa no se encuentra ejecutandose

## Funciones principales

- Generar Dashboard
- Generar satelites de ejemplo
- Generar reportes de prueba
- Generar informacion para las vistas

---

# Validation

## Descripcion

Contiene los servicios encargados de validar la informacion recibida por el sistema
Las validaciones permiten evitar errores durante el procesamiento de archivos y solicitudes

Entre ellas se encuentran:

- Validacion de XML
- Validacion de datos recibidos
- Verificacion de formatos
- Comprobacion de informacion obligatoria

---

# Relay Services

## Descripcion

Los servicios de Relay administran toda la comunicacion entre los dos hemisferios
Permiten enviar mensajes, recibir respuestas y mantener sincronizadas ambas instancias del sistema

## Funciones principales

- Envio de mensajes
- Recepcion de mensajes
- Sincronizacion de estados
- Administracion de respuestas

---

# Integracion de los Servicios

Los servicios trabajan conjuntamente con los controladores y las estructuras de datos
El flujo general del sistema es el siguiente

Usuario

↓

Controller

↓

Service

↓

DataStructures

↓

Models

↓

View

De esta forma los controladores permanecen ligeros y toda la logica del negocio queda concentrada dentro de los servicios

---

# Dependencias entre Servicios

Los servicios utilizan diferentes componentes del proyecto

Entre ellos:

- OrbitNetStore
- DataStructures
- DTO
- ViewModels
- RelayHttpService
- TickProcessor
- XmlIngestService

Esta organizacion facilita la reutilizacion del codigo y mantiene una arquitectura desacoplada

---

# Importancia de Services

La carpeta Services representa el centro de procesamiento del proyecto
Toda la logica relacionada con la simulacion, la comunicacion entre hemisferios, la carga de informacion y el procesamiento de mensajes se ejecuta dentro de estos servicios


# Vistas, Resources y Archivos Estaticos

La interfaz grafica del proyecto OrbitNet fue desarrollada utilizando ASP.NET Core MVC con Razor Views
Las vistas permiten mostrar al usuario la informacion procesada por los controladores y los servicios, ofreciendo una interfaz organizada para administrar la simulacion y consultar el estado del sistema

---

# Views

## Descripcion

La carpeta Views contiene todas las paginas que conforman la interfaz grafica del sistema
Cada vista recibe la informacion desde un controlador utilizando un ViewModel y presenta los datos al usuario mediante Razor
La organizacion de las vistas sigue la estructura de los controladores implementados en el proyecto

---

# Home

## Descripcion

Contiene la pagina principal del sistema
Desde esta vista el usuario puede visualizar la informacion general de la simulacion y acceder a los diferentes modulos disponibles

## Archivos principales

### Index.cshtml

Muestra el Dashboard principal del sistema

Presenta informacion como:

- Cantidad de satelites
- Mensajes procesados
- Mensajes pendientes
- Estado de la simulacion
- Informacion general del sistema

---

# Simulation

## Descripcion

Contiene las vistas relacionadas con la ejecucion de la simulacion
Permite observar el comportamiento del sistema durante cada Tick

## Archivos principales

### Dashboard.cshtml

Presenta el panel principal de la simulacion

### TickResult.cshtml

Muestra el resultado obtenido despues de ejecutar uno o varios ticks

Incluye la informacion actualizada del sistema

---

# Upload

## Descripcion

Permite cargar archivos XML al sistema

## Archivos principales

### Index.cshtml

Formulario para seleccionar el archivo XML

### Result.cshtml

Presenta el resultado del procesamiento indicando si la carga fue realizada correctamente o si se encontraron errores

---

# Matrix

## Descripcion

Muestra la representacion grafica de la matriz dispersa utilizada para administrar la red satelital
Permite observar la posicion de cada satelite dentro de la red

---

# Satellites

## Descripcion

Contiene las vistas encargadas de mostrar la informacion de los satelites registrados

## Archivos principales

### Index.cshtml

Presenta el listado completo de satelites

### Details.cshtml

Muestra toda la informacion correspondiente a un satelite especifico

---

# Reports

## Descripcion

Contiene las vistas utilizadas para presentar los reportes generados por el sistema
Los reportes utilizan Graphviz para representar graficamente las estructuras implementadas

## Archivos principales

### Index.cshtml

Pagina principal de reportes

### MemoryLayout.cshtml

Reporte correspondiente a la memoria utilizada por las estructuras

### Routing.cshtml

Reporte relacionado con las rutas seguidas por los mensajes

### Buffers.cshtml

Reporte correspondiente al Buffer de Mensajes

---

# Logs

## Descripcion

Permite consultar la bitacora del sistema
Cada registro corresponde a un evento generado durante la simulacion

---

# Configuration

## Descripcion

Contiene la interfaz utilizada para administrar la configuracion general del sistema
Permite modificar parametros utilizados durante la ejecucion

---

# Relay

## Descripcion

Presenta el estado de la comunicacion entre ambos hemisferios
Permite visualizar la informacion relacionada con la conexion y el intercambio de mensajes

---

# Shared

## Descripcion

La carpeta Shared contiene las vistas compartidas utilizadas por toda la aplicacion
Entre ellas se encuentran

### _Layout.cshtml

Define la estructura general utilizada por todas las paginas

Contiene:

- Barra lateral
- Barra superior
- Navegacion
- Cambio de idioma
- Contenedor principal

### Error.cshtml

Vista utilizada para mostrar errores durante la ejecucion

---

# Resources

## Descripcion

La carpeta Resources almacena todos los archivos utilizados para la internacionalizacion del sistema
Estos archivos permiten mostrar la interfaz tanto en Espanol como en Ingles
El cambio de idioma se realiza automaticamente mediante el controlador HomeController

Entre los recursos utilizados se encuentran:

- SharedResource.resx
- SharedResource.en.resx
- SharedResource.es.resx

Gracias a estos archivos toda la interfaz mantiene el mismo funcionamiento independientemente del idioma seleccionado

---

# wwwroot

## Descripcion

La carpeta wwwroot contiene todos los recursos estaticos utilizados por la aplicacion
Estos archivos son enviados directamente al navegador del usuario

Entre ellos se encuentran

- Archivos CSS
- Imagenes
- Iconos
- Archivos JavaScript
- Recursos graficos

---

# Archivos CSS

El proyecto utiliza diferentes hojas de estilo para organizar la apariencia de la aplicacion

Entre las principales se encuentran

- layout.css
- sidebar.css
- topbar.css
- buttons.css
- tables.css
- cards.css
- reports.css
- badges.css
- site.css

Cada archivo tiene una responsabilidad especifica y permite mantener organizado el diseño del sistema

---

# Organizacion de las Vistas

La estructura utilizada dentro de Views corresponde directamente con los controladores implementados

Views
│
├── Home
├── Simulation
├── Upload
├── Matrix
├── Satellites
├── Reports
├── Logs
├── Configuration
├── Relay
└── Shared

Esta organizacion facilita el mantenimiento de la aplicacion y permite localizar rapidamente cada vista correspondiente a un controlador

---

# Importancia de la Interfaz

La interfaz desarrollada permite que el usuario interactue facilmente con el sistema OrbitNet


# Graphviz, Middleware y Configuracion del Sistema

Esta seccion describe los componentes encargados de inicializar la aplicacion, configurar los servicios necesarios para su funcionamiento y generar las representaciones graficas utilizadas durante la simulacion

Estos componentes permiten que todos los modulos trabajen de forma integrada y proporcionan los recursos necesarios para ejecutar correctamente el sistema

---

# Graphviz

## Descripcion

La carpeta Graphviz contiene las clases encargadas de generar la representacion grafica de las estructuras de datos implementadas dentro del proyecto
Su principal objetivo es convertir las estructuras internas del sistema en archivos compatibles con Graphviz para visualizar su contenido de manera grafica

Las estructuras que pueden representarse son:

- Arbol AVL de satelites
- Buffer de Mensajes
- Matriz Dispersa
- Lista de Antenas
- Bitacora del sistema

## Funcionamiento

Cada estructura proporciona metodos de acceso que permiten recorrer sus nodos sin modificar su contenido
Posteriormente Graphviz utiliza esta informacion para construir el archivo DOT correspondiente y generar la imagen del reporte

## Beneficios

- Facilita la visualizacion de las estructuras
- Permite verificar el contenido almacenado
- Ayuda durante las pruebas y depuracion
- Proporciona reportes utiles para el usuario

---

# Middleware

## Descripcion

El Middleware administra el flujo de las solicitudes HTTP dentro de la aplicacion
Cada solicitud realizada por el usuario pasa por una serie de componentes antes de llegar al controlador correspondiente

Durante este proceso pueden ejecutarse tareas como:

- Validacion
- Autenticacion
- Manejo de errores
- Registro de informacion
- Configuracion de cultura

El uso de Middleware permite mantener una arquitectura organizada y separar responsabilidades

---

# Program.cs

## Descripcion

Program.cs constituye el punto de inicio de toda la aplicacion

Desde este archivo se registran los servicios necesarios para el funcionamiento del sistema y se configura el pipeline de ejecucion de ASP.NET Core

Entre las configuraciones realizadas se encuentran

- Registro de controladores MVC
- Registro de servicios personalizados
- Configuracion de internacionalizacion
- Registro de HttpClient
- Configuracion de rutas
- Configuracion de archivos estaticos
- Configuracion de excepciones
- Configuracion del puerto de ejecucion

Program.cs tambien registra servicios como

- OrbitNetStore
- BasicAuthService
- RelayHttpService
- XmlIngestService
- TickProcessor

Estos servicios quedan disponibles para toda la aplicacion mediante inyeccion de dependencias

---

# appsettings.json

## Descripcion

Este archivo almacena la configuracion general utilizada por la aplicacion

Entre los parametros configurados se encuentran:

- Informacion del sistema
- Configuracion de puertos
- Direcciones del hemisferio remoto
- Parametros generales de la simulacion

El uso de este archivo permite modificar la configuracion sin necesidad de recompilar el proyecto

---

# appsettings.Development.json

## Descripcion

Contiene configuraciones especificas para el ambiente de desarrollo
Permite modificar parametros utilizados unicamente durante las pruebas sin afectar la configuracion utilizada en produccion

---

# launchSettings.json

## Descripcion

Define la forma en que Visual Studio y Visual Studio Code ejecutan la aplicacion durante el desarrollo

Entre los parametros configurados se encuentran:

- Puerto de ejecucion
- Variables de entorno
- Perfil de lanzamiento
- URL inicial

Este archivo facilita la ejecucion del proyecto sin necesidad de configurar manualmente los parametros cada vez que se inicia la aplicacion

---

# Internacionalizacion

## Descripcion

El sistema implementa soporte para multiples idiomas utilizando los recursos de ASP.NET Core
Toda la interfaz puede mostrarse en Espanol o Ingles sin modificar el codigo de las vistas
La seleccion del idioma se realiza mediante HomeController y los archivos Resource correspondientes

Los archivos utilizados son:

- SharedResource.resx
- SharedResource.en.resx
- SharedResource.es.resx

---

# Configuracion de Puertos

## Descripcion

OrbitNet fue desarrollado para ejecutar dos instancias independientes del sistema
Cada instancia representa un hemisferio diferente y utiliza un puerto distinto para permitir la comunicacion

Durante la ejecucion cada instancia mantiene informacion como:

- Puerto local
- Direccion del hemisferio remoto
- Configuracion de autenticacion
- Servicios HTTP disponibles

Esta configuracion permite establecer comunicacion entre ambos hemisferios mediante RelayHttpService

---

# Flujo de Inicializacion

Cuando la aplicacion inicia se ejecuta el siguiente proceso

Program.cs

↓

Carga de configuracion

↓

Registro de servicios

↓

Configuracion de Middleware

↓

Inicializacion de MVC

↓

Carga de rutas

↓

Inicio del servidor web

↓

Aplicacion disponible para el usuario

---

# Integracion General

Todos los componentes descritos anteriormente trabajan conjuntamente con los controladores, servicios y estructuras de datos

El flujo general de una solicitud es el siguiente

Usuario

↓

Vista

↓

Controlador

↓

Servicio

↓

Estructura de Datos

↓

Modelos

↓

Respuesta

↓

Vista


Esta arquitectura permite mantener una clara separacion entre la interfaz, la logica del negocio y el almacenamiento de la informacion

---

# Importancia de la Configuracion

La correcta configuracion del sistema permite que todos los modulos trabajen de forma integrada


# Pruebas, Integracion y Consideraciones Finales

# OrbitNet.Tests

## Descripcion

La carpeta OrbitNet.Tests contiene todas las pruebas unitarias desarrolladas para verificar el correcto funcionamiento del sistema
Las pruebas fueron implementadas utilizando el framework xUnit y permiten comprobar que las estructuras de datos y los diferentes componentes funcionan correctamente antes de integrarse con el resto de la aplicacion
La ejecucion frecuente de estas pruebas permitio detectar errores durante el desarrollo y asegurar la estabilidad del proyecto

---

# Pruebas de Estructuras de Datos

Las pruebas unitarias verifican el funcionamiento de las estructuras implementadas manualmente
Entre las principales pruebas se encuentran:

## RegistroSatelitesTests

Comprueba el funcionamiento del Arbol AVL

Se verifican operaciones como:

- Insercion de satelites
- Busqueda por identificador
- Eliminacion de nodos
- Balanceo del arbol
- Recorrido InOrder
- Eliminacion de duplicados

---

## BufferMensajesTests

Valida el funcionamiento del Arbol Binario de Busqueda utilizado para administrar los mensajes
Las pruebas verifican:

- Insercion de mensajes
- Prioridades
- Obtencion del siguiente mensaje
- Eliminacion del mensaje procesado
- Busqueda por codigo hexadecimal
- Recorrido del arbol

---

## ListaAntenasTests

Comprueba el funcionamiento de la lista enlazada utilizada para almacenar las antenas terrestres
Las pruebas realizadas verifican:

- Insercion de antenas
- Busqueda por identificador
- Busqueda por direccion IP
- Eliminacion de datos
- Cantidad de elementos almacenados

---

## RedSatelitalPlanoTests

Verifica el correcto funcionamiento de la matriz dispersa
Las pruebas permiten comprobar:

- Insercion de nodos
- Eliminacion
- Busqueda
- Integridad de los encabezados
- Recorridos de la estructura

---

## LogAuditoriaTests

Comprueba el funcionamiento de la bitacora del sistema
Las pruebas verifican:

- Registro de eventos
- Busqueda mediante expresiones regulares
- Eliminacion de registros
- Integridad de la lista enlazada

---

# Integracion entre Modulos

El proyecto fue desarrollado utilizando una arquitectura modular
Cada modulo posee una responsabilidad especifica y se comunica con los demas mediante interfaces, servicios y modelos compartidos
Los principales modulos son:

- Frontend
- API y Comunicacion
- Motor de Simulacion
- Estructuras de Datos

Cada integrante desarrollo un modulo especifico y posteriormente se realizaron integraciones utilizando Git para unificar todos los componentes

---

# Flujo General del Sistema

El funcionamiento del proyecto puede resumirse mediante el siguiente proceso

Usuario

↓

Interfaz Web

↓

Controladores

↓

Servicios

↓

Estructuras de Datos

↓

Procesamiento de la Simulacion

↓

Generacion de Reportes

↓

Respuesta al Usuario


Durante este proceso cada componente participa unicamente en las tareas correspondientes a su responsabilidad

---

# Dependencias del Proyecto

El sistema utiliza diferentes tecnologias y bibliotecas para su funcionamiento

Entre ellas se encuentran

- ASP.NET Core MVC
- .NET 10
- Razor Pages
- xUnit
- Graphviz
- XML
- HTTP Client
- Git

Estas herramientas permiten desarrollar una aplicacion modular, escalable y facil de mantener

---


# Conclusiones

El desarrollo de OrbitNet permitio implementar una aplicacion basada en una arquitectura MVC utilizando estructuras de datos desarrolladas manualmente

Durante el proyecto se implementaron un Arbol AVL, un Arbol Binario de Busqueda, una Lista Enlazada y una Matriz Dispersa para resolver los diferentes problemas planteados por la simulacion

La separacion entre controladores, servicios, modelos y estructuras de datos permitio construir un sistema modular, reutilizable y facil de mantener

La utilizacion de pruebas unitarias permitio validar el funcionamiento de los componentes mas importantes antes de su integracion con el resto del proyecto

Finalmente la organizacion del codigo y la documentacion desarrollada facilitan futuras ampliaciones y el mantenimiento del sistema por parte de otros desarrolladores

---

# Referencias

Microsoft

Documentacion oficial de ASP.NET Core MVC

https://learn.microsoft.com/aspnet/core

Microsoft

Documentacion oficial de C#

https://learn.microsoft.com/dotnet/csharp

Microsoft

Documentacion oficial de .NET

https://learn.microsoft.com/dotnet

Graphviz

https://graphviz.org

xUnit

https://xunit.net

Git

https://git-scm.com