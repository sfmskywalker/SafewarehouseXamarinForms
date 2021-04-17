using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using SafeWarehouseApp.Annotations;
using SafeWarehouseApp.Models;

namespace SafeWarehouseApp.Areas.Reports.ViewModels
{
    public class DamagePictureViewModel : INotifyPropertyChanged
    {
        private int _number;
        private string? _pictureId;
        private string? _description = default!;

        public DamagePictureViewModel(int number, string? pictureId, string? description)
        {
            Number = number;
            PictureId = pictureId;
            Description = description;
        }

        public int Number
        {
            get => _number;
            set => SetProperty(ref _number, value);
        }

        public string? PictureId
        {
            get => _pictureId;
            set => SetProperty(ref _pictureId, value);
        }

        public string? Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        
        public DamagePicture ToModel() => new()
        {
            Number = Number,
            Description = Description,
            PictureId = PictureId
        };

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = "", Action? onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}