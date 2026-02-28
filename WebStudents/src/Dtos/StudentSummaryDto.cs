namespace WebStudents.Dtos;

public class StudentSummaryDto
{
    public Guid StudentId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public List<FinalGradeSummaryItemDto> Finals { get; set; } = new();
}

public class FinalGradeSummaryItemDto
{
    public Guid GradeSheetId { get; set; }
    public string DisciplineName { get; set; } = string.Empty;
    public int SemesterNumber { get; set; }
    public int AcademicYearStart { get; set; }
    public int AcademicYearEnd { get; set; }
    public decimal? FinalScore { get; set; }
    public string? FinalMark { get; set; }
    public DateTime UpdatedAt { get; set; }
}
