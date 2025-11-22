## Task 2: Payment Gateway Mock API

### Features
- Create payment session instantly
- Manual control: simulate **Success** or **Failure**
- Automatic webhook delivery on completion
- Full transaction history in SQL Server
- No real money, no API keys, no external services

### Endpoints

| Method | Endpoint                            | Description                              |
|--------|-------------------------------------|------------------------------------------|
| POST   | `/api/payment/create-session`       | Create new payment session               |
| GET    | `/api/payment/checkout/{sessionId}` | Mock payment page (Success/Fail)         |
| POST   | `/api/payment/webhook`              | Webhook receiver                         |

### Example Requests

#### POST `/api/payment/create-session`

POST https://localhost:7130/api/payment/create-session
Content-Type: application/json

{
  "amount": 299.00,
  "currency": "USD",
  "customerEmail": "john.doe@example.com"
}

### Sample Response

#### /api/payment/create-session

{
  "sessionId": "sess_9f8e2a1c7d4b3e6f",
  "paymentUrl": "https://localhost:7130/api/payment/checkout/sess_9f8e2a1c7d4b3e6f",
  "status": "pending"
}

#### /api/payment/checkout/{sessionId}

{
  "event_type": "payment.succeeded",
  "session_id": "sess_9f8e2a1c7d4b3e6f",
  "amount": 299.00,
  "currency": "USD",
  "customer_email": "john.doe@example.com",
  "status": "succeeded",
  "timestamp": "2025-04-05T16:45:33.821Z"
}

#### /api/payment/webhook (failed transaction sample)

{
  "event_type": "payment.failed",
  "session_id": "sess_9f8e2a1c7d4b3e6f",
  "amount": 299.00,
  "currency": "USD",
  "customer_email": "john.doe@example.com",
  "status": "failed",
  "failure_reason": "Card declined (mock)",
  "timestamp": "2025-04-05T16:47:12.104Z"
}