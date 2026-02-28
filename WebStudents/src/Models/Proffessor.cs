using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentsPerformance.Models;

public class Proffessor
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    public Genders Gender { get; set; }

    [Required]
    public DateTime EnrollmentDate { get; set; }

    [Required]
    public DateTime DateOfBirth { get; set; }

    public int? CourseId { get; set; }

    [ForeignKey("CourseId")]
    public Course? Course { get; set; }

    public ICollection<DisciplineOffering> DisciplineOfferings { get; set; } = new List<DisciplineOffering>();

    public Proffessor() { }

    public Proffessor(string firstName, string lastName, DateTime dateOfBirth, Genders gender, int? courseId)
    {
        Id = Guid.NewGuid();
        FirstName = firstName;
        LastName = lastName;
        DateOfBirth = dateOfBirth;
        EnrollmentDate = DateTime.UtcNow;
        Gender = gender;
        CourseId = courseId;
    }

    public enum StudentGender
    {
        Woman,
        Man
    }
}
