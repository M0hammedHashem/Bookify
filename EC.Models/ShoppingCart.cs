using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Models
{
    public class ShoppingCart
    {
        public int ID { get; set; }
        [ForeignKey("Product")]

        public int ProductID { get; set; }
        [ValidateNever]
        public Product Product { get; set; }
        [Range(1,1000,ErrorMessage ="Please enter value between 1 and 1000")]
        public int Count { get; set; }
        [ForeignKey("ApplicationUser")]
        [NotMapped]
        public double Price { get; set; }

        public string ApplicationUserID { get; set; }
        [ValidateNever]
        public ApplicationUser ApplicationUser { get; set; }

    }
}
