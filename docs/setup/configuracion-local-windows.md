# Configuración local del laboratorio en Windows

Este manual explica cómo preparar el laboratorio por primera vez usando **CMD de Windows**. Los comandos levantan PostgreSQL con Docker Compose y aplican las migraciones de los proyectos .NET.

Por ahora, el único proyecto con migraciones es **Academic PMS**. Cuando se agreguen otros proyectos, deberán incorporarse como nuevas subsecciones al final de este documento.

## 1. Requisitos previos

Antes de comenzar, instalá:

- Docker Desktop, con contenedores Linux habilitados.
- .NET SDK 10.
- Git, para obtener el repositorio.

Abrí Docker Desktop y esperá hasta que indique que el motor está iniciado. Luego abrí **Símbolo del sistema**: presioná `Win + R`, escribí `cmd` y presioná Enter.

Comprobá las herramientas:

```cmd
docker --version
docker compose version
dotnet --version
```

El último comando debe mostrar una versión `10.x`. Si alguno no se reconoce, cerrá y volvé a abrir CMD después de instalar la herramienta correspondiente.

## 2. Ubicarse en la raíz del repositorio

Todos los comandos de este manual se ejecutan desde:

```text
D:\Proyectos\distributed-sync-lab
```

En CMD, `cd /d` cambia al mismo tiempo de unidad y de directorio. Ejecutá:

```cmd
cd /d D:\Proyectos\distributed-sync-lab
```

Confirmá la ubicación:

```cmd
cd
dir
```

La salida de `cd` debe ser `D:\Proyectos\distributed-sync-lab` y `dir` debe mostrar, entre otros elementos, `compose.yaml`, `apps` e `infrastructure`.

## 3. Validar Docker Compose

Antes de crear contenedores, validá la sintaxis:

```cmd
docker compose config --quiet
```

Si el comando termina sin mostrar errores, la configuración es válida.

El Compose define un contenedor PostgreSQL 17 llamado `sync-lab-postgres`. El puerto interno `5432` se publica en la computadora como `5433`; por eso las aplicaciones locales deben conectarse a `localhost:5433`.

## 4. Levantar PostgreSQL y crear las bases

Ejecutá:

```cmd
docker compose up -d --wait
```

- `up` crea e inicia los recursos.
- `-d` deja el contenedor ejecutándose en segundo plano.
- `--wait` espera hasta que el control de salud de PostgreSQL indique que está listo.

Durante la primera creación del volumen, PostgreSQL ejecuta automáticamente `infrastructure\compose\postgres\init-databases.sql`. No hay que ejecutar ese archivo manualmente.

Comprobá el estado:

```cmd
docker compose ps
```

El servicio `postgres` y el contenedor `sync-lab-postgres` deben figurar como iniciados y saludables (`healthy`).

## 5. Bases creadas

El script de inicialización crea estas bases del laboratorio:

| Base | Usuario | Uso |
| --- | --- | --- |
| `rental_management` | `rental_management_user` | Fuente compartida por Academic PMS y el futuro Main Worker. |
| `ota_replace_service_db` | `ota_replace_service_user` | Destino independiente del servicio OTA Replace. |
| `ota_replace_simulator_db` | `ota_replace_simulator_user` | Destino independiente del simulador OTA Replace. |

Las contraseñas de desarrollo local están declaradas en `infrastructure\compose\postgres\init-databases.sql`. No reutilices estas credenciales en ambientes reales.

Listá las bases para verificar su creación:

```cmd
docker compose exec postgres psql -U postgres -d postgres -c "\l"
```

También podés comprobar específicamente la base fuente:

```cmd
docker compose exec postgres psql -U postgres -d rental_management -c "SELECT current_database();"
```

La respuesta debe contener `rental_management`.

## 6. Entender la persistencia

PostgreSQL guarda sus datos en el volumen de Docker `distributed-sync-lab_postgres-data`. Reiniciar Docker Desktop o el contenedor no elimina los datos.

El Compose no usa un volumen externo ni monta un directorio del host para almacenar los datos. Solamente monta el archivo de inicialización del repositorio en `/docker-entrypoint-initdb.d/`.

Para detener el entorno y conservar todo:

```cmd
docker compose stop
```

Para eliminar el contenedor y la red, pero conservar el volumen con las bases:

```cmd
docker compose down
```

La próxima vez, volvé a iniciarlo con:

```cmd
docker compose up -d --wait
```

## 7. Aplicar las migraciones .NET

Las migraciones crean y actualizan tablas dentro de una base que ya existe. Primero se levanta PostgreSQL y después se aplican las migraciones; no inviertas ese orden.

### 7.1. Preparar Entity Framework Core

Desde la raíz del repositorio, restaurá las dependencias de Academic PMS:

```cmd
dotnet restore apps\academic-pms\AcademicPms.csproj
```

