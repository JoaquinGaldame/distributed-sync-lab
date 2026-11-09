# OTA B Simulator: guía local

OTA B es una API .NET 10 independiente. Usa PostgreSQL 17, la base
`ota_b_simulator_db`, el usuario `ota_b_simulator_user` y el puerto HTTP `5161`.
No consulta las bases de OTA A ni del PMS.

## Inicializar schema y seed después de levantar Docker Compose

En una instalación nueva, `infrastructure/compose/postgres/init-databases.sql`
crea el usuario y la base al inicializar por primera vez el volumen de Compose.
Ese bootstrap no vuelve a ejecutarse sobre un volumen existente y no se debe borrar
el volumen para forzarlo.

Una vez que PostgreSQL esté levantado, situarse en la raíz del repositorio y
comprobar que el servicio esté saludable y que la base de OTA B exista:

```powershell
docker compose ps
docker compose exec postgres psql -U postgres -d postgres -tAc "SELECT datname FROM pg_database WHERE datname = 'ota_b_simulator_db';"
```

La consulta debe devolver `ota_b_simulator_db`. A continuación, aplicar primero el
schema y después el seed:

Desde PowerShell, usar `cmd /c` para redirigir cada archivo SQL a la entrada
estándar de `psql`:

```powershell
cmd /c "docker compose exec -T postgres psql -U ota_b_simulator_user -d ota_b_simulator_db -v ON_ERROR_STOP=1 --single-transaction < infrastructure\compose\postgres\ota-b\schema.sql"

cmd /c "docker compose exec -T postgres psql -U ota_b_simulator_user -d ota_b_simulator_db -v ON_ERROR_STOP=1 --single-transaction < infrastructure\compose\postgres\ota-b\seed.sql"
```

Aunque los comandos se ejecutan desde PowerShell, `cmd /c` es necesario para que
el operador `<` envíe el contenido del archivo directamente a `psql`. La opción
`-T` evita asignar una terminal interactiva dentro del contenedor.

`ON_ERROR_STOP=1` detiene `psql` ante el primer error y `--single-transaction` evita
dejar el schema o el seed parcialmente aplicados.

Verificar que las tablas y los catálogos principales hayan sido creados:

```powershell
docker compose exec postgres psql -U ota_b_simulator_user -d ota_b_simulator_db -c "\dt"
docker compose exec postgres psql -U ota_b_simulator_user -d ota_b_simulator_db -c "SELECT count(*) AS currencies FROM currencies;"
docker compose exec postgres psql -U ota_b_simulator_user -d ota_b_simulator_db -c "SELECT count(*) AS amenities FROM amenities;"
docker compose exec postgres psql -U ota_b_simulator_user -d ota_b_simulator_db -c "SELECT count(*) AS availability_states FROM availability_states;"
```

Los scripts están destinados a una base B vacía, no son idempotentes ni migraciones
incrementales. No deben ejecutarse nuevamente sobre una base ya inicializada. La
aplicación tampoco llama a `Migrate()` ni a `EnsureCreated()`.

Si el volumen fue creado antes de incorporar OTA B y la consulta inicial no devuelve
la base, un administrador debe crear una sola vez el usuario y la base, o se debe
reinicializar el volumen completo sabiendo que eso elimina todas las bases locales
del laboratorio.

## Ejecutar

```powershell
dotnet restore apps/ota-b-simulator/OtaBSimulator.csproj
dotnet build apps/ota-b-simulator/OtaBSimulator.csproj
dotnet run --project apps/ota-b-simulator/OtaBSimulator.csproj --urls http://localhost:5161
```

La conexión local predeterminada es:

```text
Host=localhost;Port=5433;Database=ota_b_simulator_db;Username=ota_b_simulator_user;Password=ota_b_simulator_password
```

Puede reemplazarse mediante `ConnectionStrings__OtaBDb`. Los ejemplos de todos los
recursos principales están en `apps/ota-b-simulator/OtaBSimulator.http`.

## Contrato de calendario

Los intervalos usan `[from, until)`, admiten como máximo 730 noches expandidas y no
pueden superponerse dentro de una solicitud. Precios y disponibilidad son llamadas
independientes y transaccionales por separado. Por eso, un error de disponibilidad
no revierte un PUT de precios que ya respondió correctamente.

La consulta de calendario siempre devuelve una entrada por noche. Un precio diario
hereda la tarifa base cuando no hay excepción; si tampoco hay base, se informa
`priceSource: "UNCONFIGURED"`. La ausencia de disponibilidad se informa como
`stateCode: "UNCONFIGURED"` y nunca se interpreta como `OPEN`.
