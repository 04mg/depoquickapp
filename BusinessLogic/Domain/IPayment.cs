namespace BusinessLogic.Domain;

public interface IPayment
{
    bool IsCaptured();
    void Capture();
}