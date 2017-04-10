using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GoalManager.Data;

namespace GoalManager.Models
{
    //public class ReportViewModels
    //{
    //}
    public class ViewDepartmentReportViewModel
    {
        // get/set?
        public User Supervisor { get; set; }
        public Department Department = new Department();
        public List<User> Employees = new List<User>();
        public Dictionary<int, List<Goal>> EmployeeGoals = new Dictionary<int, List<Goal>>();
        public Dictionary<int, List<Update>> GoalUpdates = new Dictionary<int, List<Update>>();
        public int DeptRefID { get; set; }
    }

    public class ViewEmployeeReportViewModel
    {
        public User Employee = new User();
        public Department Department { get; set; }
        public List<Goal> PendingGoals = new List<Goal>();
        public List<Goal> FailedGoals = new List<Goal>();
        public List<Goal> DeniedGoals = new List<Goal>();
        public List<Goal> ActiveGoals = new List<Goal>();
        public Dictionary<int, List<Update>> EmployeeUpdates = new Dictionary<int, List<Update>>();
        public int EmployeeRefID { get; set; }
    }
}