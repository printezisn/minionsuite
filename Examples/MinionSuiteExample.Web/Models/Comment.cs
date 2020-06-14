using System;
using System.ComponentModel.DataAnnotations;

namespace MinionSuiteExample.Web.Models
{
    public class Comment
    {
        public int Id { get; set; }

        [Required]
        public string Body { get; set; }

        [Required]
        public int PostId { get; set; }

        [Display(Name = "Created At")]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Updated At")]
        public DateTime UpdatedAt { get; set; }

        public Post Post { get; set; }
    }
}
