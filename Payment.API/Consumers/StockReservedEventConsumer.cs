﻿using MassTransit;
using Shared.Events;

namespace Payment.API.Consumers;

public class StockReservedEventConsumer : IConsumer<StockReservedEvent>
{
    private readonly IPublishEndpoint _publishEndpoint;
    public StockReservedEventConsumer(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    public async Task Consume(ConsumeContext<StockReservedEvent> context)
    {
        if (true)
        {
            PaymentCompletedEvent paymentCompletedEvent = new()
            {
                OrderId = context.Message.OrderId,
            };
            _publishEndpoint.Publish(paymentCompletedEvent);
            Console.WriteLine("Ödeme Başarılı");
        }
        else
        {
            PaymentFailedEvent paymentFailedEvent = new()
            {
                OrderId = context.Message.OrderId,
                Messsage = "Bakiye yetersiz",
                OrderItems = context.Message.OrderItems,

            };
            _publishEndpoint.Publish(paymentFailedEvent);
            Console.WriteLine("Ödeme Başarısız");
        }

    }

}