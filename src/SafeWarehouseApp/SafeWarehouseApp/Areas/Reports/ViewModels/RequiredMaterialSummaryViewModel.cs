using SafeWarehouseApp.Models;

namespace SafeWarehouseApp.Areas.Reports.ViewModels
{
    public record RequiredMaterialSummaryViewModel
    {
        public RequiredMaterialSummaryViewModel(Supplier supplier, Material material, int quantity)
        {
            Supplier = supplier;
            Material = material;
            Quantity = quantity;
        }

        public Supplier Supplier { get; }
        public Material Material { get; }
        public int Quantity { get; }
    }
}