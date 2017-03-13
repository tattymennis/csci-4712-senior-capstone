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
            string username = User.Identity.GetUserName();

            List<SelectListItem> catTempList = new List<SelectListItem>();
            catTempList.Add(new SelectListItem { Value = "1", Text = "Select a Category", Selected = true });

            List<SelectListItem> quartTempList = new List<SelectListItem>();
            quartTempList.Add(new SelectListItem { Value = "1", Text = "Select a Quarter", Selected = true });

            using (var db = new UserDBEntities())
            {
                if (String.IsNullOrWhiteSpace(username))
                {
                    // error
                }

                else
                {
                    int did = db.Users.Where(x => x.Username == username).FirstOrDefault().DID;
                    var cats = db.Categories.Where(x => x.DepID == did);
                    var quarts = db.Quarters.Where(x => x.DID == did);

                    foreach (Category c in cats)
                    {
                        catTempList.Add(new SelectListItem { Value = c.Name, Text = c.Name, Selected = false });
                    }

                    foreach (Quarter q in quarts)
                    {
                        quartTempList.Add(new SelectListItem { Value = q.Name, Text = q.Name, Selected = false });
                    }
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
            if (vm == null)
            {
                // fix error stuff
            }
            string username = User.Identity.GetUserName();

            List<SelectListItem> catTempList = new List<SelectListItem>();
            catTempList.Add(new SelectListItem { Value = "1", Text = "Select a Category", Selected = true });

            List<SelectListItem> quartTempList = new List<SelectListItem>();
            quartTempList.Add(new SelectListItem { Value = "1", Text = "Select a Quarter", Selected = true });

            if (vm == null || !ModelState.IsValid || !User.Identity.IsAuthenticated || String.IsNullOrWhiteSpace(username))
            {
                // error
            }

            else
            {
                using (var db = new UserDBEntities())
                {
                    var user = db.Users.Where(x => x.Username == username).FirstOrDefault();
                    var department = db.Departments.Where(x => x.DID == user.DID);
                    int did = db.Users.Where(x => x.Username == username).FirstOrDefault().DID;
                    var cats = db.Categories.Where(x => x.DepID == did);
                    var quarts = db.Quarters.Where(x => x.DID == did);

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

                    //goal.Title = vm.Title;
                    //goal.Description = vm.Description; // not in DB yet
                    //goal.StartDate = DateTime.Now;
                    //goal.Progress = 0.00;
                 
                    goal = new Goal(vm.Title, vm.CategoryName, "Active", 0);

                    // populate UID FK in Goal table
                    goal.User = user as User; // null check?
                    goal.StartDate = DateTime.Now;

                    // Description
                    if (!String.IsNullOrWhiteSpace(vm.Description))
                    {
                        goal.Description = vm.Description;
                    }

                    // debugging, dummy info - DateTime objecs in a DDM will have to be serialized?
                    //goal.EndDate = vm.QuarterTime;
                    goal.EndDate = DateTime.Now;
                    goal.Category = vm.CategoryName;
                    //goal.Category = "";

                    update = new Update("Initial update", "Created goal", 0, goal.StartDate);

                    // populate GID FK in Update table
                    update.Goal = goal;

                    db.Goals.Add(goal);
                    db.Updates.Add(update);
                    db.SaveChanges();

                    var userSessionData = new UserSessionData
                    {
                        Username = user.Username,
                        Role = user.Role,
                        UID = user.UID,
                        Goals = db.Goals.Where(x => x.UID == user.UID).ToList<Goal>()
                    };
                    Session["UserSessionData"] = userSessionData;

                    return RedirectToAction("EmployeeHome", "Home");
                }               
            }

            var nvm = new CreateGoalViewModel();
            nvm.CatDropDown = catTempList;
            nvm.QuartDropDown = quartTempList;
            return View(nvm);
        }

        // UpdateGoal

        public ActionResult UpdateGoal()
        {
            var vm = new UpdateGoalViewModel();
            var userSessionData = Session["UserSessionData"] as UserSessionData;
            if (vm == null)
            {
                // TODO: Error
            }

            if (userSessionData == null)
            {
                // TODO: Error
            }

            // get selected Goal
            var goal = userSessionData.Goals.Find(x => x.GID == vm.GID);
            vm.Update = new Update
            {
                Goal = goal
            };

            return View(vm);
        }

        [HttpPost]
        public ActionResult UpdateGoal(UpdateGoalViewModel vm)
        {
            if (ModelState.IsValid)
            {
                vm.Update.Time = DateTime.Now;
                using (UserDBEntities db = new UserDBEntities())
                {
                    db.Updates.Add(vm.Update);
                    db.SaveChanges();
                }
                return RedirectToAction("MainView", "Home");
            }
            var nvm = new UpdateGoalViewModel();
            return View(vm);
        }

        // ViewGoal
        [Authorize]
        public ActionResult ViewGoal()
        {
            var userSessionData = Session["UserSessionData"] as UserSessionData;
            string userID = User.Identity.GetUserId();
            string username = User.Identity.GetUserName();

            if (userSessionData == null)
            {
                // TODO: Error
            }

            foreach (int key in userSessionData.UpdateDict.Keys)
            {
                // TODO: 
                List<Update> ups = userSessionData.UpdateDict[key];
            }

            if (String.IsNullOrWhiteSpace(userID) || String.IsNullOrWhiteSpace(username))
            {
                // error
            }
            
            else
            {
                using (UserDBEntities userDB = new UserDBEntities())
                {
                    var user = userDB.Users.Where(x => x.Username == username).FirstOrDefault();
                    var goals = userDB.Goals.Where(x => x.UID == user.UID);
                    foreach (Goal g in goals)
                    {

                    }
                }
            }

            ViewGoalViewModel vm = new ViewGoalViewModel();
            return View(vm);
        }

        [HttpPost]
        public ActionResult ViewGoal(int goalid) 
        {
            var vm = new ViewGoalViewModel();

            List<Update> tmpUpdates = new List<Update>();
            
            using (var db = new UserDBEntities())
            {
                vm.Goal = db.Goals.Where(x => x.GID == goalid).FirstOrDefault();


                vm.Updates.AddRange(db.Updates.Where(x => x.GID == vm.Goal.GID));
                
            }

            List<SelectListItem> catTempList = new List<SelectListItem>();
            catTempList.Add(new SelectListItem { Value = "1", Text = "Select a Category", Selected = true });

            //find user from the db
            //find goal from the db
            //find associated updates from the goal
            return View(vm);
        }

        // GET: Goal
        public ActionResult Index()
        {
            return View();
        }
    }
}