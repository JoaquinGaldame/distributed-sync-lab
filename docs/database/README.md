# Bases de datos del laboratorio

El entorno local utiliza una instancia de PostgreSQL con tres bases de datos. Academic PMS y el futuro Main Worker comparten la base fuente `rental_management`; los simuladores OTA conservan bases de destino independientes. Esta separación permite representar la fuente interna y los estados externos para observar el proceso de sincronización.

| Base de datos | Componente propietario | Responsabilidad |
| --- | --- | --- |
| `rental_management` | Academic PMS y futuro Main Worker | Conserva las propiedades, sus calendarios, sus versiones, los canales configurados, las publicaciones y los cambios pendientes. Tras importar los datos preparados de Inside Airbnb, es la fuente de verdad interna que ambos componentes consultan. El Worker todavía no está implementado. |
| `ota_replace_service_db` | OTA Replace Service | Conserva el estado de procesamiento que necesite el servicio especializado para gestionar los comandos recibidos. |
| `ota_replace_simulator_db` | OTA Simulator | Conserva el estado de las propiedades alcanzado por el servicio externo simulado. Se consulta para verificar la convergencia con el estado deseado en el PMS. |

La existencia de una base no implica que ya tenga tablas: el esquema de cada destino se incorporará al implementar su funcionalidad. Los componentes internos comparten deliberadamente la fuente `rental_management`; no deben usar como fuente las bases de los simuladores OTA.

Todos los comandos siguientes se ejecutan desde **CMD de Windows**, en la raíz del repositorio:

```cmd
cd /d D:\Proyectos\distributed-sync-lab
```

Todos los comandos siguientes se ejecutan desde **CMD de Windows**, en la raíz del repositorio:

```cmd
cd /d D:\Proyectos\distributed-sync-lab
```

## Levantar el entorno

Levantar los servicios y esperar a que estén disponibles:

```cmd
docker compose up -d --wait
```

Comprobar su estado:

```cmd
docker compose ps
```

Consultar los últimos logs de PostgreSQL:

```cmd
docker compose logs --tail=100 postgres
```

## Detener el entorno conservando los datos

```cmd
docker compose down
```

Los datos permanecen en el volumen. Para volver a levantar el entorno:

```cmd
docker compose up -d --wait
```

## Reiniciar desde cero eliminando los volúmenes

**Este procedimiento elimina los datos de los volúmenes administrados por el proyecto de Compose, incluidas las bases y sus tablas.**

```cmd
docker compose down --volumes
docker compose up -d --wait
```

Al iniciar PostgreSQL con un volumen vacío, se ejecuta nuevamente `init-databases.sql`, que crea los usuarios y las bases.

Modificar `init-databases.sql` y reiniciar conservando el volumen **no vuelve a ejecutar ese script**.

Después de eliminar el volumen, hay que volver a aplicar los esquemas y cargar los datos de las aplicaciones.

### Eliminar un volumen obsoleto

Si un volumen antiguo ya no está declarado en el Compose actual, puede permanecer aunque ejecutes `down --volumes`.

Detener el entorno y listar los volúmenes:

```cmd
docker compose down
docker volume ls
```

Inspeccionar el volumen antiguo:

```cmd
docker volume inspect NOMBRE_REAL_DEL_VOLUMEN
```

Eliminarlo:

```cmd
docker volume rm NOMBRE_REAL_DEL_VOLUMEN
```

Volver a levantar el entorno:

```cmd
docker compose up -d --wait
```

Reemplazá `NOMBRE_REAL_DEL_VOLUMEN` por el nombre exacto mostrado por Docker. Si Docker indica que está en uso, todavía existe un contenedor que lo utiliza.


## Acceder a las bases desde Docker

Desde la raíz de `distributed-sync-lab`, comprobá que PostgreSQL esté activo:

```cmd
docker compose ps
```

### Acceder a `rental_management`

Para entrar a la base fuente compartida:

```cmd
docker compose exec postgres psql -U postgres -d rental_management
```

Dentro de `psql`, estos comandos permiten inspeccionar el PMS:

```sql
\dt
\d properties
\d property_changes
\d calendar_days
\d channels
\d publications
SELECT count(*) FROM properties;
SELECT count(*) FROM property_changes;
SELECT count(*) FROM calendar_days;
SELECT * FROM channels ORDER BY id;
SELECT count(*) FROM publications;
\q
```

`\dt` lista las tablas, `\d` muestra la estructura de una tabla y `\q` sale de PostgreSQL.

### Acceder a las otras bases

Para entrar directamente a cada una de las otras bases:

```cmd
docker compose exec postgres psql -U postgres -d ota_replace_service_db
docker compose exec postgres psql -U postgres -d ota_replace_simulator_db
```

Una vez dentro, usá `\dt` para ver sus tablas. Si todavía no se implementó el componente propietario, es normal que no aparezcan tablas.

### Listar las bases

Para listar las bases desde PostgreSQL:

```cmd
docker compose exec postgres psql -U postgres -d postgres -c "\l"
```

El **servicio de Compose se llama `postgres`** en todos los comandos; lo que cambia con `-d` es la base a la que te conectás.
