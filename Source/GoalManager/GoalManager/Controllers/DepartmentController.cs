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

            //Creating Supervisor DropDown
            List<SelectListItem> PotentialSupersList = new List<SelectListItem>();
            PotentialSupersList.Add(new SelectListItem { Text = "Select a Supervisor", Selected = true });
            using (UserDBEntities db = new UserDBEntities())
            {
                var users = db.Users.Where(u => u.Role.Equals("Supervisor")).ToList<User>();
                foreach (User u in users)
                { PotentialSupersList.Add(new SelectListItem { Text = u.FirstName + " " + u.LastName, Value = u.UID.ToString(), Selected = false }); }
            }
            vm.SupervisorsDropDown = PotentialSupersList;

            return View(vm);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateDepartment(CreateDepartmentViewModel vm)
        {
            ViewBag.Title = "Create Department";
            CreateDepartmentViewModel nvm = new CreateDepartmentViewModel();
            int _suid = -1;

            try
            {
                var userSessionData = Session["UserSessionData"] as UserSessionData;
                if (userSessionData == null)
                { throw new ArgumentNullException(); }

                if (userSessionData.Role != "Administrator")
                { throw new ArgumentException(); }

                if (ModelState.IsValid)// Checking Validation - View Model Validtion
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

                    if (!String.IsNullOrWhiteSpace(vm.Category2Name) || !String.IsNullOrEmpty(vm.Category2Name))
                    {
                        foreach (char c in vm.Category2Name)
                        {
                            if (Char.IsControl(c) || Char.IsControl(c))
                            {
                                ModelState.AddModelError("Category2Name", "Invalid symbols in name of Category 2.");
                                break;
                            }
                        }
                    }
                    // Category3Name
                    if (!String.IsNullOrWhiteSpace(vm.Category3Name) || !String.IsNullOrEmpty(vm.Category3Name))
                    {
                        foreach (char c in vm.Category3Name)
                        {
                            if (Char.IsControl(c) || Char.IsControl(c))
                            {
                                ModelState.AddModelError("Category3Name", "Invalid symbols in name of Category 3.");
                                break;
                            }
                        }
                    }
                    // Category4Name
                    if (!String.IsNullOrWhiteSpace(vm.Category4Name) || !String.IsNullOrEmpty(vm.Category4Name))
                    {
                        foreach (char c in vm.Category4Name)
                        {
                            if (Char.IsControl(c) || Char.IsControl(c))
                            {
                                ModelState.AddModelError("Category4Name", "Invalid symbols in name of Category 4.");
                                break;
                            }
                        }
                    }
                    //SuperVisorChoice
                    if (vm.SupervisorChoice.Equals("Select a Supervisor") || !int.TryParse(vm.SupervisorChoice, out _suid))
                    { ModelState.AddModelError("SuperVisorChoice", "Must Select A Supervior for the Department"); }
                    else if (_suid < 0)
                    { ModelState.AddModelError("SuperVisorChoice", "Must Select A Supervior for the Department"); }

                    //Validating DateTime
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

                if (ModelState.IsValid) //Checking Custom Validations
                {

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

                    Category Category1 = new Category
                    {
                        Name = vm.Category1Name,
                        Department = dbDepartment
                    };
                    using (var db = new UserDBEntities())
                    {

                        //Assigning Supervisor to Created Department
                        User super = db.Users.Where(x => x.UID == _suid).FirstOrDefault();
                        dbDepartment.User = super;
                         
                        //Saving Database
                        db.Departments.Add(dbDepartment);

                        // Change new Supervisor's Department to new Department
                        super.Department = dbDepartment;

                        // Save Categories Categories beyond the first only if they exist
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
                    } //End Of DataBase Change
                    return RedirectToAction("MainView", "Home");
                }//End of Custom Validation 

            } //End of Try
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


            //Failed Validation, Creating New View model
            nvm = vm;
            //Creating Supervisor Dropdown
            List<SelectListItem> PotentialSupersList = new List<SelectListItem>();
            PotentialSupersList.Add(new SelectListItem { Text = "Select a Supervisor", Selected = true });
            List<User> users = new List<User>();
            using (var db = new UserDBEntities())
            { users = db.Users.Where(u => u.Role.Equals("Supervisor")).ToList<User>(); }

            foreach (User u in users)
            { PotentialSupersList.Add(new SelectListItem { Text = u.FirstName + " " + u.LastName, Value = u.UID.ToString(), Selected = false }); }
            nvm.SupervisorsDropDown = PotentialSupersList;
            
            return View(vm);
        }

        //[Authorize]
        //public ActionResult ModifyDepartment(string IDRef)
        //{
        //    ViewBag.Title = "Modify Department";
        //    ModifyDepartmentViewModel vm = new ModifyDepartmentViewModel();
        //    try
        //    {
        //        Validating Session Data
        //        var userSessionData = Session["UserSessionData"] as UserSessionData;
        //        if (userSessionData == null)
        //        { throw new ArgumentNullException(); }

        //        if (userSessionData.Role != "Administrator")
        //        { throw new ArgumentException(); }

        //        int _did = -1;
        //        if (!int.TryParse(IDRef, out _did))
        //        { return RedirectToAction("MainView", "Home"); }

        //        vm.IDRef = _did;
        //        if (vm.IDRef > -1) // Reserved for inital entry in method.
        //        {
        //            Department tempdep;
        //            using (var db = new UserDBEntities())
        //            {
        //                tempdep = db.Departments.Where(x => x.DID == vm.IDRef).FirstOrDefault();
        //                Assigning individual values into viewmodel to be passed
        //                vm.Name = tempdep.Name;
        //                vm.Location = tempdep.Location;
        //                vm.Description = tempdep.Description;
        //                vm.DID = vm.IDRef;

        //                 Create drop down list of valid Supervisors
        //                List<SelectListItem> PotentialSupersList = new List<SelectListItem>();
        //                PotentialSupersList.Add(new SelectListItem { Text = "Select a Supervisor", Selected = true });

        //                 Get all valid Supervisors
        //                var supers = db.Users.Where(u => u.Role == "Supervisor" && u.Username.ToLower() != "placeholder").ToList<User>();
        //                foreach (var u in supers)
        //                {
        //                    if (u.UID == vm.IDRef)
        //                    { PotentialSupersList.Add(new SelectListItem { Text = u.FirstName + " " + u.LastName, Value = u.UID.ToString(), Selected = true }); }
        //                    else
        //                    { PotentialSupersList.Add(new SelectListItem { Text = u.FirstName + " " + u.LastName, Value = u.UID.ToString(), Selected = false }); }
        //                }
        //                vm.SupervisorsDropDown = PotentialSupersList;
        //            }
        //            ModelState.Clear();
        //            return View(vm);
        //        }

        //        else
        //        {  return RedirectToAction("MainView", "Home"); }
        //    }

        //    catch (ArgumentNullException ex)
        //    {
        //        TempData["ErrorMessage"] = "Null argument exception";
        //        return RedirectToAction("MainView", "Home");
        //    }

        //    catch (ArgumentException ex)
        //    {
        //        TempData["ErrorMessage"] = "Argument exception";
        //        return RedirectToAction("MainView", "Home");
        //    }

        //    catch (Exception ex)
        //    {
        //        TempData["ErrorMessage"] = "Something went very, very, wrong.";
        //        return RedirectToAction("MainView", "Home");
        //    }
        //}

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ModifyDepartment(ModifyDepartmentViewModel vm)
        {
            ViewBag.Title = "Modify Department";
            ModifyDepartmentViewModel nvm = new ModifyDepartmentViewModel();
            List<User> supers = new List<User>();
            List<Quarter> Quarters = new List<Quarter>();
            List<SelectListItem> PotentialSupersList = new List<SelectListItem>();
            int _suid = -1;

            try
            {
                //Validating Session Data
                var userSessionData = Session["UserSessionData"] as UserSessionData;
                if (userSessionData == null)
                { throw new ArgumentNullException(); }

                if (userSessionData.Role != "Administrator")
                { throw new ArgumentException(); }

                if (vm.IDRef != 0) // Reserved for inital entry in method.
                {
                    //Creating ViewModel For Initial Entry
                    Department tempdep;
                    

                    using (var db = new UserDBEntities())
                    {
                        tempdep = db.Departments.Where(x => x.DID == vm.IDRef).FirstOrDefault();
                        supers = db.Users.Where(u => u.Role == "Supervisor" && u.Username.ToLower() != "placeholder").ToList<User>();
                        Quarters = db.Quarters.Where(d => d.DID == vm.IDRef).ToList();
                    }

                    //Creating Supervisors List
                    
                    PotentialSupersList.Add(new SelectListItem { Text = "Select a Supervisor", Selected = true });

                    foreach (var u in supers)
                    { PotentialSupersList.Add(new SelectListItem { Text = u.FirstName + " " + u.LastName, Value = u.UID.ToString(), Selected = false }); }
                    nvm.SupervisorsDropDown = PotentialSupersList;

                    //Assign Quarters List
                    nvm.Quarters = Quarters;

                    //Assigning individual values into viewmodel to be passed
                    nvm.Name = tempdep.Name;
                    nvm.Location = tempdep.Location;
                    nvm.Description = tempdep.Description;
                    nvm.DID = vm.IDRef;
                    nvm.IDRef = 0;
                    ModelState.Clear();
                    return View(nvm);
                }
                
                //Second Entry
                if (ModelState.IsValid) //Checking for Validation Error in View Model Annotation before the following Custom Validation, Refer to this View Model to see these Validations                  {
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
                            ModelState.AddModelError("Description", "You have invalid symbols in your Description");
                            break;
                        }
                    }
                    //Supervisor Choice
                    if (String.IsNullOrEmpty(vm.SupervisorChoice))
                    { ModelState.AddModelError("SupervisorChoice", "Invalid Supervisor choice."); }

                    else
                    {
                        if (!int.TryParse(vm.SupervisorChoice, out _suid))
                        { ModelState.AddModelError("SupervisorChoice", "Invalid entry for SUID."); }
                    }
                }

                if (ModelState.IsValid) //Checking Custom Validation
                {
                    using (var db = new UserDBEntities())
                    {
                        //Assign reference of database entity to variable, the assign variable, might do individual fields

                        Department Department = db.Departments.Where(x => x.DID == vm.DID).FirstOrDefault();
                        Department.Name = vm.Name;
                        Department.Location = vm.Location;
                        Department.Description = vm.Description;

                        User Super = db.Users.Where(u => u.UID == _suid).FirstOrDefault();
                        Department.User = Super; // update Department's Supervisor
                        Super.Department = Department; // update Supervisor's Department

                        var managedEEs = db.Users.Where(u => u.DID == Department.DID).ToList<User>();
                        foreach (var u in managedEEs)
                        { u.User1 = Super; } // set each EE's Super in this Dept

                        db.SaveChanges();
                    } // End of Database Changes
                    return RedirectToAction("MainView", "Home");
                } //End of Sucessful Validation
                //Failed Validation - Creating View Model
                nvm = vm;

                using (var db = new UserDBEntities())
                {
                    supers = db.Users.Where(u => u.Role == "Supervisor" && u.Username.ToLower() != "placeholder").ToList<User>();
                    Quarters = db.Quarters.Where(d => d.DID == vm.IDRef).ToList();
                }

                //Creating Supervisors List
                PotentialSupersList.Add(new SelectListItem { Text = "Select a Supervisor", Selected = true });

                foreach (var u in supers)
                { PotentialSupersList.Add(new SelectListItem { Text = u.FirstName + " " + u.LastName, Value = u.UID.ToString(), Selected = false }); }
                nvm.SupervisorsDropDown = PotentialSupersList;

                //Assign Quarters List
                vm.Quarters = Quarters;
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
            { vm.Categories = db.Categories.Where(x => x.DepID == userSessionData.DID).ToList(); }

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
            { nvm.Categories = db.Categories.Where(x => x.DepID == userSessionData.DID).ToList(); }
            return View(nvm);
        }
    }

}