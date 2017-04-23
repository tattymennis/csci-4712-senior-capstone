using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GoalManager.Data;
using System.ComponentModel.DataAnnotations;

namespace GoalManager.Models
{
    public class UpdateGoalViewModel
    {
        //References
        [Required]
        public int GIDRef { get; set; }
        public int GoalRef { get; set; }
        public int MinProgress { get; set; }


        //Form Fields
        [Required(ErrorMessage = "Please enter a progress number that best represents your update.")]
        public int Progress { get; set; }

        [Required(ErrorMessage = "Please enter a subject")]
        [StringLength(50, ErrorMessage = "Please enter a valid subject", MinimumLength = 4)]
        public string Subject { get; set; }

        [Required(ErrorMessage = "Please enter notes for this update")]
        [MaxLength(256, ErrorMessage = "Please enter a valid subject.")]
        public string Notes { get; set; }

       
        //Display Fields
        public List<Update> Updates { get; set; }
        public Goal Goal { get; set; }
    }
}