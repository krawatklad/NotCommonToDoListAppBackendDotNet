# Task Management API

Backend ASP.NET Core (REST API) for task management, built according to **Clean Architecture** principles. 
## Features

### 1. Task Management (CRUD)
- Full set of operations for tasks: creation, retrieval (list and details), updating, and deletion.
- Endpoints:
  - `GET /tasks` – user's task list.
  - `GET /tasks/{id}` – specific task details.
  - `POST /tasks` – add a new task.
  - `PUT /tasks/{id}` – edit an existing task.
  - `DELETE /tasks/{id}` – delete a task.

### 2. Authentication and Authorization
- Implemented using **JWT (JSON Web Token)**.
- Signature algorithm: HMAC SHA-256.
- Claims: Sub (User Id), GivenName, FamilyName, Jti (unique token identifier).
- Registration and Login:
  - `POST /auth/register` – account creation.
  - `POST /auth/login` – exchange credentials for an access token.

### 3. Asynchronous Processing (RabbitMQ)
- Utilizes the **EasyNetQ** library for message handling.
- Upon successful user registration, a `UserRegisteredEvent` is published.
- **Worker Service**: A separate service consuming messages from the queue:
  - `EmailNotificationConsumer` – simulates sending a welcome email (visible in Mailpit).
  - `SmsNotificationConsumer` – simulates sending an SMS notification.

### 4. Data Export
- Advanced module for exporting tasks to popular formats:
  - **PDF** – generated using the `QuestPDF` library.
  - **XLSX (Excel)** – generated using the `ClosedXML` library.
- Endpoint: `GET /tasks/export?format={Pdf|Xlsx}`.

## Technologies and Tools

- **Framework**: .NET 10
- **Database**: PostgreSQL (accessible via Entity Framework Core)
- **Communication**: RabbitMQ (Message Bus)
- **Documentation**: Scalar (modern and interactive API documentation)
- **Testing**: Unit tests (xUnit, Moq)
- **Development Tools**: 
  - Docker & Docker Compose (used for infrastructure only: Database, RabbitMQ, and Mailpit)
  - Mailpit (SMTP server for email testing)

## Architecture

The project is divided into layers according to Clean Architecture:
- **Domain**: Entities, enums, and domain logic.
- **Application**: Business logic, commands, queries (CQRS), interfaces.
- **Infrastructure**: Implementation of data access, JWT handling, RabbitMQ integration.
- **Api**: REST controllers, DI configuration, middleware.
- **Worker**: Message consumers.

## Infrastructure Setup

The project uses Docker to host necessary infrastructure services. To start use the following command:

```bash
docker-compose up -d
```

Note: The API and Worker are intended to be run locally (e.g., via `dotnet run` or from your IDE), connecting to the services running in Docker containers.

## CI/CD

The project is prepared for integration with CI tools (e.g., GitHub Actions). The test structure (`*.Tests.csproj`) allows for automatic code verification upon every Pull Request.

