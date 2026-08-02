# ADR-06:Documentación final del proyecto

| Campo  | Valor |
|--------|-------|
| Autor  | Astrit Cetzal |
| Fecha  | 01/08/2026 |
| Estado | `Final`  |

---

## 1. Resumen de Desiciones Arquitectónica 

A lo largo del ciclo de desarrollo de Magic Library, la arquitectura ha evolucionado para garantizar la escalabilidad, el mantenimiento y la separación de rsponsabilidades, A continuación se consolidan las decisiones más criticas:

* Evolución del Estilo Arquitectónico: El proteco inició utilizando el patrón MVC tradicional para seárar los datos y reglas de negoio (Model), la interfaz de usuario (View) y el intermediario (Controller). Posteriormente, se evolucionó hacia una Arquitectura Hexagonal Multiproyecto.
Esta decisión invierte las dependenciasd y utiliza "Puertos" (Interfaces) para ailat completamente el núcleo del sistema (la gestión de libros y metas).

* Implementación de API REST: Se implementó una API REST utilizando ASP.NET Core Web Api, documentada con Swagger/OpenApi, permietiendo que la aplicación exponga datos estructurados en formato JSON. Esto facilita que otras aplicaciones odispositivos (como una app móvil) consuman los datos.
* Integración de Patrones GOF: Se implementaron patrones de diseño para evitar el "código espagueti". Se integró factory method para centralizar la creación de repositorios sin acoplar la capa de aplicación, Decorator para auditar y extender la funcionalidad de las consultas de libros, y el patrón Observer para desacoplar el sistema de metas de la lógica de notificaciones.
* Estrategia de Pruebas: Se adoptó el framework cUnit implementando el patrón Arrange-Act-Assert para probar ailadamente los flujos del inventario en BookControllerTest, las reglas de negocio en GoalControllerTest y la configeración personalen UserProfileControllerTests.


## 2. Modelo c4 (Niveles 1 A 3)

Entra al sigueinte enlace par ver los diferentes niveles de C4
➡️ [DIAGRAMAS-C4](diagramas-c4.md)

## 3. Evaluación ATAM (Architecture Tradeoff Analysis Method)

Basado en las decisiones de diseño documentadas en los ADRs, se presenta la siguiente evaluación arquitectónica:

### Riesgo identificado: Concurrencia en la Persistencia de Datos

- Descripción: El uso de archivos JSON como motor de base de datos principal.
- Justificación: Esto genera un riesgo crítico al no soportar operaciones concurrentes. Si dos usuarios intentan guardar su progreso en el mismo milisegundo, el sistema colapsará resultando en excepciones de Acceso Denegado (I/O) o pérdida de datos.
- Mitigación planificada: Desarrollar un nuevo adaptador de infraestructura que implemente las interfaces existentes y migrar la persistencia hacia SQL server.

### Trade-off: Flexibilidad Arquitectónica vs Eficiencia de Desarrollo inicial
- Descripción del Trade-Off: Se priorizó implementar la arquitectura Hexagonal Multiproyecto sobre otros modelos más sensilos como el Monolito tradicional.
- Justificación: Se asume una menor eficiencia en el desarrollo inicial debido a la necesidad de crear más abstracciones, interfaces y configurar más inyecciones de dependencias. A cambio, se gana que al momento de migrar de JSON a SQL Server, el proceso sea limpio y no requiere modificar una sola linea de código de las reglas de negocio.

### Punto de sensibilidad: Centralización mediante Factory Method
- Descripción del punto de sensibilidad: La instanciación de los repositorios de datos a través del patrón Factory Method (AgregarLibroRepository).
- Justificación: Este método es altamente sensible al parámetro del interno inyectadao. Un cambio en esta variable altera por completos cómo y dónde se guarda la información sin que la capa de aplicacion se entere.

## 4. Demo en vivo, Pruebas y Pipeline
➡️ [Demo en vivo](http://magiclibrary-env.eba-sbvbttiq.us-east-1.elasticbeanstalk.com/)

## 5. Declaración de uso de IA 
Declaro el uso de inteligencia artificial de manera asistida durante el desarrollo de este proyecto. La IA fue utilizada como herramienta de apoyo para auditar e código, identificar "Smell code", entender la diferencia entre modelos de arquitectura, corrgir conflictos de código y generar la bse visual de los diagrmas C4. 

Sin embargo, la autoria del código base, la dirección arquitectónica, las decisiones estratpegicas de migración, la lógica implementada y la supervisión o ajuste de cada componente generado son enteramente mi responsabilidad y cración propia, 

