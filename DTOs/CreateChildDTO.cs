public record CreateChildDTO(
    string Surname,
    string Name,
    string Patronymic,
    DateTime BirthYear,
    string ParentNumber,
    int GroupId
)
{
    public DateTime BirthYearUtc => DateTime.SpecifyKind(BirthYear, DateTimeKind.Utc);
}
