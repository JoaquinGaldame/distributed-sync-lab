# Distributed Sync Lab

Laboratorio experimental distribuido desarrollado como parte del Trabajo Final de Investigación de la carrera **Licenciatura en Ciencias de la Computación** de la Facultad de Ciencias Exactas, Físicas y Naturales de la Universidad Nacional de San Juan.

**Autor:** Joaquín Antonio Galdame

## Investigación

El laboratorio forma parte del trabajo:

**“Mecanismo de sincronización confiable de datos con servicios externos para una PyME de desarrollo de software de la provincia de San Juan”.**

El problema, la pregunta y los objetivos de la investigación ya están aprobados. El laboratorio constituye el entorno experimental para comparar estrategias arquitectónicas y fundamentar el diseño y la evaluación del mecanismo propuesto.

La investigación estudia la sincronización entre sistemas internos y múltiples servicios externos independientes, considerando:

* fallos parciales;
* reintentos y operaciones duplicadas;
* procesamiento de actualizaciones fuera de orden;
* restricciones de consumo y rate limiting;
* diferencias temporales entre el estado interno y el estado externo;
* escalabilidad, extensibilidad y facilidad de adopción en una PyME.

## Antecedente laboral

La investigación surge de la evolución del sistema RBS (Rental Booking System):

* **Legacy:** una Console App se ejecutaba cada medianoche y sincronizaba los alojamientos mediante procesamiento batch completo para todas las OTAs.
* **Migración actual:** el sistema fuente emite un pedido mediante SQS. Un Worker coordina la sincronización; para OTAs complejas se comunica por HTTP con servicios especializados y para OTAs simples realiza la integración dentro del propio Worker.
* **Alternativa a investigar:** una arquitectura orientada a eventos con comunicación asíncrona, cuya conveniencia debe establecerse mediante evidencia experimental.

Este antecedente orienta el diseño del laboratorio. No se presupone que la alternativa por eventos sea superior ni que toda OTA requiera un servicio especializado.

## Estrategias estudiadas

Se compararán tres implementaciones con coordinación y estado de sincronización propios, que compartirán únicamente la lógica de integración definida en `integrations-core`:

| Estrategia | Organización experimental |
| --- | --- |
| Procesamiento batch centralizado | Un proceso programado consulta la base SQL fuente compartida con el PMS y sincroniza los alojamientos por lotes directamente con los destinos externos. |
| Orquestación síncrona mediante HTTP | Un Worker recibe pedidos y coordina llamadas HTTP. Integra directamente los destinos simples y utiliza servicios especializados cuando la integración definida lo requiera. |
| Arquitectura orientada a eventos | Componentes consumidores procesan trabajo mediante mensajes y eventos. La distribución de responsabilidades, confirmaciones y resultados se definirá antes de implementarla. |

`apps/main-worker/` agrupará las tres arquitecturas y el proyecto dependencia `integrations-core`. Esta organización no implica un ejecutable único ni un proceso común: cada arquitectura mantiene sus propios límites de ejecución.

Cada implementación tendrá sus propios proyectos ejecutables, configuración, coordinación, procedimientos de recuperación y estado de procesamiento. Las tres podrán referenciar `integrations-core` para reutilizar consultas a la fuente SQL, tipos de contratos, objetos de integración y builders de payloads. Los reintentos, la mensajería, el ordenamiento del trabajo, la programación del batch y el seguimiento de resultados pertenecerán a cada arquitectura.

Los servicios especializados y consumidores que una arquitectura requiera se ubicarán dentro de su carpeta. Podrán ejecutarse como procesos separados: su ubicación en el repositorio no determina su límite de ejecución. La arquitectura batch no incorporará un servicio intermedio por obligación de las otras variantes.

Se ejecutará una estrategia por experimento, con cargas equivalentes y condiciones controladas. La comparación comprende arquitecturas completas; las diferencias de rendimiento no se atribuirán automáticamente a un único protocolo o componente.

## Sistema fuente: Academic PMS

`academic-pms` representa el sistema interno que conserva el estado de los alojamientos. No ejecuta estrategias de sincronización, no traduce datos a contratos de OTAs y no controla los reintentos ni los resultados externos.

### Base SQL fuente compartida

