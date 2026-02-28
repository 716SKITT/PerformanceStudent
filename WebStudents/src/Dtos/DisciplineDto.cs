using StudentsPerformance.Models;

namespace WebStudents.Dtos;

public class DisciplineDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Hours { get; set; }
    public ControlType ControlType { get; set; }
}
