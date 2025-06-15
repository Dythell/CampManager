public record UpdateEventDTO(
    int? SessionId,
    int? EventTemplateId,
    bool? IsCustomEvent,
    string? CustomName,
    string? Type,
    DateTime? DateTime,
    string? Status,
    int? CounselorId
);