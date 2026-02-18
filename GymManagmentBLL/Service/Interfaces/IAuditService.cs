using GymManagmentDAL.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GymManagmentBLL.Service.Interfaces
{
    public interface IAuditService
    {
        Task LogActionAsync(string userName, string action, string entityName, string entityId, string? oldValues = null, string? newValues = null);
        Task<IEnumerable<AuditLog>> GetRecentLogsAsync(int count = 50);
    }
}
