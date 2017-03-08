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




        ////////CreateGoal


        public ActionResult CreateGoal()
        {
            // must get logged in user Role and Department to pull drop down list of Categories + Quarters
            var vm = new CreateGoalViewModel();
            using (var db = new UserDBEntities())
            {
                // create dropdown for category 
                // creat dropdown for quarter 
            }
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
                RedirectToAction("~/Home/Index");
            }
            var vm = new CreateGoalViewModel();
            // create dropdown for category 
            // creat dropdown for quarter
            vm.Goal = tmpGoal;
            return View(vm);
        }


        //////UpdateGoal


        public ActionResult UpdateGoal()
        {

            var vm = new UpdateGoalViewModel();
            return View(vm);
        }
        [HttpPost]
        public ActionResult UpdateGoal( Update tmpUpdate)
        {
            var dbUpdate = new Goal();
            if (ModelState.IsValid == true)
            {
                using (var db = new UserDBEntities())
                {
                    db.Goals.Add(dbUpdate); //add if approved
                     //save for now until approved
                }
                RedirectToAction("~/Home/Index");
            }

            var vm = new UpdateGoalViewModel();
            vm.Update = tmpUpdate;
            return View(vm);
        }


        ///ViewGoal
        ///
        [HttpPost]
        public ActionResult ViewGoal(int ID) 
        {
            var vm = new ViewGoalViewModel();
            //find user from the db
            //find goal from the db
            //find associated updates from the goal
            return View(vm);
        }

    }
}