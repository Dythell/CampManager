public class SessionCounselor
{
    public int SessionCounselor_Id { get; set; }
    public int SessionId { get; set; }
    public int CounselorId { get; set; }

    public Session Session { get; set; }
    public Counselor Counselor { get; set; }
}
