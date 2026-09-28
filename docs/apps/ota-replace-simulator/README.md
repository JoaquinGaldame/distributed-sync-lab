# OTA Replace Simulator

API .NET 10 que representa un destino externo simulado para estudiar la sincronización por reemplazo completo. Usa exclusivamente `ota_replace_simulator_db`.

No representa una OTA comercial. El PMS y el simulador tienen identidades independientes: `externalId` identifica la propiedad en el simulador y no se supone igual al ID de la propiedad en el PMS. La correspondencia PMS–OTA queda pendiente de modelar en el PMS antes de integrar ambos componentes.

## Estado almacenado

`external_properties` guarda una fila por `externalId`, con nombre, precio, tipo de alojamiento, barrio, coordenadas, noches mínimas y `sourceVersion`. Esta última indica qué versión del PMS terminó aplicada y es un dato de observación del laboratorio; una OTA real no necesariamente almacena esa versión.

El simulador no importa Inside Airbnb ni consulta el PMS. Recibe por HTTP el estado completo de una propiedad.

## Endpoints

| Método | Ruta | Resultado |
|---|---|---|
| `PUT` | `/properties/{externalId}` | Crea o reemplaza todos los campos de la propiedad. Devuelve `200 OK`. |
| `GET` | `/properties/{externalId}` | Devuelve el estado almacenado o `404 Not Found`. |
| `GET` | `/properties?offset=0&limit=100` | Devuelve una página de propiedades con `items`, `nextOffset` y `hasMore`. |

`offset` debe ser no negativo y `limit` debe estar entre 1 y 500. La lectura paginada para comparar resultados debe hacerse cuando ya no haya escrituras en curso.

El `PUT` valida el ID externo, los campos y la versión recibida. La creación o sustitución de la fila es atómica. No rechaza versiones anteriores: prevalece la última solicitud aplicada. Esto permite observar si una estrategia de sincronización entrega estados fuera de orden.

## Ejecución local

Con PostgreSQL del laboratorio en funcionamiento:

```powershell
dotnet ef database update --project apps/ota-replace-simulator
dotnet run --project apps/ota-replace-simulator --urls http://localhost:5081
```

La conexión configurada apunta a `ota_replace_simulator_db` en el puerto local `5433`.

## Verificación del checkpoint

Se compiló el proyecto y se verificaron PUT, GET individual y GET paginado con OTA-TEST-001. Repetir un reemplazo conserva una sola fila. Aplicar después una versión anterior deja almacenada esa versión, según la regla de última solicitud aplicada.
