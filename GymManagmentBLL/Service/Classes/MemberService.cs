using AutoMapper;
using QRCoder;
using GymManagmentBLL.Service.Interfaces;
using GymManagmentBLL.Service.Interfaces.AttachmentService;
using GymManagmentBLL.ViewModels.MemberViewModel;
using GymManagmentDAL.Entities;
using GymManagmentDAL.Repositories.Interfaces;
using Microsoft.Extensions.Logging;
using GymManagmentBLL.Service.Implementations;
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
        private readonly IEmailService _emailService;
        private readonly INotificationService _notificationService;
        private readonly IGymSettingsService _gymSettingsService;
        private readonly ILogger<MemberService> _logger;

        public MemberService(
            IUnitOfWork unitOfWork, 
            IMapper mapper, 
            IAttachmentService attachmentService,
            IEmailService emailService,
            INotificationService notificationService,
            IGymSettingsService gymSettingsService,
            ILogger<MemberService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _attachmentService = attachmentService;
            _emailService = emailService;
            _notificationService = notificationService;
            _gymSettingsService = gymSettingsService;
            _logger = logger;
        }

        private bool IsArabic => System.Globalization.CultureInfo.CurrentUICulture.Name.StartsWith("ar");

        public async Task<bool> CreateMemberAsync(CreateMemberViewModel createMember)
        {
            try
            {
                if (await IsEmailExistAsync(createMember.Email))
                {
                    _logger.LogWarning("Attempt to create member with duplicate email: {Email}", createMember.Email);
                    return false;
                }
                
                if (await IsPhoneExistAsync(createMember.Phone))
                {
                    _logger.LogWarning("Attempt to create member with duplicate phone: {Phone}", createMember.Phone);
                    return false;
                }

                string? photoName = null;
                if (createMember.PhotoFile is not null)
                {
                    photoName = _attachmentService.Upload("members", createMember.PhotoFile);
                    if (string.IsNullOrEmpty(photoName))
                    {
                        _logger.LogWarning("Failed to upload photo for member: {Name}", createMember.Name);
                        return false;
                    }
                }

                var memberEntity = _mapper.Map<Member>(createMember);
                memberEntity.Photo = photoName ?? string.Empty;

                await _unitOfWork.MemberRepository.AddAsync(memberEntity);
                
                // Handle optional membership plan
                if (createMember.PlanId.HasValue)
                {
                    var plan = await _unitOfWork.GetRepository<Plane>().GetByIdAsync(createMember.PlanId.Value);
                    if (plan != null)
                    {
                        var membership = new MemberShip
                        {
                            Member = memberEntity,
                            PlanId = plan.Id,
                            EndDate = DateTime.Now.AddDays(plan.DurationDays),
                        };
                        // Add to memberships collection or repository
                        await _unitOfWork.GetRepository<MemberShip>().AddAsync(membership);
                    }
                }

                var isCreated = await _unitOfWork.SaveChangesAsync() > 0;

                if (!isCreated)
                {
                    if (photoName != null) _attachmentService.Delete(photoName, "members");
                    _logger.LogWarning("Failed to save member to database: {Name}", createMember.Name);
                    return false;
                }

                // Send Onboarding Emails & Notifications
                try
                {
                    // 0. Create System Notification for Admin
                    await _notificationService.CreateNotificationAsync(
                        "New Member Registered",
                        $"{memberEntity.Name} has joined the gym.",
                        $"/Member/Details/{memberEntity.Id}",
                        null, // Global for Admins
                        "success"
                    );

                    // 1. Send Welcome Email with QR Code
                    QRCodeGenerator qrGenerator = new QRCodeGenerator();
                    QRCodeData qrCodeData = qrGenerator.CreateQrCode(memberEntity.AccessKey, QRCodeGenerator.ECCLevel.Q);
                    
                    // Simple PNG approach with CID for better email client compatibility
                    using (PngByteQRCode qrCode = new PngByteQRCode(qrCodeData))
                    {
                        byte[] qrCodeBytes = qrCode.GetGraphic(20); // Higher resolution for crisp display
                        string qrContentId = $"qrcode_{memberEntity.Id}_{Guid.NewGuid():N}";
                        
                        bool isArabic = createMember.SendInArabic;
                        var gymSettings = await _gymSettingsService.GetSettingsAsync();
                        string welcomeTemplate = EmailTemplates.MemberQRCodeWithCID(createMember.Name, gymSettings.GymName, gymSettings.Phone, gymSettings.Address, gymSettings.Email, qrContentId, isArabic);
                        await _emailService.SendEmailWithImageAsync(
                            createMember.Email, 
                            isArabic ? "مرحباً بك! كود الدخول الخاص بك" : "Welcome! Your Access Code", 
                            welcomeTemplate,
                            qrCodeBytes,
                            qrContentId
                        );
                    }

                    // 2. If plan was selected, send Receipt Email
                    if (createMember.PlanId.HasValue)
                    {
                        var plan = await _unitOfWork.GetRepository<Plane>().GetByIdAsync(createMember.PlanId.Value);
                        if (plan != null)
                        {
                            bool isArabic = createMember.SendInArabic;
                            var gymSettings = await _gymSettingsService.GetSettingsAsync();
                            string receiptTemplate = EmailTemplates.MembershipReceipt(
                                createMember.Name, 
                                plan.Name, 
                                plan.Price, 
                                DateTime.Now.AddDays(plan.DurationDays),
                                plan.DurationDays,
                                gymSettings.GymName,
                                gymSettings.Phone,
                                gymSettings.Address,
                                gymSettings.Email,
                                isArabic
                            );
                            await _emailService.SendEmailAsync(createMember.Email, isArabic ? "إيصال تفعيل الاشتراك" : "Membership Activation Receipt", receiptTemplate);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send onboarding emails to {Email}", createMember.Email);
                    // We don't return false here because the member was already successfully created in DB
                }

                _logger.LogInformation("Member created successfully: {Name}", createMember.Name);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating member: {Name}", createMember.Name);
                return false;
            }
        }

        public async Task<IEnumerable<MemberViewModel>> GetAllMembersAsync()
        {
            var members = await _unitOfWork.MemberRepository.GetAllAsync();
            
            if (members is null || !members.Any()) 
                return Enumerable.Empty<MemberViewModel>();

            return _mapper.Map<IEnumerable<MemberViewModel>>(members);
        }

        public async Task<PagedResult<MemberViewModel>> GetMembersPagedAsync(int pageNumber, int pageSize, string? searchTerm = null)
        {
            PagedResult<Member> pagedResult;
            
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                pagedResult = await _unitOfWork.MemberRepository.GetPagedAsync(pageNumber, pageSize, null, q => q.OrderByDescending(m => m.CreatedAt));
            }
            else
            {
                var term = searchTerm.ToLower();
                pagedResult = await _unitOfWork.MemberRepository.GetPagedAsync(
                    pageNumber, 
                    pageSize,
                    m => m.Name.ToLower().Contains(term) || 
                         m.Email.ToLower().Contains(term) ||
                         m.Phone.Contains(term),
                    q => q.OrderByDescending(m => m.CreatedAt));
            }

            return new PagedResult<MemberViewModel>
            {
                Items = _mapper.Map<IEnumerable<MemberViewModel>>(pagedResult.Items),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }

        public async Task<HealthRecordViewModel?> GetMemberHealthRecordDetailsAsync(int memberId)
        {
            if (memberId <= 0) return null;
            
            var healthRecord = await _unitOfWork.GetRepository<HealthRecord>().GetByIdAsync(memberId);
            
            if (healthRecord is null) 
                return null;
            
            return _mapper.Map<HealthRecordViewModel>(healthRecord);
        }

        public async Task<MemberViewModel?> GetMemberDetailsAsync(int memberId)
        {
            if (memberId <= 0) return null;
            
            // Using specialized repo with Include for details
            var member = await _unitOfWork.MemberRepository.GetWithDetailsAsync(memberId);
            
            if (member is null) 
                return null;

            return _mapper.Map<MemberViewModel>(member);
        }

        public async Task<MemberToUpdateViewModel?> GetMemberToUpdateAsync(int memberId)
        {
            if (memberId <= 0) return null;
            
            var member = await _unitOfWork.MemberRepository.GetByIdAsync(memberId);
            
            if (member is null) 
                return null;

            return _mapper.Map<MemberToUpdateViewModel>(member);
        }

        public async Task<bool> UpdateMemberAsync(int memberId, MemberToUpdateViewModel memberToUpdate)
        {
            try
            {
                if (memberId <= 0) return false;

                var emailExists = await _unitOfWork.MemberRepository.ExistsAsync(
                    m => m.Email == memberToUpdate.Email && m.Id != memberId);
                    
                var phoneExists = await _unitOfWork.MemberRepository.ExistsAsync(
                    m => m.Phone == memberToUpdate.Phone && m.Id != memberId);

                if (emailExists || phoneExists)
                {
                    _logger.LogWarning("Duplicate email/phone for member update {Id}", memberId);
                    return false;
                }

                var member = await _unitOfWork.MemberRepository.GetByIdAsync(memberId);
                if (member is null) return false;

                string? oldPhoto = member.Photo;
                string? newPhotoName = null;

                if (memberToUpdate.PhotoFile is not null)
                {
                    newPhotoName = _attachmentService.Upload("members", memberToUpdate.PhotoFile);
                    if (string.IsNullOrEmpty(newPhotoName)) return false;
                    memberToUpdate.Photo = newPhotoName;
                }

                _mapper.Map(memberToUpdate, member);
                _unitOfWork.MemberRepository.Update(member);
                var isUpdated = await _unitOfWork.SaveChangesAsync() > 0;

                if (isUpdated && newPhotoName is not null && !string.IsNullOrEmpty(oldPhoto))
                {
                    _attachmentService.Delete(oldPhoto, "members");
                }

                return isUpdated;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating member {Id}", memberId);
                return false;
            }
        }

        public async Task<bool> RemoveMemberAsync(int memberId)
        {
            try
            {
                if (memberId <= 0) return false;

                var member = await _unitOfWork.MemberRepository.GetWithDetailsAsync(memberId);
                if (member is null) return false;

                // Business Rule: Cannot delete member with active membership
                if (member.Memberships != null && member.Memberships.Any(m => m.EndDate >= DateTime.Now && !m.IsDeleted))
                {
                    _logger.LogWarning("Deletion blocked: Member {Id} has an active membership until {Date}", memberId, member.Memberships.Max(m => m.EndDate));
                    return false;
                }

                // Business Rule: Cannot delete member with upcoming session bookings
                if (member.MemberSessions != null && member.MemberSessions.Any(ms => ms.Session.StartDate > DateTime.Now && !ms.IsDeleted))
                {
                    _logger.LogWarning("Deletion blocked: Member {Id} has upcoming session bookings.", memberId);
                    return false;
                }
                
                string? photoToDelete = member.Photo;

                // 1. Mark session bookings for deletion (past sessions)
                if (member.MemberSessions is not null && member.MemberSessions.Any())
                {
                    _unitOfWork.MemberSessionRepository.DeleteRange(member.MemberSessions);
                }

                // 2. Mark memberships for deletion (expired memberships)
                if (member.Memberships is not null && member.Memberships.Any())
                {
                    _unitOfWork.MemberShipRepository.DeleteRange(member.Memberships);
                }

                // 3. Mark member and health record for deletion
                // Manual Soft Delete to avoid Owned Type NULL column issues during State Transition
                member.IsDeleted = true;
                member.UpdatedAt = DateTime.Now;
                
                if (member.HealthRecord != null)
                {
                    member.HealthRecord.IsDeleted = true;
                    member.HealthRecord.UpdatedAt = DateTime.Now;
                }

                _unitOfWork.MemberRepository.Update(member);

                // 4. Single SaveChangesAsync call handles the internal transaction
                var result = await _unitOfWork.SaveChangesAsync() > 0;

                if (result && !string.IsNullOrEmpty(photoToDelete))
                {
                    _attachmentService.Delete(photoToDelete, "members");
                }

                return result;
            }
            catch (Exception ex)
            {
                var innerMsg = ex.InnerException?.Message ?? ex.Message;
                _logger.LogError(ex, "FATAL: Could not delete member {Id}. Error: {Msg}", memberId, innerMsg);
                return false;
            }
        }

        private async Task<bool> IsEmailExistAsync(string email)
        {
            return await _unitOfWork.MemberRepository.ExistsAsync(m => m.Email == email);
        }

        private async Task<bool> IsPhoneExistAsync(string phone)
        {
            return await _unitOfWork.MemberRepository.ExistsAsync(m => m.Phone == phone);
        }

        public async Task<bool> RefreshAccessKeyAsync(int memberId)
        {
            try
            {
                var member = await _unitOfWork.MemberRepository.GetByIdAsync(memberId);
                if (member == null || member.IsDeleted) return false;

                // Generate new access key
                member.AccessKey = Guid.NewGuid().ToString("N");
                _unitOfWork.MemberRepository.Update(member);

                var result = await _unitOfWork.SaveChangesAsync() > 0;
                if (!result) return false;

                // Send Email with NEW QR Code
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(member.AccessKey, QRCodeGenerator.ECCLevel.Q);
                
                using (PngByteQRCode qrCode = new PngByteQRCode(qrCodeData))
                {
                    byte[] qrCodeBytes = qrCode.GetGraphic(20);
                    string qrContentId = $"qrcode_refresh_{member.Id}_{Guid.NewGuid():N}";
                    
                    var gymSettings = await _gymSettingsService.GetSettingsAsync();
                    string template = EmailTemplates.MemberQRCodeWithCID(
                        member.Name, 
                        gymSettings.GymName,
                        gymSettings.Phone,
                        gymSettings.Address,
                        gymSettings.Email,
                        qrContentId, 
                        true, 
                        "لقد تم تحديث رمز الدخول الخاص بك لدواعي الأمان. يرجى استخدام الرمز الجديد من الآن فصاعداً."
                    );

                    await _emailService.SendEmailWithImageAsync(
                        member.Email, 
                        "تحديث رمز الدخول QR كود الجديد", 
                        template,
                        qrCodeBytes,
                        qrContentId
                    );
                }

                _logger.LogInformation("Access key refreshed for member {Id}", memberId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to refresh access key for member {Id}", memberId);
                return false;
            }
        }

        #region Health Progress Tracking
        public async Task<IEnumerable<HealthProgressViewModel>> GetMemberHealthProgressAsync(int memberId)
        {
            var progress = await _unitOfWork.GetRepository<HealthProgress>().GetAllAsync(hp => hp.MemberId == memberId);
            return _mapper.Map<IEnumerable<HealthProgressViewModel>>(progress.OrderByDescending(p => p.ProgressDate));
        }

        public async Task<bool> AddHealthProgressAsync(HealthProgressViewModel progressVm)
        {
            var progress = _mapper.Map<HealthProgress>(progressVm);
            if (progress.ProgressDate == default) progress.ProgressDate = DateTime.Now;
            
            await _unitOfWork.GetRepository<HealthProgress>().AddAsync(progress);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }
        #endregion
    }
}
