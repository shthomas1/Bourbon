using System;
using Microsoft.AspNetCore.Identity;

namespace API.Models
{
    public class UserService
    {
        private readonly PasswordHasher<User> hasher = new PasswordHasher<User>();

        public User CreateUserWithHashedPassword(string email, string firstName, string lastName, string plainTextPassword)
        {
            var user = new User
            {
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                CreatedDate = DateTime.UtcNow
            };

            // Generate the hashed password
            user.PasswordHash = hasher.HashPassword(user, plainTextPassword);

            // Save 'user' to the database (e.g., via an EF Core DbContext or raw SQL)

            return user;
        }

        public bool VerifyPassword(User user, string plainTextPassword)
        {
            var result = hasher.VerifyHashedPassword(user, user.PasswordHash, plainTextPassword);
            return result == PasswordVerificationResult.Success;
        }
    }
}
