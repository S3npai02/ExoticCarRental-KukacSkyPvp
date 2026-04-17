using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExoticBackend.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty; 

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Phone]
        public string PhoneNumber { get; set; } = string.Empty;
        public byte[]? ProfilePicture { get; set; }

        public string? ResetPasswordToken { get; set; }
        public DateTime? ResetPasswordExpiry { get; set; }

        public string? FullName { get; set; } = string.Empty;

        public DateTime? DateOfBirth { get; set; }

        public string? LicenseNumber { get; set; } = string.Empty;

        public DateTime? LicenseExpiryDate { get; set; }
        public bool isDriver {  get; set; }

        public int Clearance { get; set; } = 1;

        public DateTime Created_At { get; set; } = DateTime.UtcNow;

        [Column("verification_token")]
        public string? VerificationToken { get; set; }
        [Column("is_verified")]
        public int Is_Verified { get; set; }
        public List<Order> Orders { get; set; } = new List<Order>();
    }
}