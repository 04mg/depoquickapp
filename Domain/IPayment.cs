namespace Domain;

public interface IPayment
{
    double GetAmount();
    bool IsCaptured();
    void Capture();
}