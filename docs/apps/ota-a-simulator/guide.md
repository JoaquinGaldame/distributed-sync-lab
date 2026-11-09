# OTA A Simulator: inicialización local

Esta guía describe exclusivamente cómo inicializar y ejecutar `ota-a-simulator` en el entorno local. Los comandos se ejecutan desde la raíz del repositorio:

```text
D:\Proyectos\Distributed-Syn-Platform\distributed-sync-lab
```

## API de precios y disponibilidad

La API administra precio e inventario diario mediante dos operaciones integradas:

```text
POST /availability/sync
POST /availability/list
```

`/availability/sync` permite enviar por fecha solamente el precio, solamente la pareja `available_units` y `closed`, o ambos bloques. `closed` admite `0` (`AVAILABLE`), `1` (`CLOSED`) y `2` (`RESERVED`). Una fecha disponible requiere inventario mayor que cero; una fecha cerrada o reservada requiere inventario cero.

`/availability/list` incluye `start_date` y utiliza `days` para delimitar la cantidad de fechas del período consultado. Devuelve solamente los días que tengan precio, disponibilidad o ambos. Si no existe ningún dato en todo el período, responde `409 Conflict` con el error `availability_not_configured`.

Todas las respuestas HTTP utilizan el mismo envelope:

```json
{
  "data": {},
  "warning": "",
  "message": "Operación completada correctamente.",
  "error": null
}
```

Los Rate Plans devuelven `cancellationPolicy` como un objeto con `id`, `code`, `name`, `freeUntilDaysBefore` y `penaltyPercent`. `cancellationPolicyId` se utiliza solamente en los requests de creación y actualización.

Las eliminaciones jerárquicas disponibles son:

```text
DELETE /properties/{propertyId}
DELETE /properties/{propertyId}/units/{unitId}
DELETE /properties/{propertyId}/units/{unitId}/rate-plans/{ratePlanId}
```

## Requisitos previos

- .NET SDK 10.
- Docker Desktop iniciado con contenedores Linux.
- Docker Compose disponible mediante `docker compose`.
- PostgreSQL local accesible a través del puerto `5433`, publicado por Compose.

Verificá las herramientas antes de continuar:

```cmd
dotnet --version
docker --version
docker compose version
docker info
```

`dotnet --version` debe mostrar una versión `10.x`. `docker info` debe finalizar correctamente; si informa que no encuentra `dockerDesktopLinuxEngine`, Docker Desktop todavía no está iniciado o no está usando el motor de contenedores Linux.

## 1. Inicializar el proyecto .NET

> Esta sección documenta la clonación el repositorio: el proyecto y sus referencias ya están versionados.

Para Iniciar el proyecto desde cero:

En un repositorio recién clonado solamente es necesario restaurar y compilar:

```cmd
dotnet restore apps\ota-a-simulator\OtaASimulator.csproj
dotnet build apps\ota-a-simulator\OtaASimulator.csproj
```

La compilación confirma que el proyecto y sus dependencias son válidos, pero no crea la base de datos ni sus tablas.

## 2. Levantar PostgreSQL con Docker Compose

`ota-a-simulator` utiliza la base independiente `ota_a_simulator_db`. El archivo `compose.yaml` levanta una única instancia de PostgreSQL que aloja todas las bases del laboratorio.

Desde la raíz del repositorio, validá Compose y levantá PostgreSQL:

```cmd
docker compose config --quiet
docker compose up -d
docker compose ps
```

Esperá hasta que `sync-lab-postgres` figure como `healthy`. También podés comprobar la disponibilidad con:

```cmd
docker compose exec postgres pg_isready -U postgres
```

En la primera creación de un volumen vacío, PostgreSQL ejecuta `infrastructure\compose\postgres\init-databases.sql`. Ese bootstrap crea:

- el usuario `ota_a_simulator_user`;
- la base `ota_a_simulator_db`;
- la propiedad de la base a nombre de ese usuario.

El bootstrap no crea las tablas ni carga los catálogos de OTA A. Esos dos pasos se realizan explícitamente en la sección siguiente.

Comprobá que la base exista:

```cmd
docker compose exec postgres psql -U postgres -d postgres -tAc "SELECT datname FROM pg_database WHERE datname = 'ota_a_simulator_db';"
```

La salida debe contener `ota_a_simulator_db`.

### Volúmenes creados antes de incorporar OTA A

Los archivos ubicados en `/docker-entrypoint-initdb.d` solo se ejecutan al crear un volumen PostgreSQL vacío. Si el volumen local se creó antes de que OTA A fuera incorporada al bootstrap, la base no aparecerá aunque se reinicie el contenedor.

