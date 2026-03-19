using OrderFlow.Console.Data;

var orders = SampleData.Orders;
foreach (var o in orders)
{
    Console.WriteLine($"Order {o.Id}, Status {o.Status}, Total {o.TotalAmount} zlotych");

}