## Task 1: Document Storage REST API (S3 Mock)

### Features
- Upload any file  
- Automatic versioning (`v1`, `v2`, `v3`…)  
- Permanent auto-generated download links  
- Metadata + version history stored in SQL Server  
- Files saved locally in `./Storage` folder  

### Endpoints

| Method | Endpoint                                   | Description                          |
|--------|--------------------------------------------|--------------------------------------|
| POST   | `/api/documents/upload`                    | Upload file (form-data key: `file`)  |
| GET    | `/api/documents/download/{filename}`       | Download any version                 |
| GET    | `/api/documents`                           | List all files & versions            |
| GET    | `/api/documents/{filename}/versions`       | List all versions of one file        |