namespace SafeWarehouseApp.Areas.Reports.ViewModels
{
    public record DamagePictureSummaryViewModel
    {
        public DamagePictureSummaryViewModel(string id, string imagePath, string description)
        {
            Id = id;
            ImagePath = imagePath;
            Description = description;
        }
        
        public string Id { get; }
        public string ImagePath { get; }
        public string Description { get; }
    }
}