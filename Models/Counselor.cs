using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class Counselor
{
    [Key]
    public int Counselor_Id { get; set; }

    [ForeignKey("User")]
    public int User_Id { get; set; }

    public string Surname { get; set; }
    public string Name { get; set; }
    public string Patronymic { get; set; }
    public string PhoneNumber { get; set; }

    public User User { get; set; }
}
