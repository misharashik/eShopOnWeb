using System;
using System.Collections.Generic;

namespace OrderItemsReserver;

internal class Order
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public List<OrderItem> Items { get; set; }
}
internal class OrderItem
{
    public int Id { get; set; }
    public int CatalogId;
    public string ProductName { get; set; }
    public int Units { get; set; }
}
