﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MinionSuiteExample.Web.Models
{
    public class Post
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(250)]
        public string Title { get; set; }

        [Required]
        public string Body { get; set; }

        [Display(Name = "Created At")]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Updated At")]
        public DateTime UpdatedAt { get; set; }

        public IList<Comment> Comments { get; set; }
    }
}
