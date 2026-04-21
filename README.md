# BudgetTracker

## Overview

A production-grade personal budget tracking web application built to demonstrate DevOps engineering practices. Built with ASP.NET Core MVC (.NET 8), PostgreSQL, Entity Framework Core, and Auth0 authentication. Deployed to AWS App Runner with a fully automated GitHub Actions CI/CD pipeline.

## Tech Stack

| Technology | Description |
|---|---|
| ASP.NET Core MVC (.NET 8) | Web framework for the application |
| C# | Primary programming language |
| PostgreSQL 15 | Relational database |
| Entity Framework Core | ORM for database access |
| Auth0 | JWT-based authentication and authorization |
| xUnit + Moq | Unit testing framework and mocking library |
| Playwright | End-to-end browser testing |
| Coverlet | Code coverage collection |
| GitHub Actions | CI/CD automation |
| Docker multi-stage builds | Containerization with optimized image size |
| AWS App Runner | Managed container hosting service |
| AWS RDS | Managed PostgreSQL database in the cloud |
| AWS ECR | Container image registry |
| AWS Secrets Manager | Secure storage for credentials and secrets |
| SonarCloud | Static analysis and code quality reporting |

## Running Locally

**Prerequisites:** Docker Desktop, .NET 8 SDK, Visual Studio 2022

**Step 1:** Start the PostgreSQL container:
```bash
docker start budgettracker-db
```

**Step 2:** Set the environment to Development in Visual Studio debug properties:
```
ASPNETCORE_ENVIRONMENT=Development
```

**Step 3:** Run the application, it will be available at `https://localhost:7266`

> **Note:** `appsettings.Development.json` is gitignored. Auth0 credentials and the local connection string must be configured locally.

## Running Tests

**Unit tests:**
```bash
dotnet test --filter "FullyQualifiedName~UnitTests"
```
Or run `dotnet test` to execute all unit tests.

**E2E tests** (local only — not run in CI):

Set the required environment variables, then run:
```bash
export TEST_AUTH0_EMAIL=<your-email>
export TEST_AUTH0_PASSWORD=<your-password>
dotnet test --filter "FullyQualifiedName~E2E"
```

## CI/CD Pipeline

Every push to `master` triggers GitHub Actions. The pipeline:

1. Restores NuGet packages
2. Builds the solution
3. Runs xUnit unit tests with Coverlet coverage
4. Runs SonarCloud static analysis

E2E tests are excluded from CI. Pipeline configuration: `.github/workflows/ci.yml`

## AWS Deployment

The application is deployed to AWS App Runner (`eu-west-1`) pulling images from ECR. PostgreSQL runs on AWS RDS (`db.t3.micro`). Auth0 credentials and database connection strings are stored in AWS Secrets Manager and injected at runtime.
