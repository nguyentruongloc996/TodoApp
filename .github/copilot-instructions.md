# TodoApp - AI Coding Assistant Instructions

## Architecture Overview

This is a .NET 9 Clean Architecture application with React frontend, following TDD practices and CQRS patterns.

**Layer Dependencies** (inner → outer):
- `TodoApp.Domain` - Core entities (`Task`, `User`, `Group`, `SubTask`), value objects (`Email`, `Reminder`, `RepeatPattern`), no external dependencies
- `TodoApp.Application` - Use cases, DTOs, CQRS handlers (`ICommandHandle<TCommand, TResponse>`, `IQueryHandle<TQuery, TResponse>`), depends on Domain
- `TodoApp.Infrastructure` - EF Core, Identity, JWT, repositories, external services, depends on Application + Domain
- `TodoApp.API` - Controllers, middleware, DI registration, depends on all layers

**Key Pattern**: Use cases are organized as command/query handlers (e.g., `CreateTaskCommandHandle`, `GetUserTasksQueryHandle`) registered in `ApplicationServicesRegistration.cs`.

## Critical Development Workflows

### Build & Run
```powershell
# Build the solution
dotnet build

# Run with watch (auto-reload)
dotnet watch run --project TodoApp.API/TodoApp.API.csproj

# Or use VS Code tasks (Ctrl+Shift+B)
# Available tasks: build, publish, watch, docker-build, docker-run: debug
```

### Docker Development
```powershell
# Start infrastructure (PostgreSQL, Redis, pgAdmin)
docker-compose up -d

# Debug in Docker with hot reload
cd docker; docker-compose -f docker-compose.debug.yml up --build

# Use VS Code "Docker .NET Launch" configuration (F5) for breakpoint debugging
```

### Testing
```powershell
# Run all tests
dotnet test

# Test specific project
dotnet test Test/TodoApp.Domain.Tests

# Tests use xUnit + Moq, focus on TDD approach (write tests first)
```

**TDD Conventions**:
- **Naming**: `MethodName_WithCondition_ShouldExpectedBehavior` (e.g., `CreateEmail_WithValidValue_ShouldSetValue`)
- **Structure**: Arrange-Act-Assert pattern with clear comments
- **Theory Tests**: Use `[Theory]` with `[InlineData]` for parameterized tests
- **Mocking Pattern**:
  ```csharp
  // Constructor setup
  private readonly Mock<IRepository> _mockRepo;
  public ServiceTests() {
      _mockRepo = new Mock<IRepository>();
      _mockUnitOfWork.Setup(x => x.Repository).Returns(_mockRepo.Object);
  }
  
  // Test method - setup behavior
  _mockRepo.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(entity);
  
  // Verify interactions
  _mockRepo.Verify(x => x.SaveAsync(), Times.Once);
  ```
- **Database Tests**: Use EF InMemory with unique DB names per test: `UseInMemoryDatabase(Guid.NewGuid().ToString())`
- **Disable Seeding**: Pass `seedData: false` to `ApplicationDbContext` constructor in tests

### Database Migrations
```powershell
# Migrations auto-run in Development (see Program.cs)
# Manual migration: dotnet ef migrations add <name> --project TodoApp.Infrastructure --startup-project TodoApp.API
# Connection string: Host=localhost;Database=todo_db;Username=todo_user;Password=todo_pass
```

## Project-Specific Conventions

### Value Objects Pattern
Value objects validate in constructor and expose `Errors` list:
```csharp
// See TodoApp.Domain/ValueObjects/Email.cs
public Email(string value) {
    if (!IsValid(value)) {
        Errors.Add("Invalid email format.");
        return;
    }
    Value = value;
}
```

### Result Pattern for Error Handling
Use `Result<T>` (not exceptions) for expected failures in Application layer:
```csharp
// TodoApp.Application/Common/Result/Result.cs
public class Result<TValue> {
    public bool IsSuccess { get; }
    public Error Error { get; } // Error has IsNotFound, IsUnauthorized, IsValidation flags
    public TValue Value { get; } // throws if IsFailure
}

// Controllers use extension method to convert Result to HTTP responses
return this.FromResult(result); // See TodoApp.API/Extensions/ControllerBaseExtension.cs
```

