namespace ShopAI.Models;

public sealed class Order
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public Product Product { get; set; } = null!;

    public int StoreId { get; set; }

    public Store Store { get; set; } = null!;

    public string CustomerName { get; set; } = string.Empty;

    public string CustomerPhone { get; set; } = string.Empty;

    public string DeliveryAddress { get; set; } = string.Empty;

    public DateTime RequestedDeliveryDate { get; set; }

    public string Notes { get; set; } = string.Empty;

    public OrderStatus Status { get; set; } = OrderStatus.PendingDispatch;

    public decimal TotalAmount { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
