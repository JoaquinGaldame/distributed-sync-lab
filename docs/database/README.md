# Bases de datos del laboratorio

El entorno local utiliza una instancia de PostgreSQL con cuatro bases de datos independientes. Cada componente accede únicamente a su propia base mediante credenciales específicas. Esta separación permite representar estados distribuidos y observar el proceso de sincronización entre el sistema interno y el servicio externo simulado.

| Base de datos | Componente propietario | Responsabilidad |
| --- | --- | --- |
| `pms_db` | Academic PMS | Conserva las propiedades, sus versiones y los cambios pendientes. Tras importar los datos preparados de Inside Airbnb, el PMS es la fuente de verdad interna durante la ejecución. |
| `orchestrator_db` | Main Worker | Registra los trabajos de sincronización, los envíos a destinos externos y sus resultados. |
| `ota_replace_service_db` | OTA Replace Service | Conserva el estado de procesamiento que necesite el servicio especializado para gestionar los comandos recibidos. |
| `ota_replace_simulator_db` | OTA Simulator | Conserva el estado de las propiedades alcanzado por el servicio externo simulado. Se consulta para verificar la convergencia con el estado deseado en el PMS. |

La existencia de una base no implica que ya tenga tablas: el esquema de cada componente se incorporará al implementar su funcionalidad. Durante el flujo normal, ningún componente debe leer o modificar directamente las tablas de otro. La comunicación entre componentes se realiza mediante los contratos y las interfaces definidos para el laboratorio.

## Acceder a las bases desde Docker

Desde la raíz de `distributed-sync-lab`, comprobá que PostgreSQL esté activo:

```cmd
docker compose ps
```

### Acceder a `pms_db`

Para entrar a `pms_db`:

```cmd
docker compose exec postgres psql -U postgres -d pms_db
```

Dentro de `psql`, estos comandos permiten inspeccionar el PMS:

```sql
\dt
\d properties
\d property_changes
SELECT count(*) FROM properties;
SELECT count(*) FROM property_changes;
\q
```

`\dt` lista las tablas, `\d` muestra la estructura de una tabla y `\q` sale de PostgreSQL.

### Acceder a las otras bases

Para entrar directamente a cada una de las otras bases:

```cmd
docker compose exec postgres psql -U postgres -d orchestrator_db
docker compose exec postgres psql -U postgres -d ota_replace_service_db
docker compose exec postgres psql -U postgres -d ota_replace_simulator_db
```

Una vez dentro, usá `\dt` para ver sus tablas. Si todavía no se implementó el componente propietario, es normal que no aparezcan tablas.

### Listar las bases

Para listar las cuatro bases desde PostgreSQL:

```cmd
docker compose exec postgres psql -U postgres -d postgres -c "\l"
```

El **servicio de Compose se llama `postgres`** en todos los comandos; lo que cambia con `-d` es la base a la que te conectás.
