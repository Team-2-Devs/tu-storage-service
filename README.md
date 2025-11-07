# Trackunit Storage Service
![CI](https://github.com/Team-2-Devs/tu-storage-service/actions/workflows/ci.yml/badge.svg)

Storage microservice for Trackunit.

## Status
- Active Development

## Purpose
- Issue short-lived pre-signed PUT/GET URLs for object storage 
- Enforce consistent object key schemes and TTLs

## Endpoints (v1)
*Note: endpoints are defined here as part of the design. They are not yet implemented unless otherwise stated.*

- POST /internal/v1/storage/presign-put – create pre-signed URL for uploading an object  
- POST /internal/v1/storage/presign-get – create pre-signed URL for downloading an object  
- GET  /health – service health check  

## Tech
- .NET 8, ASP.NET Core Web API  
- Clean/hexagonal layering – Api, Application, Domain, Infrastructure
- Stateless service (issues URLs without persisting state)  
- CI via reusable org workflow (see [Team-2-Devs/.github](https://github.com/Team-2-Devs/.github))

## Related services
- [tu-ingestion-service](https://github.com/Team-2-Devs/tu-ingestion-service) – handles upload initiation and confirmation, publishes events
- [tu-media-access-service](https://github.com/Team-2-Devs/tu-media-access-service) – provides authorized access to media via pre-signed GET from Storage

## Local dev
```bash
dotnet run --project src/Storage.Api
```

## Developer setup
For local infrastructure (MinIO) and smoke test instructions, see [DEV.md](./docs/DEV.md).

## API Contracts
Formal versioned specifications of service-to-service interfaces. 
See [v1-storage.md](./docs/api-contracts/v1-storage.md).

Frozen contract for `/internal/v1/storage` endpoints:
- `POST /presign-put` (implemented)
- `POST /presign-get` (implemented)
- `GET  /health` (implemented)