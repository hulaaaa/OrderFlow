using OrderFlow.Console.Data;
using OrderFlow.Console.Models;
using OrderFlow.Console.Services;

var products = SampleData.Products;
var customers = SampleData.Customers;
var orders = SampleData.Orders;

// 1
Console.WriteLine("=== Zamowienia ===");
foreach (var o in orders)
    Console.WriteLine($"  Order #{o.Id}, Status: {o.Status}, Total: {o.TotalAmount} zl");

// 2
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

// 3
Console.WriteLine("\n=== Predicate ===");
var processor = new OrderProcessor(orders);

Predicate<Order> isNew = o => o.Status == OrderStatus.New;
Predicate<Order> isExpensive = o => o.TotalAmount > 1000m;
Predicate<Order> hasMultipleItems = o => o.Items.Count > 1;

Console.WriteLine("Nowe:");
foreach (var o in processor.Filter(isNew))
    Console.WriteLine($"  Order #{o.Id}");

Console.WriteLine("Drogie (>1000):");
foreach (var o in processor.Filter(isExpensive))
    Console.WriteLine($"  Order #{o.Id}, Total: {o.TotalAmount}");

Console.WriteLine("Wiele pozycji:");
foreach (var o in processor.Filter(hasMultipleItems))
    Console.WriteLine($"  Order #{o.Id}, Pozycji: {o.Items.Count}");

Console.WriteLine("\n=== Action ===");
Action<Order> printOrder = o => Console.WriteLine($"  [{o.Status}] Order #{o.Id} = {o.TotalAmount} zl");
Action<Order> markProcessing = o =>
{
    if (o.Status == OrderStatus.New)
        o.Status = OrderStatus.Processing;
};

Console.WriteLine("Przed zmiana:");
processor.Execute(orders, printOrder);
processor.Execute(orders, markProcessing);
Console.WriteLine("Po zmianie (New -> Processing):");
processor.Execute(orders, printOrder);

Console.WriteLine("\n=== Func - projekcja ===");
var projected = processor.Project(o => new { o.Id, o.TotalAmount, ItemCount = o.Items.Count });
foreach (var p in projected)
    Console.WriteLine($"  Id={p.Id}, Total={p.TotalAmount}, Items={p.ItemCount}");

Console.WriteLine("\n=== Agregacja ===");
Func<IEnumerable<Order>, decimal> sum = list => list.Sum(o => o.TotalAmount);
Func<IEnumerable<Order>, decimal> avg = list => list.Any() ? list.Average(o => o.TotalAmount) : 0;
Func<IEnumerable<Order>, decimal> max = list => list.Any() ? list.Max(o => o.TotalAmount) : 0;
Console.WriteLine($"  Suma: {processor.Aggregate(sum)}");
Console.WriteLine($"  Srednia: {processor.Aggregate(avg)}");
Console.WriteLine($"  Max: {processor.Aggregate(max)}");

Console.WriteLine("\n=== Lancuch: filtruj -> sortuj -> top 3 ===");
processor.FilterSortTopAndPrint(
    o => o.Status != OrderStatus.Cancelled,
    o => o.TotalAmount,
    3,
    o => Console.WriteLine($"  Order #{o.Id} = {o.TotalAmount} zl")
);

// 4

// 1
Console.WriteLine("\n=== 1. Join - zamowienia per miasto (method) ===");
var ordersByCity = orders
    .Join(customers, o => o.CustomerId, c => c.Id, (o, c) => new { Order = o, Customer = c })
    .GroupBy(x => x.Customer.City)
    .Select(g => new { City = g.Key, Count = g.Count(), Total = g.Sum(x => x.Order.TotalAmount) });

foreach (var item in ordersByCity)
    Console.WriteLine($"  {item.City}: {item.Count} zamowien, {item.Total} zl");

// 1a
Console.WriteLine("\n=== 1b. Join - zamowienia per miasto (query) ===");
var ordersByCityQ =
    from o in orders
    join c in customers on o.CustomerId equals c.Id
    group new { o, c } by c.City into g
    select new { City = g.Key, Count = g.Count(), Total = g.Sum(x => x.o.TotalAmount) };

