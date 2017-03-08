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

            var vm = new CreateGoalViewModel();

            List<SelectListItem> catTempList = new List<SelectListItem>();
            catTempList.Add(new SelectListItem { Value = "1", Text = "Select a Category", Selected = true });

            List<SelectListItem> quartTempList = new List<SelectListItem>();
            quartTempList.Add(new SelectListItem { Value = "1", Text = "Select a Quarter", Selected = true });

            using (var db = new UserDBEntities())
            {
                //pulling categories
                var cats = db.Categories;
                foreach (Category c in cats)
                {
                    catTempList.Add(new SelectListItem { Value = c.CatID.ToString(), Text = c.Name, Selected = false });
                }

                var quarts = db.Quarters;
                foreach (Quarter q in quarts)
                {
                    quartTempList.Add(new SelectListItem { Value = q.QID.ToString(), Text = q.Name, Selected = false });
                }



            }

            vm.CatDropDown = catTempList;
            vm.QuartDropDown = quartTempList;
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
        public ActionResult ViewGoal(int goalid) 
        {
            var vm = new ViewGoalViewModel();

            List<Update> tmpUpdates = new List<Update>();
            
            using (var db = new UserDBEntities())
            {
                vm.Goal = db.Goals.Where(x => x.GID == goalid).FirstOrDefault();


                vm.Updates.AddRange(db.Updates.Where(x => x.GID == vm.Goal.GID));
                
            }

            List<SelectListItem> catTempList = new List<SelectListItem>();
            catTempList.Add(new SelectListItem { Value = "1", Text = "Select a Category", Selected = true });

            //find user from the db
            //find goal from the db
            //find associated updates from the goal
            return View(vm);
        }

    }
}