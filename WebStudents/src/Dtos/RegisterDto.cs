namespace WebStudents.Dtos;
public class RegisterDto
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Role { get; set; } = "Student";
    public Guid? LinkedPersonId { get; set; }
}
