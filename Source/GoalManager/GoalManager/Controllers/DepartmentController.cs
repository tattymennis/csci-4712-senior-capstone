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
        [Authorize]
        public ActionResult CreateDepartment()
        {
            ViewBag.Title = "Create Department";
            var vm = new CreateDepartmentViewModel();

            List<SelectListItem> PotentialSupersList = new List<SelectListItem>();
            PotentialSupersList.Add(new SelectListItem { Text = "Select a Supervisor", Selected = true });

            using (UserDBEntities db = new UserDBEntities())
            {
                // get list of available Supervisors
                var users = db.Users.Where(u => u.Role.Equals("Supervisor")).ToList<User>();
                foreach (User u in users)
                {
                    PotentialSupersList.Add(new SelectListItem { Text = u.FirstName +" "+ u.LastName, Value = u.UID.ToString(), Selected = false });
                }
            }

            vm.SupervisorsDropDown = PotentialSupersList;
            ModelState.Clear();
            return View(vm);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateDepartment(CreateDepartmentViewModel vm)
        {
            ViewBag.Title = "Create Department";
            CreateDepartmentViewModel nvm = new CreateDepartmentViewModel();

            try
            {
                var userSessionData = Session["UserSessionData"] as UserSessionData;
                if (userSessionData == null)
                {
                    throw new ArgumentNullException();
                }

                if (userSessionData.Role != "Administrator")
                {
                    throw new ArgumentException();
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
                            ModelState.AddModelError("Description", "Invalid symbols in Description");
                            break;
                        }
                    }

                    // Category1Name
                    foreach (char c in vm.Category1Name)
                    {
                        if (Char.IsControl(c) || Char.IsControl(c))
                        {
                            ModelState.AddModelError("Category1Name", "Invalid symbols in name of Category 1.");
                            break;
                        }
                    }

                    // Category2Name
                    foreach (char c in vm.Category2Name)
                    {
                        if (Char.IsControl(c) || Char.IsControl(c))
                        {
                            ModelState.AddModelError("Category2Name", "Invalid symbols in name of Category 1.");
                            break;
                        }
                    }

                    // Category3Name
                    foreach (char c in vm.Category3Name)
                    {
                        if (Char.IsControl(c) || Char.IsControl(c))
                        {
                            ModelState.AddModelError("Category3Name", "Invalid symbols in name of Category 1.");
                            break;
                        }
                    }

                    // Category4Name
                    foreach (char c in vm.Category4Name)
                    {
                        if (Char.IsControl(c) || Char.IsControl(c))
                        {
                            ModelState.AddModelError("Category3Name", "Invalid symbols in name of Category 1.");
                            break;
                        }
                    }

                    if (DateTime.Compare(vm.Quarter1Start, vm.Quarter1End) > 0)
                        ModelState.AddModelError("Quarter 1", "Quarter 1 End Date can not be Before Quarter 1 Start Date");
                    else if (DateTime.Compare(vm.Quarter1Start, vm.Quarter1End) == 0)
                        ModelState.AddModelError("Quarter 1", "Quarter 1 End Date can not be the same as Quarter 1 Start Date");

                    if (DateTime.Compare(vm.Quarter2Start, vm.Quarter2End) > 0)
                        ModelState.AddModelError("Quarter 2", "Quarter 2 End Date can not be Before Quarter 2 Start Date");
                    else if (DateTime.Compare(vm.Quarter2Start, vm.Quarter2End) == 0)
                        ModelState.AddModelError("Quarter 2", "Quarter 2 End Date can not be the same as Quarter 2 Start Date");

                    if (DateTime.Compare(vm.Quarter3Start, vm.Quarter3End) > 0)
                        ModelState.AddModelError("Quarter 3", "Quarter 3 End Date can not be Before Quarter 3 Start Date");
                    else if (DateTime.Compare(vm.Quarter3Start, vm.Quarter3End) == 0)
                        ModelState.AddModelError("Quarter 3", "Quarter 3 End Date can not be the same as Quarter 3 Start Date");

                    if (DateTime.Compare(vm.Quarter4Start, vm.Quarter4End) > 0)
                        ModelState.AddModelError("Quarter 4", "Quarter 4 End Date can not be Before Quarter 4 Start Date");
                    else if (DateTime.Compare(vm.Quarter4Start, vm.Quarter4End) == 0)
                        ModelState.AddModelError("Quarter 4", "Quarter 4 End Date can not be the same as Quarter 4 Start Date");
                }

                if (ModelState.IsValid)
                {
                    // Supervisor drop down
                    int _suid = -1;
                    if (vm.SupervisorChoice.Equals("Select a Supervisor") || !int.TryParse(vm.SupervisorChoice, out _suid))
                    {
                        // Drop down failed
                        throw new ArgumentException();
                    }

                    Department dbDepartment = new Department
                    {
                        Name = vm.Name,
                        Location = vm.Location,
                        Description = vm.Description
                    };

                    Quarter Quarter1 = new Quarter
                    {
                        Name = vm.Quarter1Name,
                        StartDate = vm.Quarter1Start,
                        EndDate = vm.Quarter1End
                    };

                    Quarter Quarter2 = new Quarter
                    {
                        Name = vm.Quarter2Name,
                        StartDate = vm.Quarter2Start,
                        EndDate = vm.Quarter2End
                    };

                    Quarter Quarter3 = new Quarter
                    {
                        Name = vm.Quarter3Name,
                        StartDate = vm.Quarter3Start,
                        EndDate = vm.Quarter3End
                    };

                    Quarter Quarter4 = new Quarter
                    {
                        Name = vm.Quarter4Name,
                        StartDate = vm.Quarter4Start,
                        EndDate = vm.Quarter4End
                    };

                    nvm = vm;
                    using (var db = new UserDBEntities())
                    {
                        if (_suid < 0)
                        {
                            throw new ArgumentException();
                        }

                        User super = db.Users.Where(x => x.UID == _suid).FirstOrDefault();
                        dbDepartment.User = super; // add Supervisor w/ EF navigation properties
                        db.Departments.Add(dbDepartment);

                        // Change new Supervisor's Department to new Department
                        super.Department = dbDepartment;

                        // get list of available Supervisors
                        List<SelectListItem> PotentialSupersList = new List<SelectListItem>();
                        PotentialSupersList.Add(new SelectListItem { Text = "Select a Supervisor", Selected = true });
                        var users = db.Users.Where(u => u.Role.Equals("Supervisor")).ToList<User>();
                        foreach (User u in users)
                        {
                            PotentialSupersList.Add(new SelectListItem { Text = u.FirstName + " " + u.LastName, Value = u.UID.ToString(), Selected = false });
                        }
                        nvm.SupervisorsDropDown = PotentialSupersList;

                        // Save Categories associated with Department
                        db.Categories.Add(new Category
                        {
                            Name = vm.Category1Name,
                            Department = dbDepartment
                        });

                        if (!String.IsNullOrWhiteSpace(vm.Category2Name))
                        {
                            db.Categories.Add(new Category
                            {
                                Name = vm.Category2Name,
                                Department = dbDepartment
                            });
                        }

                        if (!String.IsNullOrWhiteSpace(vm.Category3Name))
                        {
                            db.Categories.Add(new Category
                            {
                                Name = vm.Category3Name,
                                Department = dbDepartment
                            });
                        }

                        if (!String.IsNullOrWhiteSpace(vm.Category4Name))
                        {
                            db.Categories.Add(new Category
                            {
                                Name = vm.Category4Name,
                                Department = dbDepartment
                            });
                        }

                        // Save Quarters Associated With the Same Department
                        //dbDepartment = db.Departments.Where(x => x.Name == dbDepartment.Name && x.SUID == super.SUID).FirstOrDefault();
                        Quarter1.Department = dbDepartment;
                        Quarter2.Department = dbDepartment;
                        Quarter3.Department = dbDepartment;
                        Quarter4.Department = dbDepartment;
                        db.Quarters.Add(Quarter1);
                        db.Quarters.Add(Quarter2);
                        db.Quarters.Add(Quarter3);
                        db.Quarters.Add(Quarter4);
                        db.SaveChanges();
                    }
                }
                return RedirectToAction("MainView", "Home");
            }

            catch (ArgumentNullException ex)
            {
                TempData["ErrorMessage"] = "Null argument exception";
                return RedirectToAction("MainView", "Home");
            }

            catch (ArgumentException ex)
            {
                TempData["ErrorMessage"] = "Argument exception";
                return RedirectToAction("MainView", "Home");
            }

            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Something went very, very, wrong.";
                return RedirectToAction("MainView", "Home");
            }

            return View(vm);
        }

        [Authorize]
        public ActionResult ModifyDepartment(ModifyDepartmentViewModel vm)
        {
            ViewBag.Title = "Modify Department";
            ModifyDepartmentViewModel nvm = new ModifyDepartmentViewModel();

            try
            {
                var userSessionData = Session["UserSessionData"] as UserSessionData;
                if (userSessionData == null)
                {
                    throw new ArgumentNullException();
                }

                if (userSessionData.Role != "Administrator")
                {
                    throw new ArgumentException();
                }

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

            catch (ArgumentNullException ex)
            {
                TempData["ErrorMessage"] = "Null argument exception";
                return RedirectToAction("MainView", "Home");
            }

            catch (ArgumentException ex)
            {
                TempData["ErrorMessage"] = "Argument exception";
                return RedirectToAction("MainView", "Home");
            }

            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Something went very, very, wrong.";
                return RedirectToAction("MainView", "Home");
            }
        }
        public ActionResult AddCategory()
        {
            var userSessionData = Session["UserSessionData"] as UserSessionData;

            AddCategoryViewModel vm = new AddCategoryViewModel();

            using (var db = new UserDBEntities())
            {
               vm.Categories = db.Categories.Where(x => x.DepID == userSessionData.DID).ToList();
            }

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddCategory(AddCategoryViewModel vm)
        {
            var userSessionData = Session["UserSessionData"] as UserSessionData;

            if (ModelState.IsValid == true)
            {
                foreach (char x in vm.Name)
                {
                    if (Char.IsControl(x) || Char.IsPunctuation(x) || Char.IsSymbol(x))
                    {
                        ModelState.AddModelError("Name", "Name can only be letters and numbers");
                        break;
                    }
                }
            }
            if (ModelState.IsValid == true)
            {

                using (var db = new UserDBEntities())
                {
                    Category DBCategory = new Category();
                    DBCategory.Name = vm.Name;
                    DBCategory.Department = db.Departments.Where(x => x.DID == userSessionData.DID).FirstOrDefault();
                    db.Categories.Add(DBCategory);
                    db.SaveChanges();
                }
            }

            AddCategoryViewModel nvm = vm;
            using (var db = new UserDBEntities())
            {
                nvm.Categories = db.Categories.Where(x => x.DepID == userSessionData.DID).ToList();
            }
                return View(nvm);
        }
    }

}