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





