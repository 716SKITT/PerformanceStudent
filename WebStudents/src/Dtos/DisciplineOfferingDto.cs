namespace WebStudents.Dtos;

public class DisciplineOfferingDto
{
    public Guid Id { get; set; }
    public Guid DisciplineId { get; set; }
    public Guid SemesterId { get; set; }
    public Guid StudentGroupId { get; set; }
    public Guid ProffessorId { get; set; }
}
