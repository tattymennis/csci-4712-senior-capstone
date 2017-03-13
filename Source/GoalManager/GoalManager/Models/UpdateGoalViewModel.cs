using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GoalManager.Data;

namespace GoalManager.Models
{
    public class UpdateGoalViewModel
    {
        public int GID { get; set; }
        public Update Update { get; set; }
        public User Employee = new User();
    }
}