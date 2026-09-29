# Importar Inside Airbnb en Academic PMS

Esta guía carga los snapshots `listings.csv` y `calendar.csv` elegidos para el laboratorio en la base fuente compartida `rental_management`. La importación se realiza mediante la **API Academic PMS**; el lector, la normalización y la escritura pertenecen al mismo proyecto .NET 10.

Ejecutá los comandos desde la raíz de `distributed-sync-lab` en Windows. El archivo fuente debe estar en:

```text
data/inside-airbnb/listings.csv
data/inside-airbnb/calendar.csv
```

Los archivos seleccionados tienen SHA-256:

```text
845472c69e282ba85caf651c918701f0ae81b86e123b238f1eec8ef176e28a00
610564cebd7c2224006ebc7d4d090c8949aa3fd1c299e41134113050a2ef5d38
```

> La fecha y la URL exacta de obtención del snapshot aún deben documentarse. El hash identifica los bytes utilizados y evita confundirlos con otra descarga de igual nombre.

## 1. Preparar PostgreSQL y la API

Iniciá Docker Desktop y comprobá el contenedor:

```cmd
docker compose up -d
docker compose ps
```

`sync-lab-postgres` debe figurar como `healthy`. Configurá la conexión a `rental_management` con el usuario y la contraseña definidos para tu instalación local. Por ejemplo, en **CMD**:

```cmd
set "ConnectionStrings__PmsDb=Host=localhost;Port=5433;Database=rental_management;Username=rental_management_user;Password=<TU_PASSWORD>"
```

En **PowerShell**:

```powershell
$env:ConnectionStrings__PmsDb = 'Host=localhost;Port=5433;Database=rental_management;Username=rental_management_user;Password=<TU_PASSWORD>'
```

Aplicá las migraciones ya versionadas e iniciá la API **en esa misma terminal**:

```cmd
dotnet ef database update --project apps\academic-pms
dotnet run --project apps\academic-pms --urls http://localhost:5080
```

No es necesario crear una migración nueva para ejecutar esta guía.

## 2. Preparar una carga inicial limpia

El endpoint de propiedades requiere que `properties`, `property_changes` y `calendar_days` estén vacías. Si conservás datos de pruebas anteriores, la solicitud responderá `409 Conflict`. Para preparar una carga nueva, desde otra terminal:

```cmd
docker compose exec postgres psql -U postgres -d rental_management -c "TRUNCATE TABLE calendar_days, property_changes, properties RESTART IDENTITY;"
```

Este comando borra el calendario, las propiedades y sus cambios en el PMS. Conserva las migraciones y las otras bases. Ejecutalo únicamente cuando no necesites esos datos de prueba. **No uses `docker compose down -v`** para preparar esta carga.

## 3. Enviar listings.csv

Desde otra terminal ubicada en la raíz del repositorio:

```cmd
curl.exe -X POST -F "file=@data\inside-airbnb\listings.csv" http://localhost:5080/imports/inside-airbnb
```

En Postman, la solicitud equivalente es:

| Opción | Valor |
|---|---|
| Método | `POST` |
| URL | `http://localhost:5080/imports/inside-airbnb` |
| Body | `form-data` |
| Clave | `file`, tipo **File** |
| Valor | Seleccionar `listings.csv` |

No envíes el CSV como `raw` JSON. Si el archivo no coincide con el hash elegido o contiene datos inválidos, la API responde `400 Bad Request`; si el PMS ya contiene propiedades o cambios, responde `409 Conflict`.

Una importación exitosa responde `200 OK` con los siguientes conteos:

```json
{
  "sourceSha256": "845472c69e282ba85caf651c918701f0ae81b86e123b238f1eec8ef176e28a00",
  "originalRows": 29685,
  "excludedMissingPrice": 1792,
  "excludedMissingMinimumNights": 3,
  "importedProperties": 27890,
  "importedAt": "<fecha y hora UTC de esta importación>"
}
```

Las exclusiones son reglas explícitas del importador. Cada alojamiento incorporado tiene versión `1` y un registro correspondiente en `property_changes`. Una falla durante la escritura revierte toda la carga.

Los IDs importados conservan el valor original de Inside Airbnb. Por ejemplo, el alojamiento fuente `54019695` se almacena como `properties.id = '54019695'` y `source_listing_id = '54019695'`.

## 4. Enviar calendar.csv

Después de importar las propiedades, enviá el calendario:

```cmd
curl.exe -X POST -F "file=@data\inside-airbnb\calendar.csv" http://localhost:5080/imports/inside-airbnb/calendar
```

El archivo se valida y se procesa secuencialmente. Las filas de los 1.795 alojamientos excluidos durante la importación de listings no se guardan. La escritura usa una transacción única: ante un error no quedan días parcialmente importados.

Una importación exitosa responde `200 OK`:

```json
{
  "sourceSha256": "610564cebd7c2224006ebc7d4d090c8949aa3fd1c299e41134113050a2ef5d38",
  "originalRows": 10835026,
  "importedCalendarDays": 10179851,
  "excludedUnknownProperties": 655175,
  "propertiesWithCalendar": 27890,
  "minimumDate": "2026-06-29",
  "maximumDate": "2027-07-01",
  "importedAt": "<fecha y hora UTC de esta importación>"
}
```

Una segunda importación del calendario responde `409 Conflict`. El archivo no contiene precios diarios; `properties.price` conserva el precio general disponible en `listings.csv`.

## 5. Verificar en Docker y por HTTP

Comprobá los conteos directamente en PostgreSQL:

```cmd
docker compose exec postgres psql -U postgres -d rental_management -c "SELECT (SELECT count(*) FROM properties) AS properties, (SELECT count(*) FROM property_changes) AS changes, (SELECT count(*) FROM calendar_days) AS calendar_days;"
```

`properties` y `changes` deben valer **27890**; `calendar_days`, **10179851**. Comprobá un alojamiento, su cambio y su calendario:

```cmd
docker compose exec postgres psql -U postgres -d rental_management -c "SELECT id, source_listing_id, name, price, room_type, minimum_nights, version FROM properties WHERE id = '54019695';"
docker compose exec postgres psql -U postgres -d rental_management -c "SELECT property_id, version, state_price, state_room_type, state_minimum_nights FROM property_changes WHERE property_id = '54019695';"
docker compose exec postgres psql -U postgres -d rental_management -c "SELECT * FROM calendar_days WHERE property_id = '54019695' ORDER BY date LIMIT 5;"
```

El alojamiento debe existir con versión `1`, `room_type = ENTIRE_HOME` y `minimum_nights = 14`; su cambio debe conservar esos mismos valores. También podés consultar:

```text
GET http://localhost:5080/properties/54019695
GET http://localhost:5080/changes?after=0&limit=2
```

## Repetir la importación

Una segunda solicitud sobre la misma base responde `409 Conflict`: los importadores exigen una carga inicial limpia para no mezclar estados experimentales. Para repetirla, vaciá las tres tablas del PMS con el comando del paso 2 e importá primero `listings.csv` y después `calendar.csv`. Los `eventId` e `importedAt` se generan nuevamente en cada ejecución.