El Academic PMS y los componentes del Worker que preparan datos consultarán **la misma base PostgreSQL y las mismas tablas fuente**. El PMS escribe el estado de negocio; batch, orquestación y eventos leen directamente esa fuente de verdad mediante `integrations-core`. No se creará una copia de los alojamientos para cada estrategia ni se exigirá consultar una API del PMS para obtener los datos fuente.

Compartir la base fuente no significa compartir una conexión en memoria, credenciales idénticas ni el estado de sincronización. Cada proceso tendrá su propia conexión y configuración. El acceso del Worker a las tablas de negocio será de lectura; sus intentos, resultados, posiciones de consumo y otros estados de ejecución se conservarán en persistencia propia de cada arquitectura, sin modificar el estado fuente para registrar resultados externos.

Para las variantes que reaccionan a cambios, el PMS registra el cambio y emite el pedido correspondiente. Batch consulta periódicamente la base fuente. La emisión efectiva de mensajes y su contrato deberán verificarse y completarse durante el desarrollo; no se consideran implementados sólo por aparecer en este diseño.

Al diseñar la lectura se definirá si un pedido sincroniza el estado actual o una versión concreta. Las consultas necesarias para construir un payload deberán observar datos coherentes; no se supondrá que recibir un evento garantiza por sí solo leer su versión histórica desde SQL.

En el antecedente laboral se utiliza SQS. En el laboratorio se prevé RabbitMQ para la mensajería; no se afirmará que reproduce todos los detalles de funcionamiento de SQS.

## Dataset experimental

La entrada experimental proviene de **Inside Airbnb**. El Academic PMS, desarrollado en .NET 10, ya importó **27.890 propiedades** de un snapshot del dataset.

Inside Airbnb aporta registros de alojamientos para construir el estado inicial y las cargas experimentales. No representa el contrato de una API de Airbnb ni constituye un simulador de esa plataforma.

Los datos se transforman a un modelo interno reducido con los atributos necesarios para estudiar la sincronización. La disponibilidad de cada atributo deberá comprobarse al delimitar las operaciones externas. Los calendarios, identificadores externos u otros datos que no provengan del archivo se generarán de forma controlada y se documentarán por separado.

### Procedencia y reproducibilidad

El dataset original no se almacena directamente en este repositorio. Para cada conjunto de datos utilizado se documentarán:

* fuente y snapshot o fecha de publicación;
* archivo y hash criptográfico;
* cantidad de registros y reglas de selección;
* transformaciones y atributos utilizados;
* datos adicionales generados, reglas y semillas;
* estado inicial y secuencia de cambios aplicada.

La generación de IDs, marcas temporales y otros valores variables se controlará o registrará según el diseño experimental. No se asumirá que repetir una importación produce automáticamente valores idénticos para todos los campos.

## Destinos externos simulados

Se prevén dos destinos independientes: **OTA Externa A** y **OTA Externa B**. Ambas serán utilizadas por las tres estrategias con los mismos contratos y configuraciones experimentales.

* **OTA A y OTA B:** destinos ficticios desarrollados para el laboratorio, con contratos, operaciones y documentación propios. Se podrán consultar fuentes públicas citables como referencia de necesidades reales, sin reproducir especificaciones privadas ni presentar los destinos como réplicas oficiales.
* Ambas cubrirán necesidades comparables, por ejemplo datos del alojamiento, disponibilidad y precios, mediante contratos y reglas diferentes. Como propuesta a evaluar, A podría actualizar precios y disponibilidad juntos por intervalos, mientras B los actualizaría mediante operaciones separadas por día. Estas diferencias deberán justificarse por los escenarios que permiten estudiar.

No se utilizará documentación privada de Airbnb para construir ni publicar estos destinos. OTA B no se presentará como una reproducción de Airbnb. Los nombres neutrales no sustituyen la documentación de las fuentes utilizadas ni autorizan la divulgación de información privada.

Los simuladores representan sistemas ajenos. Reciben y validan solicitudes en su contrato externo, mantienen su propio estado y emiten respuestas. La conversión desde las tablas fuente a los contratos externos se implementará en los builders y tipos de `integrations-core`, utilizados por las arquitecturas según sus responsabilidades. Los servicios especializados, cuando existan, ejecutarán la integración que les corresponda. Los simuladores no consultarán la base fuente ni incorporarán esa conversión.

