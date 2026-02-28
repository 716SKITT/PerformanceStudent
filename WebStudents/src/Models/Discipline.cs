using System.ComponentModel.DataAnnotations;

namespace StudentsPerformance.Models;

public class Discipline
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [Range(1, 500)]
    public int Hours { get; set; }

    public ControlType ControlType { get; set; }

    public ICollection<DisciplineOffering> DisciplineOfferings { get; set; } = new List<DisciplineOffering>();
}
