using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GoalManager.Controllers
{
    public class EmployeeController : Controller
    {
        // GET: Employee
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult CreateEmployee()
        {
            return View();
        }
        public ActionResult ModifyEmployee()
        {
            return View();
        }
    }
}