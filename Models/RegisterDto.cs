namespace restAPI.Models;

public class RegisterDto
{
    public string Email { get; set; }
    public long CardID { get; set; }
    public string Password { get; set; }
    public int RoleID { get; set; }
    public string Role { get; set; }
}