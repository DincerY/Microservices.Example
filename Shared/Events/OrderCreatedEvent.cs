﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Events.Common;
using Shared.Message;

namespace Shared.Events;

public class OrderCreatedEvent : IEvent
{
    public Guid OrderId { get; set; }
    public Guid BuyerId { get; set; }
    public List<OrderItemMessage> OrderItems { get; set; }
}