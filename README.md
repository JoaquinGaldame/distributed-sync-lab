# Distributed Sync Lab

Laboratorio experimental distribuido desarrollado como parte del Trabajo Final de Investigación de la carrera **Licenciatura en Ciencias de la Computación** de la Facultad de Ciencias Exactas, Físicas y Naturales de la Universidad Nacional de San Juan.

**Autor:** Joaquín Antonio Galdame

## Investigación

El laboratorio forma parte del trabajo:

**“Mecanismo de sincronización confiable de datos con servicios externos para una PyME de desarrollo de software de la provincia de San Juan”.**

La investigación estudia el problema de mantener información sincronizada entre sistemas internos y múltiples servicios externos independientes, considerando situaciones características de los sistemas distribuidos como:

* fallos parciales;
* reintentos y operaciones duplicadas;
* procesamiento de actualizaciones fuera de orden;
* restricciones de consumo y rate limiting;
* diferencias temporales entre el estado interno y el estado externo.

El objetivo experimental es obtener evidencia que permita analizar distintas estrategias arquitectónicas de sincronización y, posteriormente, fundamentar el diseño de un mecanismo de sincronización confiable aplicable al contexto de una PyME de desarrollo de software.

## Estrategias estudiadas

La investigación contempla tres estrategias arquitectónicas principales:

1. procesamiento batch centralizado;
2. orquestación síncrona mediante HTTP;
3. arquitectura orientada a eventos con comunicación asíncrona.

El laboratorio deberá permitir ejecutar cargas equivalentes sobre estas estrategias bajo condiciones controladas y reproducibles.

## Dataset experimental

Como fuente de datos de entrada se utilizará un conjunto de datos público proveniente de **Inside Airbnb**.

El dataset permite trabajar con registros de alojamientos reales en lugar de utilizar exclusivamente entidades inventadas para generar la carga experimental.

Los datos utilizados por el laboratorio serán transformados a un modelo canónico reducido que contenga únicamente los atributos necesarios para estudiar el proceso de sincronización.

El repositorio no tiene como objetivo estudiar el mercado de Airbnb ni reproducir el comportamiento interno de la plataforma Airbnb.

Inside Airbnb funciona exclusivamente como fuente del conjunto de datos de entrada utilizado para construir workloads reproducibles.

### Procedencia y reproducibilidad

El dataset original no se almacena directamente en este repositorio.

Para cada conjunto de datos utilizado experimentalmente se documentarán, como mínimo:

* fuente de procedencia;
* snapshot o fecha de publicación;
* archivo utilizado;
* hash criptográfico del archivo original;
* cantidad de registros;
* reglas de selección;
* transformaciones realizadas;
* atributos utilizados por el laboratorio.

Esto permitirá identificar exactamente el conjunto de datos utilizado en cada ejecución experimental.

## Alcance del laboratorio

El laboratorio implementa un sistema distribuido controlado compuesto por:

* un PMS académico que representa al sistema interno;
* un worker principal encargado de iniciar o coordinar sincronizaciones;
* servicios especializados de sincronización;
* servicios externos simulados;
* PostgreSQL para persistencia;
* RabbitMQ para mensajería asíncrona;
* un Experiment Runner para ejecutar escenarios controlados;
* mecanismos de inyección de fallos;
* instrumentación para recopilar métricas y resultados.

Los servicios externos simulados no son clones de Airbnb, Booking.com ni de otras plataformas comerciales.

Su propósito es modelar clases de comportamiento relevantes para la investigación, entre ellas:

* actualizaciones completas;
* actualizaciones parciales;
* procesamiento síncrono;
* procesamiento asíncrono;
* consistencia eventual;
* rate limiting;
* timeouts;
* resultados desconocidos;
* fallos parciales;
* procesamiento duplicado o fuera de orden.

## Principios experimentales

El desarrollo del laboratorio sigue los siguientes principios:

* misma entrada experimental para las estrategias comparadas;
* mismo modelo de datos canónico;
* separación entre lógica experimental y sistema bajo prueba;
* configuraciones versionadas;
* ejecuciones reproducibles;
* aislamiento de persistencia entre componentes;
* instrumentación común;
* identificación de cada experimento y operación mediante IDs de correlación;
* resultados exportables en formatos abiertos como JSON y CSV.

Las diferencias observadas deberán ser atribuibles, en la medida de lo posible, a las estrategias evaluadas y no a diferencias accidentales de datos, contratos o infraestructura.

## Tecnologías principales

* Node.js
* TypeScript
* Fastify
* PostgreSQL
* RabbitMQ
* Docker
* Docker Compose
* Kubernetes / Minikube
* Toxiproxy
* Prometheus
* Grafana
* Vitest

Durante la ejecución experimental oficial se congelarán las versiones relevantes del entorno.

Cada aplicación del laboratorio es un proyecto aislado: mantiene sus propias dependencias, su configuración y sus comandos de compilación y ejecución. El repositorio no utiliza un workspace JavaScript/TypeScript ni requiere un gestor de paquetes común en la raíz. Para preparar o ejecutar una aplicación, se deben seguir las instrucciones y utilizar las herramientas definidas por ese proyecto.

## Estructura prevista

```text
distributed-sync-lab/
├── apps/
│   ├── academic-pms/
│   ├── main-worker/
│   ├── ota-replace-service/
│   ├── ota-async-service/
│   ├── ota-simulator/
│   └── experiment-runner/
│
├── packages/
│   ├── contracts/
│   ├── telemetry/
│   ├── experiment-model/
│   ├── test-fixtures/
│   └── config/
│
├── data/
│   ├── raw/
│   └── generated/
│
├── infrastructure/
│   ├── compose/
│   └── kubernetes/
│
├── experiments/
│   ├── scenarios/
│   ├── workloads/
│   └── seeds/
│
├── analysis/
├── results/
└── docs/
```

## Estado

El proyecto se encuentra actualmente en fase inicial de construcción del laboratorio experimental.

La implementación se realizará incrementalmente, comenzando por:

1. definición y preparación reproducible del dataset;
2. definición del modelo canónico;
3. construcción del PMS académico;
4. construcción de los servicios externos simulados;
5. implementación de las estrategias de sincronización;
6. creación del Experiment Runner;
7. inyección controlada de fallos;
8. instrumentación y recopilación de métricas;
9. ejecución de experimentos.

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
