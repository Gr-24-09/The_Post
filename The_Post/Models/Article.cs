using System.ComponentModel.DataAnnotations;

namespace The_Post.Models
{
    public class Article
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Date Stamp")]
        public DateTime DateStamp { get; set; }

        [Required]
        [StringLength(300)]
        public string LinkText { get; set; }

        [Required]
        [Display(Name = "Editors Choice")]
        public bool EditorsChoice { get; set; }

        [Required]
        [StringLength(100)]
        public string HeadLine { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name ="Content Summary")]
        public string ContentSummary { get; set; }

        [Required]
        [StringLength(2000)]
        public string Content { get; set; }

        [Required]
        public int Views { get; set; }

        [Required]
        public int Likes { get; set; }

        [Required]
        [StringLength(500)]
        public string ImageLink { get; set; }

        //[Required]
        //[StringLength(100)]
        //public string Category { get; set; }

        [Required]
        public bool IsArchived { get; set; }

        public virtual ICollection<Category> Categories { get; set; } = new List<Category>();
    }
}

    

