# Benchmarking Episerver serializable carts #

In newer Episerver commerce versions, you work with an `ICart` instead of a `Cart`. A `Cart` is an `ICart`, but an `ICart` can be a `SerializableCart` as well. A `Cart` is stored in the old OrderGroup tables, whereas the `SerializableCart` is stored as Json within its own SerializableCart table.

This project aims to benchmark the performance improvements of Episervers new serializable carts systems. I will compare:

1. The old cart helper
2. The new order repository
3. The new order repository combined with the serializable carts feature.

In order to create good measurements, I will benchmark the same operations performed through the three different cart systems. These operations include:

1. Create an empty cart and persist it
2. Add a line item with a meta field to that cart and persist it
3. Validate the cart and apply campaigns and persist it
4. Apply payment and process it and persist the cart
5. Persist the cart as a purchase order

I find it difficult to benchmark only one of these operations at a time, therefore when I execute a step, I also execute the steps prior to that step - i.e. in order to benchmark step 3, I also run step 1 and 2 before.

## Definition of success ##

If operations 1 to 3 (the most common operations) are quicker when using serializable carts, I think we can consider it a good improvement.

To measure the performance, I execute the operations in a Web API and stress the API using Apache Benchmark:

`ab.exe -c 50 -n 1000 http://localhost:XXXXX/api/xxxxx`

* `-c 50` indicates 50 clients
* `-n 1000` indicates the total number of requests

The output of each benchmark will be the time it takes to process 1000 requests. 

## Results ##

|  | Cart helper | Order repository | Serializable carts |
|-----|---|---|---|
| 1 | 00:00:02.910 | 00:00:03.349 | 00:00:01.991 |
| 2 | 00:00:04.972 | 00:00:05.354 | 00:00:02.534 |
| 3 | 00:00:10.099 | 00:00:11.883 | 00:00:04.306 |
| 4 | 00:00:16.519 | 00:00:16.692 | 00:00:04.331 |
| 5 | 00:00:20.217 | 00:00:19.387 | 00:00:08.422 |

## Contribute ##

I'm no expert on neither benchmarking nor Episerver Commerce and there can be things to improve. Please create a pull request if you think you can improve these benchmarks.

In order to try this yourself, follow the steps below:

1. `git clone http://thisrepo.com`
2. `PM> Install-EPiDatabases`
3. `PM> Update-EPiDatabases`

## Conclusions ##

The results speaks for themselves. Serializable carts performs really well compared to the other two. The newer order repository (without serializable carts enabled) is just slightly faster than the cart helper. Just getting rid of cart helper namespaces won't bring you any performance improvements.

I think that serializable carts are mature enough to be used in production. There are drawbacks though. The serialized cart is stored as json in the database, so if you want to query carts based on some metadata, that will be tricky. If you don't need to do that, go with serializable carts!