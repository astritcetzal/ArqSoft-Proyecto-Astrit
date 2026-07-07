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

## C4 Nivel 2 - Contenedores
¿Para quién es? Arquitectos y desarrolladores.

¿Qué pregunta responde? ¿Cuáles son las piezas técnicas grandes y cómo se comunican bajo arquitectura hexagonal?

```mermaid
C4Container
    title Diagrama de Contenedores (Nivel 2) - Magic Library

    Person(lector, "Lector (Usuario)")

    System_Boundary(c1, "Magic Library (Hexagonal)") {
        Container(webApp, "Capa Web (MVC)", "ASP.NET Core 10", "Interfaz de usuario y controladores.")
        Container(appCore, "Capa de Aplicación y Dominio", "C# Class Library", "Lógica de negocio, servicios y puertos (Interfaces).")
        Container(infra, "Capa de Infraestructura", "C# Class Library", "Adaptadores: Repositorios (JSON), Notificadores y Factory.")
        ContainerDb(jsonStore, "Persistencia", "JSON Files", "Base de datos local (libros, usuarios, metas).")
    }

    Rel(lector, webApp, "Accede", "HTTPS")
    Rel(webApp, appCore, "Invoca lógica", "DI")
    Rel(appCore, infra, "Implementa puertos", "Interface")
    Rel(infra, jsonStore, "Lee/Escribe", "File I/O")

```

