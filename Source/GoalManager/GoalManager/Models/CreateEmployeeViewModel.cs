using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GoalManager.Data;
using System.ComponentModel; // for [DisplayName]
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc; // for SelectListItem

namespace GoalManager.Models
{
    public class CreateEmployeeViewModel
    {
        public IEnumerable<SelectListItem> DeptDropDown { get; set; }

        public User Employee = new User();
        // Add Data Notations
        public int UID { get; set; }
        public string Email { get; set; }
        public string Title { get; set; }
        public string Role { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int DID { get; set; }
        public bool Active { get; set; }
        public Nullable<int> SUID { get; set; }
        public string Username { get; set; }

        //Not Mapped
        public int DepRefChoice { get; set; }
    }

    public class RegisterEmployeeViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required]
        public string Role { get; set; }
    }
}