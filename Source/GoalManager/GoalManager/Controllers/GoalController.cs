﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GoalManager.Models;
using GoalManager.Data;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace GoalManager.Controllers
{
    public class GoalController : Controller
    {
        // GET: Goal
        public ActionResult Index()
        {
            return View();
        }

        // CreateGoal
        // [Authorize]
        public ActionResult CreateGoal()
        {
            var vm = new CreateGoalViewModel();
            string username = User.Identity.GetUserName();

            List<SelectListItem> catTempList = new List<SelectListItem>();
            catTempList.Add(new SelectListItem { Value = "1", Text = "Select a Category", Selected = true });

            List<SelectListItem> quartTempList = new List<SelectListItem>();
            quartTempList.Add(new SelectListItem { Value = "1", Text = "Select a Quarter", Selected = true });

            using (var db = new UserDBEntities())
            {
                if (String.IsNullOrWhiteSpace(username))
                {
                    // error
                }

                else
                {
                    int did = db.Users.Where(x => x.Username == username).FirstOrDefault().DID;
                    var cats = db.Categories.Where(x => x.DepID == did);
                    var quarts = db.Quarters.Where(x => x.DID == did);

                    foreach (Category c in cats)
                    {
                        catTempList.Add(new SelectListItem { Value = c.Name, Text = c.Name, Selected = false });
                    }

                    foreach (Quarter q in quarts)
                    {
                        quartTempList.Add(new SelectListItem { Value = q.Name, Text = q.Name, Selected = false });
                    }
                }
            }

            vm.CatDropDown = catTempList;
            vm.QuartDropDown = quartTempList;
            return View(vm);
        }

        // new HttpPost for CreateGoal
        [HttpPost]
        public ActionResult CreateGoal(CreateGoalViewModel vm)
        {
            if (vm == null)
            {
                // fix error stuff
            }
            string username = User.Identity.GetUserName();

            List<SelectListItem> catTempList = new List<SelectListItem>();
            catTempList.Add(new SelectListItem { Value = "1", Text = "Select a Category", Selected = true });

            List<SelectListItem> quartTempList = new List<SelectListItem>();
            quartTempList.Add(new SelectListItem { Value = "1", Text = "Select a Quarter", Selected = true });

            if (vm == null || !ModelState.IsValid || !User.Identity.IsAuthenticated || String.IsNullOrWhiteSpace(username))
            {
                // error
            }

            else
            {
                using (var db = new UserDBEntities())
                {
                    var user = db.Users.Where(x => x.Username == username).FirstOrDefault();
                    var department = db.Departments.Where(x => x.DID == user.DID);
                    int did = db.Users.Where(x => x.Username == username).FirstOrDefault().DID;
                    var cats = db.Categories.Where(x => x.DepID == did);
                    var quarts = db.Quarters.Where(x => x.DID == did);

                    foreach (Category c in cats)
                    {
                        catTempList.Add(new SelectListItem { Value = c.Name, Text = c.Name, Selected = false });
                    }

                    foreach (Quarter q in quarts)
                    {
                        quartTempList.Add(new SelectListItem { Value = q.Name, Text = q.Name, Selected = false });
                    }

                    Goal goal;
                    Update update;

                    //goal.Title = vm.Title;
                    //goal.Description = vm.Description; // not in DB yet
                    //goal.StartDate = DateTime.Now;
                    //goal.Progress = 0.00;
                 
                    goal = new Goal(vm.Title, vm.CategoryName, "Active", 0);

                    // populate UID FK in Goal table
                    goal.User = user as User; // null check?
                    goal.StartDate = DateTime.Now;

                    // Description
                    if (!String.IsNullOrWhiteSpace(vm.Description))
                    {
                        goal.Description = vm.Description;
                    }

                    // debugging, dummy info - DateTime objecs in a DDM will have to be serialized?
                    //goal.EndDate = vm.QuarterTime;
                    goal.EndDate = DateTime.Now;
                    goal.Category = vm.CategoryName;
                    //goal.Category = "";

                    update = new Update("Initial update", "Created goal", 0, goal.StartDate);

                    // populate GID FK in Update table
                    update.Goal = goal;

                    db.Goals.Add(goal);
                    db.Updates.Add(update);
                    db.SaveChanges();

                    return RedirectToAction("EmployeeHome", "Home");
                }               
            }

            var nvm = new CreateGoalViewModel();
            nvm.CatDropDown = catTempList;
            nvm.QuartDropDown = quartTempList;
            return View(nvm);
        }

        //[HttpPost]
        //public ActionResult CreateGoal(Goal tmpGoal)
        //{
        //    var dbGoal = new Goal(); //optional
        //    if (ModelState.IsValid == true)
        //    {
        //        using (var db = new UserDBEntities())
        //        {
        //            db.Goals.Add(dbGoal);
        //            db.SaveChangesAsync();
        //        }
        //        RedirectToAction("~/Home/Index");
        //    }
        //    var vm = new CreateGoalViewModel();
        //    // create dropdown for category 
        //    // creat dropdown for quarter
        //    vm.Goal = tmpGoal;
        //    return View(vm);
        //}

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


        // ViewGoal
        //[Authorize]
        public ActionResult ViewGoal()
        {
            string userID = User.Identity.GetUserId();
            string username = User.Identity.GetUserName();
            if (String.IsNullOrWhiteSpace(userID) || String.IsNullOrWhiteSpace(username))
            {
                // error
            }
            
            else
            {
                using (UserDBEntities userDB = new UserDBEntities())
                {
                    var user = userDB.Users.Where(x => x.Username == username).FirstOrDefault();
                    var goals = userDB.Goals.Where(x => x.UID == user.UID);
                    foreach (Goal g in goals)
                    {

                    }
                }
            }


            ViewGoalViewModel vm = new ViewGoalViewModel();
            return View(vm);
        }

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