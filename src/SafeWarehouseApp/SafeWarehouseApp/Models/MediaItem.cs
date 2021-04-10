namespace SafeWarehouseApp.Models
{
    public class MediaItem : Entity
    {
        public string FileName { get; set; } = default!;
        public string? Tag { get; set; }
    }
}