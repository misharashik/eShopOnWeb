using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Microsoft.eShopWeb.Infrastructure.Services;

// This class is used by the application to send email for account confirmation and password reset.
// For more details see https://go.microsoft.com/fwlink/?LinkID=532713
public class FuncTrigger : IFuncTrigger
{
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;

    public FuncTrigger(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    public async Task TriggerOrderToCosmosFunc(Order order)
    {
        var url = _configuration["SaveOrderToCosmosTriggerUrl"];

        var body = JsonExtensions.ToJson(new
        {
            Id = order.Id.ToString(),
            BuyerId = order.BuyerId.ToString(),
            OrderDate = order.OrderDate,
            Total = order.Total(),
            ShipToAddress = order.ShipToAddress.Formated,
            Items = order.OrderItems.Select(item => new
            {
                item.Id,
                item.ItemOrdered.CatalogItemId,
                item.ItemOrdered.ProductName,
                item.Units
            }).ToList(),
        });

        var httpRequestMessage = new HttpRequestMessage(
            HttpMethod.Post,
            url)
        {
            //Headers,
            Content = new StringContent(body),
        };

        var client = _httpClientFactory.CreateClient();
        await client.SendAsync(httpRequestMessage, CancellationToken.None);

    }
}
