# Dataset de Inside Airbnb

Este directorio contiene los archivos fuente utilizados para preparar el estado inicial de Academic PMS. Los datos corresponden al snapshot de **Buenos Aires, Ciudad Autónoma de Buenos Aires, Argentina**, publicado el **29 de junio de 2026**.

## Página de descarga

1. Ingresá a [Inside Airbnb — Get the Data](https://insideairbnb.com/es/get-the-data/).
2. Buscá **Buenos Aires, Ciudad Autónoma de Buenos Aires, Argentina**.
3. Seleccioná el snapshot del **29 June, 2026**.

También podés acceder a la página de exploración de la ciudad desde [Explore Buenos Aires](https://insideairbnb.com/buenos-aires/).

## Archivos publicados para el snapshot

| Ciudad | Archivo | Descripción |
| --- | --- | --- |
| Buenos Aires | [listings.csv.gz](https://data.insideairbnb.com/argentina/ciudad-aut%C3%B3noma-de-buenos-aires/buenos-aires/2026-06-29/data/listings.csv.gz) | Detailed Listings data. |
| Buenos Aires | [calendar.csv.gz](https://data.insideairbnb.com/argentina/ciudad-aut%C3%B3noma-de-buenos-aires/buenos-aires/2026-06-29/data/calendar.csv.gz) | Detailed Calendar Data. |
| Buenos Aires | [reviews.csv.gz](https://data.insideairbnb.com/argentina/ciudad-aut%C3%B3noma-de-buenos-aires/buenos-aires/2026-06-29/data/reviews.csv.gz) | Detailed Review Data. |
| Buenos Aires | [listings.csv](https://data.insideairbnb.com/argentina/ciudad-aut%C3%B3noma-de-buenos-aires/buenos-aires/2026-06-29/visualisations/listings.csv) | Summary information and metrics for listings in Buenos Aires (good for visualisations). |
| Buenos Aires | [reviews.csv](https://data.insideairbnb.com/argentina/ciudad-aut%C3%B3noma-de-buenos-aires/buenos-aires/2026-06-29/visualisations/reviews.csv) | Summary Review data and Listing ID (to facilitate time based analytics and visualisations linked to a listing). |
| Buenos Aires | [neighbourhoods.csv](https://data.insideairbnb.com/argentina/ciudad-aut%C3%B3noma-de-buenos-aires/buenos-aires/2026-06-29/visualisations/neighbourhoods.csv) | Neighbourhood list for geo filter. Sourced from city or open source GIS files. |
| Buenos Aires | [neighbourhoods.geojson](https://data.insideairbnb.com/argentina/ciudad-aut%C3%B3noma-de-buenos-aires/buenos-aires/2026-06-29/visualisations/neighbourhoods.geojson) | GeoJSON file of neighbourhoods of the city. |

## Archivos requeridos por el laboratorio

Solamente se utilizan estos dos archivos:

| Orden | Descarga requerida | Preparación | Ruta local esperada | SHA-256 esperado |
| ---: | --- | --- | --- | --- |
| 1 | [`visualisations/listings.csv`](https://data.insideairbnb.com/argentina/ciudad-aut%C3%B3noma-de-buenos-aires/buenos-aires/2026-06-29/visualisations/listings.csv) | No requiere descompresión. No utilizar `data/listings.csv.gz`, porque contiene el detalle completo y no es el archivo seleccionado para este experimento. | `data/inside-airbnb/listings.csv` | `845472c69e282ba85caf651c918701f0ae81b86e123b238f1eec8ef176e28a00` |
| 2 | [`data/calendar.csv.gz`](https://data.insideairbnb.com/argentina/ciudad-aut%C3%B3noma-de-buenos-aires/buenos-aires/2026-06-29/data/calendar.csv.gz) | Descomprimir el archivo y conservar el resultado con el nombre `calendar.csv`. | `data/inside-airbnb/calendar.csv` | `610564cebd7c2224006ebc7d4d090c8949aa3fd1c299e41134113050a2ef5d38` |

El hash del calendario corresponde al archivo `calendar.csv` descomprimido. Si alguno de los hashes no coincide, no realices la importación: el archivo puede pertenecer a otro snapshot o haber quedado incompleto.

`calendar.csv` y `calendar.csv.gz` no se versionan en Git debido a su tamaño. Cada desarrollador debe descargarlos localmente siguiendo estas instrucciones.

Para los comandos de descarga, descompresión, validación e importación, consultá la [guía de importación de Academic PMS](../../docs/apps/academic-pms/importacion-inside-airbnb.md).
