using AggregaConversazioni.Domain.Entities;

namespace AggregaConversazioni.Application.Transformers;

public static class TransformerFactory
{
    public static ITextTransformer Create(TransformationType type)
    {
        return type switch
        {
            TransformationType.Messenger => new MessengerTransformer(),
            TransformationType.Instagram => new InstagramTransformer(),
            TransformationType.Telegram => new TelegramTransformer(),
            TransformationType.IoLeiCiclico => new IoLeiCiclicoTransformer(),
            TransformationType.Evernote => new EvernoteTransformer(),
            TransformationType.MarkdownToWiki => new MarkdownToWikiConverter(),
            TransformationType.MarkdownToWikiPandoc => new MarkdownToWikiPandocConverter(),
            _ => throw new NotImplementedException($"Transformer for type {type} not implemented")
        };
    }
}
