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

        public List<Subscription> Subscriptions { get; set; } = new List<Subscription>();
    }
}
