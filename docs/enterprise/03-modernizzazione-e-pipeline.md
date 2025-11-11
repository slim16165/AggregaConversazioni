## Strategia di Modernizzazione Codebase

### 1. Preparazione (2-3 sprint)
- **Inventario dipendenze**: sostituire `packages.config` con `PackageReference`; valutare alternative ufficiali per Telerik (NuGet aziendale o controllo licenza).
- **Pulizia codice**: eliminare namespace orfani (`AggregaConversazioni.Utils`), completare metodi critici (`ParserBase.Transform`, `GetResult`), introdurre logging minimale (Serilog console/file).
- **Coverage test iniziale**: introdurre progetto `AggregaConversazioni.Tests` (.NET 8) con xUnit; creare fixture per `MarkdownToWikiConverter` e parser piu' stabili.
- **Tooling**: configurare `EditorConfig`, regole Roslyn, analizzatori stile (StyleCop/IDisposable).

### 2. Migrazione Tecnologica (4-6 sprint)
- **SDK-style project**: creare soluzione multi-project:
  - `AggregaConversazioni.Core` (.NET 8 class library) per logica trasformazione.
  - `AggregaConversazioni.Parsers` (.NET 8) con pipeline + interfacce.
  - `AggregaConversazioni.App.Wpf` (.NET 8 + WindowsDesktop) come thin client.
  - `AggregaConversazioni.Api` (ASP.NET Core Minimal API) esposizione servizi.
- **Porting graduale**:
  - Migrare classi condivise in `Core`, adattare a `nullable reference types`, `async` dove possibile.
  - Rimpiazzare `MessageBox` con pattern `INotificationService`.
  - Introdurre dependency injection (Microsoft.Extensions.Hosting).
- **Testing & Quality**:
  - Ampliare test unitari/regression dataset (snapshot test per output attesi).
  - Aggiungere test di integrazione per API via `WebApplicationFactory`.
- **Pandoc/Neo4j**:
  - Isolare wrapper in servizi `IPandocAdapter`, `IGraphClient` con configurazione esterna.

### 3. Hardening e Performance (continuo)
- Profilazione (BenchmarkDotNet) per pipeline regex; valutare parallelizzazione chunking.
- Implementare metriche in `Core` (contatore regex, tempi per sezione).
- Rifattorizzare `ParserBase` in pipeline modulare (input -> normalizzazione -> estrazione metadata -> generazione output).

## Roadmap CI/CD

### Pipeline CI (GitHub Actions)
1. **Build & Test** (`ci.yml`)
   - Matrix `windows-latest` per WPF e `ubuntu-latest` per librerie.
   - Step: `dotnet restore`, `dotnet build -warnaserror`, `dotnet test --collect:"XPlat Code Coverage"`.
   - Upload coverage (Coverlet + ReportGenerator).
   - Analisi statica SonarQube (`SonarCloud` o self-host).
2. **Security**:
   - `dotnet list package --vulnerable`.
   - CodeQL C# workflow settimanale.
   - Trivy scan immagini Docker (per servizi API/MCP Tools).

### Pipeline CD
- **Dev Deploy**: push su branch `main` -> deploy automatico su ambiente `dev`:
  - Pubblicazione container immagini su registry (GitHub Container Registry o ACR/ECR).
  - Deploy via Helm su namespace `dev` (AKS/EKS) o Azure Container Apps.
- **Staging**: trigger manuale con approvazione; esecuzione smoke test (Postman/Newman) e load test leggeri (k6).
- **Prod**: deployment progressivo (blue/green o canary) con feature flag (Azure App Configuration, LaunchDarkly).

### Delivery Desktop
- Generare pacchetto MSIX/WIX per client WPF, firmato con certificato aziendale.
- Automatizzare pipeline release (GitHub Actions + `dotnet publish` + `make-msix`) e distribuzione via Intune/Endpoint Manager.

## Governance del Codice
- **Branch policy**: PR obbligatorie con 2 reviewer, stati CI verdi, coverage minimo 80%.
- **Git Hooks**: `pre-commit` per lint (dotnet format), `commit-msg` per convenzione (Conventional Commits).
- **Documentazione**: integrare docfx o MkDocs alimentato da `docs/enterprise`.

## Deliverable Iniziali
1. Script `dotnet workload install wasm-tools` (se si prepara client web) e `global.json` per fissare SDK.
2. Template `Directory.Build.props` + `Directory.Packages.props` per centralizzare versioni.
3. Roadmap Kanban (Azure Boards/Jira) con epic: `CoreExtraction`, `APIExposure`, `ClientModernization`, `CI-CD`.
