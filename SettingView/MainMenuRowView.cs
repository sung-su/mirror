using Tizen.NUI;
using Tizen.NUI.BaseComponents;

namespace SettingView
{
    internal class MainMenuRowView : View
    {
        public MainMenuRowView(string iconPath, Color iconColor, string title)
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
                Text = title,
                PointSize = 18f.SpToPt(),
                //PixelSize = 18.SpToPx(),
            };
            FlexLayout.SetFlexGrow(titleTextLabel, 1);

            Add(iconBackground);
            Add(titleTextLabel);
        }
    }
}
