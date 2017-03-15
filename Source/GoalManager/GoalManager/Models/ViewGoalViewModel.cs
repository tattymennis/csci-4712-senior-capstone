using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GoalManager.Data;

namespace GoalManager.Models
{
    public class ViewGoalViewModel
    {
        public int GID { get; set; }
        public Goal Goal = new Goal();
        public List<Update> Updates = new List<Update>();
        // public User Employee = new User();
        // public DateTime Date = new DateTime();
    }
}