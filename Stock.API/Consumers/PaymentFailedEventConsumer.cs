using MassTransit;
using MongoDB.Driver;
using Shared.Events;
using Shared.Message;
using Stock.API.Services;

namespace Stock.API.Consumers;

public class PaymentFailedEventConsumer : IConsumer<PaymentFailedEvent>
{
    IMongoCollection<Models.Entities.Stock> _stockCollection;


    public PaymentFailedEventConsumer(MongoDbService mongoDbService)
    {

        _stockCollection = mongoDbService.GetCollection<Models.Entities.Stock>();
    }

    public async Task Consume(ConsumeContext<PaymentFailedEvent> context)
    {
        foreach (OrderItemMessage orderItem in context.Message.OrderItems)
        {
            Models.Entities.Stock stock =
                await(await _stockCollection.FindAsync(s => s.ProductId == orderItem.ProductId))
                    .FirstOrDefaultAsync();
            stock.Count += orderItem.Count;
            await _stockCollection.FindOneAndReplaceAsync(s => s.ProductId == orderItem.ProductId, stock);
        }
    }
}