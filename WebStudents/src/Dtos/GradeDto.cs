namespace WebStudents.Dtos;

public class GradeDto
{
    public Guid StudentId { get; set; }
    public int AssignmentId { get; set; }
    public double Score { get; set; }
}
