# ADR-01: Magic library - Patrón MVC

| Campo  | Valor |
|--------|-------|
| Autor  | Astrit Cetzal |
| Fecha  | 15/05/2026 |
| Estado | Propuesto |

---

## Contexto

Estoy en proceso de construcción de un sistema que administre los libros leídos, libros en proceso y metas diarias de lectura. La idea de este sistema surge porque muchos lectores (en especial aquellos que solo leen en línea) o estudiantes llegan a olvidad que libros han leído, cuales estás leyendo o cuales dejaron pendientes. 
Principal mente tenía en mente un sistema que administre el dinero que gasto diario, semanal y mensual (y más etc.) sin embargo debido al tiempo disponible y a otras actividades no era posible porque implicaba más tiempo, entonces opté por Magic Library el cual lo considero adecuado para poder realizarlo en este cuatrimestre. 
Para este proyecto usaré visual estudio, trabajaré con el Framework de .net y usaré el patrón MVC y claro el lenguaje principal para mi app es C#.

---

## Decisión

#### MVC 

### ¿Por qué?

Este patrón arquitectónico separa 3 responsabilidades:
Model: los datos y las reglas de negocio 
View: lo que ve el usuario 
Controller: Recibe peticiones y coordina el método index() que devuelve la lista.

### Alternativas consideradas

*(Mínimo 3 filas)*

| Alternativa | Por qué la descarté |
|-------------|---------------------|
| ...         | ...                 |
| ...         | ...                 |
| ...         | ...                 |

---

## Consecuencias

**✅ Lo que gano:**

Menciona al menos:
- Una consecuencia **técnica** — qué se vuelve más fácil de construir, mantener o escalar en tu sistema
- Una consecuencia sobre el **proceso o el equipo** — cómo afecta la forma en que vas a trabajar

**⚠️ Lo que sacrifico o asumo:**

Menciona al menos:
- Una **limitación técnica** — qué no podrás hacer fácilmente con esta decisión
- Una **deuda o riesgo** — qué podrías tener que resolver más adelante si el proyecto crece

## Diagrama

Un boceto de cómo se estructura tu sistema (draw.io, Mermaid o a mano escaneado)

![Diagrama del sistema]( ./ruta/diagrama-nivel-1.png )
