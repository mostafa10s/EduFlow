# Architecture

```
                          ┌──────────────────────┐
                          │ HTTP Request         │
                          │ (curl, browser, etc) │
                          └──────────┬───────────┘
                                     │
                                     ▼
                          ┌──────────────────────┐
                          │   Controllers        │   Api layer
                          │  (Products, Orders,  │   - knows about HTTP
                          │   Customers, Auth)   │   - DTOs in/out
                          │                      │   - role-based [Authorize]
                          └──────────┬───────────┘
                                     │ constructor-injects
                                     │ IProductService, IOrderService, ...
                                     ▼
                          ┌──────────────────────┐
                          │   Services           │   Application layer
                          │  (ProductService,    │   - use cases
                          │   OrderService)      │   - takes IUnitOfWork
                          │                      │   - knows about DTOs
                          └──────────┬───────────┘
                                     │ constructor-injects
                                     │ IUnitOfWork, IProductRepository
                                     ▼
                          ┌──────────────────────┐
                          │ Repositories +       │   Infrastructure layer
                          │ AppDbContext         │   - EF Core
                          │ (EF Core, SQL Server │   - JWT signing
                          │  or SQLite)          │   - knows about persistence
                          └──────────┬───────────┘
                                     │
                                     ▼
                          ┌──────────────────────┐
                          │ Domain Entities      │   Domain layer
                          │  (Product, Customer, │   - pure C# (zero deps)
                          │   Order, OrderItem)  │   - enforces invariants
                          │                      │   -   e.g. Product.DecreaseStock
                          └──────────────────────┘
```

## The dependency direction

This is the most important thing about Clean Architecture, and the part beginners get wrong:

```
Domain ◄── Application ◄── Infrastructure
                    ▲
                    │
                   Api
```

- Domain depends on nothing (no NuGet refs)
- Application depends on Domain only
- Infrastructure depends on both, but Domain and Application NEVER reference Infrastructure
- Api references both Application and Infrastructure, and is the only place they "meet"

If you can draw a circle around Domain + Application with no arrow pointing out, the architecture is correct. If Api imports from Infrastructure but never the other way around, you're good.

## The unit of work

The `IUnitOfWork` interface in Domain is the seam that makes the Application layer testable. Services depend on it instead of on `AppDbContext` directly, so a unit test can pass in a `Mock<IUnitOfWork>` and verify behaviour without spinning up SQL Server.

```csharp
// Production wiring (Infrastructure/DependencyInjection.cs)
services.AddScoped<IUnitOfWork, UnitOfWork>();
services.AddScoped<IProductRepository, ProductRepository>();

// Test wiring (tests/Application/ProductServiceTests.cs)
var uow = new Mock<IUnitOfWork>();
var sut = new ProductService(uow.Object);  // no database!
```

## How a request flows

`POST /api/orders` with a body like `{"customerId": 1, "items": [{"productId": 1, "quantity": 2}]}`:

1. ASP.NET Core deserializes the JSON into `CreateOrderDto`
2. JWT bearer middleware validates the `[Authorize]` token
3. `OrdersController.Create` calls `IOrderService.CreateAsync(dto)`
4. `OrderService`:
   - Loads the customer via `_unitOfWork.Customers.GetByIdAsync(...)`
   - For each item, loads the product and calls `product.DecreaseStock(quantity)` — **this is the domain enforcing the invariant**
   - Builds the `Order` aggregate with the line items
   - Calls `_unitOfWork.Orders.AddAsync(order)` and `_unitOfWork.SaveChangesAsync()` — one transaction
5. Result is mapped to `OrderDto` and returned as 201 Created with a Location header

If `DecreaseStock` throws (insufficient stock), the exception bubbles up to the controller, which catches it and returns 400 with a JSON error body. The database transaction is rolled back automatically.

## Why this design

| Symptom in bad codebases | What this template enforces |
|--------------------------|-----------------------------|
| Business rules scattered across controllers | Domain entities own their invariants (`Product.DecreaseStock` throws on insufficient stock) |
| Controllers doing `DbContext.SaveChanges()` | Controllers call `IProductService` only — no EF Core in the HTTP layer |
| Hard to test — "spin up SQL Server to run a test" | Services take `IUnitOfWork` — mock with Moq, no DB needed |
| Refactoring breaks unrelated layers | Strict dependency direction; circular refs are compile errors |
| `new MySqlConnection()` inside service logic | EF Core + repositories keep SQL in Infrastructure |

## What "Clean Architecture" buys you, concretely

- Swap SQLite → SQL Server → PostgreSQL by changing one config key. No code change.
- Swap JWT for cookie auth by writing a new auth handler. Domain and Application are untouched.
- Move from sync to async, from REST to gRPC, from controllers to minimal APIs — the Application layer doesn't care.
- Add a second front-end (mobile, CLI, worker) that calls the same `IOrderService` directly. No duplicated logic.

## What it does NOT buy you

- Performance. Clean Architecture adds interface boundaries; they have a small but non-zero cost.
- Simplicity. For a 200-line prototype this is overkill. The break-even is roughly when the project has 3+ business entities and 2+ delivery surfaces (web + worker, web + CLI, etc).
- Silver bullets. Badly written services in the Application layer are still bad code, just better organized.
