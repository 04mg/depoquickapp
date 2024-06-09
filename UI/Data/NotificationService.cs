namespace UI.Data;

public class NotificationService
{
    private const string ErrorType = "error";
    private const string MessageType = "primary";
    private const int Delay = 3000;
    private int _currentMessage;
    public event Action<string, string>? OnNotify;

    private void ClearAfterDelay()
    {
        var message = ++_currentMessage;
        Task.Delay(Delay).ContinueWith(_ =>
        {
            if (_currentMessage == message) Clear();
        });
    }

    public void ShowError(string message)
    {
        OnNotify?.Invoke(message, ErrorType);
        ClearAfterDelay();
    }

    public void ShowMessage(string message)
    {
        OnNotify?.Invoke(message, MessageType);
        ClearAfterDelay();
    }
    
    public void ShowCriticalError(string message)
    {
        OnNotify?.Invoke(message, ErrorType);
    }

    public void Clear()
    {
        OnNotify?.Invoke(string.Empty, string.Empty);
    }
}