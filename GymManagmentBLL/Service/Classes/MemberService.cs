using GymManagmentBLL.Service.Interfaces;
using GymManagmentBLL.ViewModels.MemberViewModel;
using GymManagmentDAL.Entities;
using GymManagmentDAL.Reposotories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagmentBLL.Service.Classes
{
    internal class MemberService : IMemberService
    {
        private readonly IGenericRepository<Member> _memberRepository;
        private readonly IGenericRepository<MemberShip> _membershipRepository;
        private readonly IplaneRepository _planeRepository;
        private readonly IGenericRepository<HealthRecord> _healthRecordRepository;

        public MemberService(IGenericRepository<Member> memberRepository, IGenericRepository<MemberShip> membershipRepository,IplaneRepository planeRepository,IGenericRepository<HealthRecord> healthRecordRepository)
        {
            _memberRepository = memberRepository;
            _membershipRepository = membershipRepository;
            _planeRepository = planeRepository;
            _healthRecordRepository = healthRecordRepository;
        }

        public bool CreateMember(CreateMemberViewModel createMember)
        {
            try
            {
                // Cheack if Email is already exist
                var emailExist = _memberRepository.GetAll(x => x.Email == createMember.Email).Any();
                // Cheack if Phone is already exist
                var phoneExist = _memberRepository.GetAll(x => x.Phone == createMember.Phone).Any();
                // If one of them exist return false
                if (emailExist || phoneExist) return false;
                var member = new Member()
                {
                    Name = createMember.Name,
                    Email = createMember.Email,
                    Phone = createMember.Phone,
                    Gender = createMember.Gender,
                    DateOfBirth = createMember.DateOfBirth,
                    Address = new Address()
                    {
                        City = createMember.City,
                        Street = createMember.Street,
                        BuildingNumber = createMember.BuildingNumber
                    },
                    HealthRecord = new HealthRecord()
                    {
                        Height = createMember.HealthRecord.Height,
                        Weight = createMember.HealthRecord.Weight,
                        BloodType = createMember.HealthRecord.BloodType,
                        Note = createMember.HealthRecord.Note,
                    }
                };
                return _memberRepository.Add(member) > 0;
            }
            catch (Exception)
            {

                return false;
            }
        }

        public IEnumerable<MemberViewModel> GetAllMembers()
        {
            var Members = _memberRepository.GetAll();
            if (Members is null || !Members.Any()) return [];
            #region Way01
            //var MemberViewmodel = new List<MemberViewModel>();
            //foreach (var Member in Members)
            //{
            //    var memberviewmodel = new MemberViewModel()
            //    {
            //        Id = Member.Id,
            //        Name = Member.Name,
            //        Email = Member.Email,
            //        Phone = Member.Phone,
            //        Phote = Member.Phote,
            //        Gender = Member.Gender.ToString(),

            //    };
            //    MemberViewmodel.Add(memberviewmodel);
            //}
            #endregion

            #region Way02
            var MemberViewmodel = Members.Select(Member => new MemberViewModel()
            {
                Id = Member.Id,
                Name = Member.Name,
                Email = Member.Email,
                Phone = Member.Phone,
                Phote = Member.Phote,

            }); 
            #endregion
            return MemberViewmodel;
        }

        public HealthRecordViewModel? GetHealthRecordViewModelDetails(int MemberId)
        {
            var MemberHelathRecord = _healthRecordRepository.GetById(MemberId);
            if (MemberHelathRecord is null) return null;
            return new HealthRecordViewModel()
            {
                Height = MemberHelathRecord.Height,
                Weight = MemberHelathRecord.Weight,
                BloodType = MemberHelathRecord.BloodType,
                Note = MemberHelathRecord.Note,
            };
        }

        public MemberViewModel? GetMemberDetails(int MemberId)
        {
            var Member = _memberRepository.GetById(MemberId);
            if (Member is null) return null;
            var viewModel = new MemberViewModel()
            {
                Name = Member.Name,
                Email = Member.Email,
                Phone = Member.Phone,
                Gender = Member.Gender.ToString(),
                DateOfBirth = Member.DateOfBirth.ToShortDateString(),
                Address = $"{Member.Address.BuildingNumber}, {Member.Address.Street}, {Member.Address.City}",
                Phote = Member.Phote,
            };
            // Active Membership
            var activeMembership = _membershipRepository.GetAll(x => x.MemberId == MemberId && x.Status == "Active").FirstOrDefault();
            if(activeMembership is not null)
            {
                viewModel.MembershipStartDate = activeMembership.CreatedAt.ToShortDateString();
                viewModel.MembershipEndDate = activeMembership.EndDate.ToShortDateString();
                var plane = _planeRepository.GetById(activeMembership.PlanId);
                viewModel.PlaneName = plane?.Name;

            }
            return viewModel;
        }
    }
    }
} 
