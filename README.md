# tradie

Tradie is a partially-functional Path of Exile indexer / trade site. The design is primarily event-driven, and the storage mechanism is a custom
block storage data structure for efficient searches designed around filtering out blocks that cannot match the query.
In current tests, most weighted sum searches take ~100ms against a data store of 150,000,000 items (though some particularly unpleasantly crafted queries may take multiple seconds).

**As access to the direct whisper API is blocked, and the only ways of getting character information for performing trades has now been deprecated, this project is no longer in active development, though I may occasionally go back to it.**

## Flow

The flow looks something like this:
- Tradie.Scanner is an ECS service that pulls from the stash tab river every half a second to get the latest changes. It performs minimal parsing, sending the file to S3.
- Tradie.Analyzer is a Lambda that picks up the S3 file and parses each item. It finds new base types, modifiers, modifier ranges, etc, to build a database without requiring data mining. Instead, as new uniques / content are added in, the analyzer automatically builds the database. Raw items are then written to Kinesis.
- Tradie.ItemLogBuilder is a service on a cron job that pulls from the Kinesis data streams, writing the item data to a Postgres database (in a compressed MessgePack binary format).
- Tradie.Indexer pulls data from the Postgres database into memory, in a tree designed to filter out large chunks of items for arbitrary queries.
- Tradie.Web is the web API entrypoint, which directs searches to a Tradie.Indexer instance, which it finds via AWS CloudMap.
- Tradie.Client is the frontend for the actual user-facing application, and communicated with the Web service via grpc-web.

## Infrastructure

All cloud infrastructure is set up with cdktf in the Tradie.Infrastructure project. The cloud infrastructure is hosted on AWS, with some services running on a dedicated server with much better cost-performance ratios. Given that the stash tab river pulls dozens of terabytes per month, **hosting this is not cheap** (super rough estimate is ~$500 a month, but it may be higher). Traditionally, AWS had extremely poor IO performance for RDS, so all data storage was hosted on a dedicated server (including the ItemLogBuilder), but this may no longer be necessary with GP3 storage. Cheap dedicated servers also provide considerably better CPU/RAM, which is important for the Indexer.
