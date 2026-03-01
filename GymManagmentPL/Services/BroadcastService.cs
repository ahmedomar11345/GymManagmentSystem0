using GymManagmentBLL.Service.Interfaces;
using GymManagmentPL.Hubs;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace GymManagmentPL.Services
{
    public class BroadcastService : IBroadcastService
    {
        private readonly IHubContext<InventoryHub> _hubContext;

        public BroadcastService(IHubContext<InventoryHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendNotificationAsync(string title, string message, string type = "info")
        {
            await _hubContext.Clients.All.SendAsync("ReceiveNotification", new { title, message, type });
        }

        public async Task SendInventoryAlertAsync(int productId, string productName, int newStock)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveInventoryAlert", new { productId, productName, newStock });
        }
    }
}
