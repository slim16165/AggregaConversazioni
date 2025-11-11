namespace AggregaConversazioni.Domain.Interfaces;

/// <summary>
/// Interfaccia base per tutti i transformer di testo
/// </summary>
public interface ITextTransformer
{
    /// <summary>
    /// Trasforma il testo di input nel formato di output desiderato
    /// </summary>
    /// <param name="input">Testo di input da trasformare</param>
    /// <returns>Testo trasformato</returns>
    string Transform(string input);
}
