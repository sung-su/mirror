using System;
using System.ComponentModel;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;

namespace SettingCore.Views
{
    public class BaseComponent : Control
    {
        public BaseComponent()
        {
            WidthSpecification = LayoutParamPolicies.MatchParent;
            HeightSpecification = LayoutParamPolicies.WrapContent;
            LeaveRequired = true;
            AccessibilityHighlightable = true;

            TouchEvent += OnTouchEvent;
            ControlStateChangedEvent += BaseComponent_ControlStateChangedEvent;
        }

        private void BaseComponent_ControlStateChangedEvent(object sender, ControlStateChangedEventArgs e)
        {
            // change Disabled -> Any
            if (e.PreviousState == ControlState.Disabled)
            {
                OnDisabledStateChanged(true);
            }

            // change Any -> Disabled
            if (e.CurrentState == ControlState.Disabled)
            {
                OnDisabledStateChanged(false);
            }
        }

        public event EventHandler<ClickedEventArgs> Clicked;

        [EditorBrowsable(EditorBrowsableState.Never)]
        public event EventHandler<EventArgs> MultiTap;
        private int multiTapCounter;
        private DateTime multiTapLast = DateTime.MinValue;

        protected bool isClickedEventEmpty => Clicked is null;

        private bool touchStarted = false;

        public virtual void OnChangeSelected(bool selected) { }

        public virtual void OnDisabledStateChanged(bool isEnabled) { }

        public override bool OnKey(Key key)
        {
            if (!IsEnabled || null == key)
            {
                return false;
            }

            if (key.State == Key.StateType.Up)
            {
                if (key.KeyPressedName == "Return")
                {
                    var handler = Clicked;
                    handler?.Invoke(this, new ClickedEventArgs());
                }
            }
            return base.OnKey(key);
        }

        private bool OnTouchEvent(object source, TouchEventArgs e)
        {
            if (MultiTap != null)
            {
                var now = DateTime.Now;
                if (now - multiTapLast > TimeSpan.FromSeconds(2))
                {
                    multiTapCounter = 0;
                    Logger.Verbose("multitap zeroed");
                }

                if (e.Touch.GetState(0) == PointStateType.Down)
                {
                    multiTapLast = now;
                    ++multiTapCounter;
                    Logger.Verbose($"multitap {multiTapCounter}");
                }

                if (multiTapCounter == 5)
                {
                    Logger.Verbose("multitap invoke");
                    var handler = MultiTap;
                    handler?.Invoke(this, EventArgs.Empty);
                }
                return false;
            }

            if (isClickedEventEmpty)
            {
                return false;
            }

            var state = e.Touch.GetState(0);

            if (state == PointStateType.Down)
            {
                touchStarted = true;
            }
            else if (state == PointStateType.Finished && touchStarted)
            {
                touchStarted = false;

                var handler = Clicked;
                handler?.Invoke(this, new ClickedEventArgs());
            }
            else
            {
                touchStarted = false;
            }

            OnChangeSelected(state == PointStateType.Down);

            return true;
        }
    }
}
