using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GoalManager.Data;

namespace GoalManager.Models
{
    public class SupervisorHomeViewModel
    {
        public List<Department> Departments = new List<Department>();
        public List<Goal> Goals = new List<Goal>();
        public List<Goal> GoalApprovalList = new List<Goal>();
        public List<User> Employees = new List<User>();
        public Dictionary<int, string> EmployeeDeptName = new Dictionary<int, string>();

        public List<Goal> DeniedGoals = new List<Goal>();
        public List<Goal> ActiveGoals = new List<Goal>();
        public List<Goal> FailedGoals = new List<Goal>();
        public List<Goal> PendingGoals = new List<Goal>();
        public List<Goal> CompletedGoals = new List<Goal>();
    }
}