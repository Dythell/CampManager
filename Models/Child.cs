public class Child
{
    public int ChildId { get; set; }
    public string Surname { get; set; }
    public string Name { get; set; }
    public string Patronymic { get; set; }
    public DateTime BirthYear { get; set; }
    public string ParentNumber { get; set; }

    public int GroupId { get; set; }
    public Group Group { get; set; }
}
