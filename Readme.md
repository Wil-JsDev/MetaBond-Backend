# MetaBond API 🌐

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-12.0-239120?style=flat-square&logo=c-sharp)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![Railway](https://img.shields.io/badge/Deployed%20on-Railway-0B0D0E?style=flat-square&logo=railway)](https://railway.app/)

## 📝 Description

MetaBond is a **modern social platform API** designed to build communities where users connect not only with people but
also with **goals, interests, and specific projects**.

Users can create or join communities based on common objectives such as:

- 🎓 Learning new skills (languages, programming, etc.)
- 🚀 Developing startups and business ventures
- 🎉 Organizing events and social activities
- 💪 Fitness and wellness goals
- 🎨 Creative projects and collaborations

The platform enhances user interaction through **gamification tools**, real-time features, and intelligent resource
matching.

## ✨ Key Features

### 🏘️ Community Management

- Create and manage communities with category-based organization
- Community discovery through interests and categories
- Member management and role-based permissions
- Community analytics and insights

### 📅 Event Planning

- Create and manage community events
- Event categorization and filtering
- RSVP management and attendee tracking
- Event discovery and recommendations

### 👤 User Interests & Categories

- Interest-based user profiling
- Category management for communities and interests
- Smart matching algorithms
- Personalized content recommendations

### 📊 Progress Tracking

- Goal-oriented progress boards
- Achievement tracking and milestones
- Gamification elements
- Personal and community metrics

### 💬 Social Features

- User-generated posts within communities
- Community membership management
- Social interactions and engagement tools

## 🛠️ Technology Stack

### Backend Framework

- **Framework**: .NET 8.0
- **Language**: C# 12.0
- **Architecture**: Clean Architecture + CQRS Pattern

### Data & Persistence

- **ORM**: Entity Framework Core
- **Database**: PostgreSQL + PgAdmin
- **Migrations**: EF Core Migrations

### API & Documentation

- **API Style**: RESTful Web API
- **Documentation**: Swagger/OpenAPI 3.0
- **Versioning**: API versioning support (v1)

### Validation & Testing

- **Validation**: FluentValidation
- **Testing Framework**: xUnit
- **Test Types**: Unit Tests, Integration Tests

### Architecture Patterns

- **Dependency Injection**: Built-in .NET DI Container
- **CQRS**: Command Query Responsibility Segregation
- **Repository Pattern**: Data access abstraction
- **Result Pattern**: Error handling and response wrapping
- **Mediator Pattern**: MediatR for request handling

## 🚀 Getting Started

### Prerequisites

- .NET 8.0 SDK
- SQL Server (LocalDB or full instance)
- Visual Studio 2022 or VS Code
- Git

### Installation

1. **Clone the repository**
   ```bash
   git clone [repository-url]
   cd MetaBond-Backend
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Update database connection string**
   ```bash
   cp .env.template .env
   # Edit .env file with your database connection string
   ```

4. **Run database migrations**
   ```bash
   dotnet ef database update --project MetaBond.Infrastructure.Persistence
   ```

5. **Run the application**
   ```bash
   dotnet run --project MetaBond.Presentation.Api
   ```

6. **Access the API**
    - API: `https://localhost:7001`
    - Swagger UI: `https://localhost:7001/swagger`

### Using Docker

1. **Run Docker Compose**
   ```bash
   docker-compose up
   ```

## 📋 API Endpoints

### Communities

- `GET /api/v1/communities` - Get paginated communities
- `POST /api/v1/communities` - Create new community
- `GET /api/v1/communities/{id}` - Get community by ID
- `PUT /api/v1/communities/{id}` - Update community
- `DELETE /api/v1/communities/{id}` - Delete community
- `GET /api/v1/communities/category/{categoryId}` - Get communities by category

*For complete API documentation, visit `/swagger` when running the application.*

## 🧪 Testing

### Run all tests

```bash
dotnet test
```

### Run specific test project

```bash
dotnet test MetaBond.Tests
```

### Test coverage

```bash
dotnet test --collect:"XPlat Code Coverage"
```

## 📁 Project Structure

For detailed information about the project architecture, see [ARCHITECTURE.md](./ARCHITECTURE.md)

```
MetaBond-Backend/
├── 📂 MetaBond.Domain/                    # Business entities and rules
├── 📂 MetaBond.Application/               # Business logic and use cases
├── 📂 MetaBond.Infrastructure.Persistence/ # Data access implementations
├── 📂 MetaBond.Infrastructure.Shared/     # Shared services and utilities
├── 📂 MetaBond.Presentation.Api/          # Web API controllers and configuration
├── 📂 MetaBond.Tests/                     # Test suites
├── 🐳 docker-compose.yml                 # Docker configuration
├── 📄 .env.template                      # Environment variables template
└── 📖 ARCHITECTURE.md                    # Detailed architecture documentation
```

## 🚀 Deployment

### Railway (Current)

The backend is currently deployed on **Railway** with automatic deployments from the main branch.

- **Production URL**: [Production Railway URL]
- **Swagger Documentation**: [Production URL]/swagger

### Alternative Deployment Options

- **Azure App Service**: Recommended for enterprise deployments
- **AWS ECS/Fargate**: For containerized deployments
- **Docker**: Self-hosted with Docker Compose
- **Kubernetes**: For large-scale deployments

## 📈 Development Status

- ✅ Community Management System
- ✅ Event Management System
- ✅ Category-based Organization
- ✅ Interest Management
- ✅ API Documentation
- ✅ Unit Testing Framework
- 🔄 Authentication & Authorization (In Progress)
- 🔄 Real-time Features (Planned)
- 🔄 Gamification System (Planned)
- 🔄 Notification System (Planned)F

---

**Built with ❤️ using .NET 8 and Clean Architecture principles**
