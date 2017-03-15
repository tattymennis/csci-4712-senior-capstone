using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GoalManager.Data;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace GoalManager.Models
{
    public class CreateGoalViewModel
    {
        // Form fields
        [Required(ErrorMessage = "Title is required")]
        [Display(Name = "Title")]
        [StringLength(50, ErrorMessage = "Title must be between 1-50 characters.", MinimumLength = 1)]
        public string Title { get; set; }

        [Display(Name = "Description")]
        [MaxLength(256, ErrorMessage = "Description cannot be longer than 256 characters.")]
        public string Description { get; set; }

        // Navigation properties
        public Goal Goal = new Goal();
        public User Employee = new User();

        // Drop Down
        public string CategoryName { get; set; }
        public IEnumerable<SelectListItem> CatDropDown { get; set; } 

        public string QuarterTime { get; set; } // parse to DateTime
        public IEnumerable<SelectListItem> QuartDropDown { get; set; } 
    }
}