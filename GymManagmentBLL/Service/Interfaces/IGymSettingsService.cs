using GymManagmentDAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagmentBLL.Service.Interfaces
{
    public interface IGymSettingsService
    {
        Task<GymSettings> GetSettingsAsync();
        Task UpdateSettingsAsync(GymSettings settings);
    }
}
