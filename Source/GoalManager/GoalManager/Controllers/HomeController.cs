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
                    vm.Departments = db.Departments.Where(x => x.SUID == userSessionData.UID).ToList();
<<<<<<< HEAD
                    if (vm.Departments != null)
                    {
                        foreach (Department d in vm.Departments)
                        {
                            // Users associated with this Supervisor
                            List<User> users = db.Users.Where(x => x.SUID == userSessionData.UID).ToList<User>();
                            foreach (User u in users)
                            {
                                // Goals associated with this User where Approved == false
                                List<Goal> goals = db.Goals.Where(x => x.UID == u.UID && !x.Approved).ToList<Goal>();
                                if (goals != null)
                                {
                                    // Add GIDs of associated Goals where Approved == false
                                    foreach (Goal g in goals)
                                    {
                                        // Either pass by reference or create new Goal
                                        Goal goal = new Data.Goal
                                        {
                                            GID = g.GID,
                                            Category = g.Category,
                                            Description = g.Description,
                                            StartDate = g.StartDate,
                                            EndDate = g.EndDate,
                                            Progress = g.Progress,
                                            Status = g.Status,
                                            Title = g.Title,
                                            UID = g.UID,
                                            Approved = g.Approved,
                                            User = new Data.User
                                            {
                                                FirstName = g.User.FirstName,
                                                LastName = g.User.LastName,
                                                Department = g.User.Department,
                                                Role = g.User.Role,
                                                Title = g.User.Title
                                            }
                                        };
                                        vm.GoalApprovalList.Add(goal);
                                    }
                                }
                            }
                        }
                    }
=======
                    
>>>>>>> refs/remotes/origin/master
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
                        return RedirectToAction("Index", "Home", new { exception = ex.Message});             
                }
                
            }

            catch(ArgumentNullException ex)
            {
                // Something went wrong
                return RedirectToAction("Index", "Home", new { exception = ex.Message });
            }

            catch(Exception ex)
            {
                // Something went wrong
                return RedirectToAction("Index", "Home", new { exception = ex.Message });
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