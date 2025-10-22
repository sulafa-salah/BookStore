# 📘 BookStore — Microservices in .NET 8 with Clean Architecture & DDD

---

## 🧭 Overview

**BookStore** is a modular, cloud-ready microservices system that demonstrates modern **.NET 8** development practices — combining **Clean Architecture**, **Domain-Driven Design (DDD)**, and **CQRS** patterns.  
It simulates a real-world e-commerce domain where users can browse, purchase, and manage books, featuring robust inter-service communication, authentication, and event-driven workflows.

---

## 🏗️ Architecture Highlights

| **Layer** | **Description** |
|------------|-----------------|
| **Domain** | Rich domain model with Value Objects (`ISBN`, `Money`, `SKU`) and Aggregates (`Book`, `Category`, `Author`). |
| **Application** | CQRS with MediatR — Commands, Queries, and Pipeline Behaviors (Validation, Authorization, Caching). |
| **Infrastructure** | EF Core 8 repositories, Unit of Work, SQL Server / SQLite, Redis cache, and RabbitMQ (MassTransit Outbox Pattern). |
| **API** | ASP.NET Core 8 REST endpoints, JWT Authentication, FluentValidation, API Versioning, and Swagger UI. |
| **Gateway** | YARP Reverse Proxy for centralized routing, JWT validation, CORS, and rate limiting. |

---

## 🧩 Services

| **Service** | **Description** |
|--------------|-----------------|
| 🪶 **Identity API** | Handles registration, login, JWT + refresh tokens, roles, and permissions. |
| 📚 **Catalog API** | Manages categories, authors, and books. Integrates with Azure Blob Storage for book covers. |
| 🛒 **Cart API** | Redis-backed cart management with TTL and key-value data store. |
| 🧱 **Image Processor** | Azure-Function-style service that generates thumbnails for uploaded book covers. |
| 🚪 **Gateway API** | Unified entry point (YARP) that routes authenticated requests to downstream services. |

---

## 🌐 Tech Stack

- .NET 8 / ASP.NET Core Minimal APIs  
- Entity Framework Core 8  
- SQL Server 2022 / SQLite (for testing)  
- Redis (for Cart & Caching)  
- RabbitMQ + MassTransit (Outbox Pattern)  
- Docker & Docker Compose  
- Azure Storage (Azurite emulator locally)  
- YARP API Gateway  
- xUnit Testing Suite  
- GitHub Actions (CI per service)

---

## ⚙️ Key Features

✅ Clean Architecture with strict separation of concerns  
🧩 Rich domain model using DDD principles  
🧠 CQRS + MediatR for scalable request handling  
🔐 JWT Authentication & Role-based Authorization  
💾 Redis integration for fast data access & caching  
📦 RabbitMQ Outbox Pattern ensuring eventual consistency  
☁️ Azure Blob Storage for cover uploads (+ auto thumbnail)  
🧰 CI/CD via GitHub Actions (path-filtered workflows)  
🐳 Fully containerized environment with Docker Compose  

---

## 🧠 Domain Model Example

```csharp
public class Book : AggregateRoot<Guid>
{
    public ISBN Isbn { get; private set; }
    public string Sku { get; private set; }
    public Money Price { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public Guid CategoryId { get; private set; }

    private Book() { }

    public Book(ISBN isbn, string sku, Money price, string title, string description, Guid categoryId)
    {
        Isbn = isbn;
        Sku = sku;
        Price = price;
        Title = title;
        Description = description;
        CategoryId = categoryId;
    }
}
```

---

## 🗺️ Architecture Diagram

```mermaid
flowchart LR
    subgraph Client
        U[User / Frontend]
    end

    U -->|JWT| G[Gateway API (YARP)]

    subgraph Services
        Iden[🪶 Identity API]
        Cat[📚 Catalog API]
        Cart[🛒 Cart API]
        Img[🧱 Image Processor]
    end

    subgraph Infra
        SQL[(SQL Server / SQLite)]
        REDIS[(Redis)]
        RB[🐇 RabbitMQ (MassTransit Outbox)]
        BLOB[(Azure Blob Storage / Azurite)]
    end

    G --> Iden
    G --> Cat
    G --> Cart

    Iden --> SQL
    Cat --> SQL
    Cat --> BLOB
    Cat --> RB
    Img --> RB
    Img --> BLOB
    Img --> Cat
    Cart --> REDIS

    classDef svc fill:#f3f7ff,stroke:#8aa3ff,stroke-width:1px;
    classDef infra fill:#fff7e6,stroke:#f0b429,stroke-width:1px;
    class Iden,Cat,Cart,Img svc;
    class SQL,REDIS,RB,BLOB infra;
```

---

## 🚀 Getting Started

### 1️⃣ Clone the repository
```bash
git clone https://github.com/sulafa-salah/BookStore.git
cd BookStore
```

### 2️⃣ Configure Environment
Create a `.env` file at the repository root:
```bash
JWT_SECRET=super-long-256-bit-secret
JWT_ISSUER=bookstore
JWT_AUDIENCE=bookstore-users
DB_SA_PASSWORD=StrongPassword123
CATALOG_BASE_URL=http://catalog-api:8080
INTERNAL_API_KEY=dev-internal-key-change-me
```

### 3️⃣ Run via Docker Compose
```bash
docker compose up --build
```

### 4️⃣ Access Services

| **Service** | **URL** |
|--------------|---------|
| Gateway API | http://localhost:7000 |
| Identity API | http://localhost:7100/swagger |
| Catalog API | http://localhost:7200/swagger |
| Cart API | http://localhost:7300/swagger |
| Azurite Blob Storage | http://localhost:10000/devstoreaccount1 |

---

## 🧪 Testing

- xUnit for unit + integration tests  
- SQLite for fast, isolated testing  
- CI runs automated test workflows per service path  

---

## 📦 CI/CD Workflows

Each microservice has its own GitHub Actions workflow file under .github/workflows/<service>-ci.yml, featuring:

- 🧱 Build & Test steps  
- ⚡ Path-filtered triggers (per service) 
- 🧩 Environment-specific variables  
- 🐳 Docker image publish → GitHub Container Registry  (planned for later)


---

## 🔮 Future Enhancements

- 🏷️ Inventory Service (subscribes to Catalog Domain events)  
- 💳 Order & Payment Services (for checkout flows)  
- 🧾 MongoDB for Refresh Tokens & Audit Logs  
- 📊 OpenTelemetry + Grafana for tracing  
- 🌍 Angular Frontend for Book Browsing & Checkout  

---

## 🙏 Credits & Acknowledgements

**Special thanks to [Amichai Mantinband](https://dometrain.com/author/amichai-mantinband/)** —  
his exceptional teaching style on *Clean Architecture & Domain-Driven Design in .NET* helped me clearly understand the concepts, which I later applied while building this project.

📚 **GitHub Reference:** [`amantinband/clean-architecture`](https://github.com/amantinband/clean-architecture)
