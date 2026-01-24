using GymManagmentBLL.Service.Interfaces;
using GymManagmentBLL.ViewModels.MemberViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymManagmentPL.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class MemberController : Controller
    {
        private readonly IMemberService _memberService;

        public MemberController(IMemberService memberService )
        {
            _memberService = memberService;
        } 
        public ActionResult Index()
        {
            var member = _memberService.GetAllMembers();
            return View(member);
        }

        // baseurl/Member/MemberDetails/id
        public ActionResult MemberDetails(int id)
        {
            if (id <= 0)
            {
                TempData["ErrorMessage"] = "Invalid member ID.";
                return RedirectToAction(nameof(Index));
            }
            var member = _memberService.GetMemberDetails(id);
            if (member is null)
            {
                TempData["ErrorMessage"] = "Member not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(member);
        }

        public ActionResult HealthrecordDetails(int id)
        {
            if (id <= 0)
            {
                TempData["ErrorMessage"] = "Invalid member ID.";
                return RedirectToAction(nameof(Index));
            }
            var healthRecord = _memberService.GetMemberHealthRecordDetails(id);
            if (healthRecord is null)
            {
                TempData["ErrorMessage"] = "Member not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(healthRecord);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateMember(CreateMemberViewModel createMember)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("DataMissed", "Cheack data and Missing Field");
                return View(nameof(Create), createMember);
            }
            bool result = _memberService.CreateMember(createMember);

            if (result)
            {
                TempData["SuccessMessage"] = "Member created successfully.";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to create member. Please try again.";
                return RedirectToAction(nameof(Index));
            }
        }

        public ActionResult MemberEdit(int id)
        {
            if(id <= 0)
            {
                TempData["ErrorMessage"] = "Invalid member ID.";
                return RedirectToAction(nameof(Index));
            }
            var Member = _memberService.GetMemberToUpdate(id);
            if (Member is null)
            {
                TempData["ErrorMessage"] = "Member not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(Member);
        }
        [HttpPost]
        public ActionResult MemberEdit([FromRoute] int id , MemberToUpdateViewModel memberToUpdate)
        {
            if (!ModelState.IsValid)
                return View(memberToUpdate);
            var result = _memberService.UpdateMember(id, memberToUpdate);
            if (result)
            {
                TempData["SuccessMessage"] = "Member Updated successfully.";
                
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to Update member. Please try again.";
                
            }
            return RedirectToAction(nameof(Index));
        }

        public ActionResult Delete(int id)
        {
            if (id <= 0)
            {
                TempData["ErrorMessage"] = "Invalid member ID.";
                return RedirectToAction(nameof(Index));
            }
            var member = _memberService.GetMemberDetails(id);
            if (member is null)
            {
                TempData["ErrorMessage"] = "Member not found.";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.MemberId = id;
            return View(member);
        }
        [HttpPost]
        public ActionResult DeleteConfirm([FromForm] int id)
        {
            var result = _memberService.RemoveMember(id);
            if (result)
            {
                TempData["SuccessMessage"] = "Member Deleted successfully.";

            }
            else
            {
                TempData["ErrorMessage"] = "Failed to Delete member. Please try again.";

            }
            return RedirectToAction(nameof(Index));
        }
    }
}
