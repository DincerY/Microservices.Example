using MassTransit;
using MongoDB.Driver;
using Shared;
using Stock.API.Consumers;
using Stock.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMassTransit(configurator =>
{
    configurator.AddConsumer<OrderCreatedEventConsumer>();
    configurator.AddConsumer<PaymentFailedEventConsumer>();

    configurator.UsingRabbitMq((context, _configurator) =>
    {
        _configurator.Host(builder.Configuration["RabbitMQ"]);

        _configurator.ReceiveEndpoint(RabbitMQSettings.Stock_OrderCreatedEventQueue,e=>e.ConfigureConsumer<OrderCreatedEventConsumer>(context));

        _configurator.ReceiveEndpoint(RabbitMQSettings.Stock_PaymentFailedEventQueue, e => e.ConfigureConsumer<PaymentFailedEventConsumer>(context));
    });

    
});

builder.Services.AddSingleton<MongoDbService>();


#region MongoDb ye seed data ekleme

using IServiceScope scope = builder.Services.BuildServiceProvider().CreateScope();
MongoDbService mongoDbService = scope.ServiceProvider.GetService<MongoDbService>();
var collection = mongoDbService.GetCollection<Stock.API.Models.Entities.Stock>();
if (collection.FindSync(s=>true).Any())
{
    await collection.InsertOneAsync(new(){ProductId = Guid.NewGuid(),Count = 2000});
    await collection.InsertOneAsync(new(){ProductId = Guid.NewGuid(),Count = 1000});
    await collection.InsertOneAsync(new(){ProductId = Guid.NewGuid(),Count = 3000});
    await collection.InsertOneAsync(new(){ProductId = Guid.NewGuid(),Count = 5000});
    await collection.InsertOneAsync(new(){ProductId = Guid.NewGuid(),Count = 500});
}

#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
