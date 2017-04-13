using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GoalManager.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoalManager.Models
{
    public class AddCategoryViewModel
    {
         public List<Category> Categories = new List<Category>();

        [Required]
        [StringLength(50)]
        public string Name { get; set; }


        public int DIDRef { get; set; }
    }
}