# Tradie.Scanner

Tradie.Scanner is the stash tab river parsing aspect of Tradie.

A console app runs forever on a small ECS instance that makes a request to the public stash tab API at the previously received section to parse through the river.

The next ID is parsed, and then the stashes portion of the JSON file is uploaded to S3 (with Brotli compression) for further processing. In order to ensure this scanner can keep up with the river,
no "real" JSON parsing beyond finding the start/end of the value occurs in this program.

## Flow

* Tradie.Scanner runs as a .NET Core BackgroundService.
* Changesets are polled from the PoE Stash Tab API.
* Stash tabs are upload to an S3 bucket, which triggers the next step.

## Contributing / License

Source code is provided for reference only. Distribution of the software is not allowed.

Feel free to get ideas from this repository, but Tradie is not truly open source.
