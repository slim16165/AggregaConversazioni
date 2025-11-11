## Obiettivi Enterprise
- **Disponibilita'**: SLA 99.5% con tempi di risposta < 500ms per conversioni standard (<100 KB), scaling automatico per batch piu' grandi.
- **Security & Compliance**: gestione identita' centralizzata (OAuth2/OIDC), audit trail completo, cifratura in transito (TLS 1.3) e a riposo (Azure Key Vault / AWS KMS). Aderenza GDPR (data minimization, diritto all'oblio) e logging conforme.
- **Data Governance**: versionamento trasformazioni, tracciabilita' dei pattern regex, retention configurabile per contenuti sensibili.
- **Operativita'**: pipeline CI/CD con quality gate, scanning sicurezza, rollback rapido, test automatizzati (unit, integration, regression su dataset).
- **Estensibilita'**: onboarding rapido nuovi transformer/parser, supporto a plugin di terze parti via MCP, configurazioni multi-tenant.

## Requisiti Funzionali Prioritari
- Conversione conversazioni multi-sorgente (chat, markdown, note) tramite API e interfacce utente (desktop/web).
- Preview differenze, audit delle regole applicate, esportazione verso MediaWiki, Neo4j, storage documentale.
- Scheduler per batch e webhook/eventi su completamento conversione.
- Catalogo template e regex gestito via portal/admin API con workflow di approvazione.

## Requisiti Non Funzionali
- **Performance**: pipeline streaming con backpressure; elaborazioni >5 MB spostate su job asincroni orchestrati.
- **Scalabilita'**: componenti containerizzati, autoscaling basato su CPU/memoria e queue depth.
- **Resilienza**: retry con exponential backoff, circuit breaker su integrazioni esterne, disaster recovery (RPO 15', RTO 1h).
- **Observability**: trace distribuiti (OpenTelemetry), dashboard Prometheus/Grafana, alerting su SLO.

## Architettura Target con MCP
```
Client (WPF/React/CLI) 
   ↓ HTTPS / gRPC
API Gateway (Auth, Rate limit)
   ↓ (MCP client)
MCP Gateway / Orchestrator
   ├─ Tool: Parser Service (microservizio stateless, .NET 8)
   ├─ Tool: Transformer Service (regex engine, pandoc adapter)
   ├─ Tool: Template Catalog Service (CRUD regole, versioning)
   ├─ Tool: Export Service (MediaWiki, Neo4j, file)
   ├─ Tool: Compliance/Audit Service
   └─ Context Store (Redis/SQL per conversation context)
```
- **MCP Gateway**: espone interfaccia standard per orchestrare i tool; gestisce sessione, prompt, fallback e policy (es. riduzione dati sensibili).
- **Parser/Transformer Services**: container .NET 8 isolati; implementano `ITextTransformer` evoluto con pipeline `TextInput` -> `Normalization` -> `RulesEngine` -> `Output`.
- **Export Service**: unifica integrazioni (MediaWiki API, Neo4j Bolt, S3/Azure Blob) con coda resiliente (Azure Service Bus / RabbitMQ).
- **Context Store**: memorizza step intermedi (speakers, regole applicate) e metadata per completare expander debug.

## Layering e Separazione
- **Presentation Layer**: client WPF aggiornato o nuova SPA; delega operazioni via API; mantiene solo funzioni di editing locale.
- **Application Layer** (.NET 8): orchestratore di workflow (Minimal API/ASP.NET Core) con DI, validation, logging strutturato.
- **Domain Layer**: libreria condivisa (.NET 8 class library) contenente motore regex, mapping, contratti MCP, logica business isolata da UI.
- **Infrastructure Layer**: adapter verso storage (PostgreSQL per metadata, Redis per cache, Blob per allegati/archivi) e provider segreti.

## Dati e Storage
- **Relazionale**: PostgreSQL/Azure SQL per metadati conversione, versioni regole, utenti, audit.
- **Blob Storage**: archiviazione contenuti grezzi e output, cifrati, con lifecycle policy.
- **Event Streaming**: Kafka/Event Hubs per notifiche conversione completate, hooking analytics.
- **Config & Secrets**: Azure App Configuration / AWS AppConfig, Key Vault/Secrets Manager.

## Sicurezza
- Identity provider centralizzato (Azure AD/Keycloak) con ruoli `Viewer`, `Editor`, `Administrator`.
- Policy di data masking per anteprime, logging con PII scrubbing.
- Least privilege tramite Managed Identity/pod identity; segreti mai nel codice.
- Penetration test periodici, dependency scan (OWASP DC, Snyk).

## Processi DevSecOps
- Branching Git standard (trunk-based o GitHub Flow) con policy PR.
- GitHub Actions/Azure DevOps pipeline: build, test, SonarQube, SAST (CodeQL), container scan (Trivy), deploy.
- Ambienti: `dev` (feature flags), `staging` (mirror prod), `prod` (multi-region).
- Infrastructure as Code (Bicep/Terraform) per risorse cloud, Helm chart per servizi MCP.

## Roadmap Architetturale (Macro)
1. Estrazione core logico in libreria .NET Standard/8 riusabile.
2. Definizione contract API e prototipo orchestratore MCP con 1-2 tool (es. Markdown → Wiki).
3. Containerizzazione servizi, introduzione message broker e storage condiviso.
4. Migrazione client WPF a thin client, implementazione auth centralizzata.
5. Hardening sicurezza, osservabilita' end-to-end, supporto multi-tenant.
