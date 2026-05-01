using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Artroplus.Core.Entities;

/// <summary>
/// CLAUDE.md Kural 10: Sistem güncellemelerinin tutulduğu tablo.
/// </summary>
[Table("TblGuncellemeNotlari")]
public class GuncellemeNotu : BaseEntity
{
    [Required]
    [MaxLength(200)]
    public string Baslik { get; set; } = null!;

    [Required]
    public string Icerik { get; set; } = null!;

    [MaxLength(50)]
    public string? Versiyon { get; set; }

    /// <summary>
    /// Yeni, Guncelleme, Duzeltme değerlerinden biri.
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Tip { get; set; } = null!;

    public DateTime OlusturulmaTarihi { get; set; } = DateTime.Now;
}
