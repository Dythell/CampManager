public class SessionChild
{
    public int SessionChild_Id { get; set; }
    public int SessionId { get; set; }
    public int ChildId { get; set; }

    public Session Session { get; set; }
    public Child Child { get; set; }
}
