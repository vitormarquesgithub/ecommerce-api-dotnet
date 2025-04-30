# ECommerce API

## Overview
A modern e-commerce backend system built with .NET 8, featuring JWT authentication, role-based authorization, and containerized deployment. The API provides endpoints for product management, customer authentication, and sales processing.

## Key Features
- **Product Management**: Full CRUD operations for products
- **Customer Authentication**: JWT-based auth with role claims
- **Sales Processing**: Create sales with product associations
- **Sales Analytics**: Time-based sales reporting
- **JWT Authentication**: Configurable token expiration
- **Role-Based Authorization**: Three roles (Customer/Manager/Admin)
- **Serilog Logging**: Console + File + Debug outputs
- **Docker Support**: Containerized with health checks
- **Auto Migrations**: Database schema updates on startup
- **API Versioning**: Version 1.x support
- **Swagger Docs**: Interactive API documentation
- **CORS**: Configured for frontend access

## Technology Stack

| Component          | Technology                          |
|--------------------|-------------------------------------|
| Framework          | .NET 8                              |
| Database           | SQL Server 2022                     |
| ORM                | Entity Framework Core 8             |
| Authentication     | JWT Bearer Tokens                   |
| Logging            | Serilog (Console + File)            |
| API Documentation  | Swagger/OpenAPI 3.0                 |
| Containerization   | Docker + Docker Compose             |
| Testing            | xUnit (Unit + Integration tests)    |

## Quick Start
1. Install Docker Desktop
2. Clone repository
3. Run: `make -f scripts/Makefile run` or `docker-compose up --build`
4. Access API at: `https://localhost:8080/swagger`

## Configuration
Edit `appsettings.Development.json` for:
- JWT settings (secret/expiry)
- Database connection
- CORS origins
- Logging paths

## Makefile Command Reference

| Command | Description | Category |
|---------|-------------|----------|
| `make -f scripts/Makefile run` | Build containers, start services with progress bar, and verify availability | Docker |
| `make -f scripts/Makefile up` | Start containers in detached mode | Docker |
| `make -f scripts/Makefile down` | Stop and remove containers | Docker |
| `make -f scripts/Makefile restart` | Restart running containers | Docker |
| `make -f scripts/Makefile build` | Rebuild Docker images without starting | Docker |
| `make -f scripts/Makefile logs` | Tail the last 10 log lines from API container | Docker |
| `make -f scripts/Makefile clean` | Full system reset (remove containers, volumes, and images) | Docker |
| `make -f scripts/Makefile prune` | Clean unused Docker resources | Docker |
| `make -f scripts/Makefile status` | Show container status | Docker |
| `make -f scripts/Makefile migration` | Reset database (remove migrations, volume, and recreate) | Database |
| `make -f scripts/Makefile test` | Run all unit tests | Testing |
| `make -f scripts/Makefile help` | Show available commands with descriptions | Help |
