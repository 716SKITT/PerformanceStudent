namespace WebStudents.Dtos;

public class FinalGradeDto
{
    public Guid Id { get; set; }
    public Guid GradeSheetId { get; set; }
    public Guid StudentId { get; set; }
    public decimal? FinalScore { get; set; }
    public string? FinalMark { get; set; }
    public DateTime UpdatedAt { get; set; }
}
