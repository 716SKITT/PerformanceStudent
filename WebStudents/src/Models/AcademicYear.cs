using System.ComponentModel.DataAnnotations;

namespace StudentsPerformance.Models;

public class AcademicYear
{
    [Key]
    public Guid Id { get; set; }

    [Range(2000, 3000)]
    public int StartYear { get; set; }

    [Range(2001, 3001)]
    public int EndYear { get; set; }

    public ICollection<Semester> Semesters { get; set; } = new List<Semester>();
    public ICollection<StudentGroup> StudentGroups { get; set; } = new List<StudentGroup>();
}
