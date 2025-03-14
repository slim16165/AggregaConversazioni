﻿using System;
using AggregaConversazioni.Parsers;

namespace AggregaConversazioni.Transformers;

public static class TransformerFactory
{
    public static ITextTransformer Create(TransformerType type)
    {
        return type switch
        {
            TransformerType.Messenger => new ParserMessenger(),
            TransformerType.Instagram => new ParserInstagram(),
            TransformerType.Telegram => new ParserTelegram(),
            TransformerType.IoLeiCiclico => new ParserIoLeiCiclico(),
            TransformerType.Evernote => new ParserEvernote(),
            TransformerType.MarkdownToWiki => new MarkdownToWikiConverter(),
            TransformerType.MarkdownToWikiPandoc => new MarkdownToWikiConverterPandoc(),
            _ => throw new NotImplementedException()
        };
    }
}