Este paso requiere acceso a `https://api.nuget.org`. Si aparece `NU1301`, revisá la conexión a Internet, el proxy, la VPN o las reglas del firewall antes de continuar.

Comprobá la herramienta de Entity Framework:

```cmd
dotnet ef --version
```

Si CMD indica que `dotnet ef` no existe, instalá la versión compatible con .NET 10:

```cmd
dotnet tool install --global dotnet-ef --version 10.0.9
```

Cerrá y abrí CMD si la herramienta recién instalada todavía no se reconoce. Después, regresá a la raíz:

```cmd
cd /d D:\Proyectos\distributed-sync-lab
```

### 7.2. Migraciones de Academic PMS

Academic PMS obtiene su conexión predeterminada de `apps\academic-pms\appsettings.json` y utiliza:

```text
Host=localhost;Port=5433;Database=rental_management;Username=rental_management_user
```

Listá las migraciones disponibles:

```cmd
dotnet ef migrations list --project apps\academic-pms\AcademicPms.csproj --startup-project apps\academic-pms\AcademicPms.csproj
```

Actualmente deben aparecer:

```text
20260927060537_InitialPms
20260928200414_ExpandPropertyForInsideAirbnb
```

La forma recomendada de aplicar todas las migraciones pendientes, en su orden correcto, es:

```cmd
dotnet ef database update --project apps\academic-pms\AcademicPms.csproj --startup-project apps\academic-pms\AcademicPms.csproj
```

Entity Framework registra cada migración aplicada en la tabla `__EFMigrationsHistory`. Si se ejecuta nuevamente el mismo comando, no vuelve a aplicar migraciones que ya estén registradas.

Si necesitás aplicarlas una por una con fines de aprendizaje, usá este orden:

```cmd
dotnet ef database update 20260927060537_InitialPms --project apps\academic-pms\AcademicPms.csproj --startup-project apps\academic-pms\AcademicPms.csproj
dotnet ef database update 20260928200414_ExpandPropertyForInsideAirbnb --project apps\academic-pms\AcademicPms.csproj --startup-project apps\academic-pms\AcademicPms.csproj
```

No es necesario ejecutar esos dos comandos si ya usaste `database update` sin indicar una migración.

### 7.3. Verificar las migraciones

Consultá el historial dentro de PostgreSQL:

```cmd
docker compose exec postgres psql -U postgres -d rental_management -c "SELECT migration_id, product_version FROM \"__EFMigrationsHistory\" ORDER BY migration_id;"
```

Listá las tablas creadas:

```cmd
docker compose exec postgres psql -U postgres -d rental_management -c "\dt"
```

Entre las tablas deben aparecer `properties`, `property_changes` y `__EFMigrationsHistory`.

## 8. Ejecutar Academic PMS

Con PostgreSQL activo y las migraciones aplicadas, iniciá la API:

```cmd
dotnet run --project apps\academic-pms\AcademicPms.csproj --urls http://localhost:5080
```

Sin cerrar esa ventana, abrí otra ventana de CMD y comprobá la API:

```cmd
curl.exe http://localhost:5080/health
```

Para detener la API, regresá a la primera ventana y presioná `Ctrl + C`.

## 9. Reinicialización completa opcional

El script de inicialización solo se ejecuta cuando PostgreSQL crea un volumen vacío. Si cambió la definición de las bases y necesitás empezar desde cero, ejecutá:

```cmd
docker compose down --volumes --remove-orphans
docker compose up -d --wait
dotnet ef database update --project apps\academic-pms\AcademicPms.csproj --startup-project apps\academic-pms\AcademicPms.csproj
```

> **Advertencia:** `docker compose down --volumes` elimina permanentemente todas las bases guardadas en `distributed-sync-lab_postgres-data`, incluidos los datos de `rental_management`, los destinos OTA y el historial de migraciones. No elimina el código ni los datasets almacenados en el repositorio.

## 10. Orden resumido para una instalación nueva

Desde CMD:

```cmd
cd /d D:\Proyectos\distributed-sync-lab
docker compose config --quiet
docker compose up -d --wait
docker compose ps
docker compose exec postgres psql -U postgres -d postgres -c "\l"
dotnet restore apps\academic-pms\AcademicPms.csproj
dotnet ef migrations list --project apps\academic-pms\AcademicPms.csproj --startup-project apps\academic-pms\AcademicPms.csproj
dotnet ef database update --project apps\academic-pms\AcademicPms.csproj --startup-project apps\academic-pms\AcademicPms.csproj
docker compose exec postgres psql -U postgres -d rental_management -c "\dt"
dotnet run --project apps\academic-pms\AcademicPms.csproj --urls http://localhost:5080
```

Después de completar estos pasos, PostgreSQL queda activo, las tres bases existen y el esquema de Academic PMS está aplicado en `rental_management`.
