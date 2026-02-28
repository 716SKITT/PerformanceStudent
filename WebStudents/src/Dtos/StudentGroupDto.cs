namespace WebStudents.Dtos;

public class StudentGroupDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int StudyYear { get; set; }
    public Guid AcademicYearId { get; set; }
}
