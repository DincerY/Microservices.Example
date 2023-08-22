using Shared.Events.Common;
using Shared.Message;

namespace Shared.Events;

public class PaymentFailedEvent : IEvent
{
    public Guid OrderId { get; set; }
    public string Messsage { get; set; }
    public List<OrderItemMessage> OrderItems { get; set; }
}