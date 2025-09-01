# Docker Debugging Configuration

This guide explains how to configure and use Docker debugging for the TodoApplication.

## Prerequisites

- Docker Desktop installed and running
- Visual Studio Code with the following extensions:
  - C# Dev Kit
  - Docker
  - Dev Containers (optional)

## Configuration Overview

The application has been configured with multiple debugging options:

### 1. Docker Compose Debug Configuration

**File**: `docker/docker-compose.debug.yml`

This configuration includes:
- Debug build configuration
- Volume mounts for hot reload
- Additional debug ports
- Interactive mode enabled
- File watching with `dotnet watch`

### 2. VS Code Debugging

**Files**: 
- `.vscode/launch.json` - Debug configurations
- `.vscode/tasks.json` - Build and run tasks

## How to Debug

### Option 1: Using Docker Compose (Recommended)

1. **Start the debug environment**:
   ```bash
   cd docker
   docker-compose -f docker-compose.debug.yml up --build
   ```

2. **Attach debugger in VS Code**:
   - Open VS Code in the project root
   - Go to Run and Debug (Ctrl+Shift+D)
   - Select "Docker Compose: Launch" configuration
   - Press F5 to start debugging

### Option 2: Using VS Code Docker Extension

1. **Build and run with debugging**:
   - Open VS Code
   - Go to Run and Debug (Ctrl+Shift+D)
   - Select "Docker .NET Launch" configuration
   - Press F5

### Option 3: Manual Docker Run

1. **Build the debug image**:
   ```bash
   docker build -f TodoApp.API/Dockerfile --target build -t todoapp:debug .
   ```

2. **Run with debugging**:
   ```bash
   docker run -it --rm \
     -p 5000:80 \
     -p 5001:443 \
     -p 5002:5000 \
     -v ${PWD}/TodoApp.API:/app \
     -e ASPNETCORE_ENVIRONMENT=Development \
     todoapp:debug
   ```

## Attaching to Running Containers

### Method 1: VS Code Attach to Running Container

1. **Start your containers** (if not already running):
   ```bash
   cd docker
   docker-compose -f docker-compose.debug.yml up -d
   ```

2. **In VS Code**:
   - Open the project in VS Code
   - Go to Run and Debug (Ctrl+Shift+D)
   - Click on the gear icon to open `launch.json`
   - Add this configuration:

   ```json
   {
       "name": "Attach to Running Container",
       "type": "docker",
       "request": "attach",
       "containerName": "docker-api-1",
       "netCore": {
           "appProject": "${workspaceFolder}/TodoApp.API/TodoApp.API.csproj",
           "enableDebugging": true
       }
   }
   ```

3. **Attach the debugger**:
   - Select "Attach to Running Container" from the debug dropdown
   - Press F5 or click the green play button

### Method 2: Command Line Attach

1. **Find your running container**:
   ```bash
   docker ps
   ```

2. **Attach to the container shell**:
   ```bash
   docker exec -it docker-api-1 bash
   ```

3. **Start debugging manually** (if needed):
   ```bash
   # Inside the container
   dotnet watch run --urls http://+:80 --environment Development
   ```

### Method 3: VS Code Dev Containers

1. **Install Dev Containers extension** in VS Code

2. **Attach to running container**:
   - Press `Ctrl+Shift+P`
   - Type "Dev Containers: Attach to Running Container"
   - Select your running container (`docker-api-1`)

3. **Open the project folder** in the container

### Method 4: Remote Debugging with Port Forwarding

1. **Ensure debug ports are exposed** in your docker-compose:
   ```yaml
   ports:
     - "5000:80"
     - "5002:5000"  # Debug port
   ```

2. **In VS Code**, add this configuration to `launch.json`:
   ```json
   {
       "name": "Remote Debug Container",
       "type": "coreclr",
       "request": "attach",
       "processId": "${command:pickRemoteProcess}",
       "pipeTransport": {
           "pipeCwd": "${workspaceFolder}",
           "pipeProgram": "docker",
           "pipeArgs": ["exec", "-i", "docker-api-1"],
           "debuggerPath": "/usr/bin/dbg"
       }
   }
   ```

## Debug Features

### Hot Reload
The debug configuration includes file watching, so changes to your code will automatically trigger a rebuild and restart.

### Breakpoints
You can set breakpoints in VS Code and they will be hit when running in the Docker container.

### Logging
Debug logs are available in the container output and VS Code debug console.

## Ports

- **5000**: Main API (HTTP)
- **5001**: HTTPS (if configured)
- **5002**: Debug port
- **5003**: Additional debug port
- **5432**: PostgreSQL
- **6379**: Redis
- **5050**: PgAdmin

## Environment Variables

The debug configuration includes these environment variables:
- `ASPNETCORE_ENVIRONMENT=Development`
- `DOTNET_USE_POLLING_FILE_WATCHER=1`
- `DOTNET_EnableDiagnostics=1`

## Troubleshooting

### Container won't start
1. Check if ports are already in use
2. Ensure Docker Desktop is running
3. Check the container logs: `docker logs <container-name>`

### Debugger won't attach
1. Ensure the container is running in debug mode
2. Check if the debug ports are properly exposed
3. Verify VS Code Docker extension is installed

### Hot reload not working
1. Check if volume mounts are correct
2. Ensure `DOTNET_USE_POLLING_FILE_WATCHER=1` is set
3. Verify file permissions on mounted volumes

### Attaching to running container fails
1. Ensure the container was built with debug symbols
2. Check if the container is running the debug version
3. Verify the container name matches your configuration
4. Try restarting the container with debug flags

## Commands Reference

```bash
# Start debug environment
docker-compose -f docker/docker-compose.debug.yml up --build

# Stop debug environment
docker-compose -f docker/docker-compose.debug.yml down

# View logs
docker-compose -f docker/docker-compose.debug.yml logs -f api

# Enter container shell
docker-compose -f docker/docker-compose.debug.yml exec api bash

# Rebuild without cache
docker-compose -f docker/docker-compose.debug.yml build --no-cache

# List running containers
docker ps

# Attach to running container
docker exec -it docker-api-1 bash

# View container logs
docker logs docker-api-1

# Restart container with debug
docker-compose -f docker/docker-compose.debug.yml restart api
```

## VS Code Integration

The `.vscode` folder contains configurations for:
- Launch configurations for different debugging scenarios
- Build tasks for Docker
- Debugging with breakpoints and variable inspection

## Performance Tips

1. Use `.dockerignore` to exclude unnecessary files
2. Mount only required volumes
3. Use multi-stage builds to reduce image size
4. Consider using Docker BuildKit for faster builds 