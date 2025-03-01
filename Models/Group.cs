using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Group
{
    [Key]
    public int Group_Id { get; set; }

    public string Name { get; set; }

    public int Number { get; set; } // Номер отряда

    [ForeignKey("SessionCounselor")]
    public int SessionCounselor_Id { get; set; }

    [ForeignKey("Session")]
    public int SessionId { get; set; }

    public SessionCounselor SessionCounselor { get; set; }
    public Session Session { get; set; }
}
