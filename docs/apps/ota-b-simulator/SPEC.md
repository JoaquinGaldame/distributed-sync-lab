# OTA B Simulator — especificación experimental v1

Este documento y los SQL de `infrastructure/compose/postgres/ota-b/` definen el
nuevo destino. Las decisiones son propias del laboratorio. No representan los
contratos ni las reglas reales de Airbnb ni de Booking.com.

## 1. Propósito y alcance

Servicio HTTP externo e independiente para recibir alojamientos y calendarios
desde los mecanismos de sincronización. Mantiene su propia base
`ota_b_simulator_db`; no consulta ni modifica la base del PMS ni la de OTA A.

Tecnología: ASP.NET Core .NET 10, EF Core/Npgsql 10, PostgreSQL 17. Esquema `public`.
El esquema se administra con SQL manual; EF sólo consulta y persiste.

## 2. Diferencias con el estado FINAL adjunto de OTA A

| Concepto | OTA A final | OTA B propuesta |
|---|---|---|
| Organización comercial | Property -> Unit -> RatePlan | Listing individual con una configuración comercial |
| Identificador del destino | bigint por entidad | UUID por anuncio |
opcional para agrupar anuncios de la misma propiedad origen |
| Inventario | Unit.total_units y available_units | Una unidad concreta por anuncio; sin contadores de inventario |
| Precio | Decimal por RatePlan y fecha | Tarifa base del anuncio y excepciones por fecha, en unidades monetarias menores |
| Políticas | Cancelación por RatePlan, porcentaje de penalización | Una regla por Listing, porcentaje de reembolso |
| Servicios | Facilities de Property y de Unit | Un único catálogo de amenities por Listing |
| Espacios internos | Units y sus configuraciones | Rooms dentro del Listing, clasificadas mediante room_types |
| Baños y camas | Baños por configuración, ámbito y cantidad | Baños como Rooms de tipo BATHROOM; camas registradas mediante room_beds |
| Escritura de calendario | /availability/sync acepta precio, inventario o ambos en un lote de días | Precios y disponibilidad mediante operaciones independientes sobre intervalos |
| Atomicidad | Una transacción por llamada integrada | Una transacción de precios y otra de disponibilidad; ninguna transacción abarca ambas llamadas |
| Consulta | Sólo fechas persistidas; 409 si el período no tiene datos | Todos los días solicitados; precio heredado cuando corresponde y estado UNCONFIGURED si falta disponibilidad |
| Metadata | Endpoints separados de catálogos | GET /metadata devuelve todos los catálogos |

Importante: OTA A final ya admite actualizaciones parciales de precio o inventario.
No debe describirse como un contrato que obliga a enviar ambos siempre, ni como
un contrato por intervalos: el documento adjunto usa fechas individuales.

## 3. Qué significa Listing

Un anuncio representa una unidad física concreta que puede estar reservable,
cerrada o reservada una noche. OTA B genera su identificador UUID y no almacena
identificadores ni referencias del PMS.

El mecanismo de sincronización conserva, fuera de OTA B, la correspondencia
entre los identificadores del origen y el UUID del anuncio.

Dos unidades alquilables por separado se representan como dos anuncios
independientes. Los datos comunes de ubicación y contactos se configuran
individualmente en cada anuncio.

Este modelo NO representa una categoría con varias unidades físicas
intercambiables. Para los experimentos comparables con A, seleccionar alojamientos
con inventario individual (`total_units = 1`) y una configuración tarifaria canónica.
Si el origen sólo ofrece un inventario agregado mayor que uno o múltiples planes
sin una selección canónica, el adaptador debe declarar esa incompatibilidad;
no debe inventar unidades ni elegir silenciosamente un plan.

La distribución interna se representa mediante rooms. Cada fila identifica un
espacio concreto del anuncio: dormitorio, baño, sala de estar, cocina u otro tipo
definido en room_types.

Cada espacio puede registrar nombre, descripción, superficie en metros cuadrados,
piso, uso compartido y orden de presentación. Las camas se registran en room_beds,
indicando bed_type_code y quantity.

Los dormitorios y baños se contabilizan consultando rooms por tipo. Las cantidades
de camas se obtienen de room_beds; quantity cuenta elementos del tipo indicado,
no plazas para huéspedes. Por ejemplo, una litera cuenta como una estructura.

