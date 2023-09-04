using System;
using AggregaConversazioni.Parser;

namespace AggregaConversazioni;

public static class ParserFactory
{
    public static ParserBase Create(ParserType type)
    {
        return type switch
        {
            ParserType.Messenger => new ParserMessenger(),
            ParserType.Instagram => new ParserInstagram(),
            ParserType.Telegram => new ParserTelegram(),
            ParserType.IoLeiCiclico => new ParserIoLeiCiclico(),
            ParserType.Evernote => new ParserEvernote(), // Dovrai creare una classe appropriata per questo.
            //ParserType.Facebook => new ParserFacebook(), // Dovrai creare una classe appropriata per questo.
            _ => throw new NotImplementedException()
        };
    }
}