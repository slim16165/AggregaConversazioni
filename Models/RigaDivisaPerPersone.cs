using System.Collections.Generic;

namespace AggregaConversazioni.Models;

public class RigaDivisaPerPersone
{
    public Dictionary<string, string> SpeakersMessages { get; set; } = new Dictionary<string, string>();

    // Puoi aggiungere altri metodi o proprietà se necessario.

    public string Speaker { get; set; }
    public List<string> Messages { get; set; } = new List<string>();
}
