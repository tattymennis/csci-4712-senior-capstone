using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GoalManager.Data;
using GoalManager.Models;

namespace GoalManager.Controllers
{
    public class DepartmentController : Controller
    {
        public ActionResult CreateDepartment()
        {
            ViewBag.Title = "Create Department";
            var vm = new CreateDepartmentViewModel();
            return View(vm);
        }

        [HttpPost]
        public ActionResult CreateDepartment(Department tempDep)
        {
            ViewBag.Title = "Create Department";

            var tDBDep = new Department(); //optional
            if (ModelState.IsValid == true)
            {
                using (var db = new UserDBEntities())
                {
                    db.Departments.Add(tDBDep);
                    db.SaveChangesAsync();
                }
            }
            // 
            RedirectToAction("/Home/Index");
            CreateDepartmentViewModel vm = new CreateDepartmentViewModel();
            vm.Department = tempDep;
            return View(vm);
        }
        public ActionResult ModifyDepartment()
        {
            ViewBag.Title = "Modify Department";
            return View();
        }
    }
}