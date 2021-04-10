using System;
using SafeWarehouseApp.Areas.Customers.Views;
using SafeWarehouseApp.Areas.DamageTypes.Views;
using SafeWarehouseApp.Areas.Materials.Views;
using SafeWarehouseApp.Areas.Reports.Views;
using SafeWarehouseApp.Areas.Suppliers.Views;
using Xamarin.Forms;

namespace SafeWarehouseApp
{
    public partial class AppShell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(NewDamageTypePage), typeof(NewDamageTypePage));
            Routing.RegisterRoute(nameof(EditDamageTypePage), typeof(EditDamageTypePage));
            Routing.RegisterRoute(nameof(NewSupplierPage), typeof(NewSupplierPage));
            Routing.RegisterRoute(nameof(EditSupplierPage), typeof(EditSupplierPage));
            Routing.RegisterRoute(nameof(NewMaterialPage), typeof(NewMaterialPage));
            Routing.RegisterRoute(nameof(EditMaterialPage), typeof(EditMaterialPage));
            Routing.RegisterRoute(nameof(NewCustomerPage), typeof(NewCustomerPage));
            Routing.RegisterRoute(nameof(EditCustomerPage), typeof(EditCustomerPage));
            Routing.RegisterRoute(nameof(NewReportPage), typeof(NewReportPage));
            Routing.RegisterRoute(nameof(EditReportPage), typeof(EditReportPage));
            Routing.RegisterRoute(nameof(EditLocationPage), typeof(EditLocationPage));
            Routing.RegisterRoute(nameof(EditDamagePage), typeof(EditDamagePage));
        }

        private async void OnMenuItemClicked(object sender, EventArgs e)
        {
            await Current.GoToAsync("//LoginPage");
        }
    }
}
