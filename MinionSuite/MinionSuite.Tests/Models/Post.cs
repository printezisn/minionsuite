using System;
namespace MinionSuite.Tests.Models
{
    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public int TotalViews { get; set; }
        public decimal Rating { get; set; }
        public Guid Signature { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
