
# EstiloCorte

Sistema de gestión para una barbería, desarrollado con ASP.NET Core Web API y .NET 10.

## Arquitectura

El proyecto está organizado en cuatro capas:

- **EstiloCorte.API:** controladores y endpoints HTTP.
- **EstiloCorte.Application:** DTOs, interfaces y lógica de aplicación.
- **EstiloCorte.Domain:** entidades principales del sistema.
- **EstiloCorte.Infrastructure:** acceso a datos, Entity Framework Core y repositorios.

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

## Tecnologías

- C#
- .NET 10
- ASP.NET Core Web API
- Entity Framework Core 10
- SQL Server

## Configuración pendiente

1. Configurar una instancia de SQL Server.
2. Establecer la cadena de conexión `DefaultConnection` en `appsettings.json` o mediante configuración local segura.
3. Crear y aplicar las migraciones de Entity Framework Core.
4. Insertar datos iniciales para clientes, barberos y servicios.

## Pendientes del desarrollo

- Validar que el cliente y el barbero existan y estén activos.
- Validar horarios y días laborables de la barbería.
- Implementar pruebas unitarias y de integración.
- Completar los demás módulos del sistema.
- Probar los endpoints con una base de datos real.

## Estado actual

El módulo inicial de gestión de citas está implementado y la solución compila. Todavía no se ha validado su funcionamiento contra una instancia real de SQL Server.
