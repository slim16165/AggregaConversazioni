using System.Collections.Generic;

namespace AggregaConversazioni;

public class RigaDivisaPerPersone
{
    public Dictionary<string, string> SpeakersMessages { get; set; } = new Dictionary<string, string>();

    // Puoi aggiungere altri metodi o proprietÓ se necessario.

    public string Speaker { get; set; }
    public List<string> Messages { get; set; } = new List<string>();
}
