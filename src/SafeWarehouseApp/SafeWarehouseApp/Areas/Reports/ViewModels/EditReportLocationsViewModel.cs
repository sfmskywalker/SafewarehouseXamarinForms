using System;
using SafeWarehouseApp.Areas.Reports.Views;
using SafeWarehouseApp.Models;
using SafeWarehouseApp.Services;
using SafeWarehouseApp.ViewModels;
using SkiaSharp;
using Xamarin.Forms;

namespace SafeWarehouseApp.Areas.Reports.ViewModels
{
    public class EditReportLocationsViewModel : BaseViewModel
    {
        private Report _report = default!;
        private string _schematicImagePath = default!;
        private SKBitmap? _schematicBitmap;
        private SKRectI? _rect;

        public EditReportLocationsViewModel()
        {
            AddLocation = new Command<Point>(OnAddLocationAsync);
            EditLocation = new Command<Location>(OnEditLocationAsync);
            ShowActionSheet = new Command<Location>(OnShowActionSheet);
            SaveChanges = new Command(OnSaveChangesAsync);
        }

        public Report Report
        {
            get => _report;
            set
            {
                SetProperty(ref _report, value);
                LoadSchematicAsync();
            }
        }

        public string SchematicImagePath
        {
            get => _schematicImagePath;
            set => SetProperty(ref _schematicImagePath, value);
        }

        public Command<Point> AddLocation { get; }
        public Command<Location> EditLocation { get; }
        public Command<Location> ShowActionSheet { get; set; }
        public Command SaveChanges { get; }
        public Func<Size> DimensionProvider { get; set; } = () => Size.Zero;

        public SKBitmap CreateSchematicBitmap()
        {
            var rect = _rect!.Value;
            var size = rect.Size;
            var bitmap = new SKBitmap(size.Width, size.Height);

            using var canvas = new SKCanvas(bitmap);
            DrawSchematic(canvas, rect);

            return bitmap;
        }

        public void DrawSchematic(SKCanvas canvas, SKRectI targetRect, Location? selectedLocation = default)
        {
            canvas.Clear();

            var report = Report;

            if (report == null!)
                return;

            var schematicBitmap = _schematicBitmap;

            if (schematicBitmap == null)
                schematicBitmap = _schematicBitmap = SKBitmap.Decode(SchematicImagePath);

            _rect = targetRect;
                
            canvas.DrawBitmap(schematicBitmap, targetRect);

            var reportLocations = report.Locations;

            foreach (var location in reportLocations)
            {
                var fillColor = location == selectedLocation ? SKColors.Blue : SKColors.Red;
                var strokeColor = fillColor;

                using var circlePaint = new SKPaint
                {
                    Style = SKPaintStyle.Fill,
                    Color = fillColor.WithAlpha(50),
                    StrokeWidth = 2,
                    IsAntialias = true
                };

                using var textPaint = new SKPaint
                {
                    Style = SKPaintStyle.StrokeAndFill,
                    Color = SKColors.White,
                    TextAlign = SKTextAlign.Center,
                    TextSize = 30,
                    StrokeWidth = 2,
                    IsAntialias = true
                };

                var locationLeft = location.Left;
                var locationTop = location.Top;
                var locationRadius = location.Radius;

                canvas.DrawCircle(locationLeft, locationTop, locationRadius, circlePaint);
                circlePaint.Color = strokeColor.WithAlpha(30);
                circlePaint.Style = SKPaintStyle.Stroke;
                canvas.DrawCircle(locationLeft, locationTop, locationRadius, circlePaint);

                var text = location.Number.ToString();
                var textBounds = new SKRect();
                textPaint.MeasureText(text, ref textBounds);
                var textHeight = textBounds.Height;

                canvas.DrawText(location.Number.ToString(), location.Left, location.Top + textHeight / 2, textPaint);
            }
        }

        private async void LoadSchematicAsync()
        {
            if (Report == null!)
                return;

            SchematicImagePath = (await MediaService.GetMediaItemPathAsync(Report.SchematicMediaId))!;
        }

        private async void OnAddLocationAsync(Point position)
        {
            var location = new Location
            {
                Id = Guid.NewGuid().ToString("N"),
                Left = (int) position.X,
                Top = (int) position.Y,
                Radius = 100,
                Number = Report!.Locations.Count + 1
            };

            Report.Locations.Add(location);
            await ReportStore.UpdateAsync(Report);
            OnEditLocationAsync(location);
        }

        private async void OnEditLocationAsync(Location location)
        {
            await Shell.Current.GoToAsync($"{nameof(EditLocationPage)}?{nameof(EditLocationViewModel.ReportId)}={Report.Id}&{nameof(EditLocationViewModel.LocationId)}={location.Id}", true);
        }

        private async void OnShowActionSheet(Location location)
        {
            var cancelAction = "Annuleren";
            var deleteAction = "Verwijderen";
            var action = await GetService<IActionSheetService>().ShowActionSheet($"Locatie {location.Number}", cancelAction, deleteAction);

            if (action == deleteAction)
            {
                Report.Locations.Remove(location);
                SaveChanges.Execute(null);
            }
        }

        private async void OnSaveChangesAsync() => await ReportStore.UpdateAsync(Report);
    }
}