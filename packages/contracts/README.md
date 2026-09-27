# Contratos de comunicación

Este directorio contiene los contratos independientes del lenguaje que intercambiarán las aplicaciones del laboratorio. Los esquemas usan JSON Schema Draft 2020-12 y describen la estructura existente sin agregar restricciones que no estaban definidas.

Cada aplicación debe implementar localmente los tipos que necesite y comprobar su correspondencia con estos esquemas. En particular, los tipos C# se crearán dentro de cada aplicación cuando esta participe en el intercambio. No existe una biblioteca .NET compartida de contratos.

## Versión 1

Los esquemas vigentes están en [`schemas/v1`](./schemas/v1/) y sus ejemplos válidos en [`examples/v1`](./examples/v1/). La versión forma parte de la ruta de la especificación y no agrega campos a los mensajes.

Una modificación incompatible debe publicarse en un nuevo directorio de versión y conservar las versiones anteriores mientras tengan consumidores. Las implementaciones deben declarar qué versión utilizan.

### `PropertyChangedEvent`

[`property-changed-event.schema.json`](./schemas/v1/property-changed-event.schema.json) representa el evento que comunica el cambio de una propiedad.

| Campo | Tipo | Obligatorio | Definición |
| --- | --- | --- | --- |
| `eventId` | string | sí | Identificador del evento. |
| `eventType` | string | sí | Valor fijo `PropertyChanged`. |
| `propertyId` | string | sí | Identificador de la propiedad modificada. |
| `version` | number | sí | Versión numérica de la propiedad. |
| `occurredAt` | string | sí | Momento asociado al evento, representado como texto. |

Ejemplo: [`property-changed-event.example.json`](./examples/v1/property-changed-event.example.json).

### `SyncPropertyCommand`

[`sync-property-command.schema.json`](./schemas/v1/sync-property-command.schema.json) representa el comando que solicita sincronizar el estado deseado de una propiedad con un destino.

| Campo | Tipo | Obligatorio | Definición |
| --- | --- | --- | --- |
| `experimentId` | string | no | Identificador del experimento asociado, cuando exista. |
| `jobId` | string | sí | Identificador del trabajo de sincronización. |
| `propertyId` | string | sí | Identificador de la propiedad. |
| `version` | number | sí | Versión numérica de la propiedad. |
| `target` | string | sí | Destino de sincronización: `ota-replace` u `ota-async`. |
| `idempotencyKey` | string | sí | Clave de idempotencia del comando. |
| `desiredState.name` | string | sí | Nombre incluido en el estado deseado. |
| `desiredState.price` | number | sí | Precio incluido en el estado deseado. |

Ejemplo: [`sync-property-command.example.json`](./examples/v1/sync-property-command.example.json).

## Límites de la representación JSON

Los contratos originales usaban `number`, que en TypeScript puede representar valores no finitos como `NaN` e infinitos. JSON no admite esos valores; los esquemas aceptan todos los números que sí pueden representarse en JSON y no restringen `version` a enteros ni agregan límites a `price`.

El campo opcional `experimentId` puede estar ausente o contener un string. JSON no tiene un valor equivalente a `undefined` y `null` no estaba permitido por el contrato original.

`occurredAt` continúa siendo un string sin un formato de fecha obligatorio. Tampoco se imponen formatos de UUID, longitudes mínimas, monedas ni otras reglas no presentes en las definiciones originales. Los objetos admiten propiedades adicionales para conservar el carácter estructural y extensible de esas definiciones.
