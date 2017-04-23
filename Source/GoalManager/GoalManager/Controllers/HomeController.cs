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
            ViewBag.Title = "Employee Home Page";
            try
            {
                var userSessionData = Session["UserSessionData"] as UserSessionData;
                if (userSessionData == null || userSessionData.Role != "Employee")
                {
                    return RedirectToAction("MainView", "Home");
                }

                var vm = new EmployeeHomeViewModel();
                using (UserDBEntities db = new UserDBEntities())
                {
                    vm.Goals = db.Goals.Where(x => x.UID == userSessionData.UID).ToList<Goal>();
                }
                foreach (var Goal in vm.Goals)
                {
                    if (Goal.Status == "Pending")
                    { vm.PendingGoals.Add(Goal); }
                    else if (Goal.Status == "Active")
                    { vm.ActiveGoals.Add(Goal); }
                    else if (Goal.Status == "Failed")
                    { vm.FailedGoals.Add(Goal); }
                    else if (Goal.Status == "Denied")
                    { vm.DeniedGoals.Add(Goal); }
                    else if (Goal.Status == "Completed")
                    { vm.CompletedGoals.Add(Goal); }
                }
                foreach (var Goal in vm.ActiveGoals)
                {
                    if (DateTime.Compare(DateTime.Now, Goal.EndDate) > 0)
                    { Goal.Status = "Failed"; }
                }
                return View(vm);
            }

            catch (ArgumentNullException ex)
            {
                // Something went wrong
                return RedirectToAction("MainView", "Home", new { exception = ex.Message });
            }

            catch (Exception ex)
            {
                // Something went wrong
                return RedirectToAction("MainView", "Home", new { exception = ex.Message });
            }
        }

        [Authorize]
        public ActionResult SupervisorHome()
        {
            var vm = new SupervisorHomeViewModel();
            try
            {
                var userSessionData = Session["UserSessionData"] as UserSessionData;
                if (userSessionData == null || userSessionData.Role != "Supervisor")
                {
                    // Session data is null or incorrect Role; return to MainView
                    return RedirectToAction("MainView", "Home");
                }

                // Query list of GIDs that require approval & Departments associated with this Supervisor
                using (var db = new UserDBEntities())
                {
                    // Populate Supervisor's Goals in VM
                    vm.Goals = db.Goals.Where(x => x.UID == userSessionData.UID).ToList<Goal>();
                    vm.Departments = db.Departments.Where(x => x.SUID == userSessionData.UID).ToList();
                    if (vm.Departments != null)
                    {
                        // Users associated with this Supervisor
                        vm.Employees = db.Users.Where(x => x.SUID == userSessionData.UID).ToList<User>();
                        foreach (User u in vm.Employees)
                        {
                            // Goals associated with this User where Approved == false && Status == "Pending"
                            List<Goal> goals = db.Goals.Where(x => x.UID == u.UID &&
                                                              x.Approved == false &&
                                                              x.Status == "Pending")
                                                              .ToList<Goal>();
                            if (goals != null)
                            {
                                // Add GIDs of associated Goals where Approved == false
                                foreach (Goal g in goals)
                                { vm.GoalApprovalList.Add(g); }

                                // Assign each Employee's Department Name into Dictionary
                                vm.EmployeeDeptName[u.UID] = db.Departments.
                                    Where(d => d.DID == u.DID).
                                    FirstOrDefault().Name;
                            }
                        }
                    }

                    vm.Goals = db.Goals.Where(x => x.UID == userSessionData.UID).ToList<Goal>();
                    foreach (var Goal in vm.Goals)
                    {
                        if (DateTime.Compare(DateTime.Now, Goal.EndDate) > 0)
                        { Goal.Status = "Failed"; }

                        if (Goal.Status == "Pending")
                        { vm.PendingGoals.Add(Goal); }
                        else if (Goal.Status == "Active")
                        { vm.ActiveGoals.Add(Goal); }
                        else if (Goal.Status == "Failed")
                        { vm.FailedGoals.Add(Goal); }
                        else if (Goal.Status == "Denied")
                        { vm.DeniedGoals.Add(Goal); }
                        else if (Goal.Status == "Completed")
                        { vm.CompletedGoals.Add(Goal); }
                    }
                }
                return View(vm);
            }

            catch (ArgumentNullException ex)
            {
                // Something went wrong
                return RedirectToAction("MainView", "Home", new { exception = ex.Message });
            }

            catch (Exception ex)
            {
                // Something went wrong
                return RedirectToAction("Index", "Home", new { exception = ex.Message });
            }
        }

        [Authorize]
        public ActionResult AdminHome()
        {
            ViewBag.Title = "Administrator Home Page";
            try
            {
                var userSessionData = Session["UserSessionData"] as UserSessionData;
                if (userSessionData == null || userSessionData.Role != "Administrator")
                {
                    // Session data is null or incorrect Role; return to MainView
                    return RedirectToAction("MainView", "Home");
                }
            }

            catch (ArgumentNullException ex)
            {
                // Something went wrong
                return RedirectToAction("MainView", "Home", new { exception = ex.Message });
            }

            catch (Exception ex)
            {
                // Something went wrong
                return RedirectToAction("MainView", "Home", new { exception = ex.Message });
            }

            var vm = new AdminHomeViewModel();
            using (var db = new UserDBEntities())
            {
                vm.Departments.AddRange(db.Departments);
                vm.Employees.AddRange(db.Users);
                vm.Employees = db.Users.Where(u => !u.Role.Equals("Administrator")).ToList<User>();
                vm.Administrators = db.Users.Where(u => u.Role.Equals("Administrator")).ToList<User>();
                foreach (Department d in vm.Departments)
                {
                    User super = db.Users.Where(u => u.UID == d.SUID).FirstOrDefault();
                    vm.SupervisorNames[super.UID] = super.FirstName + " " + super.LastName;
                }
            }
            return View(vm);
        }

        [Authorize]
        public ActionResult MainView()
        {
            ViewBag.Title = "Main View";
            try
            {
                if (!User.Identity.IsAuthenticated)
                {
                    // not logged in
                    return RedirectToAction("Index", "Home");
                }

                var username = User.Identity.GetUserName();
                var id = User.Identity.GetUserId();

                if (String.IsNullOrWhiteSpace(username) || String.IsNullOrWhiteSpace(id))
                {
                    throw new ArgumentNullException($"Null, empty, or whitespace Username:{username} or ID:{id}");
                }

                UserSessionData userSessionData = new UserSessionData();

                using (UserDBEntities db = new UserDBEntities())
                {
                    var user = db.Users.Where(x => x.Username.Equals(username)).FirstOrDefault();

                    if (user == null)
                    {
                        throw new ArgumentNullException($"Null User field.");
                    }

                    if (user.Username != username)
                    {
                        // problems
                    }

                    // Collect properties for UserSessionData
                    var dept = user.Department as Department;
                    var goals = db.Goals.Where(x => x.UID == user.UID).ToList<Goal>();
                    var cats = dept.Categories.ToList<Category>();
                    var quarts = dept.Quarters.ToList<Quarter>();
                    string role = user.Role;

                    // UserSessionData property Dictionary<int,List<Update>> UpdateDict
                    // Contains every Goal associated w/ User as key, and List of all Updates associated with each Goal as val
                    Dictionary<int, List<Update>> dict = new Dictionary<int, List<Update>>();
                    foreach (Goal g in goals)
                    {
                        dict.Add(g.GID, db.Updates.Where(x => x.GID == g.GID).ToList<Update>());
                    }

                    userSessionData = new UserSessionData
                    {
                        UID = user.UID,
                        DID = dept.DID,
                        DeptName = dept.Name,
                        Username = username,
                        Role = role,
                        Goals = goals,
                        GoalUpdatesTable = dict
                    };

                    // store newly-logged in user's profile
                    Session["UserSessionData"] = userSessionData;
                }

                switch (userSessionData.Role)
                {
                    case ("Employee"):
                        return RedirectToAction("EmployeeHome");
                    case ("Supervisor"):
                        return RedirectToAction("SupervisorHome");
                    case ("Administrator"):
                        return RedirectToAction("AdminHome");
                    default:
                        // Invalid Role 
                        Exception ex = new Exception($"Invalid Role for Role: {userSessionData.Role}");
                        Session.Clear(); // if failed, clean Session 
                        return RedirectToAction("Index", "Home", new { exception = ex.Message });
                }

            }

            catch (ArgumentNullException ex)
            {
                TempData["ErrorMessage"] = "Invalid session data. " + ex.Message;
                return RedirectToAction("Index", "Home");
            }

            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Invalid session data. " + ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult Index()
        {
            ViewBag.Title = "Home Page - Index";
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