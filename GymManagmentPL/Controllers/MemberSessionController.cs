using GymManagmentBLL.Service.Interfaces;
using GymManagmentBLL.ViewModels.MemberSessionViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GymManagmentPL.Controllers
{
    public class MemberSessionController : Controller
    {
        private readonly IMemberSessionService _memberSessionService;

        public MemberSessionController(IMemberSessionService memberSessionService)
        {
            _memberSessionService = memberSessionService;
        }

        public ActionResult Index()
        {
            var upcomingSessions = _memberSessionService.GetUpcomingSessions();
            var ongoingSessions = _memberSessionService.GetOngoingSessions();

            ViewBag.UpcomingSessions = upcomingSessions;
            ViewBag.OngoingSessions = ongoingSessions;

            return View();
        }

        public ActionResult GetMembersForUpcomingSession(int id)
        {
            if (id <= 0)
            {
                TempData["ErrorMessage"] = "Invalid session ID.";
                return RedirectToAction(nameof(Index));
            }

            var sessionMembers = _memberSessionService.GetMembersForUpcomingSession(id);
            if (sessionMembers is null)
            {
                TempData["ErrorMessage"] = "Session not found or is not upcoming.";
                return RedirectToAction(nameof(Index));
            }

            return View(sessionMembers);
        }

        public ActionResult GetMembersForOngoingSession(int id)
        {
            if (id <= 0)
            {
                TempData["ErrorMessage"] = "Invalid session ID.";
                return RedirectToAction(nameof(Index));
            }

            var sessionMembers = _memberSessionService.GetMembersForOngoingSession(id);
            if (sessionMembers is null)
            {
                TempData["ErrorMessage"] = "Session not found or is not ongoing.";
                return RedirectToAction(nameof(Index));
            }

            return View(sessionMembers);
        }

        public ActionResult Create(int id)
        {
            if (id <= 0)
            {
                TempData["ErrorMessage"] = "Invalid session ID.";
                return RedirectToAction(nameof(Index));
            }

            LoadMembersDropDown(id);
            ViewBag.SessionId = id;

            return View(new CreateBookingViewModel { SessionId = id });
        }

        [HttpPost]
        public ActionResult Create(CreateBookingViewModel createBooking)
        {
            if (!ModelState.IsValid)
            {
                LoadMembersDropDown(createBooking.SessionId);
                ViewBag.SessionId = createBooking.SessionId;
                return View(createBooking);
            }

            var result = _memberSessionService.CreateBooking(createBooking);
            if (result)
            {
                TempData["SuccessMessage"] = "Booking created successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to create booking. Member may not have active membership, session is full, or member already booked.";
            }

            return RedirectToAction(nameof(GetMembersForUpcomingSession), new { id = createBooking.SessionId });
        }

        [HttpPost]
        public ActionResult Cancel([FromForm] int memberId, [FromForm] int sessionId)
        {
            var result = _memberSessionService.CancelBooking(memberId, sessionId);
            if (result)
            {
                TempData["SuccessMessage"] = "Booking cancelled successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to cancel booking. Session may have already started.";
            }

            return RedirectToAction(nameof(GetMembersForUpcomingSession), new { id = sessionId });
        }

        [HttpPost]
        public ActionResult MarkAttendance([FromForm] int memberId, [FromForm] int sessionId)
        {
            var result = _memberSessionService.MarkAttendance(memberId, sessionId);
            if (result)
            {
                TempData["SuccessMessage"] = "Attendance marked successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to mark attendance.";
            }

            return RedirectToAction(nameof(GetMembersForOngoingSession), new { id = sessionId });
        }

        private void LoadMembersDropDown(int sessionId)
        {
            var members = _memberSessionService.GetMembersWithActiveMembershipForDropDown(sessionId);
            ViewBag.Members = new SelectList(members, "Id", "Name");
        }
    }
}
