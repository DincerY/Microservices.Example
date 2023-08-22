using MassTransit;
using Microsoft.EntityFrameworkCore;
using Order.API.Models;
using Order.API.Models.Enums;
using Shared.Events;

namespace Order.API.Consumers;

public class PaymentFailedEventConsumer : IConsumer<PaymentFailedEvent>
{
    private readonly OrderAPIDbContext _context;

    public PaymentFailedEventConsumer(OrderAPIDbContext context)
    {
        _context = context;
    }

    public async Task Consume(ConsumeContext<PaymentFailedEvent> context)
    {
        Models.Entities.Order? order =await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == context.Message.OrderId);
        order.OrderStatus = OrderStatus.Failed;
        await _context.SaveChangesAsync();
        Console.WriteLine(context.Message.Messsage);
    }
}