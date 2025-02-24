public record UserDTO(
    int User_Id,
    string Username,
    string Role,
    CounselorDTO CounselorData
);

public record CounselorDTO(
    string Surname,
    string Name,
    string Patronymic,
    string PhoneNumber
);
