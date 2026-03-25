using OrderFlow.Console.Models;
namespace OrderFlow.Console.Services;
public delegate bool ValidationRule(Order order, out string errorMessage);


public class OrderValidator
{
    private List<ValidationRule> _rules = new List<ValidationRule>();
    private List<Func<Order, bool>> _funcRules = new List<Func<Order, bool>>();
    private List<string> _funcRuleNames = new List<string>();
    public OrderValidator()
    {
        _rules.Add(MustHaveItems);
        _rules.Add(TotalMustNotExceedLimit);
        _rules.Add(QuantitiesMustBePositive);

        _funcRules.Add(o => o.OrderDate <= DateTime.Now);
        _funcRuleNames.Add("Data zamowienia nie moze byc z przyszlosci");

        _funcRules.Add(o => o.Status != OrderStatus.Cancelled);
        _funcRuleNames.Add("Zamowienie nie moze byc anulowane");
    }

    public static bool MustHaveItems(Order order, out string errorMessage)
    {
        errorMessage = "Zamowienie musi miec przynajmniej jedna pozycje";
        return order.Items.Count > 0;
    }

    public static bool TotalMustNotExceedLimit(Order order, out string errorMessage)
    {
        errorMessage = "Kwota zamowienia nie moze przekraczac 50000";
        return order.TotalAmount <= 50000m;
    }

    public static bool QuantitiesMustBePositive(Order order, out string errorMessage)
    {
        errorMessage = "Wszystkie ilosci musza byc wieksze od 0";
        return order.Items.All(i => i.Quantity > 0);
    }

    public List<string> ValidateAll(Order order)
    {
        var errors = new List<string>();

        foreach (var rule in _rules)
        {
            if (!rule(order, out string error))
                errors.Add(error);
        }

        for (int i = 0; i < _funcRules.Count; i++)
        {
            if (!_funcRules[i](order))
                errors.Add(_funcRuleNames[i]);
        }

        return errors;
    }
}