En un entorno local descartable puede reinicializarse todo el PostgreSQL del laboratorio:

```cmd
docker compose down --volumes --remove-orphans
docker compose up -d
docker compose ps
```

> Advertencia: `docker compose down --volumes` elimina todas las bases y datos del PostgreSQL local, incluidos Academic PMS, los simuladores y el historial de migraciones. No lo uses si necesitás conservar esos datos.

## 3. Crear las tablas e inicializar los datos de OTA A

Ejecutá esta sección únicamente después de que PostgreSQL esté iniciado y `ota_a_simulator_db` exista.

Los scripts no son migraciones incrementales ni son idempotentes. En una base nueva deben aplicarse una sola vez y en este orden:

1. `schema.sql` crea tablas, restricciones, funciones, triggers e índices.
2. `seed.sql` carga los catálogos iniciales y sincroniza sus secuencias.

### 3.1. Aplicar el esquema

Desde CMD de Windows:

```cmd
docker compose exec -T postgres psql -U ota_a_simulator_user -d ota_a_simulator_db -v ON_ERROR_STOP=1 --single-transaction < infrastructure\compose\postgres\ota-a\schema.sql
```

Desde PowerShell, invocá la redirección mediante CMD:

```powershell
cmd /c "docker compose exec -T postgres psql -U ota_a_simulator_user -d ota_a_simulator_db -v ON_ERROR_STOP=1 --single-transaction < infrastructure\compose\postgres\ota-a\schema.sql"
```

`ON_ERROR_STOP=1` interrumpe la ejecución ante el primer error. `--single-transaction` evita conservar un esquema parcialmente aplicado si alguna sentencia falla.

Verificá las tablas:

```cmd
docker compose exec postgres psql -U ota_a_simulator_user -d ota_a_simulator_db -c "\dt"
```

Entre otras, deben aparecer `properties`, `units`, `rate_plans`, `unit_inventory` y `rate_calendar`.

### 3.2. Cargar los catálogos iniciales

Desde CMD de Windows:

```cmd
docker compose exec -T postgres psql -U ota_a_simulator_user -d ota_a_simulator_db -v ON_ERROR_STOP=1 --single-transaction < infrastructure\compose\postgres\ota-a\seed.sql
```

Desde PowerShell:

```powershell
cmd /c "docker compose exec -T postgres psql -U ota_a_simulator_user -d ota_a_simulator_db -v ON_ERROR_STOP=1 --single-transaction < infrastructure\compose\postgres\ota-a\seed.sql"
```

Comprobá que los catálogos tengan datos:

```cmd
docker compose exec postgres psql -U ota_a_simulator_user -d ota_a_simulator_db -c "SELECT count(*) AS property_types FROM property_types;"
docker compose exec postgres psql -U ota_a_simulator_user -d ota_a_simulator_db -c "SELECT count(*) AS facilities FROM facilities;"
docker compose exec postgres psql -U ota_a_simulator_user -d ota_a_simulator_db -c "SELECT count(*) AS cancellation_policies FROM cancellation_policies;"
```

Si las tablas ya existen o los catálogos ya contienen registros, no vuelvas a aplicar estos scripts. Una ejecución posterior fallará por objetos o claves duplicadas.

## 4. Ejecutar OTA A Simulator
**Path raíz:** D:\Proyectos\Distributed-Syn-Platform\distributed-sync-lab>

Compilar proyecto desde path raíz:
```cmd
dotnet build apps\ota-a-simulator\OtaASimulator.csproj   
```

La configuración local utiliza esta conexión:

```text
Host=localhost;Port=5433;Database=ota_a_simulator_db;Username=ota_a_simulator_user
```

Iniciá la API desde path raíz:

```cmd
dotnet run --project apps\ota-a-simulator\OtaASimulator.csproj --urls http://localhost:5160
```

Sin cerrar esa terminal, abrí otra y consultá un catálogo:

```cmd
curl.exe http://localhost:5160/catalogs/property-types
```

Para detener la API, presioná `Ctrl + C` en la terminal donde se está ejecutando.

## 5. Arranques posteriores

Después de completar la inicialización una vez, no es necesario volver a crear el esquema ni recargar los seeds. Para retomar el entorno local alcanza con:

```cmd
docker compose up -d
docker compose ps
dotnet run --project apps\ota-a-simulator\OtaASimulator.csproj --urls http://localhost:5160
```

El volumen `postgres-data` conserva `ota_a_simulator_db` cuando se detienen o recrean los contenedores sin utilizar la opción `--volumes`.
