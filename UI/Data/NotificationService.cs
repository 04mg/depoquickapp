namespace UI.Data;

public class NotificationService
{
    private const string ErrorType = "error";
    private const string MessageType = "primary";
    public event Action<string, string>? OnNotify;

    public void ShowError(string message) => OnNotify?.Invoke(message, ErrorType);
    public void ShowMessage(string message) => OnNotify?.Invoke(message, MessageType);
}