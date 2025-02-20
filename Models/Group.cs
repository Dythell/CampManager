public class Group
{
    public int GroupId { get; set; }
    public int Number { get; set; } // Номер отряда (1-10)
    public int SessionId { get; set; }

    public Session Session { get; set; }
}
