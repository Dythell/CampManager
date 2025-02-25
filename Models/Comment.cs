using System.ComponentModel.DataAnnotations;
public class Comment
{
    [Key]
    public int Comment_Id { get; set; }
    public string Message { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int User_Id { get; set; }
    public User User { get; set; }

    public int Event_Id { get; set; }
    public Event Event { get; set; }
}
