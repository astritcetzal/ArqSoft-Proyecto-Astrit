# Magic Library - Modelo C4

Esta documentación describe la arquitectura de Magic Library, una plataforma orientada a fomentar hábitos de lectura. El diseño sigue una Arquitectura Hexagonal que separa el dominio del negocio de la infraestructura, permitiendo una evolución tecnológica sostenida.

---

## C4 Nivel 1 - Contexto

**¿Para quién es?** Para cualquier usuario lector interesado en gestionar sus hábitos.

**¿Qué pregunta responde?** ¿Cómo interactúa el usuario con el sistema y sus servicios externos?

```mermaid
C4Context
    title Diagrama de Contexto (Nivel 1) - Magic Library

    Person(lector, "Lector (Usuario)", "Persona que busca registrar sus lecturas, metas y obtener recomendaciones.")
    
    System(magicLibrary, "Magic Library", "Plataforma de gestión de hábitos de lectura.")
    
    System_Ext(geminiIA, "Oráculo de Lectura (IA)", "Servicio externo para recomendaciones de libros.")
    System_Ext(notifSys, "Sistema de Notificaciones", "Servicio encargado de enviar alertas sobre el progreso de las metas.")

    Rel(lector, magicLibrary,  "Gestiona libros y metas", "HTTPS")
    Rel(lector, geminiIA, "Consulta recomendaciones", "Web/Chat")
    Rel(magicLibrary, notifSys, "Notifica avances", "Observer Pattern")

```
