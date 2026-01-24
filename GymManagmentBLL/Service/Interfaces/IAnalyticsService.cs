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
        AnalyticsViewModel GetAnalyticsData();
    }
}
