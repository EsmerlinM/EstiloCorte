# EstiloCorte

Sistema de gestión para una barbería, desarrollado con ASP.NET Core Web API y .NET 10.

## Arquitectura

El proyecto está organizado en cuatro capas:

- **EstiloCorte.API:** controladores y endpoints HTTP.
- **EstiloCorte.Application:** DTOs, interfaces y lógica de aplicación.
- **EstiloCorte.Domain:** entidades principales del sistema.
- **EstiloCorte.Infrastructure:** acceso a datos, Entity Framework Core y repositorios.

## Tecnologías

- C#
- .NET 10
- ASP.NET Core Web API
- Entity Framework Core
- SQLite

## Base de datos local

En desarrollo, la aplicación utiliza SQLite y guarda la base de datos en `EstiloCorte.API/EstiloCorte.db`.

La primera vez que se inicia, EF Core crea el archivo y aplica las migraciones automáticamente. No se cargan datos de ejemplo.

## Módulo de citas

Actualmente incluye estas operaciones:

| Método | Endpoint | Descripción |
|---|---|---|
| POST | `/api/Citas` | Crear una cita |
| GET | `/api/Citas/{id}` | Consultar una cita |
| PATCH | `/api/Citas/{id}/cancelar` | Cancelar una cita |
| PUT | `/api/Citas/{id}/reprogramar` | Reprogramar una cita |

La creación y reprogramación calculan la hora de finalización según la duración del servicio y verifican que el barbero no tenga otra cita en ese horario.

La cancelación es lógica: cambia el estado a `Cancelada`, sin eliminar el registro.

## Probar la gestión de citas

Para iniciar la API y el frontend:

```powershell
dotnet run --project EstiloCorte.API --launch-profile http
```

Abre `http://localhost:5165` en el navegador.

Agrega clientes, barberos y servicios desde la sección de catálogos; aparecerán por nombre en las listas para crear y reprogramar citas.

También puedes consultar una cita por ID y cancelarla. El calendario muestra las citas de la semana y permite abrir sus detalles seleccionándolas.

El horario de atención para las propuestas de disponibilidad es de lunes a sábado, de 9:00 a 18:00.

Cuando un horario choca, la API propone hasta tres alternativas disponibles. La primera comienza 15 minutos después de la cita que causa el conflicto. Se pueden seleccionar desde el mensaje para completar el formulario.

La API y el frontend comparten el mismo origen, por lo que no se requiere configuración adicional de CORS. La documentación OpenAPI continúa disponible en desarrollo.

## Pendientes del desarrollo

- Validar que el cliente y el barbero existan y estén activos.
- Implementar pruebas unitarias y de integración.
- Revisar las validaciones de horarios y días laborables.
- Completar los demás módulos del sistema.
- Verificar el funcionamiento de la aplicación en un entorno compartido.