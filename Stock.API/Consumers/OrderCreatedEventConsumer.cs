using MassTransit;
using MongoDB.Driver;
using Shared;
using Shared.Events;
using Shared.Message;
using Stock.API.Services;

namespace Stock.API.Consumers;

public class OrderCreatedEventConsumer : IConsumer<OrderCreatedEvent>
{
    IMongoCollection<Models.Entities.Stock> _stockCollection;
    private readonly ISendEndpointProvider _sendEndpointProvider;
    private readonly IPublishEndpoint _publishEndpoint;

    public OrderCreatedEventConsumer(MongoDbService mongoDbService, ISendEndpointProvider sendEndpointProvider, IPublishEndpoint publishEndpoint)
    {
        _sendEndpointProvider = sendEndpointProvider;
        _publishEndpoint = publishEndpoint;
        _stockCollection = mongoDbService.GetCollection<Models.Entities.Stock>();
    }

    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        List<bool> stockResult = new();
        foreach (OrderItemMessage orderItem in context.Message.OrderItems)
        {
            stockResult.Add((await _stockCollection.FindAsync(s=>s.ProductId == orderItem.ProductId &&
                                                                s.Count >= orderItem.Count)).Any());
        }

        if (stockResult.TrueForAll(sr=>sr.Equals(true)))
        {
            foreach (OrderItemMessage orderItem in context.Message.OrderItems)
            {
                Models.Entities.Stock stock =
                    await (await _stockCollection.FindAsync(s => s.ProductId == orderItem.ProductId))
                        .FirstOrDefaultAsync();
                stock.Count -= orderItem.Count;
                await _stockCollection.FindOneAndReplaceAsync(s => s.ProductId == orderItem.ProductId, stock);
            }

            StockReservedEvent stockReservedEvent = new()
            {
                BuyerId = context.Message.BuyerId,
                OrderId = context.Message.OrderId,
                TotalPrice = context.Message.TotalPrice,
                OrderItems = context.Message.OrderItems,
            };

            ISendEndpoint sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{RabbitMQSettings.Payment_StockReservedEventQueue}"));
            await sendEndpoint.Send(stockReservedEvent);
        }
        else
        {
            StockNotReservedEvent stockNotReservedEvent = new()
            {
                BuyerId = context.Message.BuyerId,
                OrderId = context.Message.OrderId,
                Message = "..."
            };
            await _publishEndpoint.Publish(stockNotReservedEvent);


        }
    }
}