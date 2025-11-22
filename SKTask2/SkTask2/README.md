## Task 2: Payment Gateway Mock API

### Features
- Create payment session instantly
- Manual control: simulate **Success** or **Failure**
- Automatic webhook delivery on completion
- Full transaction history in SQL Server
- No real money, no API keys, no external services
- **Direct test endpoints** to trigger webhooks instantly

### Endpoints

| Method | Endpoint                                      | Description                                      |
|--------|-----------------------------------------------|--------------------------------------------------|
| POST   | `/api/payment/create-session`                 | Create new payment session                       |
| GET    | `/api/payment/checkout/{sessionId}`           | Mock payment page (click Success or Fail)        |
| GET    | `/api/payment/webhook/success/{sessionId}`    | Instantly trigger **success** webhook            |
| GET    | `/api/payment/webhook/fail/{sessionId}`       | Instantly trigger **failure** webhook            |
| POST   | `/api/payment/process`                        | Internal: called by checkout form                |
| GET    | `/api/payment/result`                         | Internal: shows success/failure result page      |