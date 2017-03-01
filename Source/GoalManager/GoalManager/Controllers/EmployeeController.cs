using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GoalManager.Models;
using GoalManager.Data;

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
            var vm = new CreateEmployeeViewModel();
            return View(vm);

        }
        [HttpPost]
        public ActionResult CreateEmployee(User tempuser)
        {
            var tpdbuser = new User(); //optional
            if (ModelState.IsValid == true)
            {
                using (var db = new UserDBEntities())
                {
                    db.Users.Add(tpdbuser);
                    db.SaveChangesAsync();
                }
                RedirectToAction("/Home/Index");
            }
            // 
            
            CreateEmployeeViewModel vm = new CreateEmployeeViewModel();
            vm.Employee = tempuser;
            return View(vm);
        }
        [HttpPost]
        public ActionResult ModifyEmployee(int ID) //Pass Employee ID to be Displayed
        {

            var vm = new ModifyEmployeeViewModel();
            //using Db, search where ID equals the Employeee ID
            return View(vm);
        }
        [HttpPost]
        public ActionResult ModifyEmployee(User tempuser)
        {

            var tpdbuser = new User(); //optional
            //Validate form fields here.

            if (ModelState.IsValid == true)
            {
                using (var db = new UserDBEntities())
                {
                    //Find user, save change
                    db.SaveChangesAsync();
                }
                RedirectToAction("/Home/Index");
            }

            var vm = new ModifyEmployeeViewModel();
            return View(vm);
        }
    }
}