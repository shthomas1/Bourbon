using System;
using System.ComponentModel.DataAnnotations;

using System;
using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class UserBlog
    {
        [Key]
        public int BlogID { get; set; }

        [Required]
        public int UserID { get; set; } // Foreign Key to Users table

        [Required]
        [MaxLength(255)]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedDate { get; set; } // Nullable for edits

        // Additional fields for author information
        public string AuthorFirstName { get; set; }
        public string AuthorLastName { get; set; }
    }
}

