# API Contract – Storage Service (v1)

**Version:** 1.1 (frozen)  
**Last updated:** November 2025  
**Owner:** Trackunit Storage Service  
**Scope:** Internal service-to-service contract used by Ingestion and Media Access.  
**Status:** Stable – breaking changes require version bump to /v2/.

---

## Overview

The Storage service is responsible for issuing **short-lived presigned URLs** for uploading and retrieving objects (e.g., media files).  
It does **not** store metadata or persist application state — it simply provides access to object storage.

---

## Base URL

```bash
http://localhost:5136/internal/v1/storage
```

Replace host and port when deployed, for example:
```bash
https://storage.<env>.trackunit.internal/internal/v1/storage
```

---

## Endpoints

### 1. POST /internal/v1/storage/presign-put

Create a **presigned PUT URL** for uploading an object to the storage bucket.

#### Description
Generates a temporary, signed S3-compatible URL that allows a client (e.g., a mobile app) to upload an object directly to object storage (MinIO, S3, etc.).

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

#### Response 200 OK
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

#### Response 422 Unprocessable Entity
```json
{
  "errors": {
    "key": ["Required"],
    "contentType": ["MustStartWithImageSlash"],
    "ttlSec": ["Range1To3600"]
  }
}

```

#### Validation error codes
| Field | Possible codes | Description |
|--------|----------------|-------------|
| `key` | `Required`, `MaxLengthExceeded`, `InvalidPathShape`, `InvalidPathTraversal`, `InvalidCharacterSet` | Object key failed domain validation. |
| `contentType` *(PUT only)* | `Required`, `MustStartWithImageSlash` | Content type must be an image MIME type. |
| `ttlSec` | `Range1To3600` | TTL must be between 1 and 3600 seconds. |

#### Response 500 Internal Server Error
See below

---

### 2. POST /internal/v1/storage/presign-get

Create a **presigned GET URL** for downloading an object from the storage bucket.

#### Description
Generates a temporary, signed S3-compatible URL that allows internal clients (e.g., Media Access service) to retrieve an object directly from object storage (MinIO, S3, etc.).  
Used in **Use Case 2 – Media Access** to grant short-lived download permissions.

#### Request
```json
{
  "key": "images/2025/11/06/sample.jpg",
  "ttlSec": 300
}
```

#### Parameters
| Field | Type | Required | Description |
|-------|------|-----------|-------------|
| `key` | string | yes | Path of the object within the bucket (validated in domain). |
| `ttlSec` | integer | yes | Time-to-live in seconds (1 – 3600). Defines how long the presigned URL remains valid. |

#### Response 200 OK
```json
{
  "url": "http://localhost:9000/trackunit-images/images/2025/11/06/sample.jpg?...",
  "expiresAt": "2025-11-06T19:32:12Z"
}
```

#### Response fields
| Field | Type | Description |
|-------|------|-------------|
| `url` | string | The full presigned GET URL. |
| `expiresAt` | string (ISO 8601) | UTC timestamp when the URL expires. |

#### Response 422 Unprocessable Entity
```json
{
  "errors": {
    "key": ["Required"],
    "ttlSec": ["Range1To3600"]
  }
}
```

---

### 3. GET /health

Simple health probe used by orchestrators or load balancers.

#### Response 200 OK
Plain text:
```text
Healthy
```

---

## Common error responses (shared across all endpoints)

#### Response 500 Internal Server Error
```json
{
  "type": "about:blank",
  "title": "Internal Server Error",
  "status": 500,
  "detail": "Unexpected failure while generating presigned URL."
}
```

---

## Example sequences

### Use case 1: upload

1. **Ingestion Service** calls:  
   ```
   POST /internal/v1/storage/presign-put  
   ```
   to obtain a temporary upload URL.

2. **Client** (e.g., mobile app) uploads directly to that URL using HTTP PUT.

3. **Ingestion Service** confirms completion via /v1/uploads/confirm (separate contract).

---

### Use case 2: media access

1. **Media Access Service** calls:  
   ```
   POST /internal/v1/storage/presign-get  
   ```
   to obtain a temporary download URL for an object.

2. **Authorized downstream service** (e.g., AI, visualization, etc.) fetches the object directly using HTTP GET.

3. The presigned URL expires automatically after its TTL, preventing further access.

---

## Notes

- URLs expire automatically — the service holds **no persistent state**.  
- The Storage service performs only **validation and presigning**, not actual uploads.  
- Internal-only endpoint (not exposed to public clients).  
- For security, future versions will require internal authentication (e.g., X-Internal-Token header).

---

## Changelog

| Date | Version | Changes |
|------|----------|----------|
| 2025-10-17 | v1.0 | Initial frozen contract for /presign-put |
| 2025-11-06 | v1.1 | Added /presign-get endpoint |

---

## Reference
For service overview and related services, see [README.md](../../README.md).

---

**End of document**
