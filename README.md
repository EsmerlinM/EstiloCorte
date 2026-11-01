# Estilo Corte

## Probar la gestión de citas

La API sirve el frontend en la raíz y expone las operaciones de citas en
`/api/citas`. Para iniciar la aplicación en desarrollo:

```powershell
dotnet run --project EstiloCorte.API --launch-profile http
```

Abre `http://localhost:5165` en el navegador. En la pantalla puedes crear una
cita, consultar una cita por ID, reprogramarla o cancelarla. Usa IDs que ya
existan en la base de datos para el cliente, barbero y servicio.

La API y el frontend comparten el mismo origen, por lo que no se requiere
configuración adicional de CORS. La documentación OpenAPI continúa disponible
en desarrollo.