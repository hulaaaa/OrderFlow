namespace OrderFlow.Console.Models;

public class Order
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public DateTime OrderDate { get; set; }
    public OrderStatus Status { get; set; }
    public List<OrderItem> Items { get; set; } = new List<OrderItem>();

    public decimal TotalAmount => Items.Sum(i => i.TotalPrice);
}