using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace API.Models
{
    public class User
    {
        public int UserID { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [JsonIgnore]
        public string PasswordHash { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}
