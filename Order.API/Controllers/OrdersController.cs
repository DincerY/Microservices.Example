using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Order.API.Models;
using Order.API.Models.Entities;
using Order.API.Models.Enums;
using Order.API.ViewModels;
using Shared.Events;
using Shared.Message;

namespace Order.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrdersController : ControllerBase
{
    private readonly OrderAPIDbContext _context;
    private readonly IPublishEndpoint _publishEndpoint;
    public OrdersController(OrderAPIDbContext context, IPublishEndpoint publishEndpoint)
    {
        _context = context;
        _publishEndpoint = publishEndpoint;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderVM createOrder)
    {
        Models.Entities.Order order = new()
        {
            OrderId = Guid.NewGuid(),
            CreatedDate = DateTime.UtcNow,
            BuyerId = createOrder.BuyerId,
            OrderStatus = OrderStatus.Suspend
        };
        order.OrderItems = createOrder.OrderItems.Select(oi => new OrderItem
        {
            ProductId = oi.ProductId,
            Price = oi.Price,
            Count = oi.Count,

        }).ToList();
        order.TotalPrice = createOrder.OrderItems.Sum(oi => (oi.Price * oi.Count));

        await _context.Orders.AddAsync(order);
        await _context.SaveChangesAsync();

        OrderCreatedEvent orderCreatedEvent = new()
        {
            BuyerId = order.BuyerId,
            OrderId = order.OrderId,
            OrderItems = order.OrderItems.Select(oi => new OrderItemMessage()
            {
                ProductId = oi.ProductId,
                Count = oi.Count
            }).ToList()
        };

        await _publishEndpoint.Publish(orderCreatedEvent);

        return Created("", null);
    }
}