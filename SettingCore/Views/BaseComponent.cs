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
        }

        public event EventHandler<ClickedEventArgs> Clicked;

        private bool touchStarted = false;

        public virtual void OnChangeSelected(bool selected) { }

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
            if (Clicked is null)
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
