
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelClasses
{
    //user in asp user table ineed more fields than the default ones  this will update on the asp table user ....
    public class ApplicationUser:IdentityUser
    {
        [Required]
        [Display(Name ="First Name")]
        public string? FirstName { get; set; }
        [Required]
        [Display(Name = "Last Name")]
        public string? LastName { get; set; }
        [Required]
        public string? Address { get; set; }
        [Required]
        public string? City { get; set; }
        public string? Country { get; set; }
        [Required]
        public string? PostalCode { get; set; }
        [Required]
        public string? State { get; set; }

    }
}