La ausencia de registros de espacios significa distribución sin configurar;
no demuestra que el alojamiento tenga cero dormitorios o baños.

Los espacios no tienen precio ni disponibilidad independientes. La unidad
alquilable sigue siendo el Listing completo. max_guests expresa su capacidad
comercial y no se calcula automáticamente a partir de las camas.

## 4. Dinero y configuración comercial

La moneda pertenece al anuncio. Los importes monetarios de tarifa base, precios
diarios y cargos fijos son bigint, en la unidad menor de esa moneda:

`amountMinor = importeDecimal * 10^currencies.decimal_places`.

ARS/USD/EUR/BRL usan dos decimales en este seed: 95000.00 -> 9500000. CLP/JPY usan
cero. La API recibe enteros; no redondea precios de precisión incompatible. La
conversión del adaptador debe usar decimal y comprobar desbordamiento, nunca double.

`currencyId` es inmutable después de crear el anuncio: un PUT que intente
cambiarlo devuelve 409. Es una regla de Application, no una restricción SQL
adicional. Los catálogos son de sólo lectura por API.

Hay como máximo una fila en listing_pricing y listing_booking_rules por anuncio.
No hay RatePlan ni un identificador tarifario en los endpoints de calendario.

Precio efectivo de una noche:

1. listing_daily_prices.amount_minor, si existe.
2. listing_pricing.base_nightly_amount_minor, si no existe excepción diaria.
3. null si tampoco está configurada la tarifa base.

Escribir un precio diario sólo modifica esa fecha. Eliminar una excepción revela
la tarifa base vigente. Cambiar la tarifa base no modifica las excepciones diarias.

Para cargos, STAY/NIGHT/GUEST_NIGHT usan amountMinor y percent=null.
LODGING_PERCENT usa percent y amountMinor=null. Las dos columnas representan
alternativas reales y una restricción SQL exige exactamente la alternativa correcta.

Cancelación: refundPercentAfterDeadline = 100 - penaltyPercent de A. El plazo
sigue expresado en días para permitir equivalencias explícitas. Antes del plazo
hay reembolso total; si no hay plazo se aplica siempre el porcentaje configurado.
El seed permite representar las 42 políticas presentes en el seed de A adjunto.
No se implementa un motor de reservas ni de liquidación de penalizaciones.

## 5. HTTP y estructura del proyecto

Proyecto `apps/ota-b-simulator/OtaBSimulator.csproj`, namespace OtaBSimulator.
Configuración ConnectionStrings:OtaBDb / ConnectionStrings__OtaBDb.
URL local propuesta: http://localhost:5161, distinta de la habitual de A (5160).

Respetar la organización FINAL de A, sin referenciar su assembly:

- Api/Controllers: HTTP y envelope.
- Api/Models: DTO; JSON camelCase, propiedades desconocidas rechazadas.
- Api/Mappers: conversión entre DTO y entidades.
- Application: validaciones, casos de uso y transacciones.
- Domain: entidades.
- Infrastructure/Persistence: OtaBDbContext y configuraciones EF.
- Infrastructure/Http: traducción de errores.

Usar el mismo envelope para controlar esa variable experimental:

```json
{
  "data": {},
  "warning": "",
  "message": "Operación completada correctamente.",
  "errors": []
}
```

Errores: 400 por formato/rango, referencia de catálogo inválida, intervalos inválidos,
solapamientos o campos desconocidos; 404 por anuncio inexistente; 409 por duplicados
respecto al estado almacenado o intento de cambiar la moneda del anuncio.
No devolver stack traces ni mensajes SQL internos. Los mensajes al cliente son en español.

POST /listings: 201 con Location y UUID generado por OTA B. PUT exitoso: 200.
DELETE exitoso: 200 con data={deleted:true,resource:"listing",id:UUID}, conservando
envelope. No se expone escritura de catálogos. Health y errores también usan envelope.

## 6. Endpoints

