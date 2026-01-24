using GymManagmentBLL.Service.Interfaces;
using GymManagmentBLL.ViewModels.MemberShipViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GymManagmentPL.Controllers
{
    public class MemberShipController : Controller
    {
        private readonly IMemberShipService _memberShipService;

        public MemberShipController(IMemberShipService memberShipService)
        {
            _memberShipService = memberShipService;
        }

        public ActionResult Index()
        {
            var memberShips = _memberShipService.GetAllActiveMemberShips();
            return View(memberShips);
        }

        public ActionResult Create()
        {
            LoadDropDownMembers();
            LoadDropDownPlans();
            return View();
        }

        [HttpPost]
        public ActionResult Create(CreateMemberShipViewModel createMemberShip)
        {
            if (!ModelState.IsValid)
            {
                LoadDropDownMembers();
                LoadDropDownPlans();
                return View(createMemberShip);
            }

            var result = _memberShipService.CreateMemberShip(createMemberShip);
            if (result)
            {
                TempData["SuccessMessage"] = "Membership created successfully.";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to create membership. Member may already have an active membership or selected plan is not active.";
                LoadDropDownMembers();
                LoadDropDownPlans();
                return View(createMemberShip);
            }
        }

        [HttpPost]
        public ActionResult Cancel([FromForm] int memberId, [FromForm] int planId)
        {
            var result = _memberShipService.CancelMemberShip(memberId, planId);
            if (result)
            {
                TempData["SuccessMessage"] = "Membership cancelled successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to cancel membership. Only active memberships can be cancelled.";
            }
            return RedirectToAction(nameof(Index));
        }

        private void LoadDropDownMembers()
        {
            var members = _memberShipService.GetMembersForDropDown();
            ViewBag.Members = new SelectList(members, "Id", "Name");
        }

        private void LoadDropDownPlans()
        {
            var plans = _memberShipService.GetActivePlansForDropDown();
            ViewBag.Plans = new SelectList(plans, "Id", "Name");
        }
    }
}
