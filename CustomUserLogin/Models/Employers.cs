namespace CustomUserLogin.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CustomUserLogin.Enums;

public class Employers
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [Column(TypeName = "nvarchar(256)")] 
    public string EnrollmentNumber { get; set; }

    [Required]
    [Column(TypeName = "nvarchar(256)")]
    public string SSNITEmployerNumber { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    public string DigitalAddress { get; set; }

    [Required, EmailAddress]
    public string Email { get; set; }

    [Required, Phone]
    public string PhoneNumber { get; set; }

    [Required]
    public EmployerStatus Status { get; set; }

    [Required]
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public string? CreatedBy { get; set; }
    public string? FileName { get; set; }
}
