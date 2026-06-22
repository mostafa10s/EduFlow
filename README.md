```
 _____ _                     ___           _     
/  __ \ |                   / _ \         | |    
| /  \/ | ___  __ _ _ __   / /_\ \_ __ ___| |__  
| |   | |/ _ \/ _` | '_ \  |  _  | '__/ __| '_ \ 
| \__/\ |  __/ (_| | | | | | | | | | | (__| | | |
 \____/_|\___|\__,_|_| |_| \_| |_/_|  \___|_| |_|
```

# .NET 8 Clean Architecture Starter

A production-shaped reference implementation of **Clean Architecture** in .NET 8 — split into four projects (Domain, Application, Infrastructure, Api) with xUnit tests, JWT auth, Swagger, Docker, and CI.

Use it as a starting point for new .NET APIs, or as a reference when reviewing code structure in interviews.

---

## ✨ What's inside

- **4-layer separation** with strict dependency direction (Api → Application → Domain ← Infrastructure)
- **Domain entities** with behaviour (`Product.DecreaseStock` enforces its own invariants)
- **Generic `IRepository<T>`** + **`IUnitOfWork`** to keep EF Core out of your business logic
- **Application services** with constructor-injected `IUnitOfWork` — easy to unit test with Moq
- **EF Core 8** with SQLite for zero-config dev, SQL Server for production — switch via `Database:Provider` config
- **JWT bearer auth** with role-based `[Authorize(Roles = "Admin")]`
- **Swagger UI** with a working "Authorize" button for the JWT token
- **FluentValidation** wired in (just add a `Validator<T>` and it's auto-registered)
- **Serilog** structured logging
- **xUnit + FluentAssertions + Moq** tests for both Domain logic and Application services
- **GitHub Actions CI** — runs `dotnet build` + `dotnet test` + uploads coverage on every push and PR
- **Multi-stage Dockerfile** + `docker-compose.yml` for one-command runs

---

## 🗂 Project structure

```
CleanArchitecture.slnx
├── src/
│   ├── CleanArchitecture.Domain/          ← entities, value objects, interfaces (zero deps)
│   │   ├── Common/BaseEntity.cs
│   │   ├── Entities/Product.cs, Customer.cs, Order.cs, OrderItem.cs
│   │   └── Interfaces/IRepository.cs, IProductRepository.cs, IUnitOfWork.cs
│   ├── CleanArchitecture.Application/     ← use cases, DTOs, services
│   │   ├── DTOs/
│   │   ├── Interfaces/IProductService.cs, ICustomerService.cs, IOrderService.cs
│   │   ├── Services/ProductService.cs, CustomerService.cs, OrderService.cs
│   │   └── DependencyInjection.cs
│   ├── CleanArchitecture.Infrastructure/  ← EF Core, repositories, JWT
│   │   ├── Persistence/AppDbContext.cs, DbSeeder.cs
│   │   ├── Repositories/Repository.cs, ProductRepository.cs, UnitOfWork.cs
│   │   ├── Authentication/JwtTokenService.cs
│   │   └── DependencyInjection.cs
│   └── CleanArchitecture.Api/             ← HTTP boundary
│       ├── Controllers/ProductsController.cs, CustomersController.cs, OrdersController.cs, AuthController.cs
│       ├── Program.cs
│       ├── appsettings.json
│       └── Dockerfile
├── tests/
│   └── CleanArchitecture.Tests/           ← xUnit + FluentAssertions + Moq
│       ├── Domain/ProductTests.cs
│       └── Application/ProductServiceTests.cs, OrderServiceTests.cs
├── .github/workflows/ci.yml               ← build + test on push/PR
├── docker-compose.yml
├── LICENSE
└── README.md
```

---

## 🚀 Quick start

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- (Optional) [Docker](https://www.docker.com/) + Docker Compose

### Run locally

```bash
# Restore + run the API
dotnet run --project src/CleanArchitecture.Api

# Swagger UI opens at:
#   http://localhost:5xxx/swagger
```

The first run auto-creates `app.db` (SQLite) and seeds it with 3 products + 2 customers.

### Run with Docker

```bash
docker compose up --build
# API at http://localhost:8080
# Swagger at http://localhost:8080/swagger
```

### Run the tests

```bash
dotnet test
```

---

## 🔐 Auth flow

The included `AuthController` is a **demo** — any email/password returns a JWT with `Admin` role. Replace it with ASP.NET Identity or an external IdP in production.

```http
POST /api/auth/login
Content-Type: application/json

{ "email": "you@example.com", "password": "anything" }
```

```json
{ "access_token": "eyJhbGciOi...", "token_type": "Bearer" }
```

Click "Authorize" in Swagger and paste the token. Admin-only endpoints (POST/PUT/DELETE products) now work.

---

## 🔌 Endpoints

| Method | Path | Auth | Description |
|--------|------|------|-------------|
| `GET`  | `/api/products`              | Public  | List all products |
| `GET`  | `/api/products/{id}`         | Public  | Get product by ID |
| `GET`  | `/api/products/in-stock`     | Public  | List products with stock > 0 |
| `POST` | `/api/products`              | Admin   | Create product |
| `PUT`  | `/api/products/{id}`         | Admin   | Update product |
| `DELETE` | `/api/products/{id}`       | Admin   | Delete product |
| `GET`  | `/api/customers`             | Auth    | List customers |
| `POST` | `/api/customers`             | Auth    | Create customer |
| `GET`  | `/api/orders/{id}`           | Auth    | Get order with line items |
| `POST` | `/api/orders`                | Auth    | Create order (decreases stock atomically) |
| `POST` | `/api/auth/login`            | Public  | Get JWT (demo) |

See `src/CleanArchitecture.Api/CleanArchitecture.Api.http` for a ready-to-run request collection (VS / Rider / VS Code REST Client).

---

## 🧠 How the layers talk

```
HTTP request
   │
   ▼
[ Controllers ]          ← Api project (knows about HTTP, DTOs, services)
   │  constructor-injects IProductService
   ▼
[ Services ]             ← Application project (knows about DTOs, IUnitOfWork)
   │  constructor-injects IUnitOfWork
   ▼
[ Repositories / DbContext ]  ← Infrastructure project (knows about EF Core, SQL)
   │
   ▼
[ Domain Entities ]      ← Domain project (zero outward dependencies)
```

The **Domain** project has no NuGet dependencies except BCL. The **Application** project references Domain only. The **Infrastructure** project references both, but Domain and Application never reference Infrastructure. This is what makes the business logic testable without a database.

---

## 🧪 Test coverage

```
Domain/ProductTests            — 6 tests   (stock invariants, audit timestamps)
Application/ProductServiceTests — 2 tests   (mapping, persistence)
Application/OrderServiceTests  — 3 tests   (stock decreases, error paths)
                              ───
                               11 tests, 100% passing
```

Run with coverage:
```bash
dotnet test --collect:"XPlat Code Coverage"
```

---

## ⚙️ Configuration

`src/CleanArchitecture.Api/appsettings.json`:

```json
{
  "Database": { "Provider": "Sqlite" },              // or "SqlServer"
  "ConnectionStrings": {
    "Default": "Data Source=app.db"                  // SQLite
    // "Default": "Server=...;Database=...;..."     // SQL Server
  },
  "Jwt": {
    "Issuer": "CleanArchitecture.Api",
    "Audience": "CleanArchitecture.Clients",
    "Secret": "REPLACE-WITH-AT-LEAST-32-CHARS-RANDOM-SECRET",
    "ExpiryMinutes": 60
  }
}
```

**Always rotate the JWT secret for production.** The default value is intentionally a placeholder.

---

## 🧱 Why this structure?

| Symptom in bad codebases | What this template enforces |
|--------------------------|-----------------------------|
| Business rules scattered across controllers | Domain entities own their invariants (`Product.DecreaseStock`) |
| Controllers doing `DbContext.SaveChanges()` | Controllers call `IProductService` only — no EF Core in HTTP layer |
| Hard to test — "spin up SQL Server to run a test" | Services take `IUnitOfWork` — mock with Moq, no DB needed |
| Refactoring breaks unrelated layers | Strict dependency direction; circular refs are compile errors |
| `new MySqlConnection()` inside service logic | EF Core + repositories keep SQL in Infrastructure |

---

## 📐 Architecture

See [docs/architecture.md](docs/architecture.md) for the layer diagram, dependency-direction rules, and the request-flow walkthrough.

---

## 📸 Sample API output

See [docs/sample-output.md](docs/sample-output.md) for real `curl` captures — including what a stock-violation error response looks like.

---

## 📚 Further reading

- [Clean Architecture — Robert C. Martin](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [Microsoft eShopOnContainers — reference microservices sample](https://github.com/dotnet-architecture/eShopOnContainers)
- [Jason Taylor's Clean Architecture template](https://github.com/jasontaylordev/CleanArchitecture) (heavier, more features — good next step after this one)

---

## 📄 License

MIT — see [LICENSE](LICENSE).
