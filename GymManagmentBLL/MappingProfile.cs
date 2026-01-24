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
        .ForMember(dest => dest.Specialties, opt => opt.MapFrom(src => Enum.GetName(typeof(Specialities), src.Specialies)))
        .ForMember(dest => dest.Address, opt => opt.MapFrom(src => $"{src.Address.BuildingNumber}, {src.Address.Street}, {src.Address.City}"));

            CreateMap<CreateTrainerViewModel, Trainer>()
                .ForMember(dest => dest.Specialies, opt => opt.MapFrom(src => src.Specialties))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => new Address
                {
                    BuildingNumber = src.BuildingNumber,
                    City = src.City,
                    Street = src.Street
                }));

            CreateMap<Trainer, TrainerToUpdateViewModel>()
                .ForMember(dest => dest.Street, opt => opt.MapFrom(src => src.Address.Street))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.Address.City))
                .ForMember(dest => dest.BuildingNumber, opt => opt.MapFrom(src => src.Address.BuildingNumber));

            CreateMap<TrainerToUpdateViewModel, Trainer>()
                .ForMember(dest => dest.Name, opt => opt.Ignore())
                .AfterMap((src, dest) =>
                {
                    if (dest.Address == null) dest.Address = new Address(); // Prevent NullReference
                    dest.Address.BuildingNumber = src.BuildingNumber;
                    dest.Address.City = src.City;
                    dest.Address.Street = src.Street;
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
                      Street = src.Street
                  })).ForMember(dest => dest.HealthRecord, opt => opt.MapFrom(src => src.HealthRecord));


            CreateMap<HealthRecordViewModel, HealthRecord>().ReverseMap();
            CreateMap<Member, MemberViewModel>()
           .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender.ToString()))
            .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.DateOfBirth.ToShortDateString()))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => $"{src.Address.BuildingNumber} - {src.Address.Street} - {src.Address.City}"));

            CreateMap<Member, MemberToUpdateViewModel>()
            .ForMember(dest => dest.BuildingNumber, opt => opt.MapFrom(src => src.Address.BuildingNumber))
            .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.Address.City))
            .ForMember(dest => dest.Street, opt => opt.MapFrom(src => src.Address.Street));

            CreateMap<MemberToUpdateViewModel, Member>()
                .ForMember(dest => dest.Name, opt => opt.Ignore())
                .ForMember(dest => dest.Photo, opt => opt.Condition(src => !string.IsNullOrEmpty(src.Photo)))
                .AfterMap((src, dest) =>
                {
                    dest.Address.BuildingNumber = src.BuildingNumber;
                    dest.Address.City = src.City;
                    dest.Address.Street = src.Street;
                    if (!string.IsNullOrEmpty(src.Photo))
                    {
                        dest.Photo = src.Photo;
                    }
                    dest.UpdatedAt = DateTime.Now;
                });
        }
        private void MapPlan()
        {
            CreateMap<Plane, PlanViewModel>();
            CreateMap<Plane, UpdatePlanViewModel>().ForMember(dest => dest.PlanName, opt => opt.MapFrom(src => src.Name));
            CreateMap<UpdatePlanViewModel, Plane>()
           .ForMember(dest => dest.Name, opt => opt.Ignore())
           .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.Now));

        }
        private void MapMemberShip()
        {
            CreateMap<MemberShip, MemberShipViewModel>()
                .ForMember(dest => dest.MemberName, opt => opt.Ignore())
                .ForMember(dest => dest.PlanName, opt => opt.Ignore())
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.CreatedAt.ToString("MMM dd, yyyy")))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate.ToString("MMM dd, yyyy")));

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
