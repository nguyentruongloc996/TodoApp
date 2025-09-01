# Todo Application

A comprehensive Todo application built with .NET 9 API and React frontend, featuring real-time collaboration, AI assistance, and distributed caching.

## 🏗️ Architecture

### Backend (.NET 9)
- **Clean Architecture** with layered approach
- **Domain Layer**: Core business logic and entities
- **Application Layer**: Use cases and orchestration
- **Infrastructure Layer**: External dependencies (DB, Email, SSO, AI)
- **API Layer**: Controllers and middleware

### Frontend (React)
- Modern React with TypeScript
- Real-time updates via SignalR
- Google SSO integration
- Responsive design

## 🚀 Features

### Core Functionality
- ✅ Create, update, and delete tasks
- ✅ Sub-tasks support
- ✅ Due dates and reminders
- ✅ Task repetition patterns
- ✅ Task status management

### Collaboration
- ✅ Group creation and management
- ✅ User invitations via email
- ✅ Google SSO integration
- ✅ Real-time task synchronization

### Advanced Features
- ✅ AI assistance for automatic task creation
- ✅ Real-time updates across browser tabs
- ✅ Distributed caching with Redis
- ✅ Containerized deployment

## 🛠️ Technology Stack

### Backend
- .NET 9
- Entity Framework Core
- PostgreSQL
- Redis (Distributed Caching)
- SignalR (Real-time Communication)
- JWT Authentication
- Google SSO

### Frontend
- React 18
- TypeScript
- SignalR Client
- Google OAuth
- Modern UI Components

### DevOps
- Docker & Docker Compose
- Redis
- PostgreSQL

## 📁 Project Structure

```
/TodoApp
│
├── /TodoApp.Domain              # Core business logic
│   ├── Entities/
│   ├── ValueObjects/
│   └── Interfaces/
│
├── /TodoApp.Application         # Use cases and orchestration
│   ├── UseCases/
│   ├── DTOs/
│   ├── Interfaces/
│   └── Services/
│
├── /TodoApp.Infrastructure     # External dependencies
│   ├── Persistence/
│   ├── SSO/
│   ├── Email/
│   ├── AI/
│   └── RealTime/
│
├── /TodoApp.API               # API Controllers and middleware
│   ├── Controllers/
│   ├── Middlewares/
│   ├── Hubs/
│   ├── Program.cs
│   └── Dockerfile
│
├── /TodoApp.Tests             # Test projects
│   ├── TodoApp.Domain.Tests/
│   ├── TodoApp.Application.Tests/
│   └── TodoApp.API.Tests/
│
├── /frontend                  # React application
│   ├── app/
│   │    ├── web/
│   │    ├── mobile/
│   ├── package/
│   │    ├── hooks/
│   │    ├── ui/
│   │    ├── utils/
│   └── package.json
│
├── /docker                    # Docker configurations
│   ├── docker-compose.yml
│   └── docker-compose.debug.yml
│
└── README.md
```

## 🧪 Development Approach

### Test-Driven Development (TDD)
- Unit tests for domain logic
- Integration tests for API endpoints
- End-to-end tests for critical workflows

### Development Phases
1. **Phase 1**: Foundation & Core Features
2. **Phase 2**: Advanced Features & Real-time
3. **Phase 3**: DevOps & Deployment

## 🚀 Getting Started

### Prerequisites
- .NET 9 SDK
- Node.js 18+
- Docker & Docker Compose
- PostgreSQL
- Redis

### Local Development
```bash
# Clone the repository
git clone <repository-url>
cd TodoApplication

# Start infrastructure services
docker-compose up -d

# Run backend
dotnet run --project TodoApp.API

# Run frontend
cd frontend
npm install
npm start
```

## 📋 Development Tasks

### Phase 1: Foundation
- [ ] Create solution structure
- [ ] Implement domain entities
- [ ] Set up Entity Framework
- [ ] Create basic API endpoints
- [ ] Implement authentication
- [ ] Set up React frontend

### Phase 2: Core Features
- [ ] Task CRUD operations
- [ ] User management
- [ ] Group functionality
- [ ] Real-time updates
- [ ] Email notifications

### Phase 3: Advanced Features
- [ ] AI integration
- [ ] Advanced caching
- [ ] Performance optimization
- [ ] Comprehensive testing

## 🧪 Testing Strategy

### Unit Tests
- Domain logic validation
- Business rule enforcement
- Value object behavior

### Integration Tests
- API endpoint functionality
- Database operations
- External service integration

### End-to-End Tests
- Complete user workflows
- Real-time synchronization
- Cross-browser functionality

## 📊 Performance Considerations

- Redis distributed caching
- Database query optimization
- SignalR connection management
- Frontend state management
- Lazy loading and code splitting

## 🔒 Security

- JWT token authentication
- Google SSO integration
- Input validation and sanitization
- CORS configuration
- Rate limiting

## 📈 Monitoring & Logging

- Application performance monitoring
- Error tracking and logging
- Real-time metrics
- Health checks

---

**Note**: This project follows Clean Architecture principles and Test-Driven Development approach for robust, maintainable code. 