public class SessionCounselor
{
    public int SessionCounselorId { get; set; }
    public int SessionId { get; set; }
    public int CounselorId { get; set; }

    public Session Session { get; set; }
    public Counselor Counselor { get; set; }
}
