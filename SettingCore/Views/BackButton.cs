using System;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;

namespace SettingCore.Views
{
    public class BackButton : View
    {
        internal class Colors
        {
            internal class Background
            {
                internal static readonly Color Normal = Color.White;
                internal static readonly Color Selected = new Color("#FF6200").WithAlpha(0.3f);
            }
            internal class Icon
            {
                internal static readonly Color Normal = Color.Black;
                internal static readonly Color Selected = new Color("#FF6200");
            }
        }

        private readonly ImageView icon;
        private readonly ImageVisual iconVisual;

        public event EventHandler<ClickedEventArgs> Clicked;

        public BackButton()
        {
            BackgroundColor = Colors.Background.Normal;
            CornerRadius = 8f.SpToPx();
            Size = new Size(40, 40).SpToPx();

            LeaveRequired = true;

            iconVisual = new ImageVisual
            {
                MixColor = Colors.Icon.Normal,
                URL = System.IO.Path.Combine(Tizen.Applications.Application.Current.DirectoryInfo.Resource, "back-button-icon.svg"),
                FittingMode = FittingModeType.ScaleToFill,
            };
            icon = new ImageView
            {
                Image = iconVisual.OutputVisualMap,
                Size = new Size(40, 40).SpToPx(),
            };
            Add(icon);

            TouchEvent += OnTouchEvent;
        }

        private void ChangeSelect(bool select)
        {
            if (select)
            {
                BackgroundColor = Colors.Background.Selected;

                iconVisual.MixColor = Colors.Icon.Selected;
                icon.Image = iconVisual.OutputVisualMap;
            }
            else
            {
                BackgroundColor = Colors.Background.Normal;

                iconVisual.MixColor = Colors.Icon.Normal;
                icon.Image = iconVisual.OutputVisualMap;
            }
        }

        private bool touchStarted = false;
        private bool OnTouchEvent(object source, TouchEventArgs e)
        {
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

            ChangeSelect(state == PointStateType.Down);

            return true;
        }
    }
}
