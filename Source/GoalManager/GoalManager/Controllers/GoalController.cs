using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GoalManager.Controllers
{
    public class GoalController : Controller
    {
        // GET: Goal
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult CreateGoal()
        {
            return View();
        }
        public ActionResult UpdateGoal()
        {
            return View();
        }
        public ActionResult ViewGoal()
        {
            return View();
        }
    }
}