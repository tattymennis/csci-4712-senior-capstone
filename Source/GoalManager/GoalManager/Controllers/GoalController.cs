using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GoalManager.Models;
using GoalManager.Data;






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
            var vm = new CreateGoalViewModel();
            return View(vm);
        }

        [HttpPost]
        public ActionResult CreateGoal(Goal tmpGoal)
        {
            var dbGoal = new Goal(); //optional
            if (ModelState.IsValid == true)
            {
                using (var db = new UserDBEntities())
                {
                    db.Goals.Add(dbGoal);
                    db.SaveChangesAsync();
                }
                RedirectToAction("/Home/Index");
            }
            var vm = new CreateGoalViewModel();
            vm.Goal = tmpGoal;
            return View(vm);
        }
        public ActionResult UpdateGoal()
        {

            var vm = new UpdateGoalViewModel();
            return View(vm);
        }
        [HttpPost]
        public ActionResult UpdateGoal(Goal tmpUpdate)
        {
            var dbUpdate = new Goal();
            if (ModelState.IsValid == true)
            {
                using (var db = new UserDBEntities())
                {
                    db.Goals.Add(dbUpdate); //add if approved
                     //save for now until approved
                }
                RedirectToAction("/Home/Index");
            }

            var vm = new UpdateGoalViewModel();
            vm.Update = tmpUpdate;
            return View(vm);
        }
        public ActionResult ViewGoal()
        {
            return View();
        }
    }
}