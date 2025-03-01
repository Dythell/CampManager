public class Event
{
    public int Event_Id { get; set; }
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
    public List<Comment> Comments { get; set; } = new List<Comment>();
    public EventTemplate? EventTemplate { get; set; }
}

//Есть 2 варианта мероприятия:
//1) по шаблону т.е. мероприятия проходящие на каждой смене и зараннее добавленные в бд
//2) спонтанные мероприятия типа(сплав на лодке и т.д. которые не обязательныы)