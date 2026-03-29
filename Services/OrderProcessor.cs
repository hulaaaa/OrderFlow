using OrderFlow.Console.Models;

namespace OrderFlow.Console.Services;

public class OrderProcessor
{
    private List<Order> _orders;

    public OrderProcessor(List<Order> orders)
    {
        _orders = orders;
    }

    public List<Order> Filter(Predicate<Order> predicate)
    {
        return _orders.FindAll(predicate);
    }

    public void Execute(List<Order> orders, Action<Order> action)
    {
        foreach (var order in orders)
            action(order);
    }

    public List<T> Project<T>(Func<Order, T> selector)
    {
        var result = new List<T>();
        foreach (var order in _orders)
            result.Add(selector(order));
        return result;
    }

    public decimal Aggregate(Func<IEnumerable<Order>, decimal> aggregator)
    {
        return aggregator(_orders);
    }

    public void FilterSortTopAndPrint(Predicate<Order> filter, Func<Order, decimal> sortKey, int topN, Action<Order> print)
    {
        var filtered = _orders.FindAll(filter);
        var sorted = filtered.OrderByDescending(sortKey).Take(topN).ToList();
        foreach (var order in sorted)
            print(order);
    }
}