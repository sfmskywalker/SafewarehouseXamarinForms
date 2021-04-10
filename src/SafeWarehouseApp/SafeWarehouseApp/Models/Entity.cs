namespace SafeWarehouseApp.Models
{
    public abstract class Entity : IEntity
    {
        public string Id { get; set; } = default!;
    }
}