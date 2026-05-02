namespace Artroplus.Core.DTOs;

public class LoginResponseDto
{
    public int Id { get; set; }
    public string Ad { get; set; } = null!;
    public string Soyad { get; set; } = null!;
    public string RolAd { get; set; } = null!;
}
