using System.ComponentModel.DataAnnotations;

namespace The_Post.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
         public string Name { get; set; }

        public virtual Article Article { get; set; }
    }
}
