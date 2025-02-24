public record CreateEventDTO(
    int SessionId,
    int? EventTemplateId, // Если выбрано мероприятие по шаблону, иначе оставить пустым
    string? CustomName,   // Если кастомное мероприятие, здесь указывается названеи
    bool IsCustomEvent,   // true для кастомных мероприятий
    string Type,          // "Спортивное", "Творческое", "Общелагерное","Другое"
    DateTime DateTime,    // Дата и время мероприятия
    string Status,        // "Запланировано", "Выполнено", "Отменено"
    int CounselorId       // Ответственный вожатый
);
