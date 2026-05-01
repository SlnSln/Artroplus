using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Artroplus.Core.Entities;

[Table("TblRoller")]
public class Rol : BaseEntity
{
    [Required]
    [MaxLength(50)]
    public string Ad { get; set; } = null!;
    
    // Navigation Property
    public virtual ICollection<Kullanici> Kullanicilar { get; set; } = new List<Kullanici>();
}
