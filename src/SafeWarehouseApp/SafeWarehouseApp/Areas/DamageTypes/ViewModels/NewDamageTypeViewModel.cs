using System;
using System.Threading.Tasks;
using SafeWarehouseApp.Models;
using SafeWarehouseApp.ViewModels;
using Xamarin.Forms;

namespace SafeWarehouseApp.Areas.DamageTypes.ViewModels
{
    public class NewDamageTypeViewModel : BaseViewModel
    {
        private string _title;

        public NewDamageTypeViewModel()
        {
            SaveCommand = new Command(OnSave, ValidateSave);
            CancelCommand = new Command(OnCancel);
            PropertyChanged += (_, __) => SaveCommand.ChangeCanExecute();
        }

        private bool ValidateSave() => !string.IsNullOrWhiteSpace(_title);

        public string DamageTypeTitle
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public Command SaveCommand { get; }
        public Command CancelCommand { get; }

        private async void OnCancel() => await CloseAsync();

        private async void OnSave()
        {
            var newItem = new DamageType
            {
                Id = Guid.NewGuid().ToString("N"),
                Name = DamageTypeTitle.Trim(),
            };

            await DamageTypeStore.AddAsync(newItem);
            await CloseAsync();
        }

        private async Task CloseAsync()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
