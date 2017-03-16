using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GoalManager.Controllers
{
    public class ReportController : Controller
    {
        // GET: Report
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ViewDepartmentReport()
        {
            ViewBag.Title = "Department Report";
            return View();
        }
        public ActionResult ViewEmployeeReport()
        {
            ViewBag.Title = "Employee Report";
            return View();
        }
    }
}