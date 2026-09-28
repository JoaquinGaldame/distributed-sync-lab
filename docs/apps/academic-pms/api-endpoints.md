# API Academic PMS: endpoints y pruebas

Academic PMS es la fuente de verdad interna del laboratorio. Guarda propiedades, incrementa su versión cuando cambia el estado y registra una instantánea de cada versión en `property_changes`.

Esta guía usa `http://localhost:5080`. Iniciá PostgreSQL y la API como indica [la guía de importación](importacion-inside-airbnb.md). En Postman, seleccioná **Body → raw → JSON** para `POST /properties` y `PUT /properties/{id}`.

## Endpoints disponibles

| Método | Ruta | Uso |
|---|---|---|
| `GET` | `/health` | Comprueba que el proceso HTTP responde. No verifica la conexión a PostgreSQL. |
| `POST` | `/properties` | Crea una propiedad en versión `1` y registra su primer cambio. |
| `GET` | `/properties/{id}` | Lee el estado actual de una propiedad. |
| `PUT` | `/properties/{id}` | Reemplaza el estado sincronizable; aumenta la versión solo si hay diferencias. |
| `GET` | `/changes?after=0&limit=50` | Lee cambios ordenados por `sequence`, en lotes. |
| `POST` | `/imports/inside-airbnb` | Carga el CSV elegido; ver [guía de importación](importacion-inside-airbnb.md). |

## Crear una propiedad manual

Usá un ID distinto de los `IA-...` reservados para el dataset:

```http
POST http://localhost:5080/properties
Content-Type: application/json
```

```json
{
  "id": "LAB-TEST-001",
  "sourceListingId": null,
  "name": "Casa de prueba",
  "price": 150,
  "roomType": "ENTIRE_HOME",
  "neighbourhood": "Palermo",
  "latitude": -34.58,
  "longitude": -58.42,
  "minimumNights": 2
}
```

Respuesta esperada: **`201 Created`**, cabecera `Location: /properties/LAB-TEST-001` y un JSON con esos campos, `version: 1` y `updatedAt`. Si el ID ya existe, responde `409 Conflict`; un estado inválido responde `400 Bad Request`.

`sourceListingId` vincula una propiedad importada con su identificador original y no se cambia mediante `PUT`. Para una propiedad manual puede ser `null`. Los valores aceptados de `roomType` son `ENTIRE_HOME`, `PRIVATE_ROOM`, `HOTEL_ROOM` y `SHARED_ROOM`. El precio debe ser no negativo y admitir como máximo dos decimales; `minimumNights` debe ser al menos `1`.

## Consultar y actualizar

```http
GET http://localhost:5080/properties/LAB-TEST-001
```

Respuesta esperada: **`200 OK`** con el estado y su versión. Un ID inexistente devuelve `404 Not Found`.

Para actualizar, enviá el **estado completo**, aunque solo cambie un valor:

```http
PUT http://localhost:5080/properties/LAB-TEST-001
Content-Type: application/json
```

```json
{
  "name": "Casa de prueba",
  "price": 170,
  "roomType": "ENTIRE_HOME",
  "neighbourhood": "Palermo",
  "latitude": -34.58,
  "longitude": -58.42,
  "minimumNights": 2
}
```

Respuesta esperada: **`200 OK`**, `price: 170` y `version: 2`. Si repetís exactamente el mismo `PUT`, permanece en versión `2` y no aparece otro cambio. Un ID inexistente devuelve `404 Not Found`; un estado inválido, `400 Bad Request`; un conflicto de actualización concurrente detectado, `409 Conflict`.

## Consultar cambios por lotes

```http
GET http://localhost:5080/changes?after=0&limit=2
```

La respuesta contiene `items`, `nextAfter` y `hasMore`. Cada item incluye `sequence`, un `event` con ID, propiedad, versión y fecha, y `desiredState` con la instantánea de esa versión. Para leer el lote siguiente, usá el `nextAfter` recibido:

```text
GET /changes?after=<nextAfter anterior>&limit=2
```

Continuá hasta recibir `items: []` y `hasMore: false`. `after` debe ser mayor o igual a `0`; `limit` debe estar entre `1` y `100` (por defecto, `50`). Leer un evento **no confirma** que una OTA esté sincronizada: el futuro Worker conservará su avance después del procesamiento.

> El cursor actual se basa en `sequence`. Antes de usarlo como mecanismo confiable bajo escrituras simultáneas, hay que resolver el posible desfase entre el orden de asignación de secuencias y el orden de confirmación de transacciones.

## Cuidado con el estado experimental

Crear o actualizar propiedades manualmente después de importar Inside Airbnb modifica los conteos y el historial del PMS. Para comprobar el baseline importado, ejecutá primero las verificaciones de la guía de importación; reservá las pruebas de este tutorial para antes de la carga o para una ejecución local que luego reiniciarás de forma controlada.
