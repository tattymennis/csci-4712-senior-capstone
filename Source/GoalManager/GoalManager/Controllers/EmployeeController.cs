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

        // GET: Employee
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult CreateEmployee()
        {
            var vm = new CreateEmployeeViewModel();
           
            List<SelectListItem> tempList = new List<SelectListItem>();
            tempList.Add(new SelectListItem { Value = "0", Text = "Select a Department", Selected = true });

            using (var db = new UserDBEntities())
            {
                var depts = db.Departments;
                foreach (Department d in depts)
                {
                    tempList.Add(new SelectListItem { Value = d.DID.ToString(), Text = d.Name, Selected = false});
                }
            }

            vm.DeptDropDown = tempList;
            return View(vm);
        }
 
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult CreateEmployee(CreateEmployeeViewModel vm)
        {
            User dbuser = new User(); //user to be added to the database
            //Validation for Each Field
            //First Name 
            if (String.IsNullOrWhiteSpace(vm.FirstName))
            {
                ModelState.AddModelError("FirstName", "First name can not be blank.");
            }
            else
            {
                foreach (char x in vm.FirstName)
                {
                    if (System.Char.IsDigit(x) || Char.IsControl(x) || Char.IsPunctuation(x) || Char.IsSymbol(x))
                    {
                        ModelState.AddModelError("FirstName", "First name can only contain characters A-z.");
                        break;
                    }
                }
                dbuser.FirstName = vm.FirstName;
            }

            //Last Name
            if (String.IsNullOrWhiteSpace(vm.LastName))
            {
                ModelState.AddModelError("LastName", "Last name can not be blank.");
            }

            else
            {
                foreach (char x in vm.LastName)
                {
                    if (System.Char.IsDigit(x) || Char.IsControl(x) || Char.IsPunctuation(x) || Char.IsSymbol(x))
                    {
                        ModelState.AddModelError("LastName", "Last name can only contain A-z.");
                        break;
                    }
                }
                dbuser.LastName = vm.LastName;
            }

            //Email is error check by the html web browser ahead of time if valid email address
            if (String.IsNullOrWhiteSpace(vm.Email))
            {
                ModelState.AddModelError("Email", "You Must Enter an Email Address");
            }

            else // TODO: Regex email address?
            {
                using (UserDBEntities db = new UserDBEntities())
                {                  
                    if (db.Users.Any(x => x.Email == vm.Email))
                    {
                        ModelState.AddModelError("Email", "Please enter a new email address.");
                    }
                    else
                    {
                        dbuser.Email = vm.Email;
                    }
                }          
            }

            //Title
            if (String.IsNullOrWhiteSpace(vm.Title))
            {
                ModelState.AddModelError("Title", "Employee must have a Title");
            }

            else
            {
                //Title can have a digit, e.g "Section 8 Specialist"
                foreach (char x in vm.Title)
                {
                    if (Char.IsControl(x) || Char.IsPunctuation(x) || Char.IsSymbol(x))
                    {
                        ModelState.AddModelError("Title", "Title can only contain A-Z or numbers");
                        break;
                    }
                }
                dbuser.Title = vm.Title;
            }

            //Role
            if (vm.Role == "0" || String.IsNullOrWhiteSpace(vm.Role))
            {
                ModelState.AddModelError("Role", "Must Select a Role");
            }

            else
            {
                dbuser.Role = vm.Role;
            }

            ////Department
            //if(vm.DepRefChoice == 0) //This field is enclosed under value="0" as a default choice, client side only able to choose default or valid. 
            //{
            //    ModelState.AddModelError("DepRefChoice", "Must Select a Department");
            //}

            //If no errors, Add to the Database
            if (ModelState.IsValid)
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
                    dbuser.Username = username;
                    dbuser.Active = true; // Active, active is true for new employees

                    dbuser.Department = db.Departments.Where(x => x.DID == vm.DepRefChoice).FirstOrDefault();

                    // make sure User1 is right nav property
                    dbuser.User1 = db.Users.Where(x => x.UID == dbuser.Department.SUID).FirstOrDefault(); // set SUID

                    db.Users.Add(dbuser);
                    db.SaveChanges();
                }

                // Account creation
                RegisterEmployeeViewModel revm = new RegisterEmployeeViewModel();
                revm.Email = dbuser.Email;
                revm.Username = dbuser.Username;
                revm.Role = dbuser.Role;
                Session["RegisterEmployeeVM"] = revm;
                return RedirectToAction("RegisterEmployee");
            }

            // New ViewModel when validation fails
            CreateEmployeeViewModel nvm = new CreateEmployeeViewModel();
            //Assigning individual values
            nvm = vm;
            List<SelectListItem> tempList = new List<SelectListItem>();
            tempList.Add(new SelectListItem { Value = "0", Text = "Select a Department", Selected = true });
            using (var db = new UserDBEntities())
            {
                var depts = db.Departments;
                foreach (Department d in depts)
                {
                    tempList.Add(new SelectListItem { Value = d.DID.ToString(), Text = d.Name, Selected = false });
                }
            }
            nvm.DeptDropDown = tempList;
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
            //return View(revm);
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

        //[HttpPost]
        //public ActionResult ModifyEmployee(int ID) //Pass Employee ID to be Displayed
        //{
        //    var vm = new ModifyEmployeeViewModel();
        //    //using Db, search where ID equals the Employeee ID
        //    return View(vm);
        // }

        [Authorize]
        public ActionResult ModifyEmployee()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult ModifyEmployee(ModifyEmployeeViewModel vm)
        {
            ModifyEmployeeViewModel nvm = new ModifyEmployeeViewModel();
            List<SelectListItem> tempList = new List<SelectListItem>();

            if (vm.IDRef != 0) // Reserved for inital entry in method.
            {
                User tempuser;
                
                using (var db = new UserDBEntities())
                {
                    tempuser = db.Users.Where(x => x.UID == vm.IDRef).FirstOrDefault();

                }
                //Assigning individual values into viewmodel to be passed
                nvm.FirstName = tempuser.FirstName;
                nvm.LastName = tempuser.LastName;
                nvm.Role = tempuser.Role;
                nvm.Title = tempuser.Title;
                nvm.Username = tempuser.Username;
                nvm.Email = tempuser.Email;

                // Department drop down
                
                tempList.Add(new SelectListItem { Value = "0", Text = "Select a Department", Selected = true });
                using (var db = new UserDBEntities())
                {
                    var depts = db.Departments;
                    foreach (Department d in depts)
                    {
                        tempList.Add(new SelectListItem { Value = d.DID.ToString(), Text = d.Name, Selected = false });
                    }
                }
                nvm.DeptDropDown = tempList;
                return View(nvm);
            }

            User dbuser = new User(); //user to be added to the database
            //Validation for Each Field
            //First Name 
            if (String.IsNullOrWhiteSpace(vm.FirstName))
            {
                ModelState.AddModelError("FirstName", "First name can not be blank.");
            }
            else
            {
                foreach (char x in vm.FirstName)
                {
                    if (System.Char.IsDigit(x) || Char.IsControl(x) || Char.IsPunctuation(x) || Char.IsSymbol(x))
                    {
                        ModelState.AddModelError("FirstName", "First name can only contain characters A-z.");
                        break;
                    }
                }
                dbuser.FirstName = vm.FirstName;
            }

            //Last Name
            if (String.IsNullOrWhiteSpace(vm.LastName))
            {
                ModelState.AddModelError("LastName", "Last name can not be blank.");
            }

            else
            {
                foreach (char x in vm.LastName)
                {
                    if (System.Char.IsDigit(x) || Char.IsControl(x) || Char.IsPunctuation(x) || Char.IsSymbol(x))
                    {
                        ModelState.AddModelError("LastName", "Last name can only contain A-z.");
                        break;
                    }
                }
                dbuser.LastName = vm.LastName;
            }

            //Email is error check by the html web browser ahead of time if valid email address
            if (String.IsNullOrWhiteSpace(vm.Email))
            {
                ModelState.AddModelError("Email", "You Must Enter an Email Address");
            }

            else // TODO: Regex email address?
            {
                dbuser.Email = vm.Email;
            }

            //Title
            if (String.IsNullOrWhiteSpace(vm.Title))
            {
                ModelState.AddModelError("Title", "Employee must have a Title");
            }

            else
            {
                //Title can have a digit, e.g "Section 8 Specialist"
                foreach (char x in vm.Title)
                {
                    if (Char.IsDigit(x) || Char.IsControl(x) || Char.IsPunctuation(x) || Char.IsSymbol(x))
                    {
                        ModelState.AddModelError("Title", "Title can only contain A-Z or numbers");
                        break;
                    }
                }
                dbuser.Title = vm.Title;
            }

            //Role
            if (vm.Role == "0" || String.IsNullOrWhiteSpace(vm.Role))
            {
                ModelState.AddModelError("Role", "Must Select a Role");
            }

            else
            {
                dbuser.Role = vm.Role;
            }

            //Department
            if (vm.DepRefChoice == 0) //This field is enclosed under value="0" as a default choice, client side only able to choose default or valid. 
            {
                ModelState.AddModelError("DepRefChoice", "Must Select a Department");
            }
            //If no errors, Add to the Database
            if (ModelState.IsValid)
            {
                using (var db = new UserDBEntities())
                {
                    // generate username

                    User User = db.Users.Find(vm.UID);
                    //TryUpdateModel
                    //if(TryUpdateModel(fields))

                    dbuser.Department = db.Departments.Where(x => x.DID ==vm.DepRefChoice).FirstOrDefault();
                    db.Users.Add(dbuser);

                    db.SaveChanges();
                }
                RedirectToAction("~/Home/Index");
            }

            // Creating new view model if validation failed
            nvm = vm;
            tempList.Add(new SelectListItem { Value = "0", Text = "Select a Department", Selected = true });
            using (var db = new UserDBEntities())
            {
                var depts = db.Departments;
                foreach (Department d in depts)
                {
                    tempList.Add(new SelectListItem { Value = d.DID.ToString(), Text = d.Name, Selected = false });
                }

            }
            nvm.DeptDropDown = tempList;
            return View(nvm);
        }
    }
}