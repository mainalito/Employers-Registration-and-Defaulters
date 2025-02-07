using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CustomUserLogin.Enums;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;

namespace CustomUserLogin.Models;

public partial class EmployerDefaulter
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [Required]  
    public int EmployerId { get; set; }

    [ValidateNever] // ✅ Exclude from validation
    [ForeignKey("EmployerId")]
    public Employers employers { get; set; }
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Affected Employees must be greater than 0.")]
    public int AffectedEmployees { get; set; }

    [Required]
    public DateOnly ContributionMonth { get; set; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Months Run must be greater than 0.")]
    public int MonthsRun { get; set; }

    public DateOnly SurchargeDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow); // ✅ Default to today

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount Defaulted must be greater than 0.")]
    [Column(TypeName = "money")]
    public decimal AmountDefaulted { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Surcharge Due must be greater than 0.")]
    [Column(TypeName = "money")]
    public decimal SurchargeDue { get; set; }

 
    [Column(TypeName = "money")]
    public decimal TotalAmountDue { get; set; }
    
    public EmployerStatus Status { get; set; }


}
