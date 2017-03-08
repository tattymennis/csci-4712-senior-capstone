using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GoalManager.Models;
using GoalManager.Data;

namespace GoalManager.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult EmployeeHome()
        {
            var vm = new EmployeeHomeViewModel();
            using (var db = new UserDBEntities())
            {
                vm.Goals.AddRange(db.Goals);
            }
                return View(vm);
        }
        public ActionResult SupervisorHome()
        {
            return View();
        }
        public ActionResult AdminHome()
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

        public ActionResult MainView()
        {
            ViewBag.Message = "View for specific Role.";

            return View();
        }
    }
}