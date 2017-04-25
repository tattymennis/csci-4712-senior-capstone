using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GoalManager.Models;
using GoalManager.Data;

namespace GoalManager.Controllers
{
    public class ReportController : Controller
    {
        [Authorize]
        [HttpPost]
        public ActionResult ViewDepartmentReport(int DeptRefID)
        {
            ViewBag.Title = "Department Report";
            ViewDepartmentReportViewModel vm = new ViewDepartmentReportViewModel();

            try
            {
                var userSessionData = Session["UserSessionData"] as UserSessionData;
                if (userSessionData == null)
                {
                    throw new ArgumentNullException("userSessionData", "Null session cookie");
                }

                if (userSessionData.Role != "Supervisor")
                {
                    throw new ArgumentException("Invalid user role", "userSessionData.Role");
                }


                using (UserDBEntities db = new UserDBEntities())
                {
                    vm.Supervisor = db.Users.Where(u => u.UID == userSessionData.UID).FirstOrDefault();
                    // TODO: Expand LINQ query to check Supervisor's UID/SUID
                    vm.Department = db.Departments.Where(x => x.DID == DeptRefID).FirstOrDefault();
                    // TODO: Expand LINQ query to check Supervisor's UID/SUID
                    vm.Employees = db.Users.Where(x => x.DID == DeptRefID).ToList<User>();
                    vm.TotalActiveGoals = 0;
                    vm.TotalCompletedGoals = 0;
                    vm.TotalDeniedGoals = 0;
                    vm.TotalFailedGoals = 0;
                    int active = 0;
                    int completed = 0;
                    int denied = 0;
                    int failed = 0;
                    if (vm.Employees.Count != 0)
                    {
                        // Get each managed Department's Employee's Goals
                        foreach (User u in vm.Employees)
                        {
                            List<Goal> goals = db.Goals.Where(x => x.UID == u.UID).ToList<Goal>();
                            vm.EmployeeGoals[u.UID] = goals;
                            if (vm.EmployeeGoals[u.UID] != null)
                            {
                                active = 0; completed = 0; denied = 0; failed = 0;
                                // Get each Employee's Goal's Updates
                                foreach (Goal g in vm.EmployeeGoals[u.UID])
                                {
                                    vm.GoalUpdates[g.GID] = db.Updates.Where(x => x.GID == g.GID).ToList<Update>();
                                    if (g.Status == "Completed")
                                    {
                                        completed++;
                                        vm.TotalCompletedGoals++;
                                    }
                                        
                                    else if (g.Status == "Denied")
                                    {
                                        denied++;
                                        vm.TotalDeniedGoals++;
                                    }                                    
                                    else if (g.Status == "Failed")
                                    {
                                        failed++;
                                        vm.TotalFailedGoals++;
                                    }
                                        
                                    else if (g.Status == "Active")
                                    {
                                        active++;
                                        vm.TotalActiveGoals++;
                                    }
                                        
                                }
                                // Tuple :: <FirstName, LastName, Active, Completed, Denied, Failed>
                                vm.EmployeeGoalCounts[u.UID] = 
                                    new Tuple<string,string,int,int,int,int>(
                                            u.FirstName, u.LastName, active,
                                            completed, denied,
                                            failed);
                            }
                        }
                    }
                }

            }

            catch (ArgumentNullException ex)
            {
                TempData["ErrorMessage"] = "Invalid session data. " + ex.Message;
                return RedirectToAction("MainView", "Home");
            }

            catch (ArgumentException ex)
            {
                TempData["ErrorMessage"] = "Invalid session data. " + ex.Message;
                return RedirectToAction("MainView", "Home");
            }

            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Invalid user role. " + ex.Message;
                return RedirectToAction("MainView", "Home");
            }

            return View(vm);
        }

        [HttpPost]
        //[Authorize]
        public ActionResult ViewEmployeeReport(int EmployeeRefID)
        {
            ViewBag.Title = "Employee Report";
            ViewEmployeeReportViewModel vm = new ViewEmployeeReportViewModel();

            try
            {
                UserSessionData userSessionData = Session["UserSessionData"] as UserSessionData;
                if (userSessionData == null)
                {
                    throw new ArgumentNullException();
                }

                if (userSessionData.Role != "Supervisor")
                {
                    return RedirectToAction("MainView", "Home");
                }

                using (UserDBEntities db = new UserDBEntities())
                {
                    var user = db.Users.Where(u => u.UID == EmployeeRefID).FirstOrDefault();
                    if (user == null)
                    {
                        throw new ArgumentNullException();
                    }

                    var goals = db.Goals.Where(g => g.UID == user.UID).ToList<Goal>();
                    foreach (Goal g in goals)
                    {
                        var updates = db.Updates.Where(u => u.GID == g.GID).ToList<Update>();
                        vm.EmployeeUpdates[g.GID] = updates;

                        if (g.Status == "Pending")
                            vm.PendingGoals.Add(g);
                        else if (g.Status == "Denied")
                            vm.DeniedGoals.Add(g);
                        else if (g.Status == "Failed")
                            vm.FailedGoals.Add(g);
                        else if (g.Status == "Active")
                            vm.ActiveGoals.Add(g);
                        else if (g.Status == "Completed")
                            vm.CompletedGoals.Add(g);
                    }
                    vm.Employee = user;
                    vm.Department = db.Departments.Where(d => d.DID == vm.Employee.DID).FirstOrDefault();
                }

                return View(vm);
            }

            catch (ArgumentNullException ex)
            {
                TempData["ErrorMessage"] = "Invalid session data. " + ex.Message;
                return RedirectToAction("MainView", "Home");
            }

            catch (ArgumentException ex)
            {
                TempData["ErrorMessage"] = "Invalid session data. " + ex.Message;
                return RedirectToAction("MainView", "Home");
            }

            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Invalid user role. " + ex.Message;
                return RedirectToAction("MainView", "Home");
            }
        }
    }
}