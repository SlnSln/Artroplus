using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Artroplus.Core.Entities;

[Table("TblKullanicilar")]
public class Kullanici : BaseEntity
{
    [Required]
    [MaxLength(50)]
    public string KullaniciAdi { get; set; } = null!;

    [Required]
    public string SifreHash { get; set; } = null!;

    [Required]
    [MaxLength(50)]
    public string Ad { get; set; } = null!;

    [Required]
    [MaxLength(50)]
    public string Soyad { get; set; } = null!;

    public int RolId { get; set; }

    [ForeignKey("RolId")]
    public virtual Rol Rol { get; set; } = null!;
}
