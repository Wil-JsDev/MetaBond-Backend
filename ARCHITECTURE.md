# MetaBond Backend - Architecture Documentation

## Overview

MetaBond Backend is a .NET-based social platform API built with **Clean Architecture** principles. The system enables
community creation, event management, user interactions, and interest-based categorization.

---

## 📁 Project Structure

```
MetaBond-Backend/
├── 📂 MetaBond.Domain/                    # Core business entities and logic
├── 📂 MetaBond.Application/               # Use cases and business operations
│   ├── 📂 DTOs/                          # Data Transfer Objects
│   ├── 📂 Feature/                       # Feature-based organization (CQRS)
│   │   ├── 📂 Communities/               # Community management features
│   │   ├── 📂 Events/                    # Event management features
│   │   ├── 📂 Interest/                  # User interests features
│   │   └── 📂 CommunityCategory/         # Category management features
│   ├── 📂 Interfaces/                    # Repository and service contracts
│   ├── 📂 Mapper/                        # Object mapping configurations
│   └── 📂 Utils/                         # Shared utilities and helpers
├── 📂 MetaBond.Infrastructure.Persistence/ # Data access implementation
│   └── 📂 Repository/                    # Repository implementations
├── 📂 MetaBond.Infrastructure.Shared/     # Shared infrastructure services
├── 📂 MetaBond.Presentation.Api/          # REST API layer
│   └── 📂 Controllers/V1/                # API Controllers
└── 📂 MetaBond.Tests/                     # Test suite
```

## 🎯 Core Architecture Principles

### 1. **CQRS (Command Query Responsibility Segregation)**

- **Commands**: Handle write operations (Create, Update, Delete)
- **Queries**: Handle read operations (Get, Filter, Pagination)
- Each feature is organized by operation type

### 2. **Feature-Based Organization**

Features are organized by domain context:

- **Communities**: Community creation, management, and membership
- **Events**: Event planning and management within communities
- **Interests**: User interest categorization and matching
- **Categories**: Classification system for communities and interests

### 3. **Repository Pattern**

- Abstract data access through interfaces
- Concrete implementations in Infrastructure layer
- Enables testability and database independence

### 4. **Dependency Injection**

- Services registered in `DependencyInjection.cs` files
- Loose coupling between layers
- Configurable service lifetimes

## 🔧 Key Components

### Application Layer Features

Each feature follows a consistent structure:

```
Feature/
├── Commands/
│   ├── Create/
│   │   ├── Create[Entity]Command.cs
│   │   ├── Create[Entity]CommandHandler.cs
│   │   └── Create[Entity]CommandValidator.cs
│   ├── Update/
│   └── Delete/
└── Query/
    ├── GetById/
    ├── Pagination/
    └── [SpecificQueries]/
```

### Core Entities

- **Communities**: Social groups with members and content
- **Events**: Time-based activities within communities
- **Interests**: User preference categories
- **Categories**: Classification for communities and interests
- **Posts**: User-generated content within communities
- **Memberships**: User-community relationships

### Data Flow

```
HTTP Request → Controller → Command/Query → Handler → Repository → Database
                     ↓
Response ← DTO ← Mapper ← Entity ← Database Response
```

## 🚀 Development Patterns

### Command Pattern

```csharp

public interface ICommand : IRequest<Result>, IBaseCommand;

public interface ICommand<TResponse> : IRequest<ResultT<TResponse>>, IBaseCommand;

public interface IBaseCommand;

public interface ICommandHandler<in TCommand> :
    IRequestHandler<TCommand, Result>
    where TCommand : ICommand;

public interface ICommandHandler<in TCommand, TResponse> :
    IRequestHandler<TCommand, ResultT<TResponse>>
    where TCommand : ICommand<TResponse>;

```

### Query Pattern

```csharp

public interface IQuery<TResponse> : IRequest<ResultT<TResponse>>;

public interface IQueryHandler<in TQuery, TResponse> :
    IRequestHandler<TQuery, ResultT<TResponse>>
    where TQuery : IQuery<TResponse>;
```

### Result Pattern

Operations return `Result<T>` objects that encapsulate success/failure states:

```csharp
public class Result<T>
{
    public bool IsSuccess { get; set; }
    public T Data { get; set; }
    public string Error { get; set; }
}
```

## 📋 API Design

### RESTful Endpoints

- **Communities**: `/api/v1/communities`
- **Events**: `/api/v1/events`
- **Interests**: `/api/v1/interests`
- **Categories**: `/api/v1/community-categories`, `/api/v1/interest-categories`

### HTTP Methods

- `GET`: Retrieve data (queries)
- `POST`: Create new resources (commands)
- `PUT`: Update existing resources (commands)
- `DELETE`: Remove resources (commands)

## 🧪 Testing Strategy

- **Unit Tests**: Individual component testing
- **Controller Tests**: API endpoint testing

## 🔄 Data Flow Examples

### Creating a Community

1. `POST /api/v1/communities` → `CommunitiesController`
2. Controller validates and maps to `CreateCommunityCommand`
3. `CreateCommunityCommandHandler` processes business logic
4. Handler calls `ICommunitiesRepository.CreateAsync()`
5. Repository persists to database
6. Response mapped to `CommunityDto` and returned

### Querying Communities by Category

1. `GET /api/v1/communities/category/{id}` → `CommunitiesController`
2. Controller creates `GetCommunitiesByCategoryIdQuery`
3. `GetCommunitiesByCategoryIdQueryHandler` processes request
4. Handler calls repository with filtering logic
5. Results mapped and returned as paginated response

## 🎯 Design Benefits

- **Maintainability**: Clear separation of concerns
- **Testability**: Isolated components with dependency injection
- **Scalability**: Modular architecture supports growth
- **Flexibility**: Easy to modify or extend individual features
- **Clean Code**: Consistent patterns and conventions

---

*This architecture documentation reflects the current state of MetaBond Backend and serves as a guide for development
and maintenance.*
