using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using CustomUserLogin.Enums;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;

namespace CustomUserLogin.Models
{
    public class PaymentDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public bool IsFullPaid { get; set; }

        [Required]
        public PaymentStatus Status { get; set; }
        [Required]
        public int EmployerDefaulterId { get; set; }

        [ValidateNever] // ✅ Exclude from validation
        [ForeignKey("EmployerDefaulterId")]
        public EmployerDefaulter employerDefaulter { get; set; }

        [Required]
        [DisplayName("Amount")]
        public float Money { get; set; }
     
        public DateOnly DatePaid { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow); // ✅ Default to today
    }
}
