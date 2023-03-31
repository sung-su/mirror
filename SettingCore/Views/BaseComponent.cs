using System;
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
