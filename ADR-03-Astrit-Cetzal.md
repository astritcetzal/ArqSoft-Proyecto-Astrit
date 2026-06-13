# ADR-02: Magic library - Definición de Vistas Arquitectónicas bajo el Patrón MVC

| Campo  | Valor |
|--------|-------|
| Autor  | Astrit Cetzal |
| Fecha  | /06/2026 |
| Estado | `Aceptado` |

 Remplazado por ADR-02

---

### Contexto

Estoy construyendo "Magic Library", un sistema desarrollado en C# y .NET Core para que los lectores administren sus libros leídps, en proceso y pendientes. Ademas, el sitema permitirá establecer metas diarias, agregar notas personales, hacer segumientos del progreso y, a futuro, recibir recomendaciones generadas por inteligencia Artificial.

### Condiciones y restricciones

Actualmente la persistencia de datos se implementará de forma rápida utilizando archivos localos JSON. Sin está la posibilidad de migrar a una base de datos relacional como MySQL (en AWS) como segunda fase. El diseño debe permitir este cambio sin reescribir la lógica central del proyecto. 





---

## Decisión

He decidio por adoptar el estilo de Arquitectura Hexagonal (Puerto y adaptadores) para la estructura global del sistema. 


### ¿Por qué?
Este estilo arquitectónico resuelve el problema de la evolución del almacenamiento en Magic Library. Al invertir las dependencias y utilizar "Puertos" (interfaces), el nucleo del sistema (la gestión de libros y metas) queda completamente aislado.

Esta caracteristica concreta me permite conectar el repositorio JSON actual y al momento de querer cambiar a base de datos, solo debo desconectar e inyectar u nuevo repositorio a MySQL sin tener que modificar una sola linea de código de las reglas de negocio o de los controladores. 


### Atributos de calidad estáticos por el cual elegí este patrón




### Alternativas consideradas

| Alternativa | Por qué la descarté |
|-------------|---------------------|
| Microservicios         | Apesar de que ofrece que cada servico escale de manera independiente se sacrifica la complejidad operativa, se tiene que mantener, monitorear y desplegar múltiples servicio, y solo hay que usarlo en sistemas grandes con equipo distintos por área de negocio.                   |
| Capas         | Porque solo hay que usarlo cuando es solo una interfaz o equipo pequeñp y se debe evitar cuando se necesitan multiples usuarios.                 |
| Monolitico         | Mezclar la lógica de negocio directamente dentro de los controladores MVC hace que el código sea dificil de probar                 |




---

## Consecuencias

**✅ Lo que gano:**



**⚠️ Lo que sacrifico o asumo:**



## Trade-off



