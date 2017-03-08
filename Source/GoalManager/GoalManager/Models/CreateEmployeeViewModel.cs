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
}