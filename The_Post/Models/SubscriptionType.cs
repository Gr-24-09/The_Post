using System.ComponentModel.DataAnnotations;

namespace The_Post.Models
{
    public class SubscriptionType
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name ="Subscription Type")]
        public string TypeName { get; set; }
        public string Description { get; set; }

        [Required]
        [DataType(DataType.Currency)]
         public decimal Price { get; set; }
    }
}
