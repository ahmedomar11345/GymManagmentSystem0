using AutoMapper;
using GymManagmentBLL.Service.Interfaces;
using GymManagmentBLL.Service.Interfaces.AttachmentService;
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
    public class MemberService : IMemberService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAttachmentService _attachmentService;

        public MemberService(IUnitOfWork unitOfWork , IMapper mapper, IAttachmentService attachmentService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _attachmentService = attachmentService;
        }

        public bool CreateMember(CreateMemberViewModel createMember)
        {
            try
            {
                if (IsEmailExist(createMember.Email) || IsPhoneExist(createMember.Phone)) return false;

                var photoName = _attachmentService.Upload("members" , createMember.PhotoFile);
                if(string.IsNullOrEmpty(photoName)) return false;
                var memberEntity = _mapper.Map< Member>(createMember);
                memberEntity.Photo = photoName;
                _unitOfWork.GetRepository<Member>().Add(memberEntity);
                var IsCreated =  _unitOfWork.SaveChanges() > 0;
                if(!IsCreated)
                {
                    _attachmentService.Delete(photoName, "members" );
                    return false;
                }
                else
                {
                    return IsCreated;
                }
            }
            catch (Exception)
            {

                return false;
            }
        }

        public IEnumerable<MemberViewModel> GetAllMembers()
        {
            var Members = _unitOfWork.GetRepository<Member>().GetAll();
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

            var Member  = _unitOfWork.GetRepository<Member>().GetAll()?? [];
            if(Member is null || !Member.Any()) return [];
            var memberViewModels = _mapper.Map<IEnumerable<MemberViewModel>>(Member);
            return memberViewModels;

        }

        public HealthRecordViewModel? GetMemberHealthRecordDetails(int MemberId)
        {
            var MemberHelathRecord = _unitOfWork.GetRepository<HealthRecord>().GetById(MemberId);
            if (MemberHelathRecord is null) return null;
            return _mapper.Map<HealthRecordViewModel>(MemberHelathRecord);
        }

        public MemberViewModel? GetMemberDetails(int MemberId)
        {
            var Member = _unitOfWork.GetRepository<Member>().GetById(MemberId);
            if (Member is null) return null;
            var viewModel = _mapper.Map<MemberViewModel>(Member);

            // Active Membership
            var activeMembership = _unitOfWork.GetRepository<MemberShip>().GetAll(x => x.MemberId == MemberId && x.Status == "Active").FirstOrDefault();
            if (activeMembership is not null)
            {
                viewModel.MembershipStartDate = activeMembership.CreatedAt.ToShortDateString();
                viewModel.MembershipEndDate = activeMembership.EndDate.ToShortDateString();
                var plane = _unitOfWork.GetRepository<Plane>().GetById(activeMembership.PlanId);
                viewModel.PlaneName = plane?.Name;

            }
            return viewModel;
        }

        public MemberToUpdateViewModel? GetMemberToUpdate(int MemberId)
        {
            var Member = _unitOfWork.GetRepository<Member>().GetById(MemberId);
            if (Member is null) return null;
            return _mapper.Map<MemberToUpdateViewModel>(Member);
        }
                
        public bool UpdateMember(int MemberId, MemberToUpdateViewModel memberToUpdate)
        {
            try
            {
                var phoneexist = _unitOfWork.GetRepository<Member>().GetAll(x => x.Phone == memberToUpdate.Phone && x.Id != MemberId);
                var emailexist = _unitOfWork.GetRepository<Member>().GetAll(x => x.Email == memberToUpdate.Email && x.Id != MemberId);
                if (emailexist.Any() || phoneexist.Any())
                    return false;

                var MemberRepo = _unitOfWork.GetRepository<Member>();
                var Member = MemberRepo.GetById(MemberId);
                if (Member is null) return false;

                string? oldPhoto = Member.Photo;
                string? newPhotoName = null;

                if (memberToUpdate.PhotoFile is not null)
                {
                    newPhotoName = _attachmentService.Upload("members", memberToUpdate.PhotoFile);
                    if (string.IsNullOrEmpty(newPhotoName))
                        return false;
                    
                    memberToUpdate.Photo = newPhotoName;
                }

                _mapper.Map(memberToUpdate, Member);
                MemberRepo.Update(Member);
                var isUpdated = _unitOfWork.SaveChanges() > 0;

                if (isUpdated && newPhotoName is not null && !string.IsNullOrEmpty(oldPhoto))
                {
                    _attachmentService.Delete(oldPhoto, "members");
                }

                return isUpdated;
            }
            catch
            {
                return false;
            }
        }

        public bool RemoveMember(int MemberId)
        {
            var MemberRepo = _unitOfWork.GetRepository<Member>();
            var Member = MemberRepo.GetById(MemberId);
            if (Member is null) return false;
             
            var HasActiveMembersession = _unitOfWork.GetRepository<MemberSession>()
                .GetAll(x=> x.MemberId == MemberId && x.Session.StartDate > DateTime.Now)
                .Any();
            if (HasActiveMembersession) return false;
            var MemberShipRepo = _unitOfWork.GetRepository<MemberShip>();
            var membership = MemberShipRepo.GetAll(x => x.MemberId == MemberId);
            try
            {
                if(membership.Any())
                {
                    foreach (var memberShip in membership)
                    {
                        MemberShipRepo.Delete(memberShip);
                    }
                }
                MemberRepo.Delete(Member) ;
                var IsDeleted =  _unitOfWork.SaveChanges() > 0;
                if (IsDeleted)
                    _attachmentService.Delete(Member.Photo, "members");
                return IsDeleted;
                
            }
            catch
            {
                return false;
            }
        }


        #region Helper Method
        private bool IsEmailExist(string email)
        {
            return _unitOfWork.GetRepository<Member>().GetAll(x => x.Email == email).Any();
             
        }

        private bool IsPhoneExist(string phone)
        {
            return _unitOfWork.GetRepository<Member>().GetAll(x => x.Phone == phone).Any();
        }
        #endregion      
    
    }
} 
