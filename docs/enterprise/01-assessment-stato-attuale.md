## Sintesi Stato Attuale
- Applicazione desktop WPF (.NET Framework 4.7.2) monolitica, soluzione singola `AggregaConversazioni`.
- Orientata a conversioni testuali via regex e trasformatori specifici, senza servizi esterni ne' storage persistente.
- Dipendenze legacy (packages.config, binding redirect manuali, riferimenti Telerik locali) e assenza di test, logging strutturato e telemetria.
- Vari componenti incompleti (`ParserBase.Transform` non implementato), namespace incoerenti, responsabilita' miste UI/business.

## Stack Tecnologico
- **Runtime**: .NET Framework 4.7.2, ToolsVersion 15.0, csproj di vecchio formato.
- **UI**: WPF con AvalonEdit, MahApps IconPacks, Telerik controls caricati da path locale (props esterno).
- **Librerie**: Fody.PropertyChanged, numerose MahApps icon pack, Neo4j.Driver, Pandoc.Windows, Microsoft.* pacchetti portati via packages.config.
- **Configurazione**: `App.config` con bindingRedirect espliciti per tredici assembly esterni; nessun uso di configurazione ambientale o segreti sicuri.

## Organizzazione Codice
- **Presentation Layer**: `MainWindow.xaml` e `MainViewModel.cs` gestiscono UI e logica di conversione. Nessun framework MVVM formale, dependency injection assente.
- **Core di conversione**: cartelle `Transformers` e `Parsers`; transformer regex funzionanti (es. `MarkdownToWikiConverter`), parser conversazionali derivano da `ParserBase` ma molte API sono placeholder (`GetResult` e `Transform` restituiscono valori fittizi).
- **Supporto/debug**: cartella `Helpers` (binding AvalonEdit, DebugHelper, SpeakerIdentification) senza test unita.
- **Integrations**: `MediaWiki2Neo4j` contiene prototipo sincrono/asciutto con credenziali hardcoded per Neo4j.

## Flussi Principali
- L'utente inserisce testo, `MainViewModel` invoca `TransformerFactory.Create` che istanzia direttamente una classe concreta.
- I parser tentano catena `WithText` -> `SplitTextIntoLines` -> `IdentifySpeakers` -> `InitializeRegexPatterns` -> `ApplyRegexAndClean`, ma mancano implementazioni per restituire dati e per riempire `AppliedRules` e `Speakers`.
- Debug panel in XAML si aspetta `AppliedRules` e `Speakers` popolati, ma pipeline corrente non li aggiorna realmente.

## Dipendenze Esterne e Vincoli
- **Telerik**: import condizionale da percorsi locali (no riproducibilita' build server). Nessun pacchetto NuGet ufficiale configurato.
- **Pandoc**: requisito runtime esterno manuale (`Pandoc 2.19+`) con gestione installazione fuori banda.
- **Neo4j**: driver referenziato ma configurazione e credenziali codificate (`bolt://localhost`, `neo4j/password`).
- **Fody.PropertyChanged**: weaving manuale, necessita di step MSBuild specifici.

## Processi e Tooling
- Nessuna cartella `tests`, nessun framework di test menzionato.
- Nessuna pipeline CI/CD presente; `Readme` suggerisce manualmente Visual Studio.
- Nessuna automazione per packaging, deploy, code quality (static analysis, sicurezza).

## Debito Tecnico e Rischi
- Monolite UI + logica, nessuna separazione in livelli/servizi, impossibile scalare o riusare funzioni in contesti serverless/API.
- Uso di `packages.config` (legacy) e binding manuali, upgrade difficile.
- Incoerenze namespace (`AggregaConversazioni.Utils` referenziato ma non esiste), segnale di codice incompleto o rimosso.
- Metodi cruciali non implementati (`ParserBase.Transform`, `GetResult`), per cui alcune feature descritte in README non funzionano.
- Gestione errori elementare (solo `MessageBox`), nessun logging.
- Credenziali hardcoded e nessuna gestione segreti/compliance.
- Dipendenza forte da UI Windows (WPF) limita portabilita' e scalabilita'.

## Gap rispetto a requisiti enterprise
- Mancanza di architettura modulare/servizi orchestrabili (necessario per MCP).
- Assenza totale di osservabilita', audit, versionamento schemi, gestione configurazioni multi-ambiente.
- Nessun controllo accessi, autenticazione o pipeline di sicurezza.
- Assenza di infrastruttura dati condivisa (DB, storage documentale) e di politiche di retention.
- Build non riproducibile: dipende da percorsi locali Telerik, strumenti installati manualmente.

## Raccomandazioni Immediate
- Rifinire mappa delle dipendenze e estrarre diagramma high-level (UI -> Service -> Transformer).
- Identificare priorita' refactoring: completare implementazioni base dei parser, separare core logico in libreria .NET Standard.
- Preparare inventario credenziali/integrazioni (Neo4j, Pandoc) per futura messa in sicurezza.
