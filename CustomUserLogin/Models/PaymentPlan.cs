using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using CustomUserLogin.Enums;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace CustomUserLogin.Models
{
    public class PaymentPlan
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public int Installments { get; set; }
        [Required]
        public float Amount { get; set; }
        [Required]
        [StringLength(1000)]
        public String Reasons { get; set; }
        public PaymentPlanStatus Status { get; set; }
        [Required]
        public int EmployerDefaulterId { get; set; }

        [ValidateNever] // ✅ Exclude from validation
        [ForeignKey("EmployerDefaulterId")]
        public EmployerDefaulter employerDefaulter { get; set; }

    }
}
