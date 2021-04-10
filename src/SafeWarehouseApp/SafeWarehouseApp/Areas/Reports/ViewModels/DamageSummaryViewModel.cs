namespace SafeWarehouseApp.Areas.Reports.ViewModels
{
    public record DamageSummaryViewModel
    {
        public DamageSummaryViewModel(string id, int number, string title)
        {
            Id = id;
            Number = number;
            Title = title;
        }

        public string Id { get; }
        public int Number { get; }
        public string Title { get; }
    }
}