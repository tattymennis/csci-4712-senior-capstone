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
        public IEnumerable<SelectListItem> DeptDropDown { get; set; }
        public User Employee = new User();
        //add data annotations


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

        [NotMapped]
        public int DepRefChoice { get; set; }
        [NotMapped]
        public int IDRef { get; set; } //Used for Initial entry into the page, must require an id to reference 
    }
}