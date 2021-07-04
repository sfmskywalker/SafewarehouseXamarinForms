using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SafeWarehouseApp.Models
{
    public class Damage
    {
        public string Id { get; set; } = default!;
        [Required] public int Number { get; set; }
        [Required] public string? DamageTypeId { get; set; }
        public ICollection<RequiredMaterial> RequiredMaterials { get; set; } = new List<RequiredMaterial>();
        public ICollection<DamagePicture> Pictures { get; set; } = new List<DamagePicture>();
        public string? Description { get; set; }
    }
}