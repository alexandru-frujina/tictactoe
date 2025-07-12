public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; }  // Store hashed password!
    public bool Connected { get; set; } = false;
}