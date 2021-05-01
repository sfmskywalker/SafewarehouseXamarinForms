using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SafeWarehouseApp.Areas.Reports.ViewModels;
using SafeWarehouseApp.Models;
using SafeWarehouseApp.Touch;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;

namespace SafeWarehouseApp.Areas.Reports.Views
{
    public partial class EditReportLocationsPage
    {
        private readonly IDictionary<long, SKPoint> _touchDictionary = new Dictionary<long, SKPoint>();
        private DateTime _lastTap = DateTime.MinValue;
        private long? _lastTouchId;
        private Location? _tappedLocation;
        private Location? _selectedLocation;
        private readonly Timer _pressAndHoldTimer;
        private bool _moveThresholdReached;

        public EditReportLocationsPage()
        {
            InitializeComponent();
            _pressAndHoldTimer = new Timer(OnPressAndHoldTimeoutReached);
        }

        private EditReportLocationsViewModel ViewModel => (EditReportLocationsViewModel) BindingContext;

        protected override void OnAppearing()
        {
            ViewModel.DimensionProvider = () => new Size(CanvasView.Width, CanvasView.Height);
            ViewModel.PropertyChanged += OnViewModelPropertyChanged;
        }

        protected override void OnDisappearing()
        {
            ViewModel.PropertyChanged -= OnViewModelPropertyChanged;
        }

        private void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            var surface = args.Surface;
            var targetRect = args.Info.Rect;

            ViewModel.DrawSchematic(surface.Canvas, targetRect, _selectedLocation);
        }

        private void OnPressAndHoldTimeoutReached(object state)
        {
            _moveThresholdReached = false;
            
            if (_tappedLocation == null)
                return;

            Device.BeginInvokeOnMainThread(() =>
            {
                ViewModel.ShowActionSheet.Execute(_tappedLocation);
                CanvasView.InvalidateSurface();
            });
        }

        private Location? GetLocation(SKPoint point) => ViewModel.Report?.Locations.FirstOrDefault(x => IsInsideCircle(point.ToFormsPoint(), new Point(x.Left, x.Top), x.Radius));

        private static bool IsInsideCircle(Point point, Point circleLocation, float circleRadius)
        {
            var x = point.X;
            var y = point.Y;
            var centerX = circleLocation.X;
            var centerY = circleLocation.Y;
            var radius = circleRadius;

            // If radius is too small, it becomes hard to pinch, so increase hit-test area.
            if (radius < 60)
                radius = 60;

            return Math.Pow(x - centerX, 2) + Math.Pow(y - centerY, 2) < Math.Pow(radius, 2);
        }

        private void OnTouchEffectAction(object sender, TouchActionEventArgs args)
        {
            var pt = args.Location;
            var point = new SKPoint((float) (CanvasView.CanvasSize.Width * pt.X / CanvasView.Width), (float) (CanvasView.CanvasSize.Height * pt.Y / CanvasView.Height));

            switch (args.Type)
            {
                case TouchActionType.Pressed:
                    // Determine double-tap
                    var now = DateTime.Now;
                    var elapsed = now - _lastTap;
                    var touchId = args.Id;
                    var doubleTapped = touchId == _lastTouchId && elapsed <= TimeSpan.FromMilliseconds(500);
                    var tappedLocation = GetLocation(point);

                    if (doubleTapped)
                    {
                        if (tappedLocation == null && _tappedLocation == null)
                        {
                            // Add location.
                            ViewModel.AddLocation.Execute(point.ToFormsPoint());
                        }
                        else if (tappedLocation == _tappedLocation)
                        {
                            // Edit location.
                            ViewModel.EditLocation.Execute(tappedLocation);
                        }
                    }
                    else
                    {
                        var selectedLocation = GetLocation(point);

                        if (selectedLocation != null)
                            _selectedLocation = selectedLocation;

                        CanvasView.InvalidateSurface();
                    }

                    //if (_tappedLocation != null)
                    _touchDictionary[args.Id] = point;

                    _lastTap = now;
                    _lastTouchId = touchId;
                    _tappedLocation = tappedLocation;
                    _moveThresholdReached = false;
                    _pressAndHoldTimer.Change(TimeSpan.FromMilliseconds(500), Timeout.InfiniteTimeSpan);
                    break;

                case TouchActionType.Moved:
                    var location = _tappedLocation ?? _selectedLocation;

                    if (location == null)
                        return;

                    if (_touchDictionary.ContainsKey(args.Id))
                    {
                        // Single-finger drag
                        if (_touchDictionary.Count == 1)
                        {
                            var prevPoint = _touchDictionary[args.Id];
                            var deltaX = point.X - prevPoint.X;
                            var deltaY = point.Y - prevPoint.Y;

                            // Adjust the matrix for the new position
                            location.Left += deltaX;
                            location.Top += deltaY;
                            CanvasView.InvalidateSurface();

                            if (deltaX >= 1 || deltaY >= 1)
                                _moveThresholdReached = true;
                        }
                        // Double-finger scale and drag
                        else if (_touchDictionary.Count >= 2)
                        {
                            _moveThresholdReached = true;

                            // Copy two dictionary keys into array
                            var keys = new long[_touchDictionary.Count];
                            _touchDictionary.Keys.CopyTo(keys, 0);

                            // Find index of non-moving (pivot) finger
                            var pivotIndex = keys[0] == args.Id ? 1 : 0;

                            // Get the three points involved in the transform
                            var pivotPoint = _touchDictionary[keys[pivotIndex]];
                            var prevPoint = _touchDictionary[args.Id];
                            var newPoint = point;

                            // Calculate two vectors
                            var oldVector = prevPoint - pivotPoint;
                            var newVector = newPoint - pivotPoint;

                            // Scaling factors are ratios of those
                            var scaleX = newVector.X / oldVector.X;
                            var scaleY = newVector.Y / oldVector.Y;

                            if (!float.IsNaN(scaleX) && !float.IsInfinity(scaleX) &&
                                !float.IsNaN(scaleY) && !float.IsInfinity(scaleY))
                            {
                                location.Radius *= scaleX;

                                if (location.Radius < 20)
                                    location.Radius = 20;

                                if (location.Radius > CanvasView.Width - 30)
                                    location.Radius = (float) (CanvasView.Width - 30);

                                CanvasView.InvalidateSurface();
                            }
                        }

                        // Store the new point in the dictionary.
                        _touchDictionary[args.Id] = point;
                    }

                    if (_moveThresholdReached)
                        _pressAndHoldTimer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);

                    break;

                case TouchActionType.Released:

                    if (_touchDictionary.ContainsKey(args.Id))
                        _touchDictionary.Remove(args.Id);

                    _pressAndHoldTimer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);

                    Task.Run(() => ViewModel.SaveChanges.Execute(null));

                    break;
                case TouchActionType.Cancelled:
                    if (_touchDictionary.ContainsKey(args.Id))
                        _touchDictionary.Remove(args.Id);
                    break;
            }
        }
        
        private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e) => Device.BeginInvokeOnMainThread(() => CanvasView.InvalidateSurface());
    }
}