using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GoalManager.Models;
using GoalManager.Data;
using Microsoft.AspNet.Identity;

namespace GoalManager.Controllers
{
    public class HomeController : Controller
    {
        [Authorize]
        public ActionResult EmployeeHome()
        {
            var userSessionData = Session["UserSessionData"] as UserSessionData;
            if (userSessionData == null)
            {
                // error;
                return RedirectToAction("Index", "Home");
            }

            var vm = new EmployeeHomeViewModel();
            vm.Goals = userSessionData.Goals;

            return View(vm);
        }

        [Authorize]
        public ActionResult SupervisorHome()
        {
            return View();
        }

        [Authorize]
        public ActionResult AdminHome()
        {
            var vm = new AdminHomeViewModel();
            using (var db = new UserDBEntities())
            {
                vm.Departments.AddRange(db.Departments);
                vm.Employees.AddRange(db.Users);
            }
            return View(vm);
        }

        [Authorize]
        public ActionResult MainView()
        {
            ViewBag.Message = "View for specific Role.";
            try
            {
                var username = User.Identity.GetUserName();
                var id = User.Identity.GetUserName();
                if (String.IsNullOrWhiteSpace(username) || String.IsNullOrWhiteSpace(id))
                {
                    throw new ArgumentNullException($"Null, empty, or whitespace Username:{username} or ID:{id}");
                }

                using (UserDBEntities db = new UserDBEntities())
                {
                    var user = db.Users.Where(x => x.Username.Equals(username)).FirstOrDefault();

                    if (user == null)
                    {
                        throw new ArgumentNullException($"Null User field.");
                    }
                    
                    // Collect properties for UserSessionData
                    int uid = user.UID;
                    int did = user.DID;
                    string role = user.Role;
                    var goals = db.Goals.Where(x => x.UID == user.UID);

                    // UserSessionData property Dictionary<int,List<Update>> UpdateDict
                    Dictionary<int, List<Update>> dict = new Dictionary<int, List<Update>>();
                    foreach (Goal g in goals)
                    {
                        dict.Add(g.GID, g.Updates.ToList<Update>());
                    }

                    var userSessionData = new UserSessionData
                    {
                        Username = username,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Role = role,
                        UID = uid,
                        DID = did,
                        Goals = goals.ToList<Goal>(),
                        UpdateDict = dict                 
                    };

                    // store newly-logged in user's profile
                    Session["UserSessionData"] = userSessionData;

                    switch (user.Role)
                    {
                        case ("Employee"):
                            return RedirectToAction("EmployeeHome");
                        case ("Supervisor"):
                            return RedirectToAction("SupervisorHome");
                        case ("Administrator"):
                            return RedirectToAction("AdminHome");
                        default:
                            // Invalid Role 
                            Exception ex = new Exception($"Invalid Role for User {user.Username}, UID:{user.UID}.");
                            Session.Clear(); // if failed, clean Session 
                            return RedirectToAction("Index", "Home", new { exception = ex.Message});             
                    }
                }
            }

            catch(ArgumentNullException ex)
            {
                // Something went wrong
                return RedirectToAction("Index", "Home", new { exception = ex.Message });
            }
        }

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }
    }
}