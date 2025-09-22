using System.ComponentModel.DataAnnotations;

namespace CodeCrewShop.Models.User
{
    public static class UserRoles
    {
        public const string Customer = "Customer";
        public const string Seller = "Seller";
        public const string Admin = "Admin";
    }
    public class Users
    {
        public int Id { get; set; }

        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Phone]
        public string? Phone { get; set; }

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = UserRoles.Customer; // Default Customer

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
}
