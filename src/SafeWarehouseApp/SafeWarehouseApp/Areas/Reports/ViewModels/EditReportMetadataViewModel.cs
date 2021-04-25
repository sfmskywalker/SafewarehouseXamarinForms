using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using SafeWarehouseApp.Models;
using SafeWarehouseApp.ViewModels;
using Xamarin.Forms;

namespace SafeWarehouseApp.Areas.Reports.ViewModels
{
    public class EditReportMetadataViewModel : BaseViewModel
    {
        private Report? _report;
        private string? _remarks;
        private DateTime? _reportDate;
        private DateTime? _nextExaminationBefore;
        private readonly Timer _updateTimer;

        public EditReportMetadataViewModel()
        {
            SaveChanges = new Command(OnSaveChangesAsync);
            _updateTimer = new Timer(OnUpdateTimerTick);
        }

        public Report Report
        {
            get => _report!;
            set
            {
                SetProperty(ref _report, value);
                Remarks = value.Remarks;
                ReportDate = value.Date;
                NextExaminationBefore = value.NextExaminationBefore;
            }
        }

        public string? Remarks
        {
            get => _remarks;
            set => SetProperty(ref _remarks, value);
        }

        public DateTime? ReportDate
        {
            get => _reportDate;
            set => SetProperty(ref _reportDate, value);
        }

        public DateTime? NextExaminationBefore
        {
            get => _nextExaminationBefore;
            set => SetProperty(ref _nextExaminationBefore, value);
        }

        public Command SaveChanges { get; }

        protected override async void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (propertyName == nameof(Remarks))
                _updateTimer.Change(TimeSpan.FromSeconds(1), Timeout.InfiniteTimeSpan);
            else
                await SaveChangesAsync();
            
            base.OnPropertyChanged(propertyName);
        }

        private void OnUpdateTimerTick(object state) => OnSaveChangesAsync();
        
        private async Task SaveChangesAsync()
        {
            Report.Remarks = Remarks?.Trim();
            Report.Date = ReportDate ?? Report.Date;
            Report.NextExaminationBefore = NextExaminationBefore;
            await ReportStore.UpdateAsync(Report);
        }
        
        private async void OnSaveChangesAsync() => await SaveChangesAsync();
    }
}