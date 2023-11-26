﻿using PesKit.Models;
using System.ComponentModel.DataAnnotations;

namespace PesKit.Areas.PestKitAdmin.ViewModels
{
    public class CreateBlogVM
    {
        [Required(ErrorMessage = "Title must be entered mutled")]
        [MaxLength(50, ErrorMessage = "It should not exceed 25 characters")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Descriptoin must be entered mutled")]
        [MaxLength(100, ErrorMessage = "It should not exceed 100 characters")]
        public string Description { get; set; }
        public DateTime DateTime { get; set; }
        [Required]
        public IFormFile Photo { get; set; }
        [Required]
        public int? AuthorId { get; set; }
        [Required]
        public int CommentCount { get; set; }
    }
}
