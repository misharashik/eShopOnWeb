using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OrderItemsReserver;

namespace SaveOrderToCosmosFunction;

public static class SaveOrderToCosmosFunction
{

    [FunctionName("SaveOrderToCosmos")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
        ILogger log)
    {
        log.LogInformation("C# HTTP trigger function processed a request.");

        var cosmosDbUri = Environment.GetEnvironmentVariable("EshopCosmosDbUri");
        var cosmosDbKey = Environment.GetEnvironmentVariable("EshopCosmosDbKey");

        var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var orderData = JsonConvert.DeserializeObject<OrderCosmos>(requestBody);

        using (var client = new CosmosClient(cosmosDbUri, cosmosDbKey))
        {
            var databaseResponse = await client.CreateDatabaseIfNotExistsAsync("Orders");
            var targetDatabase = databaseResponse.Database;
            log.LogInformation($"Database Id:\t{targetDatabase.Id}");

            var indexingPolicy = new IndexingPolicy
            {
                IndexingMode = IndexingMode.Consistent,
                Automatic = true,
                IncludedPaths =
                    {
                        new IncludedPath
                        {
                            Path = "/*"
                        }
                    }
            };
            var containerProperties = new ContainerProperties("Orders", "/id")
            {
                IndexingPolicy = indexingPolicy
            };
            var containerResponse = await targetDatabase.CreateContainerIfNotExistsAsync(containerProperties, 1000);
            var customContainer = containerResponse.Container;
            log.LogInformation($"Custom Container Id:\t{customContainer.Id}");

            await customContainer.CreateItemAsync<OrderCosmos>(orderData);
        }

        return new OkObjectResult("Order has been sent to Cosmos");
    }
}
