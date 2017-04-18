using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GoalManager.Data;

namespace GoalManager.Models
{
    public class EmployeeHomeViewModel
    {
        public List<Goal> Goals = new List<Goal>();
        public List<Goal> DeniedGoals = new List<Goal>();
        public List<Goal> ActiveGoals = new List<Goal>();
        public List<Goal> FailedGoals = new List<Goal>();
        public List<Goal> PendingGoals = new List<Goal>();       
    }
}