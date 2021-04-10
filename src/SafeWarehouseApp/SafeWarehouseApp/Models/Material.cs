namespace SafeWarehouseApp.Models
{
    public class Material : Entity
    {
        public string SupplierId { get; set; } = default!;
        public string Name { get; set; } = default!;
    }
}