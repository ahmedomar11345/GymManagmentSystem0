using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace GymManagmentPL.Hubs
{
    public class InventoryHub : Hub
    {
        // Hub methods if needed, but we mostly use it for server-to-client broadcasts
        public async Task JoinInventoryGroup()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "InventoryAdmins");
        }
    }
}
