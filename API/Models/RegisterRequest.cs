using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class RegisterRequest
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Password { get; set; } // Accepts plain text password
    }
}
