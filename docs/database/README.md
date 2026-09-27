# Bases de datos del laboratorio

El entorno local utiliza una instancia de PostgreSQL con cuatro bases de datos independientes. Cada componente accede únicamente a su propia base mediante credenciales específicas. Esta separación permite representar estados distribuidos y observar el proceso de sincronización entre el sistema interno y el servicio externo simulado.

| Base de datos | Componente propietario | Responsabilidad |
| --- | --- | --- |
| `pms_db` | Academic PMS | Conserva las propiedades, sus versiones y los cambios pendientes. Tras importar los datos preparados de Inside Airbnb, el PMS es la fuente de verdad interna durante la ejecución. |
| `orchestrator_db` | Main Worker | Registra los trabajos de sincronización, los envíos a destinos externos y sus resultados. |
| `ota_replace_service_db` | OTA Replace Service | Conserva el estado de procesamiento que necesite el servicio especializado para gestionar los comandos recibidos. |
| `ota_replace_simulator_db` | OTA Simulator | Conserva el estado de las propiedades alcanzado por el servicio externo simulado. Se consulta para verificar la convergencia con el estado deseado en el PMS. |

La existencia de una base no implica que ya tenga tablas: el esquema de cada componente se incorporará al implementar su funcionalidad. Durante el flujo normal, ningún componente debe leer o modificar directamente las tablas de otro. La comunicación entre componentes se realiza mediante los contratos y las interfaces definidos para el laboratorio.