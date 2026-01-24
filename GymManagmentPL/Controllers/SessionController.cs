using GymManagmentBLL.Service.Interfaces;
using GymManagmentBLL.ViewModels.SessionViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GymManagmentPL.Controllers
{
    public class SessionController : Controller
    {
        private readonly ISessionService _sessionService;

        public SessionController(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }
        public IActionResult Index()
        {
            var sessions = _sessionService.GetAllSession();
            return View(sessions);
        }

        public ActionResult Details(int id)
        {
            
            if(id <= 0)
            {
                TempData["ErrorMessage"] = "Invalid Session Id";
                return RedirectToAction(nameof(Index));
            }
            var session = _sessionService.GetSessionById(id);
            if(session is null)
            {
                TempData["ErrorMessage"] = "Session Not Found.";
                return RedirectToAction(nameof(Index));
            }
            return View(session);

        }

        public ActionResult Create()
        {
            LoadDropDownCategory();
            LoadDropDownTrainer();
            return View();
        }

        [HttpPost]
        public ActionResult Create(CreateSessionViewModel createSession)
        {
            if (!ModelState.IsValid)
            {
                LoadDropDownCategory();
                LoadDropDownTrainer();
                return View(createSession);
            }
            var result = _sessionService.CreateSession(createSession);
            if (result)
            {
                TempData["SuccessMessage"] = "Session Created Successfully.";
                return RedirectToAction(nameof(Index));
            }
            else {
                TempData["ErrorMessage"] = "Failed to Create Session.";
                LoadDropDownCategory();
                LoadDropDownTrainer();
                return View(createSession);
            }
        }

        public ActionResult Edit(int id)
        {
            if (id <= 0)
            {
                TempData["ErrorMessage"] = "Invalid Session Id";
                return RedirectToAction(nameof(Index));
            }
            var session = _sessionService.GetSessionToUpdate(id);
            if (session is null)
            {
                TempData["ErrorMessage"] = "Session Not Found.";
                return RedirectToAction(nameof(Index));
            }
            LoadDropDownTrainer();
            return View(session);
        }

        [HttpPost]
        public ActionResult Edit([FromRoute]int id,UpdateSessionViewModel updateSession)
        {
            if (!ModelState.IsValid)
            {
                LoadDropDownTrainer();
                return View(updateSession);
            }
            var result = _sessionService.UpdateSession(id, updateSession);
            if (result)
            {
                TempData["SuccessMessage"] = "Session Updated Successfully.";
                
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to Update Session.";
                
            }
            return RedirectToAction(nameof(Index));
        }

        public ActionResult Delete(int id)
        {
            if (id <= 0)
            {
                TempData["ErrorMessage"] = "Invalid Session Id";
                return RedirectToAction(nameof(Index));
            }
            var session = _sessionService.GetSessionById(id);
            if (session is null)
            {
                TempData["ErrorMessage"] = "Session Not Found";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.SessionId = session.Id;
            return View();
        }

        [HttpPost]
        public ActionResult DeleteConfirmed(int id)
        {
            var result = _sessionService.RemoveSession(id);
            if (result)
            {
                TempData["SuccessMessage"] = "Session Deleted Successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to Delete Session.";
            }
            return RedirectToAction(nameof(Index));
        }

        private void LoadDropDownCategory()
        {
            var category = _sessionService.GetCategoryForDropDown();
            ViewBag.Categories = new SelectList(category, "Id", "Name");
            
        }

        private void LoadDropDownTrainer()
        {
            
            var trainer = _sessionService.GetTrainerForDropDown();
            ViewBag.Trainers = new SelectList(trainer, "Id", "Name");
        }
    }
}
