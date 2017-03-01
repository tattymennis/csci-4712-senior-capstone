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
        // GET: Department
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult CreateDepartment()
        {
            var vm = new DepartmentViewModel();
            return View(vm);
        }
        [HttpPost]
        public ActionResult CreateDepartment(Department tempDep)
        {
            /* if(tempDep.Location == "")
            {
                ModelState.AddModelError("Location", "Location cannnt be empty!"); //wrong
            }
            ModelState.AddModelError("Location", "Location Cannnt be empadf;ladf!"); //wrong
            */

            var tDBDep = new Department(); //optional
            // if ModelState.isValid()
            using (var db = new UserDBEntities())
            {
                db.Departments.Add(tDBDep);
                db.SaveChangesAsync();
            }
            // 
            RedirectToAction("/Home/Index");
            DepartmentViewModel vm = new DepartmentViewModel();
            vm.Department = tempDep;
            return View(vm);
        }
        public ActionResult ModifyDepartment()
        {
            return View();
        }
    }
}