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
        public ActionResult CreateEmployee(User tempuser)
        {
            User dbuser = new User(); //user to be added to the database
            //Validation for Each Field
            //First Name 
            if (String.IsNullOrWhiteSpace(tempuser.FirstName))
            {
                ModelState.AddModelError("FirstName", "First name can not be blank.");
            }
            else
            {
                foreach (char x in tempuser.FirstName)
                {
                    if (System.Char.IsDigit(x) || Char.IsControl(x) || Char.IsPunctuation(x) || Char.IsSymbol(x))
                    {
                        ModelState.AddModelError("FirstName", "First name can only contain characters A-z.");
                        break;
                    }
                }
                dbuser.FirstName = tempuser.FirstName;
            }

            //Last Name
            if (String.IsNullOrWhiteSpace(tempuser.LastName))
            {
                ModelState.AddModelError("LastName", "Last name can not be blank.");
            }

            else
            {
                foreach (char x in tempuser.LastName)
                {
                    if (System.Char.IsDigit(x) || Char.IsControl(x) || Char.IsPunctuation(x) || Char.IsSymbol(x))
                    {
                        ModelState.AddModelError("LastName", "Last name can only contain A-z.");
                        break;
                    }
                }
                dbuser.LastName = tempuser.LastName;
            }

            //Email is error check by the html web browser ahead of time if valid email address
            if (String.IsNullOrWhiteSpace(tempuser.Email))
            {
                ModelState.AddModelError("Email", "You Must Enter an Email Address");
            }

            else // TODO: Regex email address?
            {
                dbuser.Email = tempuser.Email;
            }

            //Title
            if (String.IsNullOrWhiteSpace(tempuser.Title))
            {
                ModelState.AddModelError("Title", "Employee must have a Title");
            }

            else
            {
                //Title can have a digit, e.g "Section 8 Specialist"
                foreach (char x in tempuser.Title)
                {
                    if (Char.IsDigit(x) || Char.IsControl(x) || Char.IsPunctuation(x) || Char.IsSymbol(x))
                    {
                        ModelState.AddModelError("Title", "Title can only contain A-Z or numbers");
                        break;
                    }
                }
                dbuser.Title = tempuser.Title;
            }

            //Role
            if (tempuser.Role == "0" || String.IsNullOrWhiteSpace(tempuser.Role))
            {
                ModelState.AddModelError("Role", "Must Select a Role");
            }

            else
            {
                dbuser.Role = tempuser.Role;
            }

            //Department
            if(tempuser.DepRefChoice == 0) //This field is enclosed under value="0" as a default choice, client side only able to choose default or valid. 
            {
                ModelState.AddModelError("DepRefChoice", "Must Select a Department");
            }
            //If no errors, Add to the Database
            if (ModelState.IsValid)
            {
                using (var db = new UserDBEntities())
                {                    
                    // generate username
                    int count = 1;
                    string username = (tempuser.FirstName[0] + tempuser.LastName).ToLower();
                    while (db.Users.Any(x => x.Username == username) || count > 16) // username collision
                    {
                        username += count.ToString();
                        count++;
                    }
                    dbuser.Username = username;

                    dbuser.Department = db.Departments.Where(x => x.DID == tempuser.DepRefChoice).FirstOrDefault();
                    db.Users.Add(dbuser);

                    db.SaveChanges();
                }
                RedirectToAction("~/Home/Index");
            }

            // Active, Not able to bind to value, must discuss next video chat, or default to true
           
            CreateEmployeeViewModel nvm = new CreateEmployeeViewModel();
            nvm.Employee = tempuser;
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
          //  ViewData["DeptDropDown"] = vm.DeptDropDown;

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

            User dbuser = new User(); //user to be added to the database
            //Validation for Each Field
            //First Name 
            if (String.IsNullOrWhiteSpace(tempuser.FirstName))
            {
                ModelState.AddModelError("FirstName", "First name can not be blank.");
            }
            else
            {
                foreach (char x in tempuser.FirstName)
                {
                    if (System.Char.IsDigit(x) || Char.IsControl(x) || Char.IsPunctuation(x) || Char.IsSymbol(x))
                    {
                        ModelState.AddModelError("FirstName", "First name can only contain characters A-z.");
                        break;
                    }
                }
                dbuser.FirstName = tempuser.FirstName;
            }

            //Last Name
            if (String.IsNullOrWhiteSpace(tempuser.LastName))
            {
                ModelState.AddModelError("LastName", "Last name can not be blank.");
            }

            else
            {
                foreach (char x in tempuser.LastName)
                {
                    if (System.Char.IsDigit(x) || Char.IsControl(x) || Char.IsPunctuation(x) || Char.IsSymbol(x))
                    {
                        ModelState.AddModelError("LastName", "Last name can only contain A-z.");
                        break;
                    }
                }
                dbuser.LastName = tempuser.LastName;
            }

            //Email is error check by the html web browser ahead of time if valid email address
            if (String.IsNullOrWhiteSpace(tempuser.Email))
            {
                ModelState.AddModelError("Email", "You Must Enter an Email Address");
            }

            else // TODO: Regex email address?
            {
                dbuser.Email = tempuser.Email;
            }

            //Title
            if (String.IsNullOrWhiteSpace(tempuser.Title))
            {
                ModelState.AddModelError("Title", "Employee must have a Title");
            }

            else
            {
                //Title can have a digit, e.g "Section 8 Specialist"
                foreach (char x in tempuser.Title)
                {
                    if (Char.IsDigit(x) || Char.IsControl(x) || Char.IsPunctuation(x) || Char.IsSymbol(x))
                    {
                        ModelState.AddModelError("Title", "Title can only contain A-Z or numbers");
                        break;
                    }
                }
                dbuser.Title = tempuser.Title;
            }

            //Role
            if (tempuser.Role == "0" || String.IsNullOrWhiteSpace(tempuser.Role))
            {
                ModelState.AddModelError("Role", "Must Select a Role");
            }

            else
            {
                dbuser.Role = tempuser.Role;
            }

            //Department
            if (tempuser.DepRefChoice == 0) //This field is enclosed under value="0" as a default choice, client side only able to choose default or valid. 
            {
                ModelState.AddModelError("DepRefChoice", "Must Select a Department");
            }
            //If no errors, Add to the Database
            if (ModelState.IsValid)
            {
                using (var db = new UserDBEntities())
                {
                    // generate username

                    User User = db.Users.Find(tempuser.UID);
                    //TryUpdateModel
                    //if(TryUpdateModel(fields))

                    dbuser.Department = db.Departments.Where(x => x.DID == tempuser.DepRefChoice).FirstOrDefault();
                    db.Users.Add(dbuser);

                    db.SaveChanges();
                }
                RedirectToAction("~/Home/Index");
            }


            CreateEmployeeViewModel nvm = new CreateEmployeeViewModel();
            nvm.Employee = tempuser;
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
            //  ViewData["DeptDropDown"] = vm.DeptDropDown;

            return View(nvm);
        }
    }
}