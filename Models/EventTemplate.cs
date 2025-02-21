public class EventTemplate
{
    public int EventTemplate_Id { get; set; }
    public string Name { get; set; }
    public string Type { get; set; } // "Спортивное", "Творческое", "Общелагерное"
    public string DefaultDescription { get; set; } // Описание по умолчанию
}
