using System;
using System.Collections.Generic;
using Zammad.Sdk.Models.Tickets;
using Zammad.Sdk.Models.Users;

namespace Zammad.Sdk.RealTime.Events;

/// <summary>
/// Provides data for ticket creation events.
/// </summary>
public sealed class TicketCreatedEventArgs : EventArgs
{
    public TicketCreatedEventArgs(Ticket ticket, TicketArticle firstArticle, User creator, DateTime timestamp)
    {
        Ticket = ticket;
        FirstArticle = firstArticle;
        Creator = creator;
        Timestamp = timestamp;
    }

    public Ticket Ticket { get; }
    public TicketArticle FirstArticle { get; }
    public User Creator { get; }
    public DateTime Timestamp { get; }
}

/// <summary>
/// Provides data for ticket update events.
/// </summary>
public sealed class TicketUpdatedEventArgs : EventArgs
{
    public TicketUpdatedEventArgs(Ticket ticket, Ticket previousState, IReadOnlyDictionary<string, object?> changes, User updatedBy)
    {
        Ticket = ticket;
        PreviousState = previousState;
        Changes = changes;
        UpdatedBy = updatedBy;
    }

    public Ticket Ticket { get; }
    public Ticket PreviousState { get; }
    public IReadOnlyDictionary<string, object?> Changes { get; }
    public User UpdatedBy { get; }
}

/// <summary>
/// Provides data for ticket close events.
/// </summary>
public sealed class TicketClosedEventArgs : EventArgs
{
    public TicketClosedEventArgs(Ticket ticket, User closedBy, DateTime closedAt)
    {
        Ticket = ticket;
        ClosedBy = closedBy;
        ClosedAt = closedAt;
    }

    public Ticket Ticket { get; }
    public User ClosedBy { get; }
    public DateTime ClosedAt { get; }
}
