using System;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;

namespace SettingView
{
    internal class MainMenuRowView : View
    {
        public MainMenuRowView(string iconPath, Color iconColor, string title, Action action)
        {
            Layout = new LinearLayout
            {
                LinearOrientation = LinearLayout.Orientation.Horizontal,
                VerticalAlignment = VerticalAlignment.Center,
            };
            WidthSpecification = LayoutParamPolicies.MatchParent;

            var iconBackground = new View
            {
                CornerRadius = 5.SpToPx(),
                BackgroundColor = iconColor,
                Size = new Size(32, 32).SpToPx(),
                Margin = new Extents(16, 16, 16, 16).SpToPx(),
            };
            var iconImage = new ImageView
            {
                ResourceUrl = iconPath,
                WidthResizePolicy = ResizePolicyType.FillToParent,
                HeightResizePolicy = ResizePolicyType.FillToParent,
            };
            iconBackground.Add(iconImage);

            var titleTextLabel = new TextLabel
            {
                ThemeChangeSensitive = true,
                Text = title,
                PixelSize = 24.SpToPx(),
            };
            FlexLayout.SetFlexGrow(titleTextLabel, 1);

            Add(iconBackground);
            Add(titleTextLabel);

            // TODO: replace TouchEvent with Clicked for Accessibility
            TouchEvent += (object source, Tizen.NUI.BaseComponents.View.TouchEventArgs e) =>
            {
                if (e.Touch.GetState(0) == PointStateType.Down)
                {
                    action?.Invoke();
                }
                return true;
            };
        }
    }
}