Los IDs internos y externos son distintos. Su correspondencia se definirá y mantendrá explícitamente; un ID del dataset no se considerará automáticamente un ID válido de una OTA.

Se seleccionarán los comportamientos necesarios para las operaciones estudiadas, entre ellos:

* actualizaciones completas o parciales;
* validaciones y rechazos de solicitudes;
* límites de consumo;
* demoras, timeouts y resultados desconocidos;
* fallos parciales;
* solicitudes duplicadas y actualizaciones fuera de orden;
* procesamiento diferido y consistencia eventual, cuando corresponda al modelo elegido.

Se distinguirán las reglas respaldadas por documentación pública de los comportamientos y fallos introducidos como condiciones experimentales. No se afirmará que los simuladores reproducen internamente plataformas comerciales completas.

## Principios experimentales

La coordinación y el estado de ejecución de cada estrategia estarán aislados; la lógica de integración de `integrations-core` será una dependencia controlada común. El entorno de comparación mantendrá:

* misma base SQL fuente compartida con el PMS durante cada ejecución, restablecida al mismo estado inicial para cada estrategia, y misma carga de cambios;
* mismos contratos, correspondencias de IDs y estados iniciales externos;
* operaciones de sincronización semánticamente equivalentes, aunque sus solicitudes puedan agruparse de manera diferente;
* mismos escenarios de fallo y reglas de aplicación documentadas;
* configuración de recursos y versiones registradas;
* persistencia propia del estado de sincronización de cada arquitectura, separada de las tablas fuente, y restablecimiento entre ejecuciones;
* misma versión de `integrations-core` para las estrategias comparadas;
* criterios uniformes de medición e identificación de experimentos y operaciones;
* resultados exportables en JSON y CSV.

El Experiment Runner y los simuladores forman parte del entorno experimental. `integrations-core` es una dependencia ejecutable común del sistema bajo prueba, cuyo alcance y versión se documentarán. Así se mantiene equivalente la preparación de los datos externos y se comparan las decisiones de coordinación, comunicación y recuperación de cada arquitectura.

La consistencia se evaluará según la operación seleccionada: el estado externo esperado puede ser una proyección o transformación del estado interno, no una copia idéntica del registro del PMS.

Antes de comparar resultados se comprobará que cada implementación ejecuta correctamente la operación acordada sin fallos inyectados. Se documentarán diferencias de implementación y limitaciones que puedan afectar la interpretación, incluyendo el acceso compartido a la fuente SQL y su posible papel como cuello de botella. Compartir `integrations-core` no impide la comparación, pero las conclusiones deberán indicar que esa lógica se mantuvo constante.

Los recursos disponibles y el esfuerzo de implementación, operación y mantenimiento se registrarán para analizar la viabilidad de adopción por una PyME. Las conclusiones se limitarán a las operaciones, cargas y condiciones evaluadas.

## Tecnologías principales

* .NET 10 para el PMS académico;
* JSON Schema para especificaciones de comunicación independientes del lenguaje;
* PostgreSQL para persistencia;
* RabbitMQ para mensajería;
* Docker y Docker Compose;
* Kubernetes / Minikube;
* Toxiproxy para condiciones de comunicación controladas;
* Prometheus y Grafana para observabilidad.

Cada proyecto mantiene sus propias dependencias, configuración y comandos de compilación y ejecución. La tecnología de las aplicaciones distintas del PMS se decidirá al implementarlas y quedará registrada como parte del diseño experimental.

Minikube permitirá ejecutar componentes separados. Las condiciones de red se configurarán explícitamente; desplegar en Kubernetes no reproduce por sí solo una red de producción.

Durante las ejecuciones experimentales oficiales se congelarán las versiones relevantes del entorno.

## Contratos de comunicación

Las especificaciones de entrada y de comunicación comunes al entorno se documentarán mediante esquemas JSON y ejemplos en [`packages/contracts`](./packages/contracts/README.md), cuando corresponda al formato elegido. Los contratos XML u otros formatos externos tendrán su propia especificación.

Los tipos y builders de los contratos de integración comunes a las tres arquitecturas se implementarán en `apps/main-worker/integrations-core/`, coherentes con las especificaciones publicables de OTA A y B. Los mensajes y estados internos específicos de una arquitectura se mantendrán dentro de ella.

