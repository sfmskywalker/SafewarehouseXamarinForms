using System;
using System.Collections.Generic;
using System.Linq;
using Android.Views;
using SafeWarehouseApp.Droid;
using SafeWarehouseApp.Touch;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ResolutionGroupName("XamarinDocs")]
[assembly: ExportEffect(typeof(DroidTouchEffect), "TouchEffect")]

namespace SafeWarehouseApp.Droid
{
    public class DroidTouchEffect : PlatformEffect
    {
        Android.Views.View _view;
        Element _formsElement;
        TouchEffect _libTouchEffect;
        bool _capture;
        Func<double, double> _fromPixels;
        readonly int[] _twoIntArray = new int[2];

        static readonly Dictionary<Android.Views.View, DroidTouchEffect> ViewDictionary = new Dictionary<Android.Views.View, DroidTouchEffect>();
        static readonly Dictionary<int, DroidTouchEffect> IdToEffectDictionary = new Dictionary<int, DroidTouchEffect>();

        protected override void OnAttached()
        {
            // Get the Android View corresponding to the Element that the effect is attached to
            _view = Control == null ? Container : Control;

            // Get access to the TouchEffect class in the .NET Standard library
            var touchEffect = (TouchEffect) Element.Effects.FirstOrDefault(e => e is TouchEffect);

            if (touchEffect != null && _view != null)
            {
                ViewDictionary.Add(_view, this);

                _formsElement = Element;

                _libTouchEffect = touchEffect;

                // Save fromPixels function
                _fromPixels = _view.Context.FromPixels;

                // Set event handler on View
                _view.Touch += OnTouch;
            }
        }

        protected override void OnDetached()
        {
            if (ViewDictionary.ContainsKey(_view))
            {
                ViewDictionary.Remove(_view);
                _view.Touch -= OnTouch;
            }
        }

        void OnTouch(object sender, Android.Views.View.TouchEventArgs args)
        {
            // Two object common to all the events
            var senderView = sender as Android.Views.View;
            var motionEvent = args.Event;

            // Get the pointer index
            var pointerIndex = motionEvent.ActionIndex;

            // Get the id that identifies a finger over the course of its progress
            var id = motionEvent.GetPointerId(pointerIndex);

            senderView.GetLocationOnScreen(_twoIntArray);
            
            var screenPointerCoords = new Point(
                _twoIntArray[0] + motionEvent.GetX(pointerIndex),
                _twoIntArray[1] + motionEvent.GetY(pointerIndex));


            // Use ActionMasked here rather than Action to reduce the number of possibilities
            switch (args.Event.ActionMasked)
            {
                case MotionEventActions.Down:
                case MotionEventActions.PointerDown:
                    FireEvent(this, id, TouchActionType.Pressed, screenPointerCoords, true);

                    if (IdToEffectDictionary.ContainsKey(id))
                        IdToEffectDictionary.Remove(id);
                    
                    IdToEffectDictionary.Add(id, this);

                    _capture = _libTouchEffect.Capture;
                    break;

                case MotionEventActions.Move:
                    // Multiple Move events are bundled, so handle them in a loop
                    for (pointerIndex = 0; pointerIndex < motionEvent.PointerCount; pointerIndex++)
                    {
                        id = motionEvent.GetPointerId(pointerIndex);

                        if (_capture)
                        {
                            senderView.GetLocationOnScreen(_twoIntArray);

                            screenPointerCoords = new Point(_twoIntArray[0] + motionEvent.GetX(pointerIndex),
                                _twoIntArray[1] + motionEvent.GetY(pointerIndex));

                            FireEvent(this, id, TouchActionType.Moved, screenPointerCoords, true);
                        }
                        else
                        {
                            CheckForBoundaryHop(id, screenPointerCoords);

                            if (IdToEffectDictionary[id] != null)
                            {
                                FireEvent(IdToEffectDictionary[id], id, TouchActionType.Moved, screenPointerCoords, true);
                            }
                        }
                    }

                    break;

                case MotionEventActions.Up:
                case MotionEventActions.Pointer1Up:
                    if (_capture)
                    {
                        FireEvent(this, id, TouchActionType.Released, screenPointerCoords, false);
                    }
                    else
                    {
                        CheckForBoundaryHop(id, screenPointerCoords);

                        if (IdToEffectDictionary[id] != null)
                        {
                            FireEvent(IdToEffectDictionary[id], id, TouchActionType.Released, screenPointerCoords, false);
                        }
                    }

                    IdToEffectDictionary.Remove(id);
                    break;

                case MotionEventActions.Cancel:
                    if (_capture)
                    {
                        FireEvent(this, id, TouchActionType.Cancelled, screenPointerCoords, false);
                    }
                    else
                    {
                        if (IdToEffectDictionary[id] != null)
                        {
                            FireEvent(IdToEffectDictionary[id], id, TouchActionType.Cancelled, screenPointerCoords, false);
                        }
                    }

                    IdToEffectDictionary.Remove(id);
                    break;
            }
        }

        void CheckForBoundaryHop(int id, Point pointerLocation)
        {
            DroidTouchEffect touchEffectHit = null;

            foreach (var view in ViewDictionary.Keys)
            {
                // Get the view rectangle
                try
                {
                    view.GetLocationOnScreen(_twoIntArray);
                }
                catch // System.ObjectDisposedException: Cannot access a disposed object.
                {
                    continue;
                }

                var viewRect = new Rectangle(_twoIntArray[0], _twoIntArray[1], view.Width, view.Height);

                if (viewRect.Contains(pointerLocation))
                {
                    touchEffectHit = ViewDictionary[view];
                }
            }

            if (touchEffectHit != IdToEffectDictionary[id])
            {
                if (IdToEffectDictionary[id] != null)
                {
                    FireEvent(IdToEffectDictionary[id], id, TouchActionType.Exited, pointerLocation, true);
                }

                if (touchEffectHit != null)
                {
                    FireEvent(touchEffectHit, id, TouchActionType.Entered, pointerLocation, true);
                }

                IdToEffectDictionary[id] = touchEffectHit;
            }
        }

        void FireEvent(DroidTouchEffect touchEffect, int id, TouchActionType actionType, Point pointerLocation, bool isInContact)
        {
            // Get the method to call for firing events
            Action<Element, TouchActionEventArgs> onTouchAction = touchEffect._libTouchEffect.OnTouchAction;

            // Get the location of the pointer within the view
            touchEffect._view.GetLocationOnScreen(_twoIntArray);
            var x = pointerLocation.X - _twoIntArray[0];
            var y = pointerLocation.Y - _twoIntArray[1];
            var point = new Point(_fromPixels(x), _fromPixels(y));

            // Call the method
            onTouchAction(touchEffect._formsElement, new TouchActionEventArgs(id, actionType, point, isInContact));
        }
    }
}