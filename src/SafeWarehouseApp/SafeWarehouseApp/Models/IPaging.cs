namespace SafeWarehouseApp.Models
{
    public interface IPaging
    {
        int Skip { get; }
        int Take { get; }
    }
}