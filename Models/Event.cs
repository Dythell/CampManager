public class Event
{
    public int EventId { get; set; }
    public int SessionId { get; set; }
    public int? EventTemplateId { get; set; } // Может быть null для кастомных событий
    public string? CustomName { get; set; } // Название кастомного мероприятия
    public bool IsCustomEvent { get; set; }  // Является ли мероприятие кастомным
    public string Type { get; set; }  // "Спортивное", "Творческое", "Общелагерное"
    public DateTime DateTime { get; set; }
    public string Status { get; set; }  // "Запланировано", "Выполнено", "Отменено"
    public int CounselorId { get; set; } // Ответственный вожатый

    public Session Session { get; set; }
    public Counselor Counselor { get; set; }
}
