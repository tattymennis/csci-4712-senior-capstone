using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GoalManager.Models;
using GoalManager.Data;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;

namespace GoalManager.Controllers
{
    public class EmployeeController : Controller
    {
        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        [Authorize]
        public ActionResult CreateEmployee()
        {
            ViewBag.Title = "Create Employee";
            var vm = new CreateEmployeeViewModel();

            List<SelectListItem> DtempList = new List<SelectListItem>();
            DtempList.Add(new SelectListItem { Value = "0", Text = "Select a Department", Selected = true });
            using (var db = new UserDBEntities())
            {
                var depts = db.Departments;
                foreach (Department d in depts)
                    DtempList.Add(new SelectListItem { Value = d.DID.ToString(), Text = d.Name, Selected = false });
            }
            vm.DeptDropDown = DtempList;

            List<SelectListItem> RtempList = new List<SelectListItem>();
            RtempList.Add(new SelectListItem { Value = "0", Text = "Select Role", Selected = true});
            RtempList.Add(new SelectListItem { Value = "Employee", Text = "Employee", Selected = false });
            RtempList.Add(new SelectListItem { Value = "Supervisor", Text = "Supervisor", Selected = false });
            RtempList.Add(new SelectListItem { Value = "Administrator", Text = "Administrator", Selected = false });
            vm.RoleDropDown = RtempList;

            return View(vm);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult CreateEmployee(CreateEmployeeViewModel vm)
        {
            ViewBag.Title = "Create Employee";
            try
            {
                var userSessionData = Session["UserSessionData"] as UserSessionData;
                if (userSessionData == null)
                {
                    throw new ArgumentNullException();
                }

                if (userSessionData.Role != "Admin" && userSessionData.Role != "Administrator")
                {
                    throw new ArgumentException();
                }

                if (ModelState.IsValid)
                {
                    //First Name 
                    foreach (char x in vm.FirstName)
                    {
                        if (Char.IsDigit(x) || Char.IsControl(x) || Char.IsPunctuation(x) || Char.IsSymbol(x))
                        {
                            ModelState.AddModelError("FirstName", "First name can only contain letters.");
                            break;
                        }
                    }
                    //Last Name
                    foreach (char x in vm.LastName)
                    {
                        if (System.Char.IsDigit(x) || Char.IsControl(x) || Char.IsPunctuation(x) || Char.IsSymbol(x))
                        {
                            ModelState.AddModelError("LastName", "Last name can only contain A-z.");
                            break;
                        }
                    }
                    //Email
                    using (UserDBEntities db = new UserDBEntities())
                    {
                        if (db.Users.Any(x => x.Email == vm.Email))
                            ModelState.AddModelError("Email", "Email Address Already Exists");
                    }

                    //Title
                    foreach (char x in vm.Title)
                    {
                        if (Char.IsControl(x) || Char.IsPunctuation(x))
                        {
                            ModelState.AddModelError("Title", "Title can only contain A-Z or numbers");
                            break;
                        }
                    }

                    //Role
                    if (vm.Role == "Select Role" || String.IsNullOrEmpty(vm.Role))
                        ModelState.AddModelError("Role", "Must Select a Role");

                    //Department
                    if (String.IsNullOrWhiteSpace(vm.DeptRefChoice) || vm.DeptRefChoice.Equals("Select a Department"))
                    {
                        ModelState.AddModelError("DepRefChoice", "Must Select a Department");
                    }

                    int _did = -1;
                    if (int.TryParse(vm.DeptRefChoice, out _did))
                    {
                        using (var db = new UserDBEntities())
                        {
                            // generate username
                            int count = 1;
                            string username = (vm.FirstName[0] + vm.LastName).ToLower();
                            while (db.Users.Any(x => x.Username == username) || count > 100) // username collision
                            {
                                username = (vm.FirstName[0] + vm.LastName).ToLower();
                                username += count.ToString();
                                count++;
                            }

                            User user = new Data.User
                            {
                                FirstName = vm.FirstName,
                                LastName = vm.LastName,
                                Role = vm.Role,
                                Title = vm.Title,
                                Email = vm.Email,
                                Username = username,
                                Active = true,
                                Department = db.Departments.Where(d => d.DID == _did).FirstOrDefault()
                            };

                            // Supervisor Property
                            User super = db.Users.Where(u => u.UID == user.Department.SUID).FirstOrDefault();
                            user.User1 = super;

                            // Account Creation
                            RegisterEmployeeViewModel revm = new RegisterEmployeeViewModel();
                            revm.Email = user.Email;
                            revm.Username = user.Username;
                            revm.Role = user.Role;
                            Session["RegisterEmployeeVM"] = revm;

                            db.Users.Add(user);
                            db.SaveChanges();
                            return RedirectToAction("RegisterEmployee");
                        }                 
                    }
                }
            }

            catch { }

            // New ViewModel if validation fails
            CreateEmployeeViewModel nvm = new CreateEmployeeViewModel();
            nvm = vm;

            List<SelectListItem> DtempList = new List<SelectListItem>();
            DtempList.Add(new SelectListItem { Text = "Select Department", Selected = true });
            using (var db = new UserDBEntities())
            {
                var depts = db.Departments;
                foreach (Department d in depts)
                    DtempList.Add(new SelectListItem { Value = d.DID.ToString(), Text = d.Name, Selected = false });
            }
            nvm.DeptDropDown = DtempList;

            List<SelectListItem> RtempList = new List<SelectListItem>();
            RtempList.Add(new SelectListItem { Value = "0", Text = "Select Role", Selected = true });
            RtempList.Add(new SelectListItem { Value = "Employee", Text = "Employee", Selected = false });
            RtempList.Add(new SelectListItem { Value = "Supervisor", Text = "Supervisor", Selected = false });
            RtempList.Add(new SelectListItem { Value = "Administrator", Text = "Administrator", Selected = false });
            vm.RoleDropDown = RtempList;
            return View(nvm);
        }

        [Authorize]
        public async Task<ActionResult> RegisterEmployee()
        {
            RegisterEmployeeViewModel revm = Session["RegisterEmployeeVM"] as RegisterEmployeeViewModel;
            if (revm == null)
            {
                // failed; no data passed
                return RedirectToAction("CreateEmployee", "Employee");
            }

            var user = new ApplicationUser { UserName = revm.Username, Email = revm.Email };
            var result = await UserManager.CreateAsync(user, "Pa$$w0rd");

            Session["CurrentUser"] = revm.Role;
            return RedirectToAction("MainView", "Home");
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RegisterEmployee(RegisterEmployeeViewModel vm)
        {
            var role = vm.Role;
            var user = new ApplicationUser { UserName = vm.Username, Email = vm.Email };
            var result = await UserManager.CreateAsync(user, "Pa$$w0rd");

            return RedirectToAction("MainView", "Home");
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult ModifyEmployee(ModifyEmployeeViewModel vm)
        {
            ModifyEmployeeViewModel nvm = new ModifyEmployeeViewModel();

            List<SelectListItem> RtempList = new List<SelectListItem>();
            List<SelectListItem> DtempList = new List<SelectListItem>();
            List<SelectListItem> AtempList = new List<SelectListItem>();

            if (vm.IDRef != 0) // Reserved for inital entry in method.
            {
                User tempuser;

                using (var db = new UserDBEntities())
                    tempuser = db.Users.Where(x => x.UID == vm.IDRef).FirstOrDefault();

                //Assigning individual values into viewmodel to be passed
                nvm.FirstName = tempuser.FirstName;
                nvm.LastName = tempuser.LastName;
                nvm.Role = tempuser.Role;
                nvm.Title = tempuser.Title;
                nvm.Email = tempuser.Email;
                nvm.ID = vm.IDRef;
                nvm.Active = tempuser.Active;
                
               
                using (var db = new UserDBEntities())
                {
                    DtempList.Add(new SelectListItem { Value = "0", Text = "Select a Department", Selected = true });
                    var depts = db.Departments;

                    foreach (Department d in depts)
                        DtempList.Add(new SelectListItem { Value = d.DID.ToString(), Text = d.Name.ToString(), Selected = false });
                }
                nvm.DeptDropDown = DtempList;
                //Must Select the department the user resides
                
                RtempList.Add(new SelectListItem { Value = "0", Text = "Select Role", Selected = true });
                RtempList.Add(new SelectListItem { Value = "Employee", Text = "Employee", Selected = false });
                RtempList.Add(new SelectListItem { Value = "Supervisor", Text = "Supervisor", Selected = false });
                RtempList.Add(new SelectListItem { Value = "Administrator", Text = "Administrator", Selected = false });
                nvm.RoleDropDown = RtempList;
                

                AtempList.Add(new SelectListItem { Value = "true", Text = "True", Selected = true });
                AtempList.Add(new SelectListItem { Value = "false", Text = "False", Selected = true });
                nvm.ActiveDropDown = AtempList;

                ModelState.Clear();
                return View(nvm);
            }
            
            
            //Validation for Each Field
            //First Name 
            if (ModelState.IsValid)
            {
                //First Name 
                foreach (char x in vm.FirstName)
                {
                    if (Char.IsDigit(x) || Char.IsControl(x) || Char.IsPunctuation(x) || Char.IsSymbol(x))
                    {
                        ModelState.AddModelError("FirstName", "First name can only contain letters.");
                        break;
                    }
                }
                //Last Name
                foreach (char x in vm.LastName)
                {
                    if (System.Char.IsDigit(x) || Char.IsControl(x) || Char.IsPunctuation(x) || Char.IsSymbol(x))
                    {
                        ModelState.AddModelError("LastName", "Last name can only contain A-z.");
                        break;
                    }
                }
                //Email - Must change the way it remembers and save the email, might take out completely
                //using (UserDBEntities db = new UserDBEntities())
                //{
                //    if (db.Users.Any(x => x.Email == vm.Email))
                //        ModelState.AddModelError("Email", "Email Address Already Exists");
                //}

                //Title
                foreach (char x in vm.Title)
                {
                    if (Char.IsControl(x) || Char.IsPunctuation(x) || Char.IsSymbol(x))
                    {
                        ModelState.AddModelError("Title", "Title can only contain A-Z or numbers");
                        break;
                    }
                }
            }

            //Role
            if (vm.Role == "Select Role" || String.IsNullOrEmpty(vm.Role))
                ModelState.AddModelError("Role", "Must Select a Role");

            //Department must learn to map to the form field to prevent error
            if (vm.DepRefChoice == 0)
            { ModelState.AddModelError("DepRefChoice", "Must Select a Department"); }
            else // Checking to see if Department exist on the database to prevent error
            {
                using (var db = new UserDBEntities())
                {
                    var depts = db.Departments.ToList();
                    if (!(depts.Any(x => x.DID == vm.DepRefChoice)))
                        ModelState.AddModelError("DepRefChoice", "Department is not Valid");
                }
            }

            //If no errors, Add to the Database
            if (ModelState.IsValid)
            {
                using (var db = new UserDBEntities())
                {
                    //Assign reference of database entity to variable, the assign variable, might do individual fields

                    User User = db.Users.Where(x => x.UID == vm.ID).FirstOrDefault();

                    User.FirstName = vm.FirstName;
                    User.LastName = vm.LastName;
                    //User.Email = dbuser.Email; //Must Update in ASP .Net User table also
                    User.Title = vm.Title;
                    User.Role = vm.Role;
                    User.Active = vm.Active;
                    User.Department = db.Departments.Where(x => x.DID == vm.DepRefChoice).FirstOrDefault();

                    db.SaveChanges();

                }
                return RedirectToAction("MainView", "Home");
            }

            // Creating new view model if validation failed
            nvm = vm;
            DtempList.Add(new SelectListItem { Value = "0", Text = "Select a Department", Selected = true });
            using (var db = new UserDBEntities())
            {
                var depts = db.Departments;
                foreach (Department d in depts)
                {
                    DtempList.Add(new SelectListItem { Value = d.DID.ToString(), Text = d.Name, Selected = false });
                }

            }
            nvm.DeptDropDown = DtempList;

            RtempList.Add(new SelectListItem { Value = "0", Text = "Select Role", Selected = true });
            RtempList.Add(new SelectListItem { Value = "Employee", Text = "Employee", Selected = false });
            RtempList.Add(new SelectListItem { Value = "Supervisor", Text = "Supervisor", Selected = false });
            RtempList.Add(new SelectListItem { Value = "Administrator", Text = "Administrator", Selected = false });
            nvm.RoleDropDown = RtempList;

            AtempList.Add(new SelectListItem { Value = "true", Text = "True", Selected = true });
            AtempList.Add(new SelectListItem { Value = "false", Text = "False", Selected = true });
            nvm.ActiveDropDown = AtempList;

            return View(nvm);
        }
    }
}