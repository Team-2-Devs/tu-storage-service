# tu-storage-service
![CI](https://github.com/Team-2-Devs/tu-storage-service/actions/workflows/ci.yml/badge.svg)

Storage microservice for Trackunit.

## Purpose
- Issue short-lived pre-signed PUT/GET URLs for object storage.  
- Enforce consistent object key schemes and TTLs.  
- Serve as an internal utility service behind the platform gateway.  

## Endpoints (v1)
*Note: endpoints are defined here as part of the design. They are not yet implemented unless otherwise stated.*

- POST /internal/v1/storage/presign-put – create pre-signed URL for uploading an object  
- POST /internal/v1/storage/presign-get – create pre-signed URL for retrieving an object  
- GET  /health – service health check  

## Tech
- .NET 8, ASP.NET Core Web API  
- Clean/hexagonal layering: Api, Application, Domain, Infrastructure  
- Stateless service (issues URLs without persisting state)  
- CI via reusable org workflow (see [Team-2-Devs/.github](https://github.com/Team-2-Devs/.github))

## Related services
- [tu-ingestion-service](https://github.com/Team-2-Devs/tu-ingestion-service) – handles upload initiation and confirmation, publishes events
- [tu-media-access-service](https://github.com/Team-2-Devs/tu-media-access-service) – provides authorized access to media via pre-signed GET from Storage


## Local dev
```bash
dotnet restore
dotnet build
dotnet run --project src/Storage.Api
```