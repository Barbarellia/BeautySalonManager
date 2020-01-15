using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BeautySalonManager.Models
{
    public class AppUser : IdentityUser<int>
    {
        [Required]
        [Display(Name = "Imię")]
        public string FirstName { get; set; }
        [Required]
        [Display(Name = "Nazwisko")]
        public string LastName { get; set; }
        [Display(Name = "Numer tel.")]
        public string PhoneNumber { get; set; }
    }
}
