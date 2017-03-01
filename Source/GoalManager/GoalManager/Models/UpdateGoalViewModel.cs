using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GoalManager.Data;

namespace GoalManager.Models
{
    public class UpdateGoalViewModel
    {
        public Goal Update = new Goal();
        public User Employee = new User();
    }
}