### Proyecto dependencia: integrations-core

`integrations-core` será un único proyecto biblioteca desarrollado para el laboratorio. Contendrá:

* acceso de lectura a las tablas SQL fuente y consultas necesarias para preparar datos;
* tipos de contratos y objetos de integración de OTA A y B;
* builders y reglas de transformación de datos fuente a payloads externos;
* validaciones y conversiones necesarias para esas operaciones.

No será un servicio desplegable ni ejecutará estrategias. Los clientes HTTP, publicaciones y consumos de mensajes, políticas de reintento, deduplicación del procesamiento, temporizadores y registro de resultados se implementarán en los componentes propios de cada arquitectura.

El proyecto será código propio del laboratorio; no incorporará código ni documentación privada de las dependencias empresariales. Podrá reutilizarse en el mecanismo final y se versionará como parte de cada experimento. Su referencia directa presupone consumidores compatibles con el proyecto .NET; la tecnología y el framework concreto de esos consumidores se fijarán al implementarlos.

## Estructura prevista

```text
distributed-sync-lab/
├── apps/
│   ├── academic-pms/
│   ├── main-worker/
│   │   ├── batch/
│   │   ├── orchestration/
│   │   ├── events/
│   │   └── integrations-core/
│   ├── ota-a-simulator/
│   ├── ota-b-simulator/
│   └── experiment-runner/
├── packages/
│   └── contracts/
│       ├── schemas/
│       └── examples/
├── data/
│   ├── raw/
│   └── generated/
├── infrastructure/
│   ├── compose/
│   └── kubernetes/
├── experiments/
│   ├── scenarios/
│   ├── workloads/
│   └── seeds/
├── analysis/
├── results/
└── docs/
```

Cada carpeta de estrategia podrá contener uno o varios proyectos. El número y las responsabilidades de los procesos se definirán en el diseño de esa arquitectura.

## Estado y próximos pasos

### Construido

* Academic PMS en .NET 10 con 27.890 propiedades importadas de Inside Airbnb.
* `ota-replace-simulator` en .NET 10: recibe un estado completo, lo guarda por `externalId` en `ota_replace_simulator_db` y permite consultarlo.

El simulador existente es un prototipo técnico genérico. Su contrato no constituye todavía la integración definitiva de OTA A o B. Se decidirá qué partes conservar una vez seleccionada la operación externa; no se considera ya convertido ni renombrado. Las carpetas de la estructura prevista tampoco se consideran creadas por estar documentadas aquí.

### Secuencia de desarrollo

1. Delimitar una necesidad de sincronización comparable y diseñar contratos y comportamiento observable propios para OTA A y B, usando referencias públicas cuando aporten fundamento.
2. Comprobar las tablas y campos de la base SQL compartida con el PMS y especificar los datos adicionales y correspondencias de IDs necesarios.
3. Definir e implementar en `integrations-core` los tipos, consultas y builders necesarios para la primera operación.
4. Especificar la entrada, componentes, comunicaciones y criterio de finalización de cada arquitectura aislada.
5. Construir los destinos externos e implementar las arquitecturas incrementalmente, utilizando la misma versión de `integrations-core` y verificando primero el recorrido completo de una operación.
6. Implementar el Experiment Runner, la instrumentación y el restablecimiento de estados.
7. Definir e introducir fallos controlados y verificar su aplicación.
8. Preparar las ejecuciones en Minikube, ejecutar los experimentos y analizar resultados y limitaciones.

## Uso académico

Este repositorio fue desarrollado con fines académicos y de investigación.

Puede ser consultado, citado, estudiado, modificado y redistribuido para actividades académicas, educativas y de investigación no comerciales, respetando los términos establecidos en el archivo `LICENSE`.

Los datasets y componentes de terceros mantienen sus propias condiciones de utilización y no quedan relicenciados por este proyecto.

## Citación

Si este laboratorio, su código, documentación o diseño experimental son utilizados como parte de un trabajo académico, se solicita citar al autor y al Trabajo Final de Investigación asociado.

La referencia bibliográfica definitiva deberá actualizarse cuando el Trabajo Final de Investigación sea publicado formalmente.

## Licencia

Uso académico y no comercial.

Consulte [`LICENSE`](./LICENSE) para conocer los términos completos.
