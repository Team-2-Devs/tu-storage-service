# DEV.md – Local Development Guide (Storage Service)

This guide explains how to run the **Trackunit Storage Service** locally in two supported modes:

* **Option A (Recommended): Docker Compose** – runs Storage.Api + MinIO
* **Option B: dotnet run** – runs Storage.Api in your IDE for debugging

Each mode uses a different configuration method. Use only one at a time.

---

## 1. Prerequisites

* Docker Desktop
* .NET 8 SDK
* curl (Git Bash or PowerShell 7+)

Verify installation:

```
docker version
dotnet --version
```

---

## 2. Configuration Overview

### Docker Compose uses `.env`

* Contains MinIO credentials and internal API key.
* `.env` is **not committed**.
* Loaded automatically by Docker Compose.

### dotnet run uses **User Secrets**

* Only used for IDE debugging.
* Secrets stored outside the repository.

Do **not** mix these methods.

---

## 3. Option A – Run Storage + MinIO using Docker Compose (Recommended)

### 3.1 Create `.env` from template

```
cp .env.example .env
```

Edit `.env` and provide local development values.
The example contains placeholders such as:

```
MINIO_ROOT_USER=minioadmin
MINIO_ROOT_PASSWORD=minioadmin
INTERNAL_AUTH_API_KEY=changeme
```

### 3.2 Start the environment

```
docker compose up --build
```

This will:

* Build the Storage API container
* Start Storage API on port 8080
* Start MinIO on ports 9000/9001
* Inject credentials from `.env`

### 3.3 Verify Storage

```
curl http://localhost:8080/health
```

Expected:

```
Missing Internal token
```

(This indicates Storage.Api is running.)  
(Note: /health currently requires the internal auth token. This may change later.)


### 3.4 Verify MinIO

Open:

```
http://localhost:9001
```

Log in using the credentials from `.env`.

Create bucket `trackunit-images` **once**, if it does not already exist.

### 3.5 Stop the environment

```
docker compose down
```

Reset MinIO (optional - would require bucket creation again):

```
docker compose down -v
```

---

## 4. Option B – Run Storage with `dotnet run` (Debug Mode)

### 4.1 Start MinIO only

```
docker compose up -d minio
```

### 4.2 Configure User Secrets
Change < values >

```
dotnet user-secrets set "Minio:AccessKey" "<your-access-key>" --project src/Storage.Api
dotnet user-secrets set "Minio:SecretKey" "<your-secret-key>" --project src/Storage.Api
dotnet user-secrets set "InternalAuth:ApiKey" "<your-internal-auth-key>" --project src/Storage.Api
```

Verify:

```
dotnet user-secrets list --project src/Storage.Api
```

### 4.3 Run the API

```
dotnet run --project src/Storage.Api
```

Health check:

```
curl http://localhost:<port>/health
```

Expected:

```
Missing Internal token
```
(This indicates Storage.Api is running.)  
(Note: /health currently requires the internal auth token. This may change later.)

---

## 5. Smoke Tests

### 5.1 Request a presigned PUT URL

```
curl -s http://localhost:8080/internal/v1/storage/presign-put \
  -H "Content-Type: application/json" \
  -H "X-Internal-Token: <your-internal-auth-key>" \
  -d '{"key":"test.jpg","contentType":"image/jpeg","ttlSec":600}'
```

### 5.2 Upload a file

```
curl -T "/c/Users/<you>/Desktop/sample.jpg" \
  -H "Content-Type: image/jpeg" \
  "<presigned-url>"
```

### 5.3 Request a presigned GET URL

```
curl -s http://localhost:8080/internal/v1/storage/presign-get \
  -H "Content-Type: application/json" \
  -H "X-Internal-Token: <your-internal-auth-key>" \
  -d '{"key":"test.jpg","ttlSec":300}'
```

### 5.4 Download the file

```
curl -o "/c/Users/<you>/Desktop/downloaded.jpg" "<presigned-url>"
```

---

## 6. Troubleshooting

| Issue                   | Cause                 | Resolution                             |
| ----------------------- | --------------------- | -------------------------------------- |
| `Request has expired`   | TTL too short         | Increase `ttlSec`                      |
| `AccessDenied`          | Bucket missing        | Create `trackunit-images` in MinIO     |
| `SignatureDoesNotMatch` | Content-Type mismatch | Match Content-Type on presign + upload |

---

## 7. Developer Checklist

* [ ] `.env` created from `.env.example` (Option A)
* [ ] User Secrets configured (Option B)
* [ ] Storage.Api reachable on port 8080
* [ ] MinIO console reachable on port 9001
* [ ] Bucket `trackunit-images` exists
* [ ] Presign PUT works
* [ ] Upload works
* [ ] Presign GET works
* [ ] File download verified

---

## Reference
For service overview and related services, see [README.md](../README.md).

---
**Updated:** November 2025 (11/18)
