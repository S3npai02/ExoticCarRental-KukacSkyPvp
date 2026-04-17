using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExoticBackend.Models
{
    public class TaxiOrder
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User? User { get; set; } 

        [Required]
        public int VehicleId { get; set; }

        [Required]
        public int DriverId { get; set; }
        

        [Required]
        [StringLength(255)]
        public string PickupLocation { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string DropoffLocation { get; set; } = string.Empty;

        [Required]
        public DateTime PickupDateTime { get; set; }


        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPrice { get; set; }

        public int Status { get; set; } = 1;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}