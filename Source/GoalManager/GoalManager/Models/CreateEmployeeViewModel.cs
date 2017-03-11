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