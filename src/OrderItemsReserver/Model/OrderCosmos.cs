using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace OrderItemsReserver;

internal class OrderCosmos
{
    [JsonProperty(PropertyName = "id")]
    public string Id { get; set; }
    public string BuyerId { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal Total { get; set; }
    public string ShipToAddress { get; set; }

    public List<CosmosOrderItem> Items { get; set; }
}
internal class CosmosOrderItem
{
    public int Id { get; set; }
    public int CatalogId;
    public string ProductName { get; set; }
    public int Units { get; set; }
}
