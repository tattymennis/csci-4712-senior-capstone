using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GoalManager.Data;
using System.ComponentModel; // for [DisplayName]
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc; // for SelectListItem

namespace GoalManager.Models
{
    public class CreateEmployeeViewModel
    {
        // Drop Downs
        public IEnumerable<SelectListItem> DeptDropDown { get; set; }

        //Form Fields 
        [Required]
        [DisplayName("First Name")]
        [StringLength(50)]
        public string FirstName { get; set; }
        [Required]
        [DisplayName("Last name")]
        [StringLength(50)]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        [Required]
        public string Title { get; set; }

        [Required]
        public string Role { get; set; }
       
        [Required]
        [MaxLength(255)]
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