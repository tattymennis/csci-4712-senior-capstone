using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GoalManager.Data;
using System.Web.Mvc;

namespace GoalManager.Models
{
    public class CreateDepartmentViewModel
    {
        //References

        //Display Fields
        public IEnumerable<SelectListItem> SupervisorsDropDown { get; set; }

        //Form Fields
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        [StringLength(50)]
        public string Location { get; set; }

        [Required]
        [StringLength(255)]
        public string Description { get; set; }
        
        [Required]
        public string SupervisorChoice { get; set; }

        [Required]
        [StringLength(50)]
        public string Quarter1Name { get; set; }

        [Required]
        public DateTime Quarter1Start { get; set; }

        [Required]
        public DateTime Quarter1End { get; set; }

        [Required]
        [StringLength(50)]
        public string Quarter2Name { get; set; }

        [Required]
        public DateTime Quarter2Start { get; set; }

        [Required]
        public DateTime Quarter2End { get; set; }

        [Required]
        [StringLength(50)]
        public string Quarter3Name { get; set; }
        [Required]
        public DateTime Quarter3Start { get; set; }

        [Required]
        public DateTime Quarter3End { get; set; }

        [Required]
        [StringLength(50)]
        public string Quarter4Name { get; set; }
        [Required]
        public DateTime Quarter4Start { get; set; }

        [Required]
        public DateTime Quarter4End { get; set; }

        [Required]
        public string Category1Name { get; set; }

        public string Category2Name { get; set; }

        public string Category3Name { get; set; }

        public string Category4Name { get; set; }
    }

}