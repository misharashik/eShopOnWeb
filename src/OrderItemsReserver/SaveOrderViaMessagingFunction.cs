using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace OrderItemsReserver;

public static class SaveOrderViaMessagingFunction
{
    public static async Task<BlobContainerClient> GetBlobClient()
    {
        var connectionString = Environment.GetEnvironmentVariable("EshopStorageConnectionString");
        var containerName = "orders";

        var blobServiceClient = new BlobServiceClient(connectionString);
        var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
        await containerClient.CreateIfNotExistsAsync();
        return containerClient;
    }

    [FunctionName("SaveOrderViaMessagingFunction")]
    public static async Task RunAsync([ServiceBusTrigger("orders", Connection = "EshopServiceBusConnectionString")] string myQueueItem, ILogger log)
    {
        //throw new System.Exception();

        log.LogInformation($"C# ServiceBus queue trigger function processed message: {myQueueItem}");

        var orderData = JsonConvert.DeserializeObject<Order>(myQueueItem);

        var blobClient = await GetBlobClient();
        await blobClient.UploadBlobAsync($"o-{orderData.Id}.json",
            new MemoryStream(Encoding.ASCII.GetBytes(myQueueItem)), CancellationToken.None);
    }
}
