using Microsoft.AspNetCore.Identity;
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
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string Name { get; set; }
        public string? StreetAddrees { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PostalCode { get; set; }

        [ForeignKey("Company")]
        public int? CompanyID { get; set; }
        [ValidateNever]
        public Company ?Company { get; set; }
        [NotMapped]
        public string Role { get; set; }
    }
}
/*
 SLY
 TAN
 TOT
7 
17-x*(4/10) = 42.5
4x = (17-42.5) * 10
2x 
 */