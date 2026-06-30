# Bitacora Personal de Desarrollo

**Nombre:** Gustavo Alejandro Chip Aguilar

**Modulo:** Estructuras de Datos

# Semana 1
## Trabajo realizado

- Revision del documento del proyecto
- Configuracion del entorno de desarrollo
- Se empezo a trabajar con el codigo de <LogNodo.cs> y <LogAuditoria.cs> 

* Nota: Use el comando de dotnet new mvc -n, para copiar codigo de archivos .json y Program.cs para poder realizar los test

# Semana 2
## Trabajo realizado

- Se trabajo con <HeaderNode.cs> implementando una lista doblemente enlazada
- Se trabajo con <MatrixNode.cs> implementando una matriz para representar dentro a lo satelites
- Se trabajo con <RedSatelitalPlano.cs> agregando la estructura para la matriz disperza
- Se trabajo con <IMatriz.cs> para indicar a una clase que implemente una matrix, el cual seria usada para <RedSatelitalPlano.cs>
- Se realizaron las pruebas unitarias de <HeaderNoteTests.cs>, <MatrixNodeTests.cs> y <RedSatelitelPlanoTest.cs>

* Nota: sin nada relevante que contar

# Semana 3
## Trabajo realizado

- Se modifico el nombre de metodos de las pruebas unitarias
- Se ocultaron las carpetas de </obj> y </bin> de Orbinet.Web y OrbitNet.test ya que eras demasiados e innecesarios para el proyecto
- Se corrigio errores de nombres de using y namespace
- Se trabajo con <MessageBuffer.cs> para manejar mensajes con prioridad usando un ABB (arbol binario de busqueda)
- Se trabajo con <IMessageBuffer.cs> para la creacion de la interfaz para establecer un conjunto de metodos que debe implementar cualquier estructura encargada de administrar el Buffer de Mensajes del sistema
- Se trabajo con <AbbNode.cs> es el nodo que almacena cada MessagePacket
- Se creo la una carpeta de </Antenas> con los archivos de <ListasAntenas.cs> y <AntenaNode.cs> para no mezclar las antenas con los satelites
- Se realizaron las pruebas unitarias de <ListaAntenasTests.cs> y <BufferMensajesTests.cs>

* Nota: se agregaron metodos en español en el codigo de <IMessageBuffer.cs> y en el codigo de <MessegeBuffer.cs> se agregaro metodos publicos que hacen puente con los metodos ya creados para que los pueda usar el compañero que trabaja con el motor-de-simulacion 

# Semana 4
## Trabajo realizado
- Se trabajo con <AvlNode.cs> con nodos de base que almacena como objeto al satellite
- Se trabajo con <RegistroSatelites.cs> con operaciones de insercion, busqueda, eliminacion y balanceo
- Se realizaron las pruebas unitaria de <AVLTests.cs> y <RegistroSatelitesTests.cs>
- Se agrego el metodo SearchByIp() en ListaAntenas.cs para permitir la busqueda de antenas terrestres mediante su direccion IP

* Nota:
