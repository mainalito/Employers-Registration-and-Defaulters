using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class EntityRoleType
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]  // This makes Id auto-increment

    public int Id { get; set; }  // Changed from "id" to "Id"

    [Required]
    [StringLength(50)]
    public string Name { get; set; }
}
