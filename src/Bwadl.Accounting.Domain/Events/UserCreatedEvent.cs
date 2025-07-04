using Bwadl.Accounting.Domain.Entities;

namespace Bwadl.Accounting.Domain.Events;

public record UserCreatedEvent(User User, DateTime CreatedAt) : IDomainEvent;

public interface IDomainEvent
{
    DateTime CreatedAt { get; }
}
