# Importar Inside Airbnb en Academic PMS

Esta guía carga el snapshot `listings.csv` elegido para el laboratorio en la base fuente compartida `rental_management`. La importación se realiza mediante la **API Academic PMS**; el lector, la normalización y la escritura pertenecen al mismo proyecto .NET 10.

Ejecutá los comandos desde la raíz de `distributed-sync-lab` en Windows. El archivo fuente debe estar en:

```text
data/inside-airbnb/listings.csv
```

El archivo seleccionado tiene SHA-256:

```text
845472c69e282ba85caf651c918701f0ae81b86e123b238f1eec8ef176e28a00
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

El endpoint de importación requiere que `properties` y `property_changes` estén vacías. Si conservás propiedades de pruebas anteriores, la solicitud responderá `409 Conflict`. Para eliminar únicamente los datos importados de la fuente compartida, desde otra terminal:

```cmd
docker compose exec postgres psql -U postgres -d rental_management -c "TRUNCATE TABLE property_changes, properties RESTART IDENTITY;"
```

Este comando borra todas las propiedades y sus cambios en el PMS. Conserva las migraciones y las otras bases. Ejecutalo únicamente cuando no necesites esos datos de prueba. **No uses `docker compose down -v`** para preparar esta carga.

## 3. Enviar el CSV

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

## 4. Verificar en Docker y por HTTP

Comprobá los conteos directamente en PostgreSQL:

```cmd
docker compose exec postgres psql -U postgres -d rental_management -c "SELECT (SELECT count(*) FROM properties) AS properties, (SELECT count(*) FROM property_changes) AS changes;"
```

Ambas columnas deben valer **27890**. Comprobá un alojamiento y su cambio:

```cmd
docker compose exec postgres psql -U postgres -d rental_management -c "SELECT id, source_listing_id, name, price, room_type, minimum_nights, version FROM properties WHERE id = 'IA-54019695';"
docker compose exec postgres psql -U postgres -d rental_management -c "SELECT property_id, version, state_price, state_room_type, state_minimum_nights FROM property_changes WHERE property_id = 'IA-54019695';"
```

El alojamiento debe existir con versión `1`, `room_type = ENTIRE_HOME` y `minimum_nights = 14`; su cambio debe conservar esos mismos valores. También podés consultar:

```text
GET http://localhost:5080/properties/IA-54019695
GET http://localhost:5080/changes?after=0&limit=2
```

## Repetir la importación

Una segunda solicitud sobre la misma base responde `409 Conflict`: el importador exige una carga inicial limpia para no mezclar estados experimentales. Para repetirla, vaciá **solo las dos tablas del PMS** con el comando del paso 2 y volvé a enviar el mismo archivo. El estado y el orden inicial de propiedades serán reproducibles; los `eventId` y `importedAt` se generan nuevamente en cada ejecución.
