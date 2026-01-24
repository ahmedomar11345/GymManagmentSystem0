using GymManagmentBLL.Service.Interfaces;
using GymManagmentBLL.ViewModels.AnalyticsViewModel;
using GymManagmentDAL.Entities;
using GymManagmentDAL.Reposotories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagmentBLL.Service.Classes
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AnalyticsService(IUnitOfWork unitOfWork)
        { 
            _unitOfWork = unitOfWork;
        }
        public AnalyticsViewModel GetAnalyticsData()
        {
            var Sessions = _unitOfWork.GetRepository<Session>().GetAll(); 
            return new AnalyticsViewModel
            {
                ActiveMembers = _unitOfWork.GetRepository<MemberShip>().GetAll(x=> x.Status == "Active").Count(),
                TotalMembers = _unitOfWork.GetRepository<Member>().GetAll().Count(),
                TotalTrainers = _unitOfWork.GetRepository<Trainer>().GetAll().Count(),
                UpcomingSessions = Sessions.Where(x => x.StartDate > DateTime.Now).Count(),
                OngoingSessions = Sessions.Where(x => x.StartDate <= DateTime.Now && x.EndDate >= DateTime.Now).Count(),
                CompletedSessions = Sessions.Where(x => x.EndDate < DateTime.Now).Count()
            };
        }
    }
}
