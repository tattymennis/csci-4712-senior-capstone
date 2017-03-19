using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GoalManager.Data;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Web.Mvc;

namespace GoalManager.Models
{
    public class ModifyEmployeeViewModel
    {
        // Drop Downs
        public IEnumerable<SelectListItem> DeptDropDown { get; set; }
        public IEnumerable<SelectListItem> RoleDropDown { get; set; }
        public IEnumerable<SelectListItem> ActiveDropDown { get; set; }

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

        public int DepRefChoice = 0;

        [Required]
        public bool Active { get; set; }

        [NotMapped]
        public int IDRef { get; set; } //Used for Initial entry into the page, must require an id to reference 

        public int ID { get; set; }
    }
}