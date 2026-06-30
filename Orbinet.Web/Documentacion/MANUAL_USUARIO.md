# Manual de Usuario - Orbinet.Web

## 1. Introduccion
Orbinet.Web es la interfaz web del sistema Orbital Network, orientada a la visualizacion y control de operaciones del frontend. Desde esta aplicacion el usuario puede consultar el estado general del sistema, cargar configuraciones XML, ejecutar simulaciones, revisar reportes, monitorear logs, inspeccionar satelites y gestionar funciones relacionadas con la comunicacion entre hemisferios.

Este manual describe el uso de la interfaz web, los requisitos para ejecutarla, el flujo de navegacion y los mensajes de error o situaciones comunes que el usuario puede encontrar.

## 2. Alcance del manual
Este documento cubre unicamente el frontend de Orbinet.Web.
No describe implementaciones internas, servicios del backend ni detalles de codigo fuente.

## 3. Requisitos de sistema

### 3.1 Requisitos minimos
- Sistema operativo Windows, Linux o macOS con soporte para .NET.
- Navegador moderno: Microsoft Edge, Google Chrome, Mozilla Firefox o equivalente.
- .NET SDK compatible con el proyecto.
- Conexiones locales permitidas en los puertos usados por la aplicacion.

### 3.2 Requisitos recomendados
- Resolucion de pantalla minima de 1366 x 768.
- Conexion estable a la red local cuando se usen dos instancias del sistema.
- Memoria RAM suficiente para ejecutar la aplicacion y el navegador sin lentitud.

### 3.3 Puertos de ejecucion
- Hemisferio Norte: normalmente en http://localhost:5000
- Hemisferio Sur: normalmente en http://localhost:5001

## 4. Instalacion

### 4.1 Descarga o acceso al proyecto
- Abrir la carpeta del proyecto Orbinet.Web.
- Verificar que existan los archivos del proyecto y las carpetas de Views, Controllers, Services y wwwroot.

### 4.2 Configuracion del entorno
- Verificar que el SDK de .NET este instalado.
- Confirmar que el puerto elegido no este ocupado por otra aplicacion.
- Revisar el archivo de configuracion correspondiente al hemisferio que se va a ejecutar.

### 4.3 Ejecucion local
1. Abrir una terminal en la carpeta Orbinet.Web.
2. Definir la variable de entorno ASPNETCORE_URLS con el puerto deseado.
3. Definir la variable de entorno HEMISPHERE con North o South.
4. Ejecutar el proyecto con `dotnet run --project .\Orbinet.Web.csproj --no-launch-profile`.

### 4.4 Ejecucion de dos instancias
- Instancia Norte: puerto 5000 y HEMISPHERE=North.
- Instancia Sur: puerto 5001 y HEMISPHERE=South.

## 5. Compilacion

### 5.1 Compilacion desde consola
- Ubicarse en la carpeta del proyecto.
- Ejecutar `dotnet build .\Orbinet.Web.csproj`.
- Revisar que el proceso termine sin errores.

### 5.2 Resultado esperado
- La compilacion debe generar los binarios en la carpeta bin.
- Si existe un error de compilacion, revisar el detalle mostrado por la consola y corregirlo antes de continuar.

## 6. Acceso a la aplicacion

### 6.1 Inicio
- Abrir el navegador.
- Ingresar a la direccion configurada para la instancia activa.
- Esperar la carga de la pagina principal.

### 6.2 Idioma
- La interfaz permite cambiar entre espanol e ingles.
- El selector de idioma aparece en la barra superior y en el menu lateral.
- El idioma elegido se conserva mientras el usuario navega por la aplicacion.

## 7. Estructura general de la interfaz
La aplicacion esta organizada en tres zonas principales:

- Menu lateral: permite acceder a las secciones principales.
- Barra superior: contiene el boton de despliegue del menu y el cambio de idioma.
- Area de contenido: muestra la informacion de cada modulo.

## 8. Flujo de uso general

### 8.1 Secuencia recomendada
1. Iniciar la aplicacion en el hemisferio deseado.
2. Revisar el dashboard inicial.
3. Cargar una configuracion XML en caso de requerirse.
4. Validar el estado de satelites, matriz, logs y reportes.
5. Ejecutar simulaciones o consultar reportes segun la necesidad.

### 8.2 Navegacion basica
- Dashboard para revisar resumen general.
- Upload para cargar archivos XML.
- Simulation para ejecutar ticks y revisar la simulacion.
- Matrix para visualizar la matriz de ocupacion.
- Satellites para consultar la lista y detalle de satelites.
- Communication para revisar el estado de los enlaces de relay.
- Reports para abrir reportes visuales.
- Logs para inspeccionar eventos del sistema.
- Configuration para revisar ajustes del sistema, si la vista esta habilitada en la version usada.

## 9. Funcionalidades principales

### 9.1 Dashboard
- Muestra el tick actual.
- Presenta el numero de satelites activos.
- Indica mensajes pendientes y procesados.
- Resume el estado de la simulacion.
- Muestra la imagen del hemisferio correspondiente.
- Permite ejecutar ticks rapidos y detener la simulacion.

