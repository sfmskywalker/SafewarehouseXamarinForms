namespace SafeWarehouseApp.ViewModels
{
    public class SelectableItem<T>
    {
        public SelectableItem(T item, bool isSelected = false)
        {
            Item = item;
            IsSelected = isSelected;
        }
        
        public T Item { get; set; } = default!;
        public bool IsSelected { get; set; } 
    }
}