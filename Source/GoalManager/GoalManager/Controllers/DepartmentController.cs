using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GoalManager.Controllers
{
    public class DepartmentController : Controller
    {
        // GET: Department
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult CreateDepartment()
        {
            return View();
        }
        public ActionResult ModifyDepartment()
        {
            return View();
        }
    }
}