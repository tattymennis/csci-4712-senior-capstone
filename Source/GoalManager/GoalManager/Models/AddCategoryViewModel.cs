using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GoalManager.Data;

namespace GoalManager.Models
{
    public class AddCategoryViewModel
    {
        List<Category> Categories = new List<Category>();
        public string Name { get; set; }
        public int DIDRef { get; set; }
    }
}