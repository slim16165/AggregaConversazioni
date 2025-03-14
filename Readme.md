# Text Converter Pro

Un'applicazione WPF avanzata per la conversione di formati testuali e chat in sintassi MediaWiki e altri formati strutturati.

## 📑 Descrizione

Strumento professionale per:
- Conversione di conversazioni da social media (Messenger, Instagram, Telegram)
- Trasformazione di note Evernote in formato wiki
- Conversione Markdown → MediaWiki con 2 motori (Regex e Pandoc)
- Pulizia e normalizzazione testi con sistema regex avanzato
- Debug integrato delle trasformazioni

## ✨ Funzionalità Principali

- **Supporto Multi-piattaforma**:
  - Messenger • Instagram • Telegram • Facebook • Evernote
  - Modalità "Io/Lei ciclico" per dialoghi
  - Doppio motore Markdown-Wiki (Regex e Pandoc)

- **Editor Avanzato**:
  - Input/Output affiancati con syntax highlighting
  - Numerazione righe • Scroll sincronizzato • Ricerca integrata
  - Statistiche testo in tempo reale

- **Sistema di Conversione**:
  - 50+ regex preconfigurate
  - Estensibile con nuovi profili
  - Supporta template complessi e link semantic

- **Debugging**:
  - Visualizzazione regole applicate
  - Monitoraggio sostituzioni
  - Identificazione automatica speaker

## 🚀 Installazione

**Prerequisiti**:
- [Pandoc 2.19+](https://pandoc.org/installing.html) (per conversione avanzata)

**Package NuGet**:
```bash
Install-Package MahApps.Metro
Install-Package Telerik.UI.for.WPF
Install-Package AvalonEdit
Install-Package ConsoleTableExt
Install-Package Pandoc.Windows
```

## 🖥 Utilizzo Base

1. **Seleziona Tipo Conversione**  
   Dal menu a tendina scegli il formato sorgente (es. Messenger)

2. **Carica/Copia Testo**  
   Usa il pulsante "Carica Esempio" o incolla direttamente nell'editor

3. **Converti**  
   Clicca "Converti" per ottenere la versione processata

4. **Debug**  
   Attiva il pannello debug per vedere:
   - Regole applicate
   - Conteggio sostituzioni
   - Speaker identificati

## ⚙ Configurazione Avanzata

**Aggiungi Profili Personalizzati**:
1. Crea nuova classe in `Transformers`
2. Implementa `ITextTransformer`
3. Registra in `TransformerFactory.cs`

**Esempio Regex Personalizzata**:
```csharp
RegexReplacements.Add((@"^\d{2}/\d{2}/\d{4}", "DATA_RIMMOSSA"));
```

## 🔍 Sistema di Debug

Key features del pannello debug:
- **Regole Applicate**: Visualizza tutte le regex eseguite
- **Statistiche**: Numero sostituzioni e occorrenze uniche
- **Speaker Detection**: Elenco partecipanti identificati

## 📚 Dependencies

- [MahApps.Metro](https://mahapps.com/) - Modern UI Styling
- [Telerik UI](https://www.telerik.com/) - Advanced Grid Controls
- [AvalonEdit](https://github.com/icsharpcode/AvalonEdit) - Text Editor Component
- [Pandoc](https://pandoc.org/) - Universal Document Converter

## 🤝 Contributi

1. Fork del repository
2. Crea branch per la feature (`git checkout -b feature/AmazingFeature`)
3. Commit delle modifiche (`git commit -m 'Add some AmazingFeature'`)
4. Push nel branch (`git push origin feature/AmazingFeature`)
5. Apri una Pull Request

## 📄 Licenza

Distribuito con licenza MIT. Vedi `LICENSE` per dettagli.

---

**Ottimizzato per**: Windows 10/11 • .NET 6.0 • Visual Studio 2022  
**Version**: 1.2.0 • **Ultimo Aggiornamento**: 2023-10-15




ci sono diverse librerie per l'elaborazione del linguaggio naturale che supportano C# o .NET.

Una di queste è il Microsoft.Recognizers.Text, una libreria di riconoscimento del testo per C# e .NET che fornisce funzionalità per l'analisi del linguaggio naturale, come il riconoscimento dell'entità, l'estrazione di informazioni, la comprensione del testo e la generazione di testo.

Oltre a questo, c'è anche l'Accord.NET Framework, un'altra libreria open-source per C# e .NET che offre una vasta gamma di algoritmi di apprendimento automatico, tra cui il riconoscimento dei pattern nel testo.

Inoltre c'è anche l' NLTK.NET è una libreria di elaborazione del linguaggio naturale per C# e .NET basata sulla libreria Python NLTK, che offre una vasta gamma di funzionalità per l'elaborazione del linguaggio naturale, tra cui il riconoscimento dei pattern nel testo.

In generale ci sono molte librerie che supportano C# o .NET, e la scelta dipenderà dalle specifiche esigenze del tuo progetto e dalle funzionalità di cui hai bisogno.