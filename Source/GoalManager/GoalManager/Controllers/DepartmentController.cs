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
            var vm = new CreateDepartmentViewModel();
            ModelState.Clear();
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateDepartment(CreateDepartmentViewModel vm)
        {
            if (ModelState.IsValid)
            {
                //Name 
                foreach (char x in vm.Name)
                {
                    if (Char.IsControl(x) || Char.IsPunctuation(x) || Char.IsSymbol(x))
                    {
                        ModelState.AddModelError("Name", "Name can only be letters and numbers");
                        break;
                    }
                }
                //Location
                foreach (char x in vm.Location)
                {
                    if (Char.IsControl(x) || Char.IsPunctuation(x) || Char.IsSymbol(x))
                    {
                        ModelState.AddModelError("Location", "Location can only be letters and numbers");
                        break;
                    }
                }
                //Description
                foreach (char x in vm.Description)
                {
                    if (Char.IsControl(x) || Char.IsSymbol(x))
                    {
                        ModelState.AddModelError("Description", "You have invlaid symbols in your Description");
                        break;
                    }
                }
                if (DateTime.Compare(vm.Quarter1Start, vm.Quarter1End) > 0)
                    ModelState.AddModelError("Quarter 1", "Quarter 1 End Date can not be Before Quarter 1 Start Date");
                else if (DateTime.Compare(vm.Quarter1Start, vm.Quarter1End) == 0)
                    ModelState.AddModelError("Quarter 1", "Quarter 1 End Date can not be the same as Quarter 1 Start Date");
                else { }
                if (DateTime.Compare(vm.Quarter2Start, vm.Quarter2End) > 0)
                    ModelState.AddModelError("Quarter 2", "Quarter 2 End Date can not be Before Quarter 2 Start Date");
                else if (DateTime.Compare(vm.Quarter2Start, vm.Quarter2End) == 0)
                    ModelState.AddModelError("Quarter 2", "Quarter 2 End Date can not be the same as Quarter 2 Start Date");
                else { }
                if (DateTime.Compare(vm.Quarter3Start, vm.Quarter3End) > 0)
                    ModelState.AddModelError("Quarter 3", "Quarter 3 End Date can not be Before Quarter 3 Start Date");
                else if (DateTime.Compare(vm.Quarter3Start, vm.Quarter3End) == 0)
                    ModelState.AddModelError("Quarter 3", "Quarter 3 End Date can not be the same as Quarter 3 Start Date");
                else { }
                if (DateTime.Compare(vm.Quarter4Start, vm.Quarter4End) > 0)
                    ModelState.AddModelError("Quarter 4", "Quarter 4 End Date can not be Before Quarter 4 Start Date");
                else if (DateTime.Compare(vm.Quarter4Start, vm.Quarter4End) == 0)
                    ModelState.AddModelError("Quarter 4", "Quarter 4 End Date can not be the same as Quarter 4 Start Date");
                else { }
            }
            Department dbDepartment = new Department();
            dbDepartment.Name = vm.Name;
            dbDepartment.Location = vm.Location;
            dbDepartment.Description = vm.Description;
            Quarter Quarter1 = new Quarter();
            Quarter1.Name = vm.Quarter1Name;
            Quarter1.StartDate = vm.Quarter1Start;
            Quarter1.EndDate = vm.Quarter1End;
            Quarter Quarter2 = new Quarter();
            Quarter2.Name = vm.Quarter2Name;
            Quarter2.StartDate = vm.Quarter2Start;
            Quarter2.EndDate = vm.Quarter2End;
            Quarter Quarter3 = new Quarter();
            Quarter3.Name = vm.Quarter3Name;
            Quarter3.StartDate = vm.Quarter3Start;
            Quarter3.EndDate = vm.Quarter3End;
            Quarter Quarter4 = new Quarter();
            Quarter4.Name = vm.Quarter4Name;
            Quarter4.StartDate = vm.Quarter4Start;
            Quarter4.EndDate = vm.Quarter4End;
            if (ModelState.IsValid == true)
            {
                using (var db = new UserDBEntities())
                {

                    //Save Department
                    dbDepartment.User = db.Users.Where(x => x.UID == 2038).FirstOrDefault();
                    db.Departments.Add(dbDepartment);
                    db.SaveChanges();

                    // Save Quarters Associated With the Same Department
                    Quarter1.Department = db.Departments.Where(x => x.Name == dbDepartment.Name).FirstOrDefault();
                    Quarter2.Department = db.Departments.Where(x => x.Name == dbDepartment.Name).FirstOrDefault();
                    Quarter3.Department = db.Departments.Where(x => x.Name == dbDepartment.Name).FirstOrDefault();
                    Quarter4.Department = db.Departments.Where(x => x.Name == dbDepartment.Name).FirstOrDefault();
                    db.Quarters.Add(Quarter1);
                    db.Quarters.Add(Quarter2);
                    db.Quarters.Add(Quarter3);
                    db.Quarters.Add(Quarter4);
                    db.SaveChanges();
                }
                return RedirectToAction("MainView", "Home");
            }

            CreateDepartmentViewModel nvm = new CreateDepartmentViewModel();
            nvm = vm;
            return View(vm);
        }
        public ActionResult ModifyDepartment(ModifyDepartmentViewModel vm)
        {

            ModifyDepartmentViewModel nvm = new ModifyDepartmentViewModel();

            if (vm.IDRef != 0) // Reserved for inital entry in method.
            {
                Department tempdep;

                using (var db = new UserDBEntities())
                    tempdep = db.Departments.Where(x => x.DID == vm.IDRef).FirstOrDefault();

                //Assigning individual values into viewmodel to be passed
                nvm.Name = tempdep.Name;
                nvm.Location = tempdep.Location;
                nvm.Description = tempdep.Description;
                nvm.DID = vm.IDRef;

                ModelState.Clear();
                return View(nvm);
            }

            if (ModelState.IsValid)
            {
                //Name 
                foreach (char x in vm.Name)
                {
                    if (Char.IsControl(x) || Char.IsPunctuation(x) || Char.IsSymbol(x))
                    {
                        ModelState.AddModelError("Name", "Name can only be letters and numbers");
                        break;
                    }
                }
                //Location
                foreach (char x in vm.Location)
                {
                    if (Char.IsControl(x) || Char.IsPunctuation(x) || Char.IsSymbol(x))
                    {
                        ModelState.AddModelError("Location", "Location can only be letters and numbers");
                        break;
                    }
                }
                //Description
                foreach (char x in vm.Description)
                {
                    if (Char.IsControl(x) || Char.IsSymbol(x))
                    {
                        ModelState.AddModelError("Description", "You have invlaid symbols in your Description");
                        break;
                    }
                }
            }

            if (ModelState.IsValid)
            {

                using (var db = new UserDBEntities())
                {
                    //Assign reference of database entity to variable, the assign variable, might do individual fields

                    Department Department = db.Departments.Where(x => x.DID == vm.DID).FirstOrDefault();

                    Department.Name = vm.Name;
                    Department.Location = vm.Location;
                    Department.Description = vm.Description;

                    db.SaveChanges();
                }
                return RedirectToAction("MainView", "Home");
            }
            nvm = vm;
            return View(nvm);
        }
        public ActionResult AddCategory()
        {

            return View();
        }
    }

}