foreach (var item in ordersByCityQ)
    Console.WriteLine($"  {item.City}: {item.Count} zamowien, {item.Total} zl");

// 2
Console.WriteLine("\n=== 2. SelectMany - produkty z zamowien (method) ===");
var allProducts = orders
    .SelectMany(o => o.Items, (o, item) => new { OrderId = o.Id, item.Product.Name, item.Product.Category });

foreach (var p in allProducts)
    Console.WriteLine($"  Order #{p.OrderId}: {p.Name} ({p.Category})");

// 2a
Console.WriteLine("\n=== 2b. SelectMany (query) ===");
var allProductsQ =
    from o in orders
    from item in o.Items
    select new { OrderId = o.Id, item.Product.Name, item.Product.Category };

foreach (var p in allProductsQ)
    Console.WriteLine($"  Order #{p.OrderId}: {p.Name} ({p.Category})");

// 3
Console.WriteLine("\n=== 3. GroupBy - top klienci (method) ===");
var topCustomers = orders
    .GroupBy(o => o.CustomerId)
    .Select(g => new { CustomerId = g.Key, Total = g.Sum(o => o.TotalAmount) })
    .OrderByDescending(x => x.Total);

foreach (var tc in topCustomers)
    Console.WriteLine($"  Customer #{tc.CustomerId}: {tc.Total} zl");

// 3a
Console.WriteLine("\n=== 3b. GroupBy - srednia per kategoria (query) ===");
var avgPerCategory =
    from o in orders
    from item in o.Items
    group item by item.Product.Category into g
    select new { Category = g.Key, Avg = g.Average(i => i.TotalPrice) };

foreach (var a in avgPerCategory)
    Console.WriteLine($"  {a.Category}: {a.Avg} zl");

// 4
Console.WriteLine("\n=== 4. GroupJoin - klienci z zamowieniami (method) ===");
var customerOrders = customers
    .GroupJoin(orders, c => c.Id, o => o.CustomerId,
        (c, ords) => new { c.Name, Count = ords.Count(), Total = ords.Sum(o => o.TotalAmount) });

foreach (var co in customerOrders)
    Console.WriteLine($"  {co.Name}: {co.Count} zamowien, {co.Total} zl");

// 4b. query syntax
Console.WriteLine("\n=== 4b. GroupJoin (query) ===");
var customerOrdersQ =
    from c in customers
    join o in orders on c.Id equals o.CustomerId into customerOrd
    select new { c.Name, Count = customerOrd.Count(), Total = customerOrd.Sum(o => o.TotalAmount) };

foreach (var co in customerOrdersQ)
    Console.WriteLine($"  {co.Name}: {co.Count} zamowien, {co.Total} zl");

// 5
Console.WriteLine("\n=== 5. Mixed syntax - ulubiona kategoria klienta ===");
var report =
    from c in customers
    join o in orders on c.Id equals o.CustomerId
    from item in o.Items
    group item by c.Name into g
    select new
    {
        Customer = g.Key,
        FavCategory = g.GroupBy(i => i.Product.Category).OrderByDescending(cg => cg.Count()).First().Key,
        TotalSpent = g.Sum(i => i.TotalPrice)
    };

foreach (var r in report)
    Console.WriteLine($"  {r.Customer}: kategoria={r.FavCategory}, wydal={r.TotalSpent} zl");

// 6
Console.WriteLine("\n=== 6. Zamowienia posortowane po dacie (method) ===");
var sorted = orders
    .Where(o => o.Status != OrderStatus.Cancelled)
    .OrderBy(o => o.OrderDate)
    .Select(o => new { o.Id, o.OrderDate, o.Status, o.TotalAmount });

foreach (var s in sorted)
    Console.WriteLine($"  Order #{s.Id}: {s.OrderDate:d}, {s.Status}, {s.TotalAmount} zl");
