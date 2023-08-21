using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Order.API.Models;
using Order.API.Models.Entities;
using Order.API.Models.Enums;
using Order.API.ViewModels;

namespace Order.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrdersController : ControllerBase
{
    private readonly OrderAPIDbContext _context;
    public OrdersController(OrderAPIDbContext context)
    {
        _context = context;
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

        return Created("", null);
    }
}