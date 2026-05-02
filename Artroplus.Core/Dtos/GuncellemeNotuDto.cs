namespace Artroplus.Core.DTOs;

public class GuncellemeNotuDto
{
    public int Id { get; set; }
    public string Baslik { get; set; } = null!;
    public string Icerik { get; set; } = null!;
    public string? Versiyon { get; set; }
    public string Tip { get; set; } = null!;
    public DateTime OlusturulmaTarihi { get; set; }
}
