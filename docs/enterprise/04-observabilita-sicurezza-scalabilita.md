## Osservabilita'

### Foundation
- **Logging strutturato**: Serilog + sink (Seq/ELK). Correlazione tramite `TraceId`, `ConversationId`.
- **Tracing distribuito**: OpenTelemetry SDK (.NET) con export verso OTLP Collector, backend Grafana Tempo/Jaeger.
- **Metriche**: Prometheus scraper (via OpenTelemetry Metrics) per:
  - latenza trasformazioni (`conversion_duration_seconds`),
  - throughput (`conversions_total`),
  - errori per sorgente (`conversion_failures_total{source=...}`),
  - utilizzo risorse worker (`worker_cpu_percent`).
- **Log utente**: Eventi client WPF inviati a Application Insights/Azure Monitor per diagnosi UX.

### Maturita' avanzata
- SLO definiti (es. 99% richieste < 750ms), error budget e alert Prometheus Alertmanager/PagerDuty.
- Dashboard unificati (Grafana) per Product, DevOps, Security (audit).
- Synthetic monitoring (k6 operator, Azure Load Testing) e RUM (App Insights).

## Sicurezza

### Identita' e Accesso
- Integrazione con IdP enterprise (Azure AD/Keycloak), supporto OAuth2 Client Credentials per API, PKCE per client interattivi.
- Ruoli granulari (`Reader`, `Operator`, `Admin`, `Auditor`) gestiti in DB.
- Endpoint MCP protetti con policy zero-trust (mutual TLS tra servizi).

### Protezione Dati
- Cifratura at-rest tramite Transparent Data Encryption (PostgreSQL/Azure SQL) e Blob SSE-KMS.
- Secret management centralizzato (Azure Key Vault/AWS Secrets Manager), rotazione automatica.
- Data Loss Prevention: mascheramento PII nei log, classificazione metadati, retention e soft-delete configurabili.

### Secure SDLC
- Threat modeling periodico (STRIDE), secure code review checklist (OWASP ASVS).
- Dependency management (RenovateBot) + vulnerabilita' (GitHub Dependabot, Trivy).
- Pen test annuali + bug bounty interno.

## Scalabilita' e Resilienza

### Architettura
- Deployment container su Kubernetes (AKS/EKS) con HPA su CPU/QPS.
- Pod affinities per separare parsing CPU intensive e worker export IO bound.
- Redis/Memory cache per regole regex e template; invalidazione tramite evento.
- Message broker (Service Bus/RabbitMQ) per isolare workload asincroni e smoothing carico.

### Resilienza
- Circuit breaker (Polly) su API esterne (MediaWiki, Neo4j).
- Retry idempotenti con backoff e dead-letter queue.
- Chaos engineering light (Azure Chaos Studio, Litmus) per validare failover.
- Backup & DR: snapshot giornalieri DB, replica geografica, runbook recovery testata (RTO 1h, RPO 15').

## Roadmap Implementativa (6-9 mesi)
1. **Sprint 1-2**: introdurre logging strutturato e OpenTelemetry nei nuovi servizi core; configurare collector e dashboard base.
2. **Sprint 3-4**: implementare identity provider, rimuovere credenziali hardcoded, secret store, integrare scanning pipeline.
3. **Sprint 5-6**: containerizzare servizi, deploy su cluster dev con autoscaling, introdurre probe readiness/liveness.
4. **Sprint 7-8**: attivare SLO + alert, definire DR strategy completa, esercitazioni failover.
5. **Sprint 9**: audit sicurezza finale, pen test, certificazione go-live enterprise.
