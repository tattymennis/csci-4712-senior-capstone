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
        // GET: Report
        [Authorize]
        public ActionResult ViewDepartmentReport(int DeptRefID)
        {
            ViewBag.Title = "Department Report";
            ViewDepartmentReportViewModel vm = new ViewDepartmentReportViewModel();
            vm.DeptRefID = DeptRefID;

            try
            {
                var userSessionData = Session["UserSessionData"] as UserSessionData;
                if (userSessionData == null)
                {
                    // error
                }

                if (vm.DeptRefID == -1)
                {
                    // error
                }

                using (UserDBEntities db = new UserDBEntities())
                {
                    // TODO: Expand LINQ query to check Supervisor's UID/SUID
                    vm.Department = db.Departments.Where(x => x.DID == vm.DeptRefID).FirstOrDefault();
                    // TODO: Expand LINQ query to check Supervisor's UID/SUID
                    vm.Employees = db.Users.Where(x => x.DID == vm.DeptRefID).ToList<User>();
                    if (vm.Employees.Count != 0)
                    {
                        // Get each managed Department's Employee's Goals
                        foreach (User u in vm.Employees)
                        {
                            vm.EmployeeGoals[u.UID] = db.Goals.Where(x => x.UID == u.UID).ToList<Goal>();
                            if (vm.EmployeeGoals[u.UID] != null)
                            {
                                // Get each Employee's Goal's Updates
                                foreach (Goal g in vm.EmployeeGoals[u.UID])
                                {
                                    vm.GoalUpdates[g.GID] = db.Updates.Where(x => x.GID == g.GID).ToList<Update>();
                                }
                            }
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                // do work
            }

            return View(vm);
        }

        [Authorize]
        [HttpPost]
        public ActionResult ViewDepartmentReport(ViewDepartmentReportViewModel vm)
        {
            ViewBag.Title = "Department Report";
            try
            {
                var userSessionData = Session["UserSessionData"] as UserSessionData;
                if (userSessionData == null)
                {
                    // error
                }

                if (vm.DeptRefID == -1)
                {
                    // error
                }

                using (UserDBEntities db = new UserDBEntities())
                {
                    // TODO: Expand LINQ query to check Supervisor's UID/SUID
                    vm.Department = db.Departments.Where(x => x.DID == vm.DeptRefID).FirstOrDefault();
                    // TODO: Expand LINQ query to check Supervisor's UID/SUID
                    vm.Employees = db.Users.Where(x => x.DID == vm.DeptRefID).ToList<User>();
                    if (vm.Employees.Count != 0)
                    {
                        // Get each managed Department's Employee's Goals
                        foreach (User u in vm.Employees)
                        {
                            vm.EmployeeGoals[u.UID] = db.Goals.Where(x => x.UID == u.UID).ToList<Goal>();
                            if (vm.EmployeeGoals[u.UID] != null)
                            {
                                // Get each Employee's Goal's Updates
                                foreach (Goal g in vm.EmployeeGoals[u.UID])
                                {
                                    vm.GoalUpdates[g.GID] = db.Updates.Where(x => x.GID == g.GID).ToList<Update>();
                                }
                            }
                        }
                    }
                }
            }

            catch(Exception ex)
            {
                // do work
            }

            ViewDepartmentReportViewModel nvm = new ViewDepartmentReportViewModel();
            return View(nvm);
        }

        public ActionResult ViewEmployeeReport()
        {
            ViewBag.Title = "Employee Report";
            return View();
        }

        public ActionResult Index()
        {
            return View();
        }
    }
}