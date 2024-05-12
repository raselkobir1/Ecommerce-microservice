using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;


public static class Extentions
{
    public static void AddMongoDb(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MongoOptions>(configuration.GetSection("mongo"));
        services.AddSingleton(c =>
        {
            var options = c.GetService<IOptions<MongoOptions>>();

            return new MongoClient(options.Value.ConnectionString);
        });
        services.AddSingleton(c =>
        {
            var options = c.GetService<IOptions<MongoOptions>>();
            var client = c.GetService<MongoClient>();

            return client.GetDatabase(options.Value.Database);
        });
    }
}