| Método | Ruta | Semántica |
|---|---|---|
| GET | /health | Liveness |
| GET | /health/ready | Readiness, incluida conexión a PostgreSQL |
| GET | /metadata | Todos los catálogos con sus campos completos |
| GET | /listings/{listingId}/rooms | Listar espacios con sus camas |
| POST | /listings/{listingId}/rooms | Crear un espacio |
| GET | /listings/{listingId}/rooms/{roomId} | Consultar un espacio con sus camas |
| PUT | /listings/{listingId}/rooms/{roomId} | Reemplazar datos del espacio |
| DELETE | /listings/{listingId}/rooms/{roomId} | Eliminar espacio y sus camas |
| PUT | /listings/{listingId}/rooms/{roomId}/beds | Reemplazar todas las camas del espacio |
| POST | /listings | Crear un anuncio |
| GET | /listings/{listingId} | General y todas las secciones, sin calendarios completos |
| PUT | /listings/{listingId} | Reemplazar datos generales |
| DELETE | /listings/{listingId} | Eliminar anuncio y dependencias |
| PUT | /listings/{listingId}/location | Reemplazo/upsert de ubicación |
| PUT | /listings/{listingId}/contacts | Reemplazo completo por roleCode |
| PUT | /listings/{listingId}/texts | Reemplazo completo por languageCode |
| PUT | /listings/{listingId}/amenities | Reemplazo completo de códigos |
| PUT | /listings/{listingId}/house-rules | Reemplazo/upsert de reglas de estancia |
| PUT | /listings/{listingId}/booking-rules | Reemplazo/upsert de condiciones comerciales |
| PUT | /listings/{listingId}/base-rate | Reemplazo/upsert de tarifa nocturna base |
| PUT | /listings/{listingId}/fees | Reemplazo completo de cargos por code |
| PUT | /listings/{listingId}/prices | Aplicar excepciones de precio por intervalos |
| DELETE | /listings/{listingId}/prices?from=...&until=... | Quitar excepciones; reaparece la tarifa base si existe |
| PUT | /listings/{listingId}/availability | Aplicar estados de disponibilidad por intervalos |
| DELETE | /listings/{listingId}/availability?from=...&until=... | Quitar estados; las fechas vuelven a UNCONFIGURED |
| GET | /listings/{listingId}/calendar?from=...&until=... | Consulta diaria completa del intervalo |

Listado: page>=1, pageSize entre 1 y 100, defaults 1 y 50; ordenar por createdAt y
luego id; data={items,page,pageSize,total}.

Todos los PUT de secciones usan el estado completo deseado. Las colecciones
aceptan [] para vaciar, rechazan null, elementos null y claves duplicadas.
Para objetos de escritura, cada columna de negocio de la sección se recibe en
camelCase, sin IDs de propietario ni timestamps; los campos anulables admiten
null explícito. Eliminar una colección no borra otras secciones.

Metadata data contiene: currencies, languages, listingCategories, rentalModes,
amenityGroups,roomTypes, bedTypes, amenities, contactRoles, listingStatuses, availabilityStates,
cancellationTerms y feeBases. Devuelve todos los campos del catálogo, ordenados
por code; no crea una dependencia operativa hacia OTA A.

El detalle de Listing incluye rooms y sus camas. Las rutas de espacios validan
que roomId pertenezca al listingId indicado; de lo contrario, devuelven 404.
Los cambios generales del Listing no reemplazan sus espacios.

El reemplazo de camas recibe una colección de bedTypeCode y quantity.
Rechaza tipos repetidos, referencias desconocidas y cantidades no positivas.
Una colección vacía elimina las camas registradas de ese espacio.


## 7. Ejemplos de escritura

### Crear / reemplazar datos generales

```http
POST /listings
```

```json
{
  "title": "Casa del Sol",
  "categoryCode": "HOUSE",
  "rentalModeCode": "ENTIRE_PLACE",
  "currencyCode": "ARS",
  "primaryLanguageCode": "es-AR",
  "timezone": "America/Argentina/San_Juan",
  "statusCode": "DRAFT",
  "maxGuests": 4
}
```

```http
POST /listings/{listingId}/rooms
```

```json
{
  "roomTypeCode": "BEDROOM",
  "name": "Dormitorio principal",
  "description": "Dormitorio con ventana al jardín.",
  "areaM2": 18.00,
  "floorNumber": 0,
  "isShared": false,
  "displayOrder": 1
}
```

