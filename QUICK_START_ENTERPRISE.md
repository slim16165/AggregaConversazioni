# ⚡ Quick Start - Setup Enterprise

Guida rapida per iniziare la migrazione enterprise con esempi pratici di configurazione.

---

## 🏃 Primi Passi

### 1. Creazione Solution Structure

```bash
# Crea la nuova struttura
mkdir AggregaConversazioni.Enterprise
cd AggregaConversazioni.Enterprise

# Crea i progetti
dotnet new sln -n AggregaConversazioni.Enterprise

# Domain Layer
dotnet new classlib -n AggregaConversazioni.Domain -f net8.0
dotnet sln add AggregaConversazioni.Domain

# Application Layer
dotnet new classlib -n AggregaConversazioni.Application -f net8.0
dotnet sln add AggregaConversazioni.Application
dotnet add AggregaConversazioni.Application reference AggregaConversazioni.Domain

# Infrastructure Layer
dotnet new classlib -n AggregaConversazioni.Infrastructure -f net8.0
dotnet sln add AggregaConversazioni.Infrastructure
dotnet add AggregaConversazioni.Infrastructure reference AggregaConversazioni.Domain
dotnet add AggregaConversazioni.Infrastructure reference AggregaConversazioni.Application

# API Layer
dotnet new webapi -n AggregaConversazioni.Api -f net8.0
dotnet sln add AggregaConversazioni.Api
dotnet add AggregaConversazioni.Api reference AggregaConversazioni.Application
dotnet add AggregaConversazioni.Api reference AggregaConversazioni.Infrastructure

# MCP Server
dotnet new classlib -n AggregaConversazioni.McpServer -f net8.0
dotnet sln add AggregaConversazioni.McpServer
dotnet add AggregaConversazioni.McpServer reference AggregaConversazioni.Application

# Tests
dotnet new xunit -n AggregaConversazioni.UnitTests -f net8.0
dotnet sln add AggregaConversazioni.UnitTests
dotnet add AggregaConversazioni.UnitTests reference AggregaConversazioni.Application
```

---

## 📦 Pacchetti NuGet Essenziali

### Domain Layer
```bash
cd AggregaConversazioni.Domain
# Nessun pacchetto esterno necessario (pure domain logic)
```

### Application Layer
```bash
cd AggregaConversazioni.Application
dotnet add package MediatR
dotnet add package FluentValidation
dotnet add package AutoMapper
```

### Infrastructure Layer
```bash
cd AggregaConversazioni.Infrastructure
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add package Neo4j.Driver
dotnet add package StackExchange.Redis
dotnet add package Serilog
dotnet add package Serilog.Sinks.Console
dotnet add package Serilog.Sinks.File
dotnet add package Serilog.Sinks.Elasticsearch
dotnet add package Microsoft.Extensions.Configuration
dotnet add package Microsoft.Extensions.DependencyInjection
```

### API Layer
```bash
cd AggregaConversazioni.Api
dotnet add package Swashbuckle.AspNetCore
dotnet add package HotChocolate.AspNetCore
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add package AspNetCoreRateLimit
dotnet add package Serilog.AspNetCore
dotnet add package HealthChecks.UI
dotnet add package HealthChecks.UI.Client
```

---

## 🔧 Configurazione Base

### appsettings.json (API)

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "PostgreSQL": "Host=localhost;Database=aggrega_conv;Username=postgres;Password=password",
    "Neo4j": "bolt://localhost:7687",
    "Redis": "localhost:6379"
  },
  "Jwt": {
    "SecretKey": "your-secret-key-here-min-32-chars",
    "Issuer": "AggregaConversazioni",
    "Audience": "AggregaConversazioni",
    "ExpirationMinutes": 60
  },
  "RateLimiting": {
    "EnableEndpointRateLimiting": true,
    "StackBlockedRequests": false,
    "HttpStatusCode": 429,
    "GeneralRules": [
      {
        "Endpoint": "*",
        "Period": "1m",
        "Limit": 60
      }
    ]
  },
  "Serilog": {
    "Using": [ "Serilog.Sinks.Console", "Serilog.Sinks.File" ],
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning"
      }
    },
    "WriteTo": [
      {
        "Name": "Console",
        "Args": {
          "outputTemplate": "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"
        }
      },
      {
        "Name": "File",
        "Args": {
          "path": "logs/log-.txt",
          "rollingInterval": "Day",
          "retainedFileCountLimit": 30
        }
      }
    ]
  }
}
```

### Program.cs (API) - Setup Base

```csharp
using AggregaConversazioni.Application;
using AggregaConversazioni.Infrastructure;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Application & Infrastructure
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Health Checks
builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("PostgreSQL"))
    .AddRedis(builder.Configuration.GetConnectionString("Redis"));

