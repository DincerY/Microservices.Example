using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using Shared.Events.Common;

namespace Shared.Events;

public class PaymentCompletedEvent : IEvent
{
    public Guid OrderId { get; set; }
}