### CQRS Implementation
Commands/queries are handled by separate handlers:
```csharp
// Command definition
public sealed record CreateTaskCommand(CreateTaskDto Dto) : ICommand<TaskDto>;

// Handler registration in ApplicationServicesRegistration.cs
services.AddScoped<ICommandHandle<CreateTaskCommand, TaskDto>, CreateTaskCommandHandle>();

// Usage in controller
public class TaskController(ICommandHandle<CreateTaskCommand, TaskDto> createHandler) {
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTaskDto dto) {
        var result = await createHandler.Handle(new CreateTaskCommand(dto), cancellationToken);
        return Ok(result);
    }
}
```

### Repository Pattern & Unit of Work
Always use Unit of Work, never instantiate repositories directly:
```csharp
// TodoApp.Infrastructure/Persistence/UnitOfWork.cs provides lazy-loaded repositories
public class UnitOfWork : IInfrastructureUnitOfWork {
    public ITaskRepository Tasks { get; } // Property initializes TaskRepository on first access
    public IUserRepository DomainUsers { get; }
    // ... etc
    
    public async Task<int> SaveChangesAsync();
    public async Task BeginTransactionAsync();
    public async Task CommitTransactionAsync();
}

// Repositories eagerly load related entities
var task = await _context.Tasks.Include(t => t.SubTasks).FirstOrDefaultAsync(...);
```

### Authentication & Authorization
- JWT tokens managed by `ITokenProvider` (Infrastructure layer)
- Identity uses `ApplicationUser` (IdentityUser<Guid>) NOT Domain `User` entity
- Controllers use `[Authorize]` attribute, current user via `ICurrentUserService`
- Google SSO via `GoogleLoginCommandHandle`

## Important Integration Points

### Frontend Communication
- CORS configured for `http://localhost:3000` (React dev server) in `Program.cs`
- Frontend is React + TypeScript monorepo in `frontend/apps/web`
- API base: `http://localhost:8080` when running via Docker, `https://localhost:5001` for local .NET

### Frontend Structure
- **Monorepo**: npm workspaces in `frontend/` with TypeScript path aliases
- **Main App**: `frontend/apps/web` - React 18 + Vite + Tamagui UI
- **Shared Packages**: `@todo-app/ui`, `@todo-app/hooks`, `@todo-app/utils` (via path aliases)
- **Development**: `npm run dev:web` from `frontend/` or `npm run dev` from `frontend/apps/web`
- **Build**: Vite builds to `frontend/apps/web/dist`

### External Dependencies
- **PostgreSQL**: EF Core migrations, seeded test data in Development
- **Redis**: Configured for distributed caching via `IDistributedCache`, intended for:
  - Session/token caching
  - User profile caching
  - Task list caching for performance
  - Connection: `Redis__ConnectionString` in appsettings
- **SignalR**: Planned for real-time task updates across browser tabs (see DEVELOPMENT_PLAN.md Phase 4)

### Central Package Management
All NuGet versions defined in `Directory.Packages.props` with `<ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>`. Don't specify versions in individual `.csproj` files.

## Common Pitfalls

- **Don't** mix `Domain.Entities.User` with `Infrastructure.Persistence.Auth.ApplicationUser` - they serve different purposes
- **Always** use `Include()` in repositories to load navigation properties (SubTasks, Group, User)
- **Never** return `Result.Value` without checking `IsSuccess` first - it throws
- **Remember** async/await: handlers return `Task<T>`, EF methods are async
- **Use** `IInfrastructureUnitOfWork` in Infrastructure, `IUnitOfWork` in Application (same implementation)

## Key Files for Reference

- Service registration: `ApplicationServicesRegistration.cs`, `InfrastructureServicesRegistration.cs`, `Program.cs`
- EF configuration: `TodoApp.Infrastructure/Persistence/Configurations/*.cs`
- Test examples: `Test/TodoApp.Domain.Tests/ValueObjects/EmailTests.cs`
- Docker setup: `docker-compose.yml`, `DOCKER_DEBUG.md`
