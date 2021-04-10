namespace SafeWarehouseApp.Models
{
    public record Paging : IPaging
    {
        public static Paging Page(int page, int pageSize) => new(page * pageSize, pageSize);
        
        public Paging(int skip, int take)
        {
            Skip = skip;
            Take = take;
        }

        public int Skip { get; init; }
        public int Take { get; init;}
    }
}