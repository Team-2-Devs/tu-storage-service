# API Contract – Storage Service (v1)

**Version:** 1.0 (frozen)  
**Last updated:** October 2025  
**Owner:** Trackunit Storage Service  
**Scope:** Internal service-to-service contract used by Ingestion and Media Access.  
**Status:** Stable – breaking changes require version bump to `/v2/`.

---

## Overview

The Storage service is responsible for issuing **short-lived presigned URLs** for uploading and retrieving objects (e.g., media files).  
It does **not** store metadata or persist application state — it simply provides access to object storage.

---

## Base URL

```bash
http://localhost:5136/internal/v1/storage
```

> Replace host and port when deployed (e.g., `https://storage.<env>.trackunit.internal/internal/v1/storage`).

---

## Endpoints

### 1. `POST /internal/v1/storage/presign-put`

Create a **presigned PUT URL** for uploading an object to the storage bucket.

#### Description
Generates a temporary, signed S3-compatible URL that allows a client (e.g. a mobile app) to upload an object directly to object storage (MinIO, S3, etc.).

Invoked by the Ingestion service, which requests presigned URLs on behalf of upload clients.

#### Request
```json
{
  "key": "images/2025/10/17/sample.jpg",
  "contentType": "image/jpeg",
  "ttlSec": 300
}
```

#### Parameters
| Field | Type | Required | Description |
|-------|------|-----------|--------------|
| `key` | string | yes | Path of the object within the bucket (validated in domain). |
| `contentType` | string | yes | MIME type, must start with `image/` for now. |
| `ttlSec` | integer | yes | Time-to-live in seconds (1 – 3600). Defines how long the presigned URL remains valid. |

#### Response `200 OK`
```json
{
  "url": "http://localhost:9000/trackunit-images/images/2025/10/17/sample.jpg?X-Amz-Algorithm=AWS4-HMAC-SHA256&X-Amz-Credential=...",
  "expiresAt": "2025-10-17T18:49:57Z"
}
```

| Field | Type | Description |
|-------|------|--------------|
| `url` | string | The full presigned PUT URL. |
| `expiresAt` | string (ISO 8601) | UTC timestamp when the URL expires. |

#### Response `422 Unprocessable Entity`
```json
{
  "errors": {
    "key": ["Key cannot be empty"],
    "contentType": ["Unsupported content type"]
  }
}
```

#### Response `500 Internal Server Error`
```json
{
  "type": "about:blank",
  "title": "Internal Server Error",
  "status": 500,
  "detail": "Unexpected failure while generating presigned URL."
}
```

---

### 2. `POST /internal/v1/storage/presign-get` *(planned)*

Reserved for future use.  
Will issue presigned GET URLs for retrieving uploaded media.

---

### 3. `GET /health`

Simple health probe used by orchestrators or load balancers.

#### Response `200 OK`
Plain text:
```
Healthy
```

---

## Example sequence (use case 1: upload)

1. **Ingestion Service** calls:
   ```
   POST /internal/v1/storage/presign-put
   ```
   to obtain a temporary upload URL.

2. **Client** (e.g., mobile app) uploads directly to that URL using HTTP PUT.

3. **Ingestion Service** confirms completion via `/v1/uploads/confirm` (separate contract).

---

## Notes

- URLs expire automatically — the service holds **no persistent state**.  
- The Storage service performs only **validation and presigning**, not actual uploads.  
- Internal-only endpoint (not exposed to public clients).  
- For security, future versions will require internal authentication (e.g., `X-Internal-Token` header).

---

## Changelog

| Date | Version | Changes |
|------|----------|----------|
| 2025-10-17 | v1.0 | Initial frozen contract for `/presign-put` |

---

## Reference
For service overview and related services, see [README.md](../../README.md).

---

**End of document**
