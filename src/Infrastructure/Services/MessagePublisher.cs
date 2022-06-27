using System.Linq;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Microsoft.eShopWeb.Infrastructure.Services;

public class MessagePublisher : IMessagePublisher
{
    static ServiceBusClient client;
    static ServiceBusSender sender;
    readonly IConfiguration _configuration;

    public MessagePublisher(IConfiguration configuration)
    {
        _configuration = configuration;

        var connectionString = _configuration["ServiceBus:ConnectionString"];
        var queueName = _configuration["ServiceBus:QueueName"];

        client = new ServiceBusClient(connectionString);
        sender = client.CreateSender(queueName);
    }

    public async Task Publish(Order order)
    {
        var body = new
        {
            order.Id,
            order.OrderDate,
            Items = order.OrderItems.Select(item => new
            {
                item.Id,
                item.ItemOrdered.CatalogItemId,
                item.ItemOrdered.ProductName,
                item.Units
            })
            .ToList()
        };

        await sender.SendMessagesAsync(new ServiceBusMessage[] { new ServiceBusMessage(body.ToJson()) });
    }
}
