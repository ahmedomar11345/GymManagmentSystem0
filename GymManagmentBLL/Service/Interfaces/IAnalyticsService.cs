using GymManagmentBLL.ViewModels.AnalyticsViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagmentBLL.Service.Interfaces
{
    public interface IAnalyticsService 
    {
        Task<AnalyticsViewModel> GetAnalyticsDataAsync(int months = 6, DateTime? startDate = null, DateTime? endDate = null);
    }
}
