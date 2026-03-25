namespace OrderFlow.Console.Models;

public class OrderItem
{
    public Product Product { get; set; } = new Product();
    public int Quantity { get; set; }

    public decimal TotalPrice => Product.Price * Quantity;
}