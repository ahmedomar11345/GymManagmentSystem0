using GymManagmentBLL.Service.Interfaces;
using GymManagmentDAL.Entities;
using GymManagmentDAL.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GymManagmentBLL.Service.Classes
{
    public class AuditService : IAuditService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AuditService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task LogActionAsync(string userName, string action, string entityName, string entityId, string? oldValues = null, string? newValues = null)
        {
            var log = new AuditLog
            {
                UserName = userName,
                Action = action,
                EntityName = entityName,
                EntityId = entityId,
                OldValues = oldValues,
                NewValues = newValues,
                Timestamp = System.DateTime.Now
            };

            await _unitOfWork.GetRepository<AuditLog>().AddAsync(log);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<AuditLog>> GetRecentLogsAsync(int count = 50)
        {
            return (await _unitOfWork.GetRepository<AuditLog>().GetAllAsync())
                   .OrderByDescending(l => l.Timestamp)
                   .Take(count);
        }
    }
}
