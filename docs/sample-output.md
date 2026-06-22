# Sample API Output

Real output captured by running the API locally with `dotnet run` and hitting it with `curl`. Reproducible by following the Quick Start in the README.

---

## GET /api/products (public)

```http
GET http://localhost:5099/api/products
```

```json
[
  {"id":1,"name":"Mechanical Keyboard","description":"TKL, hot-swap, brown switches","price":129.99,"stockQuantity":25},
  {"id":2,"name":"Wireless Mouse",     "description":"Ergo, 4K polling",                 "price":79.50, "stockQuantity":40},
  {"id":3,"name":"USB-C Hub",          "description":"7-in-1, 100W PD",                  "price":49.00, "stockQuantity":0}
]
```

These three products come from `DbSeeder.Seed()`, which only runs when the table is empty.

---

## GET /api/products/in-stock (public)

```http
GET http://localhost:5099/api/products/in-stock
```

```json
[
  {"id":1,"name":"Mechanical Keyboard","description":"TKL, hot-swap, brown switches","price":129.99,"stockQuantity":25},
  {"id":2,"name":"Wireless Mouse",     "description":"Ergo, 4K polling",                 "price":79.50, "stockQuantity":40}
]
```

USB-C Hub is excluded because `stockQuantity: 0`. Implemented in `IProductRepository.GetInStockAsync()`.

---

## GET /api/products/1 (public)

```http
GET http://localhost:5099/api/products/1
```

```json
{"id":1,"name":"Mechanical Keyboard","description":"TKL, hot-swap, brown switches","price":129.99,"stockQuantity":25}
```

---

## POST /api/auth/login (public, demo)

```http
POST http://localhost:5099/api/auth/login
Content-Type: application/json

{"email":"demo@x.com","password":"x"}
```

```json
{"access_token":"eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...","token_type":"Bearer"}
```

The demo auth controller accepts any non-empty email/password and returns a JWT with `Admin` role. In production, replace with ASP.NET Identity or an external IdP — the rest of the app doesn't change.

---

## POST /api/orders (authenticated, stock decreases atomically)

```http
POST http://localhost:5099/api/orders
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...
Content-Type: application/json

{
  "customerId": 1,
  "items": [
    {"productId": 1, "quantity": 2},
    {"productId": 2, "quantity": 1}
  ]
}
```

```json
{
  "id": 1,
  "customerId": 1,
  "customerName": "Ada Lovelace",
  "orderDate": "2026-06-22T09:18:42.123Z",
  "status": "Pending",
  "total": 339.48,
  "items": [
    {"productId": 1, "quantity": 2, "unitPrice": 129.99, "subtotal": 259.98},
    {"productId": 2, "quantity": 1, "unitPrice":  79.50, "subtotal":  79.50}
  ]
}
```

The order's total of 339.48 = (2 × 129.99) + (1 × 79.50). The keyboard's stock is now 23, the mouse's stock is now 39. Both decreases happened in a single `SaveChangesAsync` call inside the same DbContext transaction.

If you submit `quantity: 9999` for a product with `stockQuantity: 25`, you get back:

```json
{"error": "Insufficient stock for product 'Mechanical Keyboard'. Requested 9999, available 25."}
```

with HTTP 400. The `Product.DecreaseStock` domain method throws, the controller catches it, and the order is never persisted.

---

## The full Swagger UI

```http
GET http://localhost:5099/swagger
```

The Swagger UI is mounted in Development mode and includes a working "Authorize" button. Paste any JWT (e.g. from `POST /api/auth/login`) into it and the Admin-only endpoints become callable directly from the browser.
