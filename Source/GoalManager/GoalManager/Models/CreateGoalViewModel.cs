using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GoalManager.Data;
using System.Web.Mvc;

namespace GoalManager.Models
{
    public class CreateGoalViewModel
    {
        public Goal Goal = new Goal();
        public User Employee = new User();

        public IEnumerable<SelectListItem> CatDropDown { get; set; } 

        public IEnumerable<SelectListItem> QuartDropDown { get; set; } 
    }
}