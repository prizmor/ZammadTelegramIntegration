using System;
using Zammad.Sdk.Models.Tickets;

namespace Zammad.Sdk.RealTime.Events;

/// <summary>
/// Provides data for article creation events.
/// </summary>
public sealed class ArticleCreatedEventArgs : EventArgs
{
    public ArticleCreatedEventArgs(TicketArticle article, Ticket ticket, bool isSplit, long? splitFromTicketId, long? splitFromArticleId)
    {
        Article = article;
        Ticket = ticket;
        IsSplit = isSplit;
        SplitFromTicketId = splitFromTicketId;
        SplitFromArticleId = splitFromArticleId;
    }

    public TicketArticle Article { get; }
    public Ticket Ticket { get; }
    public bool IsSplit { get; }
    public long? SplitFromTicketId { get; }
    public long? SplitFromArticleId { get; }
}
