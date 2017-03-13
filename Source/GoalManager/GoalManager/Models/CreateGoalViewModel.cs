﻿using System;
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
        [Required(ErrorMessage = "Title is required")]
        [Display(Name = "Title")]
        [StringLength(50, ErrorMessage = "Title must be between 1-50 characters.", MinimumLength = 1)]
        public string Title { get; set; }

        [Display(Name = "Description")]
        [MaxLength(256, ErrorMessage = "Description cannot be longer than 256 characters.")]
        public string Description { get; set; }

        public Goal Goal = new Goal();
        public User Employee = new User();

        public IEnumerable<SelectListItem> CatDropDown { get; set; } 

        public IEnumerable<SelectListItem> QuartDropDown { get; set; } 
    }
}