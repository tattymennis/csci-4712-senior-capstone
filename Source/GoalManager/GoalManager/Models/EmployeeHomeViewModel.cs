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
        public List<Update> Updates = new List<Update>();
        
    }
}