# Tradie.Infrastructure

Tradie.Infrastructure is a cdk-tf (Terraform generated via code) IaC project for all Tradie infrastructure.

No solution exists for this project, and due to some terraform asset limitations, this project is
outside of the solution and directory for all other Tradie projects.

Currently this project handles both the building and deploying of Tradie. In the future, one or both
of these will be moved into a proper CI/CD pipeline, which may just use this project.

## Terraform Providers

Tradie.Infrastructure uses the following Terraform providers:
- AWS (primary provider; prebuilt)
- Random (generate secret keys; prebuilt)
- Null (handle builds and publishes; generated as prebuilt is incompatible with ARM64)

## Contributing / License

Source code is provided for reference only. Distribution of the software is not allowed.

Feel free to get ideas from this repository, but Tradie is not truly open source.
