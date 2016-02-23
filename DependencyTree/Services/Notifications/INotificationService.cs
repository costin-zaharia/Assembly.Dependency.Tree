namespace DependencyTree.Services.Notifications
{
    public interface INotificationService
    {
        void ShowMessage(string message, string caption);
    }
}