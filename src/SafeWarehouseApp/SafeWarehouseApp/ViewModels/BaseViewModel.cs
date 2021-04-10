using SafeWarehouseApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using SafeWarehouseApp.Persistence;
using Microsoft.Extensions.DependencyInjection;
using SafeWarehouseApp.Persistence.Stores;
using SafeWarehouseApp.Services;

namespace SafeWarehouseApp.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        private bool _isBusy;
        private string _title = string.Empty;

        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }
        
        public IStore<DamageType> DamageTypeStore => GetService<IStore<DamageType>>();
        public IStore<Supplier> SupplierStore => GetService<IStore<Supplier>>();
        public IStore<Material> MaterialStore => GetService<IStore<Material>>();
        public IStore<Customer> CustomerStore => GetService<IStore<Customer>>();
        public IStore<Report> ReportStore => GetService<IStore<Report>>();
        public IStore<MediaItem> MediaItemStore => GetService<IStore<MediaItem>>();
        public IMediaService MediaService => GetService<IMediaService>();
        
        public event PropertyChangedEventHandler? PropertyChanged;

        protected T GetService<T>() where T:class => App.Services.GetRequiredService<T>();

        protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = "", Action? onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }
        
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            changed?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