### 9.2 Carga XML
- Permite seleccionar un archivo XML.
- Acepta seleccion por boton y, segun la interfaz, arrastre y suelta.
- Muestra una vista previa del archivo antes de confirmar.
- Al finalizar, presenta el resultado de carga con nombre de archivo, satelites, antenas y orbitas cargadas.

### 9.3 Simulacion
- Ejecuta un tick individual.
- Ejecuta 10 o 100 ticks de forma rapida.
- Permite detener la simulacion.
- Muestra satelites activos, mensajes procesados y mensajes pendientes.

### 9.4 Matriz
- Visualiza la matriz espacial y su ocupacion.
- Muestra cantidad de filas, columnas y nodos ocupados.
- Incluye leyenda para satelite activo, inactivo y espacio vacio.
- Permite exportar el contenido visual como SVG.

### 9.5 Satelites
- Lista satelites con identificador, nombre, orbita, tipo y estado.
- Permite buscar satelites por texto.
- Permite abrir el detalle de cada satelite.
- El detalle incluye estadisticas, posicion y historial reciente.

### 9.6 Communication
- Presenta el estado de las rutas de comunicacion entre hemisferios.
- Muestra ocupacion de colas, paquetes procesados y eventos recientes.
- Permite refresco automatico y manual.
- Ofrece acciones como reenviar paquetes, probar conectividad, limpiar buffers y exportar estado.

### 9.7 Reports
- Centraliza el acceso a reportes visuales.
- Incluye reportes de memoria, matriz, buffers y routing.
- Permite descargar el reporte como SVG.

### 9.8 Logs
- Lista eventos del sistema con fecha, nivel, evento y detalle.
- Incluye filtro por texto para localizar eventos rapidamente.
- Permite refrescar la lista.

## 10. Reportes disponibles

### 10.1 Reporte de memoria
- Muestra la estructura de memoria o vista asociada al almacenamiento interno del sistema.

### 10.2 Reporte de matriz
- Muestra la matriz espacial con su distribucion actual.

### 10.3 Reporte de buffers
- Muestra el estado de los buffers, su ocupacion y uso.

### 10.4 Reporte de routing
- Muestra las rutas de comunicacion entre satelites y antenas.

### 10.5 Exportacion
- Cada reporte puede descargarse como archivo SVG.
- Si el contenido no esta disponible, la pantalla mostrara un mensaje indicando que no hay contenido para visualizar.

## 11. Mensajes, validaciones y errores comunes

### 11.1 Carga de archivo
- Si no se selecciona un archivo, la carga no puede continuar.
- Si el XML es invalido o vacio, la aplicacion puede mostrar un error de validacion.
- Si el archivo no cumple con la estructura esperada, los detalles del error deben revisarse en la pantalla de resultado.

### 11.2 Simulacion
- Si la simulacion esta detenida, algunos estados pueden verse como inactivos.
- Si el boton de detener se usa durante una simulacion ya detenida, la interfaz reflejara el estado actual y no deberia producir fallos visuales.

### 11.3 Comunicacion
- Si un buffer no existe, la accion de limpieza no podra completarse.
- Si la conectividad entre hemisferios no esta disponible, la prueba devolvera un estado negativo o un mensaje de no conexion.
- Si el puerto remoto no responde, puede mostrarse un error de comunicacion o un mensaje de indisponibilidad.

### 11.4 Reportes
- Si no hay datos para el reporte, la vista puede mostrar un mensaje de contenido no disponible.
- Si la exportacion SVG falla, se debe revisar que el reporte tenga contenido generado.

### 11.5 Errores de navegador
- Verificar que JavaScript este habilitado.
- Verificar que el navegador no bloquee ventanas emergentes o descargas.
- Si la pagina no carga estilos o imagenes, revisar la conexion y la ruta local del proyecto.

### 11.6 Error general
- Cuando ocurre un error inesperado, la aplicacion muestra una pagina de error con un identificador de solicitud.
- Ese identificador sirve para diagnosticar el incidente si se reporta al administrador.

## 12. Solucion de problemas

### 12.1 El puerto esta ocupado
- Cerrar la aplicacion que este usando el puerto.
- Cambiar a otro puerto disponible.
- Verificar que ASPNETCORE_URLS apunte al puerto correcto.

### 12.2 La aplicacion no abre
- Confirmar que dotnet run se ejecuto correctamente.
- Revisar que no existan errores en la consola.
- Verificar que el navegador use la URL correcta.

### 12.3 No cargan reportes o graficos
- Revisar que el sistema tenga datos generados.
- Confirmar que la seccion de reportes se haya abierto desde el menu correcto.
- Recargar la pagina.

### 12.4 No se muestran satelites o logs
- Verificar que la carga inicial del sistema se haya realizado.
- Recargar la vista.
- Revisar que el origen de datos este disponible para la instancia activa.

### 12.5 El idioma no cambia
- Usar los botones ES o EN de la barra superior o del menu lateral.
- Confirmar que el navegador no este bloqueando las cookies o la persistencia local de la sesion.

## 13. Buenas practicas de uso
- Mantener abiertas ambas instancias solo cuando sea necesario.
- Verificar siempre el hemisferio correcto antes de cargar datos o ejecutar simulaciones.
- Usar reportes y logs para validar el estado del sistema despues de cada accion importante.
- Guardar evidencia visual de cargas, simulaciones y errores cuando sea necesario para soporte.

