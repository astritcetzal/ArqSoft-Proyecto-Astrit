# ADR-05: Patrones GOF implementados

| Campo  | Valor |
|--------|-------|
| Autor  | Astrit Cetzal |
| Fecha  | 26/06/2026 |
| Estado | `Aceptado`  |

---

## Contexto

Magic library es una plataforma de gestion de hábitos de lectura. El sistem permte registrar libros, gestionar metas de lectura. El sistema permite registrar libros, gestionar metas de lectura y recibir recomendaciones. 
Actualmente, el proyecto utiliza una Arquitectura Hexagonal, lo que requiere una clara separación de responsabiliddes y desacomplamiento entre las cpas de Domain, Application, Infrastructure y Web.

Debido a que el sistem está en una etapa de evoluvión constante (de JSON a futuras bases de datosm y de vistas estáticas a consultas más dinámicas), enfrento el reto de mantener el código limpio, escalable y siguiendo los principios SOLID. El tiempo de entrea el limitado, por lo que busco soluciones robustas que eviten el "código espagueti" conforme agrego funcionalidades como notificaciones o reportes.


---

## Decisión

>  Patrones de diseño GOF
- Factory Method (Credencial) implementando la creacion de los repositorios de datos.

- Decorator (Estructural): Implementando para extender dinámicamente la funcionalidad de los resultados de búsqueda/consulta de libros.

- Observer (Comportamiento): Para enviar una notificacion cada vez que se agregan libros a la meta.

### ¿Por qué?

- Factory: Para crear objetos sin saber exactamente cual, porque mientras este en desarrollo se van a consultar los datos del JSON y cuando esté en producción se establece por el momento a memoria, pero más adelante lo voy a establecer para una base de datos. Lo elegí porque en mi arquitectura hexagonal necesito que la capa de Application no sepa cómo se instancia el repositorio (si es un JsonBookRepository o en el futuro un SqlBookRepository). El patrón Factory centraliza esta creación, permitiendo que mi sistema sea "agnóstico" a la tecnología de persistencia.
- Decorator: Me permite añadir comportamientos extra sin modificar la clase base Book, respetando el principio de Abierto/Cerrado.
- Observer: Lo elegí para desacoplar el sistema de metas de la lógica de notificaciones. Cuando un usuario marca una meta o agrega un libro, el GoalService no necesita saber cómo se envía el mensaje (WhatsApp, correo, etc.); el Observer notifica a los suscriptores registrados automáticamente.



## Implementacion de GOF

Factory por el momento tengo establecido un archivo para que se guarde en memoria los datos en producción pero más adelante lo cambiare por una base de datos

````
public static IBookRepository AgregarLibroRepository(string entorno, IWebHostEnvironment env)
        {
            return entorno switch
            {
                "Production" => new MemoryBookRepository(),
                _ => new JsonBookRepository(env)
            };
        }
````

Decorator 
La clase LoggingBookRepository actúa como el decorador de infraestructura: implementa la interfaz IBookRepository y recibe otra instancia de la misma interfaz a través de su constructor para envolverla. Esto permite auditar y verificar el estado de disponibilidad de la información en tiempo de ejecución dentro de los siguientes métodos:

```
public List<Book> ObtenerTodos()
public Book? ObtenerPorId(int id)

```

Observer - La principal funcion es que notifique cuando el usario agregue un libro en Metas, implementa de `IGoalObserver`y en `GoalService` tenemos el método para confirmar el libros agregado

### Service
```
public void ConfirmarLibroAgregado(Goal goal)
        {
            //notificar 
            foreach (var observer in _observers)
            {
                observer.OnSavedBook(goal);
            }

        }

````
### Interfaces

````
 public interface IGoalObserver
    {
        void OnSavedBook(Goal goal);
    }
````
### Infrastructure

````
public class EmailObserver: IGoalObserver
    {
        public void OnSavedBook(Goal goal) => Console.WriteLine($"[Email] Haz agregado un nuevo libro a tu meta {goal.IdMeta} - Ahora tienes {goal.LibrosAsignados.Count} libros asignados");
    }

````



### Alternativas consideradas



| Alternativa | Por qué la descarté |
|-------------|---------------------|
| Simple Factory (Solo instanciación)         | Es menos flexible que el Factory Method; el Factory Method permite sub-clases para decidir qué instanciar sin cambiar el cliente.                 |
| Inheritance (Herencia para extender)         | La herencia es rígida y causa una explosión de clases si quiero combinar varias funciones (ej: un libro decorado con 'Alerta de Tiempo' y 'Alerta de Género').                 |
| Service Locator         | Es considerado un anti-patrón en arquitecturas modernas y rompe la inyección de dependencias que ya tengo configurada en .NET.                 |

---

## Consecuencias

**✅ Lo que gano:**

> Técnica: * Escalabilidad: El Factory permite cambiar la base de datos sin tocar la lógica.

- Flexibilidad: El Decorator permite enriquecer los datos de libros sin tocar el núcleo.

- Desacoplamiento: El Observer permite que el sistema de notificaciones sea opcional y fácil de escalar a otros canales (ej: notificaciones push).

> Proceso: Facilita el trabajo en equipo; cada patrón aísla una responsabilidad, permitiendo modificar notificaciones sin tocar la persistencia de datos.

**⚠️ Lo que sacrifico o asumo:**

Menciona al menos:
- Limitación técnica: El uso de Factory y Observer añade una capa de indirección, lo que puede hacer que el flujo de depuración sea más complejo al inicio.

- Riesgo: Un uso excesivo de Observer puede generar "efectos secundarios" difíciles de rastrear si no se documenta bien qué componentes están escuchando a quién.

## Diagrama

### C1

![Diagrama del sistema]( images/C1-actualizado.png)

### C2

![Diagrama del sistema]( images/C2-actualizado.png )

### C3

![Diagrama del sistema]( images/C-3.png )


## Declaración de uso de IA
Declaro el uso de Inteligencia Artifiacial para corregir errores, entender mejor conceptos. 
Lo usé para el CSS pero la lógica es mia y me ayudó para corregir conflictos al momento de correr.

