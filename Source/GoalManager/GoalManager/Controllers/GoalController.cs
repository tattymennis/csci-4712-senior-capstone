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
            var vm = new CreateGoalViewModel();
            var userSessionData = Session["UserSessionData"] as UserSessionData;
            if (userSessionData == null)
            {
                // TODO: Error handling
            }

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
                    quartTempList.Add(new SelectListItem { Value = q.Name, Text = q.Name, Selected = false });
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
            if (vm == null || !User.Identity.IsAuthenticated)
            {
                // TODO: Error handling
            }

            var userSessionData = Session["UserSessionData"] as UserSessionData;
            if (userSessionData == null)
            {
                // TODO: Error handling
            }

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
                        quartTempList.Add(new SelectListItem { Value = q.Name, Text = q.Name, Selected = false });
                    }

                    Goal goal;
                    Update update;

                    // populate UID FK in Goal table
                    goal = new Goal(vm.Title, vm.CategoryName, "Active", 0);
                    goal.User = user; // null check?
                    goal.StartDate = DateTime.Now;
                    goal.EndDate = DateTime.Now; // test driver
                    //goal.EndDate = Convert.ToDateTime(vm.QuarterTime);
                    goal.Category = vm.CategoryName;

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

                    update.Goal = db.Goals.Add(goal);
                    db.Updates.Add(update);
                    db.SaveChanges();
                }
                return RedirectToAction("MainView", "Home");
            }

            var nvm = new CreateGoalViewModel();
            nvm.CatDropDown = catTempList;
            nvm.QuartDropDown = quartTempList;
            return View(nvm);
        }

        // GET UpdateGoal
        // Vulnerable. User can pass any value into Query string
        [Authorize]
        [HttpGet]
        public ActionResult UpdateGoal(int GIDRef)
        {
            var vm = new UpdateGoalViewModel();
            vm.GIDRef = GIDRef;
            try
            {
                var userSessionData = Session["UserSessionData"] as UserSessionData;

                if (userSessionData == null)
                {
                    // TODO: Error
                }

                else
                {
                    var goal = userSessionData.Goals.Where(x => x.GID == vm.GIDRef).FirstOrDefault();
                    List<Update> updates = userSessionData.GoalUpdatesTable[vm.GIDRef].ToList<Update>();

                    //vm.Goal = goal as Goal;
                    //vm.Updates = updates;
                }
                //return View("UpdateGoal", vm);
                return View(vm);
            }

            catch (Exception ex)
            {

            }
            //return View("UpdateGoal", vm);
            return View(vm);
        }

        [Authorize]
        [HttpPost]
        public ActionResult UpdateGoal(UpdateGoalViewModel vm)
        {
            var nvm = new UpdateGoalViewModel();
            try
            {
                if (!(vm.Progress > -1 && vm.Progress < 101))
                {
                    // TODO: Error
                }

                if (ModelState.IsValid)
                {
                    var userSessionData = Session["UserSessionData"] as UserSessionData;
                    if (userSessionData == null || userSessionData.GoalUpdatesTable == null)
                    {
                        ArgumentNullException ex = new ArgumentNullException("userSessionData", "Problem with Session data.");
                    }

                    else
                    {
                        var goal = userSessionData.Goals.Where(x => x.GID == vm.GIDRef).FirstOrDefault();
                        List<Update> updates = userSessionData.GoalUpdatesTable[vm.GIDRef].ToList<Update>();

                        //vm.Goal = goal as Goal;
                        //vm.Updates = updates;                       

                        Update update = new Update
                        {
                            Subject = vm.Subject,
                            Notes = vm.Notes,
                            Time = DateTime.Now,
                            Progress = vm.Progress,
                        };

                        nvm.GIDRef = vm.GIDRef;
                        //nvm.Goal = goal;
                        //nvm.Updates = updates;
                        //nvm.Updates.Add(update);

                        using (UserDBEntities db = new UserDBEntities())
                        {
                            update.Goal = db.Goals.Where(x => x.GID == vm.GIDRef).FirstOrDefault();

                            db.Updates.Add(update);
                            db.SaveChanges();
                        }
                        return RedirectToAction("MainView", "Home");
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

            // failed validation
            return View(nvm);
        }

        // ViewGoal
        [Authorize]
        public ActionResult ViewGoal(int GID)
        {
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

        // GET: Goal
        public ActionResult Index()
        {
            return View();
        }
    }
}