var app = builder.Build();

// Configure pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
```

---

## 🗄️ Database Setup

### Docker Compose per Sviluppo

Crea `docker-compose.yml`:

```yaml
version: '3.8'

services:
  postgres:
    image: postgres:16-alpine
    environment:
      POSTGRES_DB: aggrega_conv
      POSTGRES_USER: postgres
      POSTGRES_PASSWORD: password
    ports:
      - "5432:5432"
    volumes:
      - postgres_data:/var/lib/postgresql/data

  neo4j:
    image: neo4j:5-community
    environment:
      NEO4J_AUTH: neo4j/password
      NEO4J_PLUGINS: '["apoc"]'
    ports:
      - "7474:7474"  # HTTP
      - "7687:7687"  # Bolt
    volumes:
      - neo4j_data:/data

  redis:
    image: redis:7-alpine
    ports:
      - "6379:6379"
    volumes:
      - redis_data:/data

  seq:
    image: datalust/seq:latest
    environment:
      ACCEPT_EULA: Y
    ports:
      - "5341:80"
    volumes:
      - seq_data:/data

volumes:
  postgres_data:
  neo4j_data:
  redis_data:
  seq_data:
```

Avvia con:
```bash
docker-compose up -d
```

---

## 🧪 Esempio Test Unit

### AggregaConversazioni.UnitTests/Parsers/MessengerParserTests.cs

```csharp
using Xunit;
using AggregaConversazioni.Application.Parsers;
using FluentAssertions;

namespace AggregaConversazioni.UnitTests.Parsers;

public class MessengerParserTests
{
    [Fact]
    public void Transform_ValidMessengerFormat_ReturnsWikiFormat()
    {
        // Arrange
        var parser = new MessengerParser();
        var input = "[10:00 AM] John: Ciao, come stai?\n[10:01 AM] Jane: Bene, grazie!";

        // Act
        var result = parser.Transform(input);

        // Assert
        result.Should().Contain("John");
        result.Should().Contain("Jane");
        result.Should().Contain("{{Conversazione");
    }

    [Fact]
    public void Transform_EmptyInput_ReturnsEmptyString()
    {
        // Arrange
        var parser = new MessengerParser();
        var input = string.Empty;

        // Act
        var result = parser.Transform(input);

        // Assert
        result.Should().BeEmpty();
    }
}
```

---

## 🚀 Dockerfile Esempio

### AggregaConversazioni.Api/Dockerfile

```dockerfile
# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore
COPY ["AggregaConversazioni.Api/AggregaConversazioni.Api.csproj", "AggregaConversazioni.Api/"]
COPY ["AggregaConversazioni.Application/AggregaConversazioni.Application.csproj", "AggregaConversazioni.Application/"]
COPY ["AggregaConversazioni.Domain/AggregaConversazioni.Domain.csproj", "AggregaConversazioni.Domain/"]
COPY ["AggregaConversazioni.Infrastructure/AggregaConversazioni.Infrastructure.csproj", "AggregaConversazioni.Infrastructure/"]