```http
PUT /listings/{listingId}/rooms/{roomId}/beds
```

```json
[
  {
    "bedTypeCode": "QUEEN",
    "quantity": 1
  }
]
```

### Tarifa base

```http
PUT /listings/{listingId}/base-rate
```

```json
{
  "baseNightlyAmountMinor": 9500000
}
```

### Condiciones comerciales

```http
PUT /listings/{listingId}/booking-rules
```

```json
{
  "cancellationTermCode": "FREE_7D_THEN_REFUND_0",
  "minNights": 2,
  "maxNights": null
}
```

### Amenities

```http
PUT /listings/{listingId}/amenities
```

```json
["wifi", "parking", "kitchen", "air-conditioning"]
```

Los slugs son claves de catálogo, no IDs de A. Amenities deben mapearse por su
significado: no unir indiscriminadamente todos los facilities de Property y Unit.
Un servicio compartido accesible puede aplicarse al anuncio; eso no lo convierte
en privado. El catálogo distingue, por ejemplo, pool-access, shared-pool y private-pool.

### Precios: operación independiente

```http
PUT /listings/{listingId}/prices
```

```json
{
  "periods": [
    { "from": "2026-11-09", "until": "2026-11-11", "amountMinor": 9500000 },
    { "from": "2026-11-11", "until": "2026-11-12", "amountMinor": 10500000 }
  ]
}
```

### Disponibilidad: otra operación independiente

```http
PUT /listings/{listingId}/availability
```

```json
{
  "periods": [
    { "from": "2026-11-09", "until": "2026-11-10", "stateCode": "OPEN" },
    { "from": "2026-11-10", "until": "2026-11-11", "stateCode": "BLOCKED" },
    { "from": "2026-11-11", "until": "2026-11-12", "stateCode": "BOOKED" }
  ]
}
```

Son dos llamadas; no existe endpoint combinado ni transacción entre ellas.
Un error en la segunda no deshace la primera.

## 8. Intervalos, fechas y atomicidad

- Las fechas son DateOnly ISO YYYY-MM-DD, interpretadas como noches locales del anuncio.
- `from` está incluido y `until` está excluido: [from, until).
- Un intervalo de un día usa until=from+1 día. Así se conserva la actualización diaria.
- Un request admite de 1 a 730 intervalos. Cada uno exige until>from.
- El total de fechas expandidas no puede superar 730 por request. Rechazar rangos
  superpuestos, aunque declaren el mismo valor. Los rangos contiguos sí se permiten.
- Se permite recibir rangos desordenados; ordenarlos antes de validar solapamientos.
- Rechazar desbordamientos de fecha. No se impone que las fechas sean futuras.
- En consultas y DELETE: 1 a 730 noches, calculadas por until-from.
- Se pueden cargar al menos 18 meses de datos, repartidos en solicitudes si hace falta;
  no se exige reenviar 18 meses para actualizar un solo día. A final también acepta
  operaciones parciales de 1 a 730 días.
- Cada PUT valida todo el lote antes de escribir, bloquea el Listing y realiza todos
  sus upserts en una transacción. No aplica cambios fuera de las fechas declaradas.
- Repetir una operación produce el mismo estado funcional; no garantiza timestamps
  idénticos. Last write wins, igual que el comportamiento actual documentado de A.
- Un DELETE de calendario elimina sólo las filas del recurso indicado y del período;
  no elimina el anuncio, la tarifa base ni el otro calendario.
- Los reemplazos de secciones también bloquean el Listing y son transaccionales.
- La consulta agregada de anuncio y la de calendario utilizan RepeatableRead.

El máximo 730 limita el tamaño de la operación, no la cantidad total de fechas que
pueden existir en la base. El SQL almacena filas diarias: los intervalos son una
forma propia del contrato HTTP y se expanden en Application.

## 9. Consulta de calendario

```http
GET /listings/{listingId}/calendar?from=2026-11-09&until=2026-11-13
```

Ejemplo después de configurar la tarifa base y aplicar ambos requests de la sección 7:

