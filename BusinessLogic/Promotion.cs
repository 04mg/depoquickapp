namespace BusinessLogic.Test;

public class Promotion
{
    private string _label;
    private int _discount;
    private DateOnly _from;
    private DateOnly _to;

    public Promotion(string label, int discount, DateOnly from, DateOnly to)
    {
        _label = label;
        _discount = discount;
        _from = from;
        _to = to;
    }
}