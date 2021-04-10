using System.ComponentModel.DataAnnotations;

namespace SafeWarehouseApp.Models
{
    public class RequiredMaterial
    {
        [Required]public string MaterialId { get; set; } = default!;
        public int Quantity { get; set; }
    }
}