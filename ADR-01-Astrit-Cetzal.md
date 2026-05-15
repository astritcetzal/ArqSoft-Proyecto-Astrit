# ADR-01: Magic library - Patrón MVC

| Campo  | Valor |
|--------|-------|
| Autor  | Astrit Cetzal |
| Fecha  | 15/05/2026 |
| Estado | Propuesto |

---

## Contexto

Estoy en proceso de construcción de un sistema que administre los libros leídos, libros en proceso y metas diarias de lectura. La idea de este sistema surge porque muchos lectores (en especial aquellos que solo leen en línea) o estudiantes llegan a olvidad que libros han leído, cuales estás leyendo o cuales dejaron pendientes. 
Principalmente tenía en mente un sistema que administre el dinero que gasto diario, semanal y mensual (y más etc.) sin embargo me convenció más el proyecto de los libros, entonces opté por Magic Library el cual lo considero adecuado para poder realizarlo en este cuatrimestre. 
Para este proyecto usaré visual estudio, trabajaré con el Framework de .net y usaré el patrón MVC y claro el lenguaje principal para mi app es C#.

---

## Decisión

#### MVC 

### ¿Por qué?

Este patrón arquitectónico separa 3 responsabilidades:
- Model: los datos y las reglas de negocio. No sabe que existe una pantalla. 
- View: lo que ve el usuario. No consulta datos por sí misma.
- Controller: Recibe peticiones y es el intermediario entre la vista y el modelo. No genera HTML.

Me convence más este pattrón porque es fácil de entender y tiene una gran comunidad de soporte. 

### Alternativas consideradas

*(Mínimo 3 filas)*

| Alternativa | Por qué la descarté |
|-------------|---------------------|
| MVVM         | la view y viewmodel se sincronizan y no hay controller.                  |
| MVP         | Android nativo; la view es muy “pasiva”, no toma ninguna decisión.                 |
| Hexagonal         | más para sistemas empresariales.                  |

---

## Consecuencias

**✅ Lo que gano:**

Menciona al menos:
- Una consecuencia **técnica** — qué se vuelve más fácil de construir, mantener o escalar en tu sistema
- Una consecuencia sobre el **proceso o el equipo** — cómo afecta la forma en que vas a trabajar
- Al separarlo en responsabilidades se vuelve más fácil de escalar y permite que se agreguen más funcionalidades. Cada componente se puede desarrollar, mantener y probar por separado. 
- Como es un proyecto individual, debo conocer muy bien cuales clases corresponden a cada capa lo que resulta en mucha responsabilidad.



**⚠️ Lo que sacrifico o asumo:**

Menciona al menos:
- Una **limitación técnica** — qué no podrás hacer fácilmente con esta decisión
- Una **deuda o riesgo** — qué podrías tener que resolver más adelante si el proyecto crece
- Introduce complejidad: la aplicación requiere más archivos y carpetas lo que puede ocasionar que sea más difícil de entender y seguir. 
- Puede no adaptarse a todos los escenarios porque algunas aplicaciones pueden tener requisitos diferentes o específicos que no encajan bien con este patrón. 


## Diagrama

Un boceto de cómo se estructura tu sistema (draw.io, Mermaid o a mano escaneado)

![Diagrama del sistema]( ./ruta/diagrama-nivel-1.png )
