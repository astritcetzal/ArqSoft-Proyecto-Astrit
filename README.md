# MAGIC LIBRARY
Un sistema web diseñado para administrar y fomentar el hábito de la lectura. Este proyecto nace de la necesidad de ayudar a los lectores (especialmente a aquellos que leen en línea o en PDF) a no olvidar qué libros han leído, cuáles están leyendo actualmente y cuáles dejaron pendientes


## Funcionalidades Principales
- **Registro de catálogo**: Gestión de libros terminados, en proceso y pendientes.

- **Seguimiento de metas**: Sistema para establecer y monitorear metas diarias de lectura.

- **Notas y reflexiones**: Espacio para agregar reseñas y comentarios personales sobre cada libro.

## Tecnologías y Arquitectura
- **Lenguaje**: C#

- **Framework**: ASP.NET Core

- **Arquitectura**: Patrón MVC (Model-View-Controller) para mantener una separación limpia de responsabilidades.

- **Almacenamiento Actual**: Archivos locales en formato JSON.

- **Entorno de Desarrollo**: Visual Studio

## Estructura del Código


- **Models**: Define las entidades centrales (Libro, Usuario, Meta, Reseña).

- **Views**: Las pantallas con las que interactúa el usuario.

- **Controllers**: Los intermediarios que reciben las peticiones y orquestan la lógica (LibroController, etc.).

- **Infrastructure**: Repositorios encargados de leer y escribir la información en los archivos JSON.

##  Próximos Pasos 

Este proyecto está en fase de crecimiento. Las siguientes mejoras planificadas incluyen: 

- Migración a Base de Datos: Cambiar el almacenamiento de JSON a una base de datos relacional MySQL.

- Despliegue en la Nube: Alojar la aplicación en Amazon Web Services (usando EC2 para la app y RDS para la base de datos).

- Recomendaciones con IA: Integrar una API de Inteligencia Artificial para sugerir lecturas basadas en el historial y gustos del usuario.


### Declaración de uso de IA

Declaro que utlice inteligencia artificial para mejorar la ortografia y redacción. 

















