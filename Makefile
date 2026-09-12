.PHONY: test test-be test-fe coverage coverage-be coverage-fe
.PHONY: run run-be run-fe run-aspire
.PHONY: up down logs build ps
.PHONY: dev

# --- Tests ---

test: test-be test-fe

test-be:
	dotnet test

test-fe:
	cd src/Web && pnpm test

coverage: coverage-be coverage-fe

coverage-be:
	dotnet test --collect:"XPlat Code Coverage"

coverage-fe:
	cd src/Web && pnpm test:coverage

# --- Run standalone (expects external PostgreSQL) ---

run: run-be run-fe

run-be:
	dotnet run --project src/Api

run-fe:
	cd src/Web && pnpm dev

# --- Run full stack via .NET Aspire ---

run-aspire:
	dotnet run --project src/AppHost

# --- Podman containers (full stack: PG + API + Web) ---

up:
	podman-compose up -d --build

down:
	podman-compose down

logs:
	podman-compose logs -f

build:
	podman-compose build

ps:
	podman-compose ps

# --- Dev: full local stack in Podman ---

dev: up
