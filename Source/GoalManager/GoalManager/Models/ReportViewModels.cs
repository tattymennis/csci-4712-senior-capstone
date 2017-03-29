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
        public Department Department = new Department();
        public List<User> Employees = new List<User>();
        public Dictionary<int, List<Goal>> EmployeeGoals = new Dictionary<int, List<Goal>>();
        public Dictionary<int, List<Update>> GoalUpdates = new Dictionary<int, List<Update>>();
        //public string DeptRefKey { get; set; }
        public int DeptRefID { get; set; }
    }

    public class ViewEmployeeReportViewModel
    {
        public User Employee = new User();
        public List<Goal> Goals = new List<Goal>();
        public int EmployeeRefID = -1;
    }
}