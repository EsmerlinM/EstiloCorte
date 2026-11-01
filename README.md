# Estilo Corte

## Base de datos local

En desarrollo, la aplicación usa SQLite y guarda la base en
`EstiloCorte.API/EstiloCorte.db`. La primera vez que se inicia, EF Core crea el
archivo y aplica las migraciones automáticamente. No se cargan datos de ejemplo.

## Probar la gestión de citas

Para iniciar la API y el frontend:

```powershell
dotnet run --project EstiloCorte.API --launch-profile http
```

Abre `http://localhost:5165` en el navegador. Agrega clientes, barberos y
servicios desde la sección de catálogos; aparecerán por nombre en las listas
para crear y reprogramar citas. También puedes consultar una cita por ID y
cancelarla. El calendario muestra las citas de la semana y permite abrir sus
detalles seleccionándolas.

El horario de atención para las propuestas de disponibilidad es de lunes a
sábado, de 9:00 a 18:00. Cuando un horario choca, la API propone hasta tres
alternativas disponibles: la primera comienza 15 minutos después de la cita
que causa el conflicto. Se pueden seleccionar desde el mensaje para completar
el formulario.

La API y el frontend comparten el mismo origen, por lo que no se requiere
configuración adicional de CORS. La documentación OpenAPI continúa disponible
en desarrollo.