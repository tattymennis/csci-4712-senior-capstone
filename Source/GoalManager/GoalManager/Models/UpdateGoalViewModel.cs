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
        [Required]
        public int GIDRef { get; set; }

        [Required(ErrorMessage = "Please enter a percentage completion.")]
        [Range(0,100,ErrorMessage = "Please enter a number between 0-100.")]
        public int Progress { get; set; }

        [Required(ErrorMessage = "Please enter a subject.")]
        [StringLength(50, ErrorMessage = "Please enter a valid subject.", MinimumLength = 4)]
        public string Subject { get; set; }

        [MaxLength(256, ErrorMessage = "Please enter a valid subject.")]
        public string Notes { get; set; }

        public List<Update> Updates { get; set; }
        public Goal Goal { get; set; }
    }
}