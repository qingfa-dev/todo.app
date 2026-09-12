# Todo App

Full-stack todo application with .NET 10 API and Vue 3 frontend.

## Tech Stack

- **Backend:** ASP.NET Core 10, Carter, FluentValidation, EF Core + PostgreSQL
- **Frontend:** Vue 3, Pinia, Vue Router, Vite, Tailwind CSS
- **Testing:** xUnit + FluentAssertions (backend), Vitest + Vue Test Utils (frontend)
- **Infra:** Podman/Docker Compose, .NET Aspire, GitHub Actions CI/CD

## Quick Start

### With Podman/Docker (recommended)

```bash
make dev
```

Starts PostgreSQL, API, and Web in containers. Open http://localhost:5173.

### With .NET Aspire

```bash
make run-aspire
```

### Standalone (requires local PostgreSQL)

```bash
make run
```

## Commands

| Command | Description |
|---|---|
| `make test` | Run all tests |
| `make test-be` | Backend tests only |
| `make test-fe` | Frontend tests only |
| `make coverage` | Run tests with coverage |
| `make up` | Start full stack in Podman |
| `make down` | Stop all containers |
| `make logs` | Follow container logs |
| `make run-aspire` | Start via .NET Aspire |

## Project Structure

```
todo.app/
├── src/
│   ├── Api/           # ASP.NET Core Web API
│   ├── AppHost/       # .NET Aspire orchestration
│   ├── ServiceDefaults/
│   └── Web/           # Vue 3 frontend
├── tests/
│   ├── Api.Tests/         # Integration tests
│   └── Api.UnitTests/     # Unit tests
├── Containerfile          # API container image
├── podman-compose.yml     # Full stack compose
└── Makefile               # Dev commands
```

## CI/CD

- **CI:** Tests run on push/PR to main (GitHub Actions)
- **CD:** Docker images built and pushed to GHCR on main

## Roadmap.sh Project
This project was built as part of the Roadmap.sh Todo List API project:
https://roadmap.sh/projects/todo-list-api