```json
{
  "data": {
    "listingId": "40000000-0000-0000-0000-000000000007",
    "currencyCode": "ARS",
    "from": "2026-11-09",
    "until": "2026-11-13",
    "days": [
      { "date": "2026-11-09", "amountMinor": 9500000, "priceSource": "OVERRIDE", "stateCode": "OPEN" },
      { "date": "2026-11-10", "amountMinor": 9500000, "priceSource": "OVERRIDE", "stateCode": "BLOCKED" },
      { "date": "2026-11-11", "amountMinor": 10500000, "priceSource": "OVERRIDE", "stateCode": "BOOKED" },
      { "date": "2026-11-12", "amountMinor": 9500000, "priceSource": "BASE", "stateCode": "UNCONFIGURED" }
    ]
  },
  "warning": "",
  "message": "Consulta completada correctamente.",
  "errors": []
}
```

Para un anuncio existente, incluso sin precios ni estados, la consulta responde 200
con TODOS los días: amountMinor=null, priceSource=UNCONFIGURED y stateCode=UNCONFIGURED.
UNCONFIGURED nunca significa OPEN ni BOOKED, y no se admite en el PUT de disponibilidad.
La consulta no inserta datos. Un estado OPEN expresa apertura del calendario; por sí
solo no certifica publicación, precio ni todas las condiciones para realizar una reserva.

## 10. Transformaciones para el futuro adaptador B

| Fuente conceptual | Representación en B |
|---|---|
| Propiedad + unidad física | Un anuncio identificado por el UUID generado por OTA B; el mecanismo de sincronización mantiene la correspondencia fuera de la OTA |
| Datos comunes de propiedad | Se configuran por anuncio |
| Una configuración tarifaria seleccionada | listing_pricing y listing_booking_rules |
| Importe decimal | Entero según decimalPlaces de la moneda |
| Penalización del X% | Reembolso del 100-X% |
| A.closed 0 / 1 / 2 | OPEN / BLOCKED / BOOKED |
| A.available_units | No se envía; sólo son comparables unidades individuales |
| Información de espacios y camas del origen | Rooms clasificadas por roomTypeCode y camas registradas por bedTypeCode y quantity |
| Facilities aplicables de distintos ámbitos | Slugs del catálogo de amenities, con mapeo semántico explícito |
| Días consecutivos con valor idéntico | Pueden agruparse en un intervalo; no es obligatorio |

Un conteo agregado de dormitorios o baños no proporciona por sí solo la identidad,
superficie ni distribución de espacios concretos. El adaptador no debe inventar
esos detalles; sólo enviará información disponible o reglas de preparación
explícitas y comunes a las estrategias experimentales.

El futuro Worker construye ambas integraciones desde el PMS; esta tabla explica
correspondencias conceptuales, no un flujo OTA A -> OTA B.

Para comparar estrategias, utilizar el mismo subconjunto de datos, adaptadores y
reglas de selección en las tres estrategias. Medir por destino fechas/valores
convergidos además de requests: un intervalo B puede transportar varios días y dos
operaciones B pueden ser necesarias para el estado que A recibe en una sola llamada.
Las diferencias de los destinos son parte del escenario; no atribuirlas por sí solas
a una superioridad de la arquitectura de sincronización.

## 11. Primera entrega que debe implementar Codex

Crear el proyecto y completar persistencia, metadata, CRUD de anuncios, secciones,
operaciones separadas de calendario y consultas de esta especificación. No integrar
el Worker en este checkpoint. Reutilizar el patrón estructural y de errores de A,
pero desarrollar contratos y entidades B independientes, basados en estos SQL.

Agregar ejemplos OtaBSimulator.http y guía local. Compilar y comprobar contra una
base temporal PostgreSQL 17: claves, cascadas, colecciones, dinero entero, herencia
de tarifa base, intervalos sin solapamientos, consultas densas y fallo de disponibilidad
sin revertir precios ya confirmados.

SQL y semillas se ejecutan manualmente. No llamar a Migrate() ni EnsureCreated().
No modificar OTA A ni sus scripts. No borrar volúmenes, bases ni datos existentes.

Comprobar también pertenencia de Rooms al Listing, catálogos room_types y
bed_types, cantidades positivas de camas y eliminación en cascada:
Listing → Rooms → RoomBeds.
