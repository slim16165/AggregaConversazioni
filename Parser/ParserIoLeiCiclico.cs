using System;
using System.Collections.Generic;
using AggregaConversazioni.Parser;

namespace AggregaConversazioni;

public class ParserIoLeiCiclico : ParserBase
{
    public override (string text, IEnumerable<RigaDivisaPerPersone> righeDivisePerPersone, List<string> speakers) Parse(string text)
    {
        return ParserStatic.ParseIo_LeiCiclico2(text);
    }
}