﻿using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace The_Post.Models.VM
{
    public class AddArticleVM
    {
        [Required]
        [StringLength(100)]
        public string HeadLine { get; set; }

        [Required]
        [StringLength(300)]
        public string LinkText { get; set; }

        [Required]
        [StringLength(500)]
        [Display(Name = "Content Summary")]
        public string ContentSummary { get; set; }

        [Required]
        [StringLength(10000)]
        public string Content { get; set; }

        [Required]
        public IFormFile ImageLink { get; set; }

        public List<SelectListItem> AvailableCategories { get; set; } = new List<SelectListItem>();

        [Required]
        public List<int> SelectedCategoryIds { get; set; }
    }
}
