using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GoalManager.Models;
using GoalManager.Data;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace GoalManager.Controllers
{
    public class GoalController : Controller
    {
        // CreateGoal
        [Authorize]
        public ActionResult CreateGoal()
        {
            ViewBag.Title = "Create Goal";
            var vm = new CreateGoalViewModel();
            var userSessionData = Session["UserSessionData"] as UserSessionData;
            if (userSessionData == null)
            {
                // TODO: Error handling
            }

            if (userSessionData.Role != "Supervisor" && userSessionData.Role != "Employee")
            {
                // TODO: Error handling
            }

            if (userSessionData.Role == "Supervisor")
                vm.Role = "Supervisor";
            else
                vm.Role = "Employee";

            List<SelectListItem> catTempList = new List<SelectListItem>();
            catTempList.Add(new SelectListItem { Value = "1", Text = "Select a Category", Selected = true });

            List<SelectListItem> quartTempList = new List<SelectListItem>();
            quartTempList.Add(new SelectListItem { Value = "1", Text = "Select a Quarter", Selected = true });

            using (var db = new UserDBEntities())
            {
                int uid = userSessionData.UID;
                //int did = db.Users.Where(x => x.Username == userSessionData.Username).FirstOrDefault().DID;
                int did = userSessionData.DID;
                List<Category> cats = db.Categories.Where(x => x.DepID == did).ToList<Category>();
                List<Quarter> quarts = db.Quarters.Where(x => x.DID == did).ToList<Quarter>();

                foreach (Category c in cats)
                {
                    catTempList.Add(new SelectListItem { Value = c.Name, Text = c.Name, Selected = false });
                }

                foreach (Quarter q in quarts)
                {
                    quartTempList.Add(new SelectListItem { Value = q.Name, Text = q.Name + " - " + q.EndDate.ToString("D"), Selected = false });
                }
            }

            vm.CatDropDown = catTempList;
            vm.QuartDropDown = quartTempList;
            return View(vm);
        }

        // new HttpPost for CreateGoal
        [Authorize]
        [HttpPost]
        public ActionResult CreateGoal(CreateGoalViewModel vm)
        {
            ViewBag.Title = "Create Goal";
            if (vm == null || !User.Identity.IsAuthenticated)
            {
                // TODO: Error handling
            }

            var userSessionData = Session["UserSessionData"] as UserSessionData;
            if (userSessionData == null)
            {
                // TODO: Error handling
            }

            if (userSessionData.Role != "Supervisor" && userSessionData.Role != "Employee")
            {
                // TODO: Error handling
            }

            if (vm.Role != "Supervisor" && vm.Role != "Employee")
            {
                // TODO: Error handling
            }

            if (userSessionData.Role == "Supervisor")
                vm.Role = "Supervisor";
            else
                vm.Role = "Employee";

            List<SelectListItem> catTempList = new List<SelectListItem>();
            catTempList.Add(new SelectListItem { Value = "1", Text = "Select a Category", Selected = true });

            List<SelectListItem> quartTempList = new List<SelectListItem>();
            quartTempList.Add(new SelectListItem { Value = "1", Text = "Select a Quarter", Selected = true });

            if (ModelState.IsValid)
            {
                using (var db = new UserDBEntities())
                {
                    // var user = userSessionData.User ?? db.Users.Where(x => x.UID == vm.Employee.UID).FirstOrDefault();
                    // var department = userSessionData.Department ?? db.Departments.Where(x => x.DID == user.DID).FirstOrDefault();
                    var cats = db.Categories.Where(x => x.DepID == userSessionData.DID).ToList<Category>();
                    var quarts = db.Quarters.Where(x => x.DID == userSessionData.DID).ToList<Quarter>();
                    User user = db.Users.Where(x => x.UID == userSessionData.UID).FirstOrDefault();

                    foreach (Category c in cats)
                    {
                        catTempList.Add(new SelectListItem { Value = c.Name, Text = c.Name, Selected = false });
                    }

                    foreach (Quarter q in quarts)
                    {
                        quartTempList.Add(new SelectListItem { Value = q.Name, Text = q.Name + " - " + q.EndDate.ToString("D"), Selected = false });
                    }

                    Goal goal;
                    Update update;

                    // populate UID FK in Goal table
                    goal = new Goal(vm.Title, vm.CategoryName, "Pending", 0);
                    goal.User = user;
                    goal.StartDate = DateTime.Now;
                    goal.Category = vm.CategoryName;

                    // Find Quarter associated with QuartTime
                    Quarter quart = db.Quarters.Where(q => q.Name == vm.QuarterTime).FirstOrDefault();
                    goal.EndDate = quart.EndDate;

                    // Description
                    if (!String.IsNullOrWhiteSpace(vm.Description))
                    {
                        goal.Description = vm.Description;
                    }

                    // create an initial Update
                    update = new Update
                    {
                        Subject = "Created goal",
                        Notes = "",
                        Progress = 0,
                        Time = goal.StartDate
                    };

                    if (vm.Role == "Supervisor" && vm.PushToDept == true)
                    {
                        List<User> users = db.Users.Where(u => u.SUID == user.UID).ToList<User>();
                        foreach (User u in users)
                        {
                            goal.User = u;
                            goal.Approved = true;
                            goal.Status = "Active";
                            update.Goal = db.Goals.Add(goal); // add Goal for each EE
                            db.Updates.Add(update); // add Update for each EE
                            db.SaveChanges();
                        }
                    }

                    else if (vm.Role == "Supervisor" && vm.PushToDept == false)
                    {
                        goal.Approved = true; // Super goal is approved automatically
                        goal.Status = "Active";
                        update.Goal = db.Goals.Add(goal);
                        db.Updates.Add(update);
                        db.SaveChanges();
                    }

                    else
                    {
                        update.Goal = db.Goals.Add(goal);
                        db.Updates.Add(update);
                        db.SaveChanges();
                    }
                }
                return RedirectToAction("MainView", "Home");
            }

            var nvm = new CreateGoalViewModel();
            nvm.CatDropDown = catTempList;
            nvm.QuartDropDown = quartTempList;
            nvm.Role = userSessionData.Role;
            return View(nvm);
        }

        // GET UpdateGoal
        [Authorize]
        [HttpGet]
        public ActionResult UpdateGoal(string GIDRef)
        {
            ViewBag.Title = "Update Goal";
            var vm = new UpdateGoalViewModel();
            try
            {
                var userSessionData = Session["UserSessionData"] as UserSessionData;
                if (userSessionData == null)
                {
                    throw new ArgumentNullException("userSessionData", "Null session data");
                }

                int _GIDRef = -1;
                if (int.TryParse(GIDRef, out _GIDRef))
                {
                    vm.GIDRef = _GIDRef;
                    if (vm.GIDRef < 0)
                    {
                        throw new ArgumentException("Invalid GID reference", "GIDRef");
                    }
                }

                var goal = userSessionData.Goals.Where(x => x.GID == vm.GIDRef).FirstOrDefault();
                List<Update> updates = userSessionData.GoalUpdatesTable[vm.GIDRef].ToList<Update>();

                vm.Goal = goal;
                vm.Updates = updates;
                return View(vm);
            }

            catch (ArgumentNullException ex)
            {
                TempData["ErrorMessage"] = "Invalid session data. " + ex.Message;
                return RedirectToAction("MainView", "Home");
            }

            catch (ArgumentException ex)
            {
                TempData["ErrorMessage"] = "Invalid GID reference. " + ex.Message;
                return RedirectToAction("MainView", "Home");
            }

            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Invalid user role. " + ex.Message;
                return RedirectToAction("MainView", "Home");
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult UpdateGoal(UpdateGoalViewModel vm)
        {
            ViewBag.Title = "Update Goal";
            try
            {
                var userSessionData = Session["UserSessionData"] as UserSessionData;
                if (userSessionData == null || userSessionData.GoalUpdatesTable == null)
                {
                    throw new ArgumentNullException("userSessionData", "Null session data.");
                }

                if (userSessionData.Role != "Employee" && userSessionData.Role != "Supervisor")
                {
                    throw new ArgumentException("Session data role invalid.", "userSessionData");
                }

                if (!(vm.Progress > -1 && vm.Progress < 101))
                {
                    ModelState.AddModelError("Progress", "Progress value not bounded [0,100].");
                }

                var goal = userSessionData.Goals.Where(x => x.GID == vm.GIDRef).FirstOrDefault();
                List<Update> updates = userSessionData.GoalUpdatesTable[vm.GIDRef].ToList<Update>();

                // Progress logic - must be >= updated Goal
                if (vm.Progress < goal.Progress)
                {
                    ModelState.AddModelError("Progress", "Updated progress value is less than old progress value.");
                }

                if (ModelState.IsValid)
                {
                    Update update = new Update
                    {
                        Subject = vm.Subject,
                        Notes = vm.Notes,
                        Time = DateTime.Now,
                        Progress = vm.Progress,
                    };

                    using (UserDBEntities db = new UserDBEntities())
                    {
                        update.Goal = db.Goals.Where(x => x.GID == vm.GIDRef).FirstOrDefault();
                        var _goal = db.Goals.Where(g => g.GID == vm.GIDRef).FirstOrDefault();
                        _goal.Progress = vm.Progress;

                        db.Updates.Add(update);
                        db.SaveChanges();
                    }
                    return RedirectToAction("MainView", "Home");                  
                }
            }

            catch (ArgumentNullException ex)
            {
                TempData["ErrorMessage"] = "Invalid session data. " + ex.Message;
                return RedirectToAction("MainView", "Home");
            }

            catch (ArgumentException ex)
            {
                TempData["ErrorMessage"] = "Invalid GID reference. " + ex.Message;
                return RedirectToAction("MainView", "Home");
            }

            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Invalid user role. " + ex.Message;
                return RedirectToAction("MainView", "Home");
            }

            // failed validation
            return View(vm.GIDRef.ToString());
        }

        // ViewGoal
        [Authorize]
        public ActionResult ViewGoal(int GID)
        {
            ViewBag.Title = "View Goal";
            ViewGoalViewModel vm = new ViewGoalViewModel();
            vm.GID = GID;
            try
            {
                var userSessionData = Session["UserSessionData"] as UserSessionData;

                if (userSessionData == null)
                {
                    ArgumentNullException ex = new ArgumentNullException("userSessionData", "Problem with Session data.");
                }

                else
                {
                    vm.Goal = userSessionData.Goals.Where(x => x.GID == vm.GID).First();
                    vm.Updates = userSessionData.GoalUpdatesTable[vm.GID];
                }

                return View(vm);

            }
            catch (ArgumentNullException ex)
            {
                // TODO: Error handling
            }

            catch (Exception ex)
            {
                // TODO: Error handling
            }

            return View(vm);
        }

        [Authorize]
        [HttpPost]
        public ActionResult ViewGoal(ViewGoalViewModel vm) 
        {
            ViewBag.Title = "View Goal";
            try
            {
                if (ModelState.IsValid)
                {
                    var userSessionData = Session["UserSessionData"] as UserSessionData;

                    if (userSessionData == null)
                    {
                        ArgumentNullException ex = new ArgumentNullException("userSessionData", "Problem with Session data.");
                    }

                    else
                    {
                        vm.Goal = userSessionData.Goals.Where(x => x.GID == vm.GID).First();
                        vm.Updates = userSessionData.GoalUpdatesTable[vm.GID];
                    }
                }    
            }

            catch (ArgumentNullException ex)
            {
                // TODO: Error handling
            }

            catch (Exception ex)
            {
                // TODO: Error handling
            }

            return View(vm);
        }

        // GET: ApproveGoal
        public ActionResult ApproveGoal(string gid)
        {
            try
            {
                UserSessionData userSessionData = Session["UserSessionData"] as UserSessionData;
                if (userSessionData == null)
                {
                    throw new ArgumentNullException();
                }

                if (userSessionData.Role != "Supervisor")
                {
                    throw new Exception();
                }

                int _gid = -1;
                if(int.TryParse(gid, out _gid))
                {
                    if (_gid < 0)
                    {
                        throw new Exception(); // error
                    }
                    using (UserDBEntities db = new UserDBEntities())
                    {
                        var goal = db.Goals.Where(x => x.GID == _gid).FirstOrDefault();
                        if (goal == null)
                        {
                            throw new Exception();
                        }
                        goal.Approved = true;
                        goal.Status = "Active";
                        db.SaveChanges();   
                    }
                }
                return RedirectToAction("MainView", "Home");
            }

            // refine exceptions
            catch (ArgumentNullException ex)
            {
                TempData["ErrorMessage"] = "Invalid GID reference. " + ex.Message;
                return RedirectToAction("MainView", "Home");
            }

            catch (ArgumentException ex)
            {
                TempData["ErrorMessage"] = "Invalid GID reference. " + ex.Message;
                return RedirectToAction("MainView", "Home");
            }

            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Invalid GID reference. " + ex.Message;
                return RedirectToAction("MainView", "Home");
            }
        }

        // GET: DenyGoal
        public ActionResult DenyGoal(string gid)
        {
            try
            {
                int _gid = -1;
                if (int.TryParse(gid, out _gid))
                {
                    if (_gid < 0)
                    {
                        throw new Exception(); // error
                    }
                    using (UserDBEntities db = new UserDBEntities())
                    {
                        var goal = db.Goals.Where(x => x.GID == _gid).FirstOrDefault();
                        if (goal == null)
                        {
                            throw new Exception();
                        }
                        goal.Approved = false;
                        goal.Status = "Denied";
                        db.SaveChanges();
                    }
                }
                return RedirectToAction("MainView", "Home");
            }

            catch (ArgumentException ex)
            {
                TempData["ErrorMessage"] = "Invalid GID reference. " + ex.Message;
                return RedirectToAction("MainView", "Home");
            }

            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Invalid GID reference. " + ex.Message;
                return RedirectToAction("MainView", "Home");
            }
        }
    }
}