using System.Threading.Tasks;

namespace GymManagmentBLL.Service.Interfaces
{
    public interface IBroadcastService
    {
        Task SendNotificationAsync(string title, string message, string type = "info");
        Task SendInventoryAlertAsync(int productId, string productName, int newStock);
    }
}
