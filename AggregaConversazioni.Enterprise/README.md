# AggregaConversazioni Enterprise

Piattaforma enterprise per la trasformazione di conversazioni e testi in formato MediaWiki.

## 🚀 Quick Start

### Prerequisiti
- .NET 8 SDK
- Docker e Docker Compose
- PostgreSQL 16+
- Neo4j 5+
- Redis 7+

### Setup Locale

1. **Clona il repository**
```bash
git clone <repository-url>
cd AggregaConversazioni.Enterprise
```

2. **Avvia i servizi con Docker Compose**
```bash
cd docker
docker-compose up -d
```

3. **Esegui le migrazioni del database**
```bash
dotnet ef database update --project src/AggregaConversazioni.Infrastructure --startup-project src/AggregaConversazioni.Api
```

4. **Avvia l'API**
```bash
cd src/AggregaConversazioni.Api
dotnet run
```

5. **Accedi a Swagger**
```
http://localhost:5000/swagger
```

## 📁 Struttura del Progetto

```
AggregaConversazioni.Enterprise/
├── src/
│   ├── AggregaConversazioni.Domain/          # Entità e interfacce
│   ├── AggregaConversazioni.Application/    # Use cases e business logic
│   ├── AggregaConversazioni.Infrastructure/ # DB, repository, servizi esterni
│   ├── AggregaConversazioni.Api/             # API REST/GraphQL
│   ├── AggregaConversazioni.McpServer/      # Server MCP
│   └── AggregaConversazioni.Worker/          # Background jobs
├── tests/
│   ├── AggregaConversazioni.UnitTests/
│   └── AggregaConversazioni.IntegrationTests/
├── docker/
│   └── docker-compose.yml
└── k8s/
    └── deployment.yaml
```

## 🔧 Configurazione

Modifica `appsettings.json` per configurare:
- Connection strings per database
- JWT settings
- Logging

## 📚 API Documentation

L'API è documentata con Swagger/OpenAPI. Accedi a `/swagger` quando l'applicazione è in esecuzione.

## 🔌 MCP Server

Il server MCP è disponibile su `http://localhost:5001/mcp` e supporta:
- `transform_conversation`: Trasforma conversazioni in formato MediaWiki

## 🧪 Testing

```bash
# Esegui tutti i test
dotnet test

# Test con coverage
dotnet test --collect:"XPlat Code Coverage"
```

## 🐳 Docker

```bash
# Build immagine
docker build -t aggrega-conversazioni/api -f docker/Dockerfile .

# Run container
docker run -p 8080:80 aggrega-conversazioni/api
```

## ☸️ Kubernetes

```bash
# Applica deployment
kubectl apply -f k8s/deployment.yaml
```

## 📖 Documentazione Completa

Vedi:
- `ENTERPRISE_MIGRATION_PLAN.md` - Piano completo di migrazione
- `MCP_IMPLEMENTATION_GUIDE.md` - Guida implementazione MCP
- `QUICK_START_ENTERPRISE.md` - Quick start dettagliato

## 📄 Licenza

MIT License
