public class User
{
    public int User_Id { get; set; }
    public string Username { get; set; }
    public string PasswordHash { get; set; }
    public string Role { get; set; } // Admin, Counselor
    public bool IsConfirmed { get; set; }
    public Counselor? Counselor { get; set; }
}
