# ADR-02: Magic library - Patrón MVC

| Campo  | Valor |
|--------|-------|
| Autor  | Astrit Cetzal |
| Fecha  | 05/06/2026 |
| Estado | 'Aceptado' |

 Remplazado por ADR-02

---

## Contexto

### Descripción del sistema
Estoy en proceso de construcción de un sistema que administre los libros leídos, los libros pendientes y los que estan en proceso de lectura. La idea de este sistema surge porque muchos lectores (en especial aquellos que solo leen en linea y descargan PDFs en internet) llegan a olvidar que libros han leido, cuales estapan leyendo o cuales dejaron pendientes. 
Para este proyecto usaré visual estudio, trabajaré con el Framework de .net y usaré el patrón MVC y claro el lenguaje principal para mi app es C#.

### Primera idea del sistema
Principalmente tenia en mente un sistema que administre el dinero que gasto, sin embargo me convencio mas el proyecto de los libros, entonces opte por Magic Library el cual lo considero adecuado para poder realizarlo en este cuatrimestre.

### Funcionalidades
- Magic library busca llevar un registro de los libros que el usuario ha leído, los que está leyendo actualmente y los que tiene pendientes.
- El usuairo puede establecer cuantos libros quiere leer este año 
- El sistema permitirá establecer metas diarias de lectura para fomentar el hábito de la lectura.
- También se podrán agregar notas o comentarios sobre cada libro para recordar detalles importantes o reflexiones personales.
- Este sistema planea incluir Inteligencia Artificial para recomendar libros basados en las preferencias del usuario y su historial de lectura.
- Además, se planea implementar una función de seguimiento del progreso de lectura, donde el usuario pueda marcar los capítulos o páginas que ha leído y recibir estadísticas sobre su avance.
- El usuario podrá resibir notificaciones para recordar sus metas diarias de lectura o para sugerir nuevos libros basado en sus intereses. 

---

## Decisión

#### MVC 

### ¿Por qué?

Este patrón arquitectónico separa 3 responsabilidades:
- Model: los datos y las reglas de negocio. No sabe que existe una pantalla. 
- View: lo que ve el usuario. No consulta datos por sí misma.
- Controller: Recibe peticiones y es el intermediario entre la vista y el modelo. No genera HTML.

Me convence más este patrón porque es fácil de entender y tiene una gran comunidad de soporte. 

### Atributos de calidad estáticos por el cual elegí este patrón
- **Mantenibilidad**: Al separar las responsabilidades, es más fácil de mantener y actualizar el sistema. Si necesito cambiar la forma en que se muestran los datos, solo tengo que modificar la vista sin afectar el modelo o el controlador.
- **Escalabilidad**: Al tener una estructura clara, es más fácil de escalar el sistema a medida que crece. Puedo agregar nuevas funcionalidades sin afectar las partes existentes.
- **Modularidad**: Cada componente (modelo, vista, controlador) es independiente, lo que facilita la reutilización de código y la colaboración entre desarrolladores.



### Alternativas consideradas


| Alternativa | Por qué la descarté |
|-------------|---------------------|
| MVVM         | la view y viewmodel se sincronizan y no hay controller.                  |
| MVP         | Android nativo; la view es muy “pasiva”, no toma ninguna decisión.                 |
| Hexagonal         | más para sistemas empresariales.                  |



---

## Consecuencias

**✅ Lo que gano:**

- Al separarlo en responsabilidades se vuelve más fácil de escalar y permite que se agreguen más funcionalidades. Cada componente se puede desarrollar, mantener y probar por separado. 
- Al ser un desarrollo individual, adoptar la estructura estricta de carpetas de MVC reduce la carga de decisiones . El patrón dicta donde colocar cada archivo, lo que acelera el flujo de trabajo y mantiene el código organizado.


**⚠️ Lo que sacrifico o asumo:**

- Introduce complejidad: la aplicación requiere más archivos y carpetas lo que puede ocasionar que sea más difícil de entender y seguir. 
- Si el proyecto escala en el futuro y se quiere una aplicación movil nativa, se tendria que contruir una API REST separada, debido a que actualmente la interfaz estaria fuertemente ligada al servidor. 


## Diagrama  - vistas arquitectónicas

### Vista lógica:
#### Se muestran los modulos principales del sistema y sus relaciones.

![Vista lógica]( docs/ArqV1.png )

### Vista de desarrollo:
#### Se muestra como está organizado el código en carpetas y archivos, siguiendo la estructura del patrón MVC.
![Vista de desarrollo]( docs/ArqV2.png )

### Vista de procesos:
#### Se muestra el flujo de procesos y cómo interactúan los diferentes componentes del sistema.
![Vista de procesos]( docs/ArqV3.png )

### Vista de despliegue:

#### La primera imagen se adapta a como estoy implementando el sistema actualmente usando archivos JSON
![Vista de despliegue-v1]( docs/ArqV4-1.png )

#### La segunda imagen se adapta a como planeo implementar a futuro sustituyendo JSON por la base de datos relacional MySQL. 
![Vista de desplieguev2]( docs/ArqV4-2.png )

## Declaración de uso de IA

Declaro que utilicé inteligencia artificial (gemini), para los diagramas solo utilicé para revisar que los diagramas estuvieran correctos y para sugerirme mejoras, ya que no me encontraba convencida y le pedía sugerencias. Con respecto a la redacción de este ADR, utilicé la IA para revisar la redacción y la ortografía del contexto, la IA de visual me proporcionaba opciones de autocompletado para las frases y si me convencían las usaba, si no, las modificaba.