RUN dotnet restore "AggregaConversazioni.Api/AggregaConversazioni.Api.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/AggregaConversazioni.Api"
RUN dotnet build "AggregaConversazioni.Api.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "AggregaConversazioni.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 80
EXPOSE 443

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AggregaConversazioni.Api.dll"]
```

---

## 📋 Kubernetes Manifests

### k8s/deployment.yaml

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: aggrega-conversazioni-api
  labels:
    app: aggrega-conversazioni-api
spec:
  replicas: 3
  selector:
    matchLabels:
      app: aggrega-conversazioni-api
  template:
    metadata:
      labels:
        app: aggrega-conversazioni-api
    spec:
      containers:
      - name: api
        image: aggrega-conversazioni/api:latest
        ports:
        - containerPort: 80
        env:
        - name: ConnectionStrings__PostgreSQL
          valueFrom:
            secretKeyRef:
              name: db-secrets
              key: postgres-connection
        - name: ConnectionStrings__Neo4j
          valueFrom:
            secretKeyRef:
              name: db-secrets
              key: neo4j-connection
        livenessProbe:
          httpGet:
            path: /health
            port: 80
          initialDelaySeconds: 30
          periodSeconds: 10
        readinessProbe:
          httpGet:
            path: /health/ready
            port: 80
          initialDelaySeconds: 10
          periodSeconds: 5
        resources:
          requests:
            memory: "256Mi"
            cpu: "250m"
          limits:
            memory: "512Mi"
            cpu: "500m"
---
apiVersion: v1
kind: Service
metadata:
  name: aggrega-conversazioni-api
spec:
  selector:
    app: aggrega-conversazioni-api
  ports:
  - protocol: TCP
    port: 80
    targetPort: 80
  type: LoadBalancer
```

---

## 🔄 GitHub Actions CI/CD

### .github/workflows/ci-cd.yml

```yaml
name: CI/CD Pipeline

on:
  push:
    branches: [ main, develop ]
  pull_request:
    branches: [ main ]

env:
  REGISTRY: ghcr.io
  IMAGE_NAME: ${{ github.repository }}

jobs:
  build:
    runs-on: ubuntu-latest
    
    steps:
    - uses: actions/checkout@v4
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: '8.0.x'
    
    - name: Restore dependencies
      run: dotnet restore
    
    - name: Build
      run: dotnet build --no-restore -c Release
    
    - name: Test
      run: dotnet test --no-build -c Release --verbosity normal --collect:"XPlat Code Coverage"
    
    - name: Publish coverage
      uses: codecov/codecov-action@v3
      with:
        files: '**/coverage.cobertura.xml'
    
    - name: Build Docker image
      run: |
        docker build -t ${{ env.REGISTRY }}/${{ env.IMAGE_NAME }}:latest .
    
    - name: Push Docker image
      if: github.event_name == 'push' && github.ref == 'refs/heads/main'
      run: |
        echo ${{ secrets.GITHUB_TOKEN }} | docker login ${{ env.REGISTRY }} -u ${{ github.actor }} --password-stdin
        docker push ${{ env.REGISTRY }}/${{ env.IMAGE_NAME }}:latest
```

---

## 📊 Monitoring Setup

### Prometheus Configuration

Crea `prometheus.yml`:

```yaml
global:
  scrape_interval: 15s

scrape_configs:
  - job_name: 'aggrega-conversazioni-api'
    metrics_path: '/metrics'
    static_configs:
      - targets: ['localhost:5000']
```

### Grafana Dashboard JSON

Esporta metriche chiave:
- Conversioni per tipo
- Tempo di risposta API
- Errori per endpoint
- Throughput richieste

---

## ✅ Checklist Quick Start

- [ ] Solution creata con struttura modulare
- [ ] Pacchetti NuGet installati
- [ ] Database configurati (Docker Compose)
- [ ] API base funzionante
- [ ] Health checks implementati
- [ ] Logging configurato
- [ ] Test unitari base scritti
- [ ] Dockerfile creato
- [ ] CI/CD pipeline configurata
- [ ] Documentazione API (Swagger) accessibile

---

## 🎯 Prossimi Passi

1. **Implementa Domain Layer**: Definisci entità e interfacce
2. **Implementa Application Layer**: Use cases con MediatR
3. **Implementa Infrastructure**: Repository e servizi esterni
4. **Implementa API**: Controllers e GraphQL
5. **Aggiungi MCP Server**: Segui guida MCP_IMPLEMENTATION_GUIDE.md
6. **Aggiungi Autenticazione**: JWT e SSO
7. **Aggiungi Background Jobs**: Hangfire o Quartz.NET
8. **Deploy su Kubernetes**: Usa manifesti forniti

---

**Versione**: 1.0  
**Ultimo aggiornamento**: 2024
