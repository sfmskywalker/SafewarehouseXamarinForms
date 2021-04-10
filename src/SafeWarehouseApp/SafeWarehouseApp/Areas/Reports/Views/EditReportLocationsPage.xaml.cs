using System;
using System.Collections.Generic;
using System.Linq;
using SafeWarehouseApp.Areas.Reports.ViewModels;
using SafeWarehouseApp.Models;
using SafeWarehouseApp.Touch;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;

namespace SafeWarehouseApp.Areas.Reports.Views
{
    public class Circle
    {
        public float Left { get; set; }
        public float Top { get; set; }
        public float Radius { get; set; }
        public Point Location => new Point(Left, Top);
    }

    public partial class EditReportLocationsPage
    {
        private readonly IDictionary<long, SKPoint> _touchDictionary = new Dictionary<long, SKPoint>();
        private DateTime _lastTap = DateTime.MinValue;
        private long? _lastTouchId;
        private Location? _tappedLocation;

        public EditReportLocationsPage()
        {
            InitializeComponent();
        }

        private EditReportLocationsViewModel ViewModel => (EditReportLocationsViewModel)BindingContext;
        

        void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            var info = args.Info;
            var surface = args.Surface;
            var canvas = surface.Canvas;

            canvas.Clear();

            var report = ViewModel.Report;

            if (report == null!)
                return;

            var reportLocations = report.Locations;

            foreach (var location in reportLocations)
            {
                using var paint = new SKPaint
                {
                    Style = SKPaintStyle.Stroke,
                    Color = SKColors.Red,
                    TextAlign = SKTextAlign.Center,
                    TextSize = 100,
                    StrokeWidth = 5
                };
                canvas.DrawCircle(location.Left, location.Top, location.Radius, paint);
            }
        }

        void OnTouchEffectAction(object sender, TouchActionEventArgs args)
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
                    var tappedLocation = ViewModel.Report?.Locations.FirstOrDefault(x => IsInsideCircle(point.ToFormsPoint(), new Point(x.Left, x.Top), x.Radius));

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

                    if (_tappedLocation != null)
                        _touchDictionary[args.Id] = point;

                    _lastTap = now;
                    _lastTouchId = touchId;
                    _tappedLocation = tappedLocation;
                    break;

                case TouchActionType.Moved:
                    if (_tappedLocation == null)
                        return;

                    if (_touchDictionary.ContainsKey(args.Id))
                    {
                        // Single-finger drag
                        if (_touchDictionary.Count == 1)
                        {
                            var prevPoint = _touchDictionary[args.Id];

                            // Adjust the matrix for the new position
                            _tappedLocation.Left += point.X - prevPoint.X;
                            _tappedLocation.Top += point.Y - prevPoint.Y;
                            CanvasView.InvalidateSurface();
                        }
                        // Double-finger scale and drag
                        else if (_touchDictionary.Count >= 2)
                        {
                            // Copy two dictionary keys into array
                            long[] keys = new long[_touchDictionary.Count];
                            _touchDictionary.Keys.CopyTo(keys, 0);

                            // Find index of non-moving (pivot) finger
                            var pivotIndex = (keys[0] == args.Id) ? 1 : 0;

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
                                _tappedLocation.Radius *= scaleX;
                                if (_tappedLocation.Radius < 20)
                                    _tappedLocation.Radius = 20;

                                if (_tappedLocation.Radius > CanvasView.Width - 30)
                                    _tappedLocation.Radius = (float) (CanvasView.Width - 30);

                                CanvasView.InvalidateSurface();
                            }
                        }

                        // Store the new point in the dictionary.
                        _touchDictionary[args.Id] = point;
                    }

                    break;

                case TouchActionType.Released:
                case TouchActionType.Cancelled:
                    if (_touchDictionary.ContainsKey(args.Id))
                        _touchDictionary.Remove(args.Id);
                    break;
            }
        }

        private bool IsInsideCircle(Point point, Point circleLocation, float circleRadius)
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
    }
}