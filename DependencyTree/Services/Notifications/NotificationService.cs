using System.ComponentModel.Composition;
using System.Windows;

namespace DependencyTree.Services.Notifications
{
    [Export(typeof(INotificationService))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class NotificationService : INotificationService
    {
        public void ShowMessage(string message, string caption)
        {
            MessageBox.Show(message, caption);
        }
    }
}
