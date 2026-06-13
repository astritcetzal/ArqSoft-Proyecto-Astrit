# ADR-02: Magic library - Definición de Vistas Arquitectónicas bajo el Patrón MVC

| Campo  | Valor |
|--------|-------|
| Autor  | Astrit Cetzal |
| Fecha  | 12/06/2026 |
| Estado | `Aceptado` |

 Remplazado por ADR-02

---

### Contexto

Estoy construyendo "Magic Library", un sistema desarrollado en C# y .NET Core para que los lectores administren sus libros leídps, en proceso y pendientes. Ademas, el sitema permitirá establecer metas diarias, agregar notas personales, hacer segumientos del progreso y, a futuro, recibir recomendaciones generadas por inteligencia Artificial.

### Condiciones y restricciones

Actualmente la persistencia de datos se implementará de forma rápida utilizando archivos localos JSON. Sin está la posibilidad de migrar a una base de datos relacional como MySQL (en AWS) como segunda fase. El diseño debe permitir este cambio sin reescribir la lógica central del proyecto. 





---

## Decisión - Arquitectura Hexagonal Multiproyecto

He decidio por adoptar el estilo de Arquitectura Hexagonal (Puerto y adaptadores) para la estructura global del sistema. 


### ¿Por qué?
Este estilo arquitectónico resuelve el problema de la evolución del almacenamiento en Magic Library. Al invertir las dependencias y utilizar "Puertos" (interfaces), el nucleo del sistema (la gestión de libros y metas) queda completamente aislado.

Esta caracteristica concreta me permite conectar el repositorio JSON actual y al momento de querer cambiar a base de datos, solo debo desconectar e inyectar u nuevo repositorio a MySQL sin tener que modificar una sola linea de código de las reglas de negocio o de los controladores. 


### Alternativas consideradas

| Alternativa | Por qué la descarté |
|-------------|---------------------|
| Microservicios         | Apesar de que ofrece que cada servico escale de manera independiente se sacrifica la complejidad operativa, se tiene que mantener, monitorear y desplegar múltiples servicio, y solo hay que usarlo en sistemas grandes con equipo distintos por área de negocio.                   |
| Capas         | Porque solo hay que usarlo cuando es solo una interfaz o equipo pequeñp y se debe evitar cuando se necesitan multiples usuarios.                 |
| Monolitico         | Mezclar la lógica de negocio directamente dentro de los controladores MVC hace que el código sea dificil de probar                 |




---

## Consecuencias

**✅ Lo que gano:**

- Conscuencia técnica: Cuando migre de JSON  a MySQL será un proceso limpio esto gracia a la inyección de dependencias. Lo que permite escalabilidad.
- Consecuencia sobre el proceso: Puedo enfocarme primero en la lógica de negocio y luego realizar las configuraciones dificiles de la base de datos para el final.
- Testeabilidad: al tener el dominio ailado de los archivos JSON y la vista, puedo realizar puebras unitarias ráídas solo para las reglas de mis libros y metas. 
- Mantenibilidad: El aislamiento asegura que cuando migre la infrasturutura de JSON  a MySQL, el riesgo de introducir bugs en la lógica central de Magic Library es casi nulo. 

**⚠️ Lo que sacrifico o asumo:**

- Limitación técnica: se requiere la creación de más archivos para operaciones simples que en otros estilos tomarían un solo archivo.
- Deuda o riesgo: Existe el riesgo de OverKill. Debo mantner los puertos y adaptadores lo más simples posibles para no complicar un CRUD.
- Eficiencia en el desarrollo inicial: La estructura multiproyecto requiere crear más abstracciones, interfaces y configurar más inyecciones de dependencias por lo que a diferencia de otros sistemas que tomaria un minuto, con este se requiere la creación de varios archivos. 
- Rendimiento: El uso excesivo de interfaces y la división de la memoria entre las diferentes capas añade un overhead computacional en comparación con hacer llamadas directas, aunque para este sistema es imperceptible. 






