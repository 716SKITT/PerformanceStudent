namespace WebStudents.Dtos;

public class CourseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<StudentDto> Students { get; set; } = new();
}