using StudentsPerformance.Models;

namespace WebStudents.Dtos;

public class GradeSheetDto
{
    public Guid Id { get; set; }
    public Guid DisciplineOfferingId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ClosedAt { get; set; }
    public SheetStatus Status { get; set; }
}
