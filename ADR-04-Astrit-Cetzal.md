# ADR-04: Magic library - API REST

| Campo  | Valor |
|--------|-------|
| Autor  | Astrit Cetzal |
| Fecha  | 19/06/2026 |
| Estado | `Aceptado` |

 Remplazado por ADR-02

---

### Contexto

El sistema requiere comunicación externa para permitir que otras aplicaciones o dispositivos (como una App movel o frontend) consuman los datos de Magic Library. Actualmente el acceso a datos es lcal (archivos JSON), lo cual es insuficiente para un entorno de producción donde se requiere concurrencia y acceso remoto.


---

## Decisión 
Implementar una API REST utilizando ASP.Net Core Web Api y documentarla con Swagger/OpenApi

### ¿Por qué?
1. Estandar de la industria: REST es el lenguaje universal de la web.
2. Desacoplamiento: Permite que el frontend y el backend evoluciones de forma independiente.
3. Swagger: Permite documentar automáticamente los endpoints (Books y Recommendations), facilitando la validación del sistema por terceros.


### Alternativas consideradas

| Alternativa | Por qué la descarté |
|-------------|---------------------|
| gRCP         | Es excelente para comunicación interna entre servicios, pero es difícil de probar y consumir desde navegadores (requiere librerías adicionales).                   |
| Capas         | Ofrece flexibilidad, pero para este sistema de lectura, REST cubre las necesidades con mucha menos complejidad de configuración.                  |
| Monolitico         | Son tecnologías obsoletas y demasiado pesadas/verbosas para un proyecto moderno en .NET. MVC hace que el código sea difícil de probar.                 |




---

## Consecuencias

**✅ Lo que gano:**

- Técnica: La API expone los datos de maner estructurada (JSON), haciendo que la migración a una base de datos sea invisible para el cliente.
- Proceso: La documentación automática con swagger que permite probar la API al instante sin tener que crear un frontend complejo para validar los endpoints.


**⚠️ Lo que sacrifico o asumo:**

- Seguridad: Al abrir endpoints, asumo la responsabilidad de implementar autenticación (JWT) en futuras entregas, ya que actaulmente están abiertos.
- Latencia: Al pasar de lectura de archivos locales a peticiones HTTP, el tiempo de respuesta aumenta ligeramente debido al stack de red. 

### Estrategia de persistencia y produccción 
- Acceso a datos en producción: Migraré de archivos JSON  a una Base de datos NoSQL (como MongoDB o DynamoDB).
- ¿Por qué NoSQL? A pesar de que mis datos son estrcutrados, la necesidad de almacenar imagenes (portadas de libros) junto con la metadata hace que un eaquema flexible sea más eficiente que que un modelo relacional estricto, facilitando la escalabilidad del sistema sin migración complejos.

## Endpoints 
![diagrama ]( docs/todos.png )
### Todos los libros
![diagrama ]( docs/book-1.png)
![diagrama ]( docs/book-2.png )
### Libro por ID
![diagrama ]( docs/book-id-1.png )
![diagrama ]( docs/book-id-2.png )
### Todas las recomendaciones
![diagrama ]( docs/recommendation.png )
![diagrama ]( docs/recommendation-2.png )
### Libro recomendado por ID
![diagrama ]( docs/recommendation-id-1.png )
![diagrama ]( docs/recommendation-id-2.png )
### Filtrar libro por género
![diagrama ]( docs/recommendation-filtro-1.png )
![diagrama ](docs/recommendation-filtro-2.png )


## Diagrama

![Diagrama del sistema]( docs/hexagonal.png )


### Cláusula de uso de IA

Se declara el uso de inteligencia artificial como herramienta de apoyo para el diseño de la arquitectura de la API en la correción de errores y la generacion de datos de pruba para las recomendaciones y para esta se basó de mis libros registrados en "libro.json".


