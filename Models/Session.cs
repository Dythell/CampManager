using System.ComponentModel.DataAnnotations;

public class Session
{
    [Key]
    public int Session_Id { get; set; }
    public int Number { get; set; } // Номер смены
    public string Type { get; set; } // Обычная или профильная смена
    public int Year { get; set; } // Год, например 2023, 2024, 2025
    public string Season { get; set; } // Сезон, например Летний, Зимний
}
