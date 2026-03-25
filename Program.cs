using OrderFlow.Console.Data;
using OrderFlow.Console.Services;


var orders = SampleData.Orders;
foreach (var o in orders)
{
    Console.WriteLine($"Order {o.Id}, Status {o.Status}, Total {o.TotalAmount} zlotych");

}

Console.WriteLine("\n=== Walidacja ===");
var validator = new OrderValidator();

Console.WriteLine("Order #1:");
var errors1 = validator.ValidateAll(orders[0]);
if (errors1.Count == 0)
    Console.WriteLine("  OK");

Console.WriteLine("Order #6:");
var errors6 = validator.ValidateAll(orders[5]);
foreach (var err in errors6)
    Console.WriteLine($"  BLAD: {err}");