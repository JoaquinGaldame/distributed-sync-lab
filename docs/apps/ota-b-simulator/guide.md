# Inicialización local de OTA B

Copiar `infrastructure/` y `docs/` de este paquete a la raíz de
`D:\Proyectos\distributed-sync-lab`. Los scripts crean sólo la base y el modelo B.

Los comandos siguientes son para CMD de Windows, desde la raíz del repositorio.
No eliminan el volumen existente ni requieren reiniciar las bases de OTA A/PMS.

## 1. Levantar la instancia de PostgreSQL existente

```cmd
cd /d D:\Proyectos\distributed-sync-lab
docker compose up -d --wait
```

## 2. Crear usuario y base B, una sola vez

Este script se ejecuta como administrador de la instancia y sin
`--single-transaction`, porque CREATE DATABASE no puede ejecutarse en una transacción.
Las credenciales del archivo son exclusivamente las del entorno de desarrollo local.

```cmd
docker compose exec -T postgres psql -U postgres -d postgres -v ON_ERROR_STOP=1 < infrastructure\compose\postgres\ota-b\create-database.sql
```

Si el usuario y la base ya están creados, omitir este paso; no se reemplazan ni se
eliminan. El script no es idempotente. Para que una instalación futura con volumen
vacío incluya OTA B, Codex debe incorporar esas dos sentencias al init de bases
existente, una sola vez, siguiendo la organización del repositorio. Eso no vuelve
a ejecutar el init sobre el volumen actual.

## 3. Crear tablas y cargar catálogos, en este orden

Sobre una base B nueva y vacía:

```cmd
docker compose exec -T postgres psql -U ota_b_simulator_user -d ota_b_simulator_db -v ON_ERROR_STOP=1 --single-transaction < infrastructure\compose\postgres\ota-b\schema.sql
```

```cmd
docker compose exec -T postgres psql -U ota_b_simulator_user -d ota_b_simulator_db -v ON_ERROR_STOP=1 --single-transaction < infrastructure\compose\postgres\ota-b\seed.sql
```

Los scripts no son migraciones incrementales ni actualizaciones para una base con
el modelo B ya creado. No generan anuncios ni importan datos del PMS.

## 4. Comprobar

```cmd
docker compose exec postgres psql -U ota_b_simulator_user -d ota_b_simulator_db -c "\dt"
docker compose exec postgres psql -U ota_b_simulator_user -d ota_b_simulator_db -c "SELECT count(*) FROM cancellation_terms;"
docker compose exec postgres psql -U ota_b_simulator_user -d ota_b_simulator_db -c "SELECT * FROM availability_states ORDER BY code;"
```

Se esperan 22 tablas, 43 términos de cancelación y los estados OPEN, BLOCKED y BOOKED.

## 5. Ejecutar la API cuando Codex la haya implementado

```cmd
set "ConnectionStrings__OtaBDb=Host=localhost;Port=5433;Database=ota_b_simulator_db;Username=ota_b_simulator_user;Password=ota_b_simulator_password"
dotnet build apps\ota-b-simulator\OtaBSimulator.csproj
dotnet run --project apps\ota-b-simulator\OtaBSimulator.csproj --no-launch-profile --urls http://localhost:5161
```

El puerto 5433 es el indicado por el contexto actual de OTA A; comprobarlo contra
el Compose real si se modificó. La API utiliza otra base y otro usuario, y escucha
en 5161 para convivir con OTA A en 5160.
