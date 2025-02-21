public class Notification
{
    public int Notification_Id { get; set; }
    public int UserId { get; set; } // Получатель уведомления
    public string Message { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsRead { get; set; } // Прочитано или нет

    public User User { get; set; }
}
