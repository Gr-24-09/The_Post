using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace The_Post.Models
{
    public class User : IdentityUser
    {
        [Display(Name ="First Name")]
        [Required,MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Display(Name = "Last Name")]
        [Required, MaxLength(100)]
        public string LastName { get; set; }= string.Empty;


        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        public DateTime? DOB { get; set; }

        [Required(ErrorMessage = "Delivery address is required.")]
        [StringLength(50, ErrorMessage = "Delivery address can't be longer than 50 characters.")]
        
        public string Address { get; set; }

        [Required(ErrorMessage = "Delivery City is required.")]
        [StringLength(50, ErrorMessage = "Delivery city can't be longer than 50 characters.")]
        
        public string City { get; set; }

        [Required(ErrorMessage = "Delivery Zip is required.")]
        [RegularExpression(@"^\d{5}$", ErrorMessage = "Delivery zip must be a 5-digit number.")]
        public string Zip { get; set; }


        public List<Subscription> Subscriptions { get; set; } = new List<Subscription>();
    }
}
