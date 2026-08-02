
<div align="center">
  <img src="docs/images/perfil.png" alt="Magic Library Logo" width="250"  alt="Centrada"/>

  
  # Magic Library
  
  **Transformando la lectura ocasional en un hábito constante.**
  
  ![C#](https://img.shields.io/badge/c%23-%23239120.svg?style=for-the-badge&logo=csharp&logoColor=white)
  ![.NET](https://img.shields.io/badge/.NET-5C2D91?style=for-the-badge&logo=.net&logoColor=white)
  ![Microsoft SQL Server](https://img.shields.io/badge/Microsoft%20SQL%20Server-CC2927?style=for-the-badge&logo=microsoft%20sql%20server&logoColor=white)
  ![AWS Elastic Beanstalk](https://img.shields.io/badge/AWS%20Elastic%20Beanstalk-%23FF9900.svg?style=for-the-badge&logo=amazon-aws&logoColor=white)
  ![AWS RDS](https://img.shields.io/badge/AWS%20RDS-%23527FFF.svg?style=for-the-badge&logo=amazon-aws&logoColor=white)
  ![Bootstrap](https://img.shields.io/badge/bootstrap-%238511FA.svg?style=for-the-badge&logo=bootstrap&logoColor=white)
</div>

---

## ¿Qué es Magic Library?

Magic Library no es solo un inventario de libros; es una **plataforma web gamificada** diseñada para ayudar a las personas a desarrollar, mantener y disfrutar el hábito de la lectura a largo plazo. 

A través del establecimiento de metas medibles, recordatorios inteligentes y recomendaciones potenciadas por Inteligencia Artificial, el sistema acompaña al lector para asegurar que los libros pendientes se conviertan en metas cumplidas, evitando que las lecturas digitales se pierdan en el olvido.

---

## Funcionalidades Principales

*    *Motor de Metas de Lectura:* Permite a los usuarios configurar objetivos anuales, calculando automáticamente las páginas diarias necesarias para alcanzar la meta y visualizando el progreso en tiempo real.
*    *Gestión de Inventario:* Control total sobre el catálogo personal con estados dinámicos (Terminados, En Proceso y Pendientes).
*    *Recomendaciones Inteligentes (IA):* Sugerencias literarias personalizadas consumiendo APIs externas de Modelos de Lenguaje Grandes (LLMs), basadas en el perfil de lector y géneros favoritos.
*    *Sistema de Notificaciones Asíncronas:* Alertas automatizadas ejecutadas mediante Background Services (`IHostedService`) para mantener al usuario enfocado en sus metas semanales.

---

## Infraestructura y Despliegue en AWS (Producción)

El sistema ha sido diseñado para operar en la nube, garantizando alta disponibilidad y persistencia segura de los datos. La arquitectura en producción está alojada íntegramente en **Amazon Web Services (AWS)** utilizando los siguientes servicios:

### 1. AWS Elastic Beanstalk (Web Server)
*   **Implementación:** Actúa como el entorno de alojamiento PaaS (Platform as a Service) para la aplicación ASP.NET Core MVC.
*   **Configuración:** Se aprovisionó un entorno Linux/Windows optimizado para el runtime de .NET. Las credenciales sensibles (como API Keys de Gemini/Groq y credenciales SMTP) no están en el código fuente, sino que se inyectan dinámicamente mediante las **Propiedades del Entorno (Environment Variables)** de Elastic Beanstalk.

### 2. AWS RDS (Relational Database Service)
*   **Implementación:** Motor de base de datos relacional **Microsoft SQL Server** en la nube, sustituyendo el almacenamiento local en JSON.
*   **Configuración y Seguridad:** La instancia fue configurada con un *Endpoint* público seguro para permitir la gestión desde herramientas locales (SQL Server Management Studio / Visual Studio), protegiendo el acceso a través del Grupo de Seguridad (Security Group) de la VPC y aplicando reglas de entrada específicas (Inbound Rules) por puerto y protocolo.

### 3. Tolerancia a Fallos (Resiliencia)
*   **Patrón Fallback:** La integración con las APIs de IA externas está protegida. Si el servicio de AWS detecta un *Timeout* o una falla de red al solicitar una recomendación, el orquestador degrada el servicio suavemente entregando contenido de contingencia predeterminado, evitando que la aplicación web colapse (Pantalla de error 500).

---

## Arquitectura de Software

El sistema está construido bajo los principios de **Clean Architecture (Arquitectura Hexagonal)**, asegurando un desacoplamiento total entre las reglas de negocio, la interfaz de usuario y la persistencia de datos.

### 1. Capas del Sistema
*   **Domain & Application (Núcleo):** Contiene las entidades, la lógica de cálculo de metas y las interfaces (Puertos) como `IBookRepository` o `IGoalRepository`.
*   **Infrastructure (Adaptadores):** Implementa las interfaces mediante **Entity Framework Core** (`BookRepositoryEf`, `GoalRepositoryEf`) para interactuar con AWS RDS.
*   **Web (MVC):** Capa de presentación y Controladores que gestionan las peticiones HTTP y las sesiones de usuario.

### 2. Patrones de Diseño (GoF) Implementados
*   **Decorator (`LoggingBookRepository`):** Audita y extiende dinámicamente las consultas al inventario (ej. verificando la disponibilidad de la información en tiempo de ejecución) sin alterar la lógica de la clase base.
*   **Observer (`EmailObserver`):** Desacopla el motor de metas del sistema de envío de notificaciones. Cuando un libro se agrega a una meta, el observador reacciona y dispara el aviso sin bloquear el hilo principal.
*   **Factory Method:** Centraliza la instanciación de repositorios para mantener la capa de Aplicación agnóstica a la tecnología de persistencia.

---

## Documentación y Pruebas

*   **Modelos C4 y ADRs:** El proyecto cuenta con documentación técnica detallada, registrando la evolución de la arquitectura, compensaciones (Trade-offs) y análisis de riesgos (ATAM).
    *   ➡️ [Haz clic aquí para ver los Diagramas C4 y ADRs](docs/diagramas-c4.md)
*   **Pruebas Unitarias:** Integración de pruebas automatizadas con **xUnit** utilizando el marco **Arrange-Act-Assert**, enfocadas en validar la integridad de las reglas de negocio en los controladores principales (`BookController`, `GoalController`).

---

## Declaración de Uso de IA

Se declara el uso de Inteligencia Artificial como herramienta de apoyo analítico durante el desarrollo de este proyecto. Fue empleada para:
1.  Asistir en la refactorización y limpieza de código, identificando proactivamente "Code Smells" (Long Method, Feature Envy).
2.  Estructurar de manera semántica la redacción y mejorar la claridad de la documentación técnica.
3.  Generar la base visual (sintaxis Mermaid) para los diagramas C4.

La arquitectura, el código central, el modelado de la base de datos, el despliegue en AWS y las decisiones estructurales del dominio son de total autoría y responsabilidad de la desarrolladora.