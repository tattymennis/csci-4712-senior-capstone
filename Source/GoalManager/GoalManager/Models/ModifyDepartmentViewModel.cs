using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace GoalManager.Models
{
    public class ModifyDepartmentViewModel
    {
        public int DID { get; set; }
        public string Name { get; set; }        
        public string Location { get; set; }
        public string Description { get; set; }
        public int IDRef { get; set; }

        public IEnumerable<SelectListItem> SupervisorsDropDown { get; set; }

        [Required]
        public string SupervisorChoice { get; set; }

        //[Required]
        //[StringLength(50)]
        //public string Quarter1Name { get; set; }

        //[Required]
        //public DateTime Quarter1Start { get; set; }
      
        //[Required]
        //public DateTime Quarter1End { get; set; }

        //[Required]
        //[StringLength(50)]
        //public string Quarter2Name { get; set; }

        //[Required]
        //public DateTime Quarter2Start { get; set; }

        //[Required]
        //public DateTime Quarter2End { get; set; }

        //[Required]
        //[StringLength(50)]
        //public string Quarter3Name { get; set; }
        //[Required]
        //public DateTime Quarter3Start { get; set; }

        //[Required]
        //public DateTime Quarter3End { get; set; }

        //[Required]
        //[StringLength(50)]
        //public string Quarter4Name { get; set; }
        //[Required]
        //public DateTime Quarter4Start { get; set; }

        //[Required]
        //public DateTime Quarter4End { get; set; }
    }
}