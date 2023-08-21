﻿using MongoDB.Driver;

namespace Stock.API.Services;

public class MongoDbService
{
    private readonly IMongoDatabase _database;

    public MongoDbService(IConfiguration configuration)
    {
        MongoClient client = new(configuration.GetConnectionString("MongoDB"));
        _database = client.GetDatabase("StockAPIDB");
    }
}