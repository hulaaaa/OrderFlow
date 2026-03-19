using OrderFlow.Console.Models;

namespace OrderFlow.Console.Data;

public static class SampleData
{
    public static List<Product> Products = new List<Product>
    {
        new Product { Id = 1, Name = "Towar1", Category = "Elektronika", Price = 3500m },
        new Product { Id = 2, Name = "Towar2", Category = "Elektronika", Price = 80m },
        new Product { Id = 3, Name = "Towar3", Category = "Elektronika", Price = 1200m },
        new Product { Id = 4, Name = "Towar4", Category = "Meble", Price = 900m },
        new Product { Id = 5, Name = "Towar5", Category = "Meble", Price = 15m },
        new Product { Id = 6, Name = "Towar6", Category = "Meble", Price = 5m }
    };

    public static List<Customer> Customers = new List<Customer>
    {
        new Customer { Id = 1, Name = "Dmytro", City = "Krakow", IsVip = true },
        new Customer { Id = 2, Name = "Alina", City = "Krakow", IsVip = true },
        new Customer { Id = 3, Name = "Maria", City = "Warszawa", IsVip = false },
        new Customer { Id = 4, Name = "Wasyl", City = "Lwow", IsVip = false }
    };

    public static List<Order> Orders = new List<Order>
    {
        new Order
        {
            Id = 1, CustomerId = 1, OrderDate = new DateTime(2025, 3, 10), Status = OrderStatus.Completed,
            Items = new List<OrderItem>
            {
                new OrderItem { Product = Products[0], Quantity = 1 },
                new OrderItem { Product = Products[1], Quantity = 2 }
            }
        },
        new Order
        {
            Id = 2, CustomerId = 2, OrderDate = new DateTime(2025, 3, 12), Status = OrderStatus.New,
            Items = new List<OrderItem>
            {
                new OrderItem { Product = Products[2], Quantity = 1 },
                new OrderItem { Product = Products[3], Quantity = 1 }
            }
        },
        new Order
        {
            Id = 3, CustomerId = 1, OrderDate = new DateTime(2025, 3, 15), Status = OrderStatus.Processing,
            Items = new List<OrderItem>
            {
                new OrderItem { Product = Products[4], Quantity = 10 },
                new OrderItem { Product = Products[5], Quantity = 20 }
            }
        },
        new Order
        {
            Id = 4, CustomerId = 3, OrderDate = new DateTime(2025, 3, 18), Status = OrderStatus.Cancelled,
            Items = new List<OrderItem>
            {
                new OrderItem { Product = Products[0], Quantity = 2 }
            }
        },
        new Order
        {
            Id = 5, CustomerId = 4, OrderDate = new DateTime(2025, 3, 20), Status = OrderStatus.New,
            Items = new List<OrderItem>
            {
                new OrderItem { Product = Products[1], Quantity = 5 },
                new OrderItem { Product = Products[3], Quantity = 1 }
            }
        },
        new Order
        {
            Id = 6, CustomerId = 2, OrderDate = DateTime.Now.AddDays(5), Status = OrderStatus.New,
            Items = new List<OrderItem>()
        }
    };
}