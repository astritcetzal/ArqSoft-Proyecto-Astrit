# ADR-03: Magic library - Definición de Estilo arquitectónico

| Campo  | Valor |
|--------|-------|
| Autor  | Astrit Cetzal |
| Fecha  | 12/06/2026 |
| Estado | `Aceptado` |

 Remplazado por ADR-02

---

### Contexto

Estoy en proceso de construcción de un sistema que administre los libros leídos, los libros pendientes y los que estan en proceso de lectura. La idea de este sistema surge porque muchos lectores (en especial aquellos que solo leen en linea y descargan PDFs en internet) llegan a olvidar que libros han leido, cuales estapan leyendo o cuales dejaron pendientes. 
Para este proyecto usaré visual estudio, trabajaré con el Framework de .net y usaré el patrón MVC y claro el lenguaje principal para mi app es C#.

### Condiciones y restricciones

Actualmente la persistencia de datos se implementará de forma rápida utilizando archivos locales JSON. Sin embargo está la posibilidad de migrar a una base de datos relacional como MySQL (en AWS) como segunda fase. El diseño debe permitir este cambio sin reescribir la lógica central del proyecto. 

### Funcionalidades
- Magic library busca llevar un registro de los libros que el usuario ha leído, los que está leyendo actualmente y los que tiene pendientes.
- El usuairo puede establecer cuantos libros quiere leer este año 
- El sistema permitirá establecer metas diarias de lectura para fomentar el hábito de la lectura.
- También se podrán agregar notas o comentarios sobre cada libro para recordar detalles importantes o reflexiones personales.
- Este sistema planea incluir Inteligencia Artificial para recomendar libros basados en las preferencias del usuario y su historial de lectura.
- Además, se planea implementar una función de seguimiento del progreso de lectura, donde el usuario pueda marcar los capítulos o páginas que ha leído y recibir estadísticas sobre su avance.
- El usuario podrá resibir notificaciones para recordar sus metas diarias de lectura o para sugerir nuevos libros basado en sus intereses. 




---

## Decisión - Arquitectura Hexagonal Multiproyecto

He decidido por adoptar el estilo de Arquitectura Hexagonal (Puerto y adaptadores) para la estructura global del sistema. 


### ¿Por qué?
Este estilo arquitectónico resuelve el problema de la evolución del almacenamiento en Magic Library. Al invertir las dependencias y utilizar "Puertos" (interfaces), el núcleo del sistema (la gestión de libros y metas) queda completamente aislado.

Esta característica concreta me permite conectar el repositorio JSON actual y al momento de querer cambiar a base de datos, solo debo desconectar e inyectar u nuevo repositorio a MySQL sin tener que modificar una sola linea de código de las reglas de negocio o de los controladores. 


### Alternativas consideradas

| Alternativa | Por qué la descarté |
|-------------|---------------------|
| Microservicios         | A pesar de que ofrece que cada servicio escale de manera independiente se sacrifica la complejidad operativa, se tiene que mantener, monitorear y desplegar múltiples servicio, y solo hay que usarlo en sistemas grandes con equipo distintos por área de negocio.                   |
| Capas         | Porque solo hay que usarlo cuando es solo una interfaz o equipo pequeño y se debe evitar cuando se necesitan multiples usuarios.                 |
| Monolitico         | Mezclar la lógica de negocio directamente dentro de los controladores MVC hace que el código sea difícil de probar.                 |




---

## Consecuencias

**✅ Lo que gano:**

- Consecuencia técnica: Cuando migre de JSON  a MySQL será un proceso limpio esto gracias a la inyección de dependencias. Lo que permite escalabilidad.
- Consecuencia sobre el proceso: Puedo enfocarme primero en la lógica de negocio y luego realizar las configuraciones dificiles de la base de datos para el final.
- Testeabilidad: al tener el dominio aislado de los archivos JSON y la vista, puedo realizar puebras unitarias rápidas solo para las reglas de mis libros y metas. 
- Mantenibilidad: El aislamiento asegura que cuando migre la infrastructura de JSON  a MySQL, el riesgo de introducir bugs en la lógica central de Magic Library es casi nulo. 
- Uso en móviles: gracias a la inyeccción de dependencias puedo agregar un nuevo proyeto. Dicha API será el nuevo adapter de entreda. 

**⚠️ Lo que sacrifico o asumo:**

- Limitación técnica: se requiere la creación de más archivos para operaciones simples que en otros estilos tomarían un solo archivo.
- Deuda o riesgo: Existe el riesgo de OverKill. Debo mantener los puertos y adaptadores lo más simples posibles para no complicar un CRUD.
- Eficiencia en el desarrollo inicial: La estructura multiproyecto requiere crear más abstracciones, interfaces y configurar más inyecciones de dependencias por lo que a diferencia de otros sistemas que tomaria un minuto, con este se requiere la creación de varios archivos. 
- Rendimiento: El uso excesivo de interfaces y la división de la memoria entre las diferentes capas añade un overhead computacional en comparación con hacer llamadas directas, aunque para este sistema es imperceptible. 

## Diagrama

![Diagrama del sistema]( docs/diagrama-hx.png )


### Cláusula de uso de IA

Se declara el uso de inteligencia artificial de manera asistida, por lo que se requirió para validar información, entender mejor la diferencia entre cada uno de los modelos de arquitectura y como apoyo para validar la información escrita y para ayudar a mejorar la ortografia. 



