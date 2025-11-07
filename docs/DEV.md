# Developer Guide – Storage Service

This document explains how to run the **Trackunit Storage Service** locally, connect it to a local MinIO instance, and perform a smoke test to verify presigned URL generation and uploads.

---

## 1. Prerequisites

- **Docker Desktop** (WSL2 + Virtual Machine Platform enabled)
- **.NET 8 SDK**
- Optional: `curl` (included in Git Bash / PowerShell 7+)

Check installation:
```bash
docker version
dotnet --version
```

---

## 2. Local infrastructure (MinIO)

> **Note:** Ensure Docker Desktop is running before executing this command:

**Start MinIO:**
```
docker compose up -d
```

**Verify:**
```bash
docker ps
```

Expected output includes:
```
IMAGE                PORTS
minio/minio:latest   0.0.0.0:9000-9001->9000-9001/tcp
```

**Access MinIO Console:**
- URL: http://localhost:9001
- Username: `minioadmin`
- Password: `minioadmin`

**Default bucket:**
`trackunit-images`

If it doesn’t exist, create it once from the console.

---

## 3. Configure secrets

The service uses .NET User Secrets for local credentials.

Running the following commands automatically creates the secret store if it does not already exist:

Run once:
```bash
dotnet user-secrets set "Minio:AccessKey" "minioadmin" --project src/Storage.Api
dotnet user-secrets set "Minio:SecretKey" "minioadmin" --project src/Storage.Api
```

Verify:
```bash
dotnet user-secrets list --project src/Storage.Api
```

Expected keys:
```
Minio:AccessKey = minioadmin
Minio:SecretKey = minioadmin
```

---

## 4. Run the API

Start the service:
```bash
dotnet run --project src/Storage.Api
```

Check health:
```bash
curl -s http://localhost:5136/health
```

Expected output:
```
Healthy
```

---

## 5. Smoke tests

> Note: Create or download a sample JPEG to use for testing. Place it on Desktop for ease of testing or note its path and change commands accordingly.

### 5.1 Presign PUT → Upload

Verify that the service can issue a presigned PUT URL and MinIO accepts the upload.

**Step 1 – Request a presigned PUT URL**
```bash
curl -s http://localhost:5136/internal/v1/storage/presign-put -H "Content-Type: application/json" -d '{"key":"images/2025/11/06/sample.jpg","contentType":"image/jpeg","ttlSec":300}'
```

Expected response (example):
```json
{
  "url": "http://localhost:9000/trackunit-images/images/2025/11/06/sample.jpg?...",
  "expiresAt": "2025-11-06T18:49:57Z"
}
```

**Step 2 – Upload the file**

Git Bash:
```bash
curl -T "/c/Users/<you>/Desktop/sample.jpg" -H "Content-Type: image/jpeg" "<paste-url-here>"
```

PowerShell:
```powershell
curl.exe -T "C:\Users\<you>\Desktop\sample.jpg" -H "Content-Type: image/jpeg" "<paste-url-here>"
```

Expected: silent success (`HTTP 200` or `204`).

**Step 3 – Verify in MinIO Console**

Open http://localhost:9001 → bucket `trackunit-images` → confirm the object  
`images/2025/11/06/sample.jpg` exists.

---

### 5.2 Presign GET → Download

Verify that the service can issue a presigned GET URL and that the uploaded object can be retrieved.

**Step 1 – Request a presigned GET URL**
```bash
curl -s http://localhost:5136/internal/v1/storage/presign-get -H "Content-Type: application/json" -d '{"key":"images/2025/11/06/sample.jpg","ttlSec":300}'
```

Expected response (example):
```json
{
  "url": "http://localhost:9000/trackunit-images/images/2025/11/06/sample.jpg?...",
  "expiresAt": "2025-11-06T19:32:12Z"
}
```

**Step 2 – Download the object**
```bash
curl -o downloaded.jpg "<paste-url-here>"
```

Expected: silent success (`HTTP 200`).

**Step 3 – Verify file integrity**
```bash
sha256sum sample.jpg downloaded.jpg
```

Expected: identical checksums for both files.

---

## 6. Troubleshooting

| Problem | Cause | Fix |
|----------|--------|-----|
| `Request has expired` | The presigned URL TTL expired before use | Re-issue presigned URL with higher `ttlSec` |
| `AccessDenied` | Bucket missing | Create bucket `trackunit-images` once in MinIO console |
| `SignatureDoesNotMatch` | Wrong `Content-Type` used | Match header between presign and upload |


---

## 7. Tear down

Stop MinIO:
```bash
docker compose down
```

Remove all volumes (optional):
```bash
docker compose down -v
```

---

## 8. Developer checklist

- [ ] `docker compose up -d`
- [ ] Secrets configured via user-secrets
- [ ] `dotnet run` → `/health` returns Healthy
- [ ] Presign URL works
- [ ] Upload succeeds via `curl`
- [ ] Object visible in MinIO Console

---

## Reference
For service overview and related services, see [README.md](../README.md).

---

**Updated:** November 2025
