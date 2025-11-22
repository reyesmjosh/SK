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

### Example: Upload File

POST https://localhost:5001/api/documents/upload
Content-Type: multipart/form-data

form-data:
  file = report.pdf

### Sample Response

#### /api/documents/upload`

{
  "message": "File uploaded successfully.",
  "downloadLink": "https://localhost:5001/api/documents/download/report.pdf_v1",
  "version": 1,
  "metadata": {
    "size": 1258304,
    "contentType": "application/pdf",
    "uploadTimestamp": "2025-04-05T14:22:10.123Z"
  }
}

#### /api/documents

[
  {
    "fileName": "report.pdf",
    "version": 3,
    "size": 1258304,
    "contentType": "application/pdf",
    "uploadTimestamp": "2025-04-05T15:10:22Z",
    "downloadLink": "https://localhost:5001/api/documents/download/report.pdf_v3"
  },
  {
    "fileName": "logo.png",
    "version": 1,
    "size": 45678,
    "contentType": "image/png",
    "uploadTimestamp": "2025-04-05T14:05:11Z",
    "downloadLink": "https://localhost:5001/api/documents/download/logo.png_v1"
  }
]

#### /api/documents/{filename}/versions

[
  {
    "version": 1,
    "uploadTimestamp": "2025-04-05T10:15:30Z",
    "downloadLink": "https://localhost:5001/api/documents/download/report.pdf_v1"
  },
  {
    "version": 2,
    "uploadTimestamp": "2025-04-05T12:30:45Z",
    "downloadLink": "https://localhost:5001/api/documents/download/report.pdf_v2"
  },
  {
    "version": 3,
    "uploadTimestamp": "2025-04-05T15:10:22Z",
    "downloadLink": "https://localhost:5001/api/documents/download/report.pdf_v3"
  }
]
