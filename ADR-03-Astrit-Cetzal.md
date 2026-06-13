# ADR-02: Magic library - Definición de Vistas Arquitectónicas bajo el Patrón MVC

| Campo  | Valor |
|--------|-------|
| Autor  | Astrit Cetzal |
| Fecha  | /06/2026 |
| Estado | `Aceptado` |

 Remplazado por ADR-02

---

### Contexto

Estoy construyendo "Magic Library", un sistema desarrollado en C# y .NET Core para que los lectores administren sus libros leídps, en proceso y pendientes. Ademas, el sitema permitirá establecer metas diarias, agregar notas personales, hacer segumientos del progreso y, a futuro, recibir recomendaciones generadas por inteligencia Artificial.

### Condiciones y restricciones

Actualmente la persistencia de datos se implementará de forma rápida utilizando archivos localos JSON. Sin está la posibilidad de migrar a una base de datos relacional como MySQL (en AWS) como segunda fase. El diseño debe permitir este cambio sin reescribir la lógica central del proyecto. 





---

## Decisión




### ¿Por qué?
Como estilo principal tenia en mente el Modelo vista controlador


### Atributos de calidad estáticos por el cual elegí este patrón


### Alternativas consideradas





---

## Consecuencias

**✅ Lo que gano:**



**⚠️ Lo que sacrifico o asumo:**



#



