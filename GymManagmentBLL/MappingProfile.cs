using AutoMapper;
using GymManagmentBLL.ViewModels.MemberViewModel;
using GymManagmentBLL.ViewModels.MemberShipViewModel;
using GymManagmentBLL.ViewModels.MemberSessionViewModel;
using GymManagmentBLL.ViewModels.PlanViewModel;
using GymManagmentBLL.ViewModels.SessionViewModel;
using GymManagmentBLL.ViewModels.TrainerViewModel;
using GymManagmentDAL.Entities;
using GymManagmentDAL.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagmentBLL
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Create your mappings here
            MapMember();
            MapTrainer();
            MapSession();
            MapPlan();
            MapMemberShip();
            MapMemberSession();
        }
        private void MapTrainer()
        {
            CreateMap<Trainer, TrainerViewModel>()
                .ForMember(dest => dest.Specialties, opt => opt.MapFrom(src => src.Specialty != null ? src.Specialty.Name : null))
                .ForMember(dest => dest.ShiftStart, opt => opt.MapFrom(src => src.ShiftStart.ToString(@"hh\:mm")))
                .ForMember(dest => dest.ShiftEnd, opt => opt.MapFrom(src => src.ShiftEnd.ToString(@"hh\:mm")))
                .ForMember(dest => dest.AccessKey, opt => opt.MapFrom(src => src.AccessKey))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => $"{src.Address.BuildingNumber}, {src.Address.Street}, {src.Address.City}, {src.Address.Country}"));

            CreateMap<CreateTrainerViewModel, Trainer>()
                .ForMember(dest => dest.ShiftStart, opt => opt.MapFrom(src => src.ShiftStart))
                .ForMember(dest => dest.ShiftEnd, opt => opt.MapFrom(src => src.ShiftEnd))
                .ForMember(dest => dest.SpecialtyId, opt => opt.MapFrom(src => src.SpecialtyId))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => new Address
                {
                    BuildingNumber = src.BuildingNumber,
                    City = src.City,
                    Street = src.Street,
                    Country = src.Country
                }));

            CreateMap<Trainer, TrainerToUpdateViewModel>()
                .ForMember(dest => dest.Street, opt => opt.MapFrom(src => src.Address.Street))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.Address.City))
                .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Address.Country))
                .ForMember(dest => dest.BuildingNumber, opt => opt.MapFrom(src => src.Address.BuildingNumber));

            CreateMap<TrainerToUpdateViewModel, Trainer>()
                .AfterMap((src, dest) =>
                {
                    if (dest.Address == null) dest.Address = new Address();
                    dest.Name = src.Name;
                    dest.DateOfBirth = src.DateOfBirth;
                    dest.Gender = src.Gender;
                    dest.Address.BuildingNumber = src.BuildingNumber;
                    dest.Address.City = src.City;
                    dest.Address.Street = src.Street;
                    dest.Address.Country = src.Country;
                    dest.ShiftStart = src.ShiftStart;
                    dest.ShiftEnd = src.ShiftEnd;
                    dest.SpecialtyId = src.SpecialtyId;
                    dest.UpdatedAt = DateTime.Now;
                });
        }
        private void MapSession()
        {
            CreateMap<CreateSessionViewModel, Session>();
            CreateMap<Session, SessionViewModel>()
                        .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.SessionCategory.CategoryName))
                        .ForMember(dest => dest.TrainerName, opt => opt.MapFrom(src => src.SessionTrainer.Name))
                        .ForMember(dest => dest.AvailableSlot, opt => opt.Ignore()); // Will Be Calculated After Map
            CreateMap<UpdateSessionViewModel, Session>().ReverseMap();

            CreateMap<Trainer, TrainerSelectViewModel>();
            CreateMap<Category, CategorySelectViewModel>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.CategoryName));



        }
        private void MapMember()
        {
            CreateMap<CreateMemberViewModel, Member>()
                  .ForMember(dest => dest.Address, opt => opt.MapFrom(src => new Address
                  {
                      BuildingNumber = src.BuildingNumber,
                      City = src.City,
                      Street = src.Street,
                      Country = src.Country
                  })).ForMember(dest => dest.HealthRecord, opt => opt.MapFrom(src => src.HealthRecord));


            CreateMap<HealthRecord, HealthRecordViewModel>().ReverseMap();

            CreateMap<Member, MemberViewModel>()
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender.ToString()))
                .ForMember(dest => dest.JoinDate, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.DateOfBirth.ToShortDateString()))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => $"{src.Address.BuildingNumber} - {src.Address.Street} - {src.Address.City} - {src.Address.Country}"))
                .ForMember(dest => dest.HealthRecord, opt => opt.MapFrom(src => src.HealthRecord))
                .ForMember(dest => dest.SessionsBooked, opt => opt.MapFrom(src => src.MemberSessions != null ? src.MemberSessions.Count : 0))
                .ForMember(dest => dest.PlaneName, opt => opt.MapFrom(src => src.Memberships != null && src.Memberships.Any() ? src.Memberships.OrderByDescending(m => m.CreatedAt).First().Plan.Name : null))
                .ForMember(dest => dest.MembershipStartDate, opt => opt.MapFrom(src => src.Memberships != null && src.Memberships.Any() ? src.Memberships.OrderByDescending(m => m.CreatedAt).First().CreatedAt.ToShortDateString() : null))
                .ForMember(dest => dest.MembershipEndDate, opt => opt.MapFrom(src => src.Memberships != null && src.Memberships.Any() ? src.Memberships.OrderByDescending(m => m.CreatedAt).First().EndDate.ToShortDateString() : null))
                .ForMember(dest => dest.AccessKey, opt => opt.MapFrom(src => src.AccessKey));

            CreateMap<Member, MemberToUpdateViewModel>()
                .ForMember(dest => dest.BuildingNumber, opt => opt.MapFrom(src => src.Address.BuildingNumber))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.Address.City))
                .ForMember(dest => dest.Street, opt => opt.MapFrom(src => src.Address.Street))
                .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Address.Country))
                .ForMember(dest => dest.HealthRecord, opt => opt.MapFrom(src => src.HealthRecord));

            CreateMap<MemberToUpdateViewModel, Member>()
                .ForMember(dest => dest.Photo, opt => opt.Condition(src => !string.IsNullOrEmpty(src.Photo)))
                .ForMember(dest => dest.HealthRecord, opt => opt.MapFrom(src => src.HealthRecord))
                .AfterMap((src, dest) =>
                {
                    if (dest.Address == null) dest.Address = new Address();
                    dest.Name = src.Name;
                    dest.Address.BuildingNumber = src.BuildingNumber;
                    dest.Address.City = src.City;
                    dest.Address.Street = src.Street;
                    dest.Address.Country = src.Country;
                    if (!string.IsNullOrEmpty(src.Photo))
                    {
                        dest.Photo = src.Photo;
                    }
                    dest.UpdatedAt = DateTime.Now;
                });

            CreateMap<HealthProgress, HealthProgressViewModel>().ReverseMap();
        }
        private void MapPlan()
        {
            CreateMap<CreatePlanViewModel, Plane>();
            CreateMap<Plane, PlanViewModel>();
            CreateMap<Plane, UpdatePlanViewModel>().ForMember(dest => dest.PlanName, opt => opt.MapFrom(src => src.Name));
            CreateMap<UpdatePlanViewModel, Plane>()
           .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.PlanName))
           .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.Now));

        }
        private void MapMemberShip()
        {
            CreateMap<MemberShip, MemberShipViewModel>()
                .ForMember(dest => dest.MemberName, opt => opt.MapFrom(src => src.Member.Name))
                .ForMember(dest => dest.PlanName, opt => opt.MapFrom(src => src.Plan.Name))
                .ForMember(dest => dest.MemberPhoto, opt => opt.MapFrom(src => src.Member.Photo))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.CreatedAt.ToString("MMM dd, yyyy")))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate.ToString("MMM dd, yyyy")))
                .ForMember(dest => dest.DaysRemaining, opt => opt.MapFrom(src => (int)(src.EndDate - DateTime.Now).TotalDays));

            CreateMap<CreateMemberShipViewModel, MemberShip>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.EndDate, opt => opt.Ignore());

            CreateMap<Member, MemberSelectViewModel>();
            CreateMap<Plane, PlanSelectViewModel>();
        }
        private void MapMemberSession()
        {
            // MemberSession → BookingViewModel
            CreateMap<MemberSession, BookingViewModel>()
                .ForMember(dest => dest.MemberName, opt => opt.Ignore())
                .ForMember(dest => dest.BookingDate, opt => opt.MapFrom(src => src.CreatedAt.ToString("g")));

            // CreateBookingViewModel → MemberSession
            CreateMap<CreateBookingViewModel, MemberSession>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.IsAttended, opt => opt.MapFrom(src => false));

            // Member → MemberForBookingSelectViewModel
            CreateMap<Member, MemberForBookingSelectViewModel>();
        }
    }
}
