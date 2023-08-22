using MassTransit;
using Microsoft.EntityFrameworkCore;
using Order.API.Models;
using Order.API.Models.Enums;
using Shared.Events;

namespace Order.API.Consumers;

public class StockNotReservedEventConsumer : IConsumer<StockNotReservedEvent>
{
    private readonly OrderAPIDbContext _context;

    public StockNotReservedEventConsumer(OrderAPIDbContext context)
    {
        _context = context;
    }

    public async Task Consume(ConsumeContext<StockNotReservedEvent> context)
    {
        Models.Entities.Order? order =
            await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == context.Message.OrderId);
        order.OrderStatus = OrderStatus.Failed;
        await _context.SaveChangesAsync();
    }
}