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
            List<Department> l;
            List<SelectListItem> tempList = new List<SelectListItem>();
            tempList.Add(new SelectListItem { Value = "default", Text = "Select a Department", Selected = true });

            using (var context = new UserDBEntities())
            {
                l = new List<Department>();
                var depts = context.Departments.Select(x => x);
                foreach (Department d in depts)
                {
                    l.Add(d);
                    tempList.Add(new SelectListItem { Value = d.Name, Text = d.Name, Selected = false });
                }
                vm.Departments = l;
            }

            vm.DeptDropDown = tempList;
            ViewData["DeptDropDown"] = vm.DeptDropDown;
            return View(vm);
        }
            

        [HttpPost]
        public ActionResult CreateEmployee(User u, CreateEmployeeViewModel vm)
        {
            User user = new Data.User();
            ViewData["DeptDropDown"] = vm.DeptDropDown; // I'm just here so I don't get fined
            //u.Department = vm.SelectedDept;

            //Validation for Each Field
            //First Name 
            if (String.IsNullOrWhiteSpace(u.FirstName))
            {
                ModelState.AddModelError("FirstName", "First name can not be blank.");
            }

            else
            {
                foreach (char x in u.FirstName)
                {
                    if (System.Char.IsDigit(x) || Char.IsControl(x) || Char.IsPunctuation(x) || Char.IsSymbol(x))
                    {
                        ModelState.AddModelError("FirstName", "First name can only contain characters A-z.");
                        break;
                    }
                }
                user.FirstName = u.FirstName;
            }

            //Last Name
            if (String.IsNullOrWhiteSpace(u.LastName))
            {
                ModelState.AddModelError("LastName", "Last name can not be blank.");
            }

            else
            {
                foreach (char x in u.LastName)
                {
                    if (System.Char.IsDigit(x) || Char.IsControl(x) || Char.IsPunctuation(x) || Char.IsSymbol(x))
                    {
                        ModelState.AddModelError("LastName", "Last name can only contain A-z.");
                        break;
                    }
                }
                user.LastName = u.LastName;
            }

            //Email is error check by the html web browser ahead of time if valid email address
            if (String.IsNullOrWhiteSpace(u.Email))
            {
                ModelState.AddModelError("Email", "Email is invalid.");
            }

            else // TODO: Regex email address?
            {
                user.Email = u.Email;
            }

            //Title
            if (String.IsNullOrWhiteSpace(u.Title))
            {
                ModelState.AddModelError("Title", "Employee must have a Title");
            }

            else
            {
                foreach (char x in u.Title)
                {
                    if (System.Char.IsDigit(x) || Char.IsControl(x) || Char.IsPunctuation(x) || Char.IsSymbol(x))
                    {
                        ModelState.AddModelError("Title", "Title can only contain A-z.");
                        break;
                    }
                }
                user.Title = u.Title;
            }

            //Role
            if (u.Role == "Please Select Role" || String.IsNullOrWhiteSpace(u.Role))
            {
                ModelState.AddModelError("Role", "Must Select a Role");
            }

            else
            {
                user.Role = u.Role;
            }

            ////Department, Might need to change based on, value reference or the way the departments are put pulled form the database, currently working
            //if(tempuser.DID == 0) //Must pull department list, to compare the foreign key of departments
            //{
            //    ModelState.AddModelError("DID", "Must Select a Department");
            //}

            //If no errors, Add to the Database
            if (ModelState.IsValid)
            {
                using (var db = new UserDBEntities())
                {
                    //User user = new Data.User();
                    //user.FirstName = "Andrew";
                    //user.LastName = "AssignDID";
                    //user.Email = "abcd@letme.in";
                    //user.Title = "Title";
                    //user.Role = "Role";
                    //user.Username = "AnotherTester";
                    //user.Active = true;
                    //user.UID = 1000;
                    //Department dept = db.Departments.Where(x => x.DID == 1).FirstOrDefault();

                    // get department choice from drop down field
                    if (String.IsNullOrEmpty(vm.DeptChoice))
                    {
                        ModelState.AddModelError("InvalidDeptChoice", "Department choice is null.");
                    }

                    // generate username
                    int count = 1;
                    string uname = user.FirstName[0] + user.LastName;
                    while (db.Users.Any(x => x.Username == uname) || count > 16) // username collision
                    {
                        uname += count.ToString();
                        count++;
                    }
                    user.Username = uname;

                    user.Department = db.Departments.Where(x => x.Name == vm.DeptChoice).FirstOrDefault();
                    db.Users.Add(user);

                    db.SaveChanges();
                }
            }

            RedirectToAction("/Home/Index");


            // Active, Not able to bind to value, must discuss next video chat, or default to true
           
            CreateEmployeeViewModel nvm = new CreateEmployeeViewModel();
            nvm.Employee = user;
            nvm.DeptDropDown = vm.DeptDropDown;
            return View(nvm);
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
            //FirstName
            if (tempuser.FirstName == null)
            {
                ModelState.AddModelError("FirstName", "First Name Can Not Be Blank");
            }
            if (tempuser.FirstName != null)
            {
                foreach (char x in tempuser.FirstName)
                {

                    if (System.Char.IsDigit(x))
                    {
                        ModelState.AddModelError("FirstName", "First Name Can Not Contains a Number");
                        break;
                    }

                }
            }
            //Last Name
            if (tempuser.LastName == null)
            {
                ModelState.AddModelError("LastName", "Last Name Can Not Be Blank");
            }
            if (tempuser.LastName != null)
            {
                foreach (char x in tempuser.LastName)
                {

                    if (System.Char.IsDigit(x))
                    {
                        ModelState.AddModelError("LastName", "Last Name Can Not Contains a Number");
                        break;
                    }

                }
            }
            //Email is error check by the html web browser ahead of time if valid email address
            if (tempuser.Email == null)
            {
                ModelState.AddModelError("Email", "Email Can Not Be Black");
            }

            //Title
            if (tempuser.Title == null)
            {
                ModelState.AddModelError("Title", "Employee must have a Title");
            }

            //Role
            if (tempuser.Role == "Please Select Role")
            {
                ModelState.AddModelError("Role", "Must Select a Role");
            }

            //Department, Might need to change based on, value reference or the way the departments are put pulled form the database, currently working
            if (tempuser.DID == 0) //Must pull department list, to compare the foreign key of departments
            {
                ModelState.AddModelError("DID", "Must Select a Department");
            }
            /* 
             * if (ModelState.IsValid == true)
            {
                using (var db = new UserDBEntities())
                {
                    //Find user, save changes for that user
                    db.SaveChangesAsync();
                }
                RedirectToAction("/Home/Index");
            }
            */
            var vm = new ModifyEmployeeViewModel();
            return View(vm);
        }
    }
}