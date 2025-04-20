# MetaBond API

## Description

MetaBond is an API designed to build social networks where users connect not
only with people but also with **goals, interests, or specific projects**. Users
can create or join communities based on common objectives (e.g., learning a
language, developing a startup, organizing events, etc.), while the API enhances
interaction through **gamification tools**, real-time chat, and smart resources.

## Tech Stack

- **Backend:** ASP.NET Core Web API
- **Database:** PostgreSQL with EF Core as ORM
- **Infrastructure & Deployment:** Docker, GitHub Actions (CI/CD), AWS Elastic
  Beanstalk
- **Caching:** Redis
- **Media Storage:** Cloudinary for image uploads

## Deployment

The backend is deployed using **Railway**.

## Architecture
MetaBond-Backend/
├── 📂 src/                                 # Source code root folder
│   ├── 🧠 MetaBond.Application/               # Application logic and use cases (see detailed breakdown below) ⬇️
│   ├── 🏗️ MetaBond.Domain/                    # Domain entities, enums, and repository interfaces
│   ├── 💾 MetaBond.Infrastructure.Persistence/ # Repository implementations and EF Core database configuration
│   ├── 🔧 MetaBond.Infrastructure.Shared/      # Reusable services and implementations (cloudinary,email)
│   ├── 🌐 MetaBond.Presentation.Api/           # API controllers, middleware, and general API configuration
│   └── 🧪 MetaBond.Tests/                      # Unit and integration tests (using xUnit and FluentAssertions)
├── 🛠️ .github/workflows/                  # CI/CD workflows (GitHub Actions)
├── 🐳 docker-compose.yml                   # Docker configuration file to spin up the environment
├── 🧷 MetaBond.sln                         # Visual Studio solution file
└── 📄 README.md                            # Main repository documentation