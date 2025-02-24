public record RegisterRequestDTO(
    string Username,
    string Password,
    string Role,

    string? Surname,
    string? Name,
    string? Patronymic,
    string? PhoneNumber
    );
