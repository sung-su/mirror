using Tizen.NUI;
using Tizen.NUI.BaseComponents;

namespace SettingCore.Views
{
    public class StorageIndicator : View
    {
        // TODO : make it dynamic and renewable 
        public StorageIndicator()
        {
            WidthSpecification = LayoutParamPolicies.MatchParent;

            Layout = new LinearLayout()
            {
                LinearOrientation = LinearLayout.Orientation.Horizontal,
                VerticalAlignment = VerticalAlignment.Center,
            };

            Margin = new Extents(40, 40, 24, 24).SpToPx();
            SizeHeight = 8.SpToPx();
            CornerRadius = 4.SpToPx();

            BackgroundColor = new Color("#FF6200").WithAlpha(0.1f);

            AddParts();
        }

        private void AddParts()
        {
            Add(CreateColoredView(new Color("#FFC700"), 100, new Vector4(4, 0, 0, 4)));
            Add(CreateColoredView(new Color("#FF8A00"), 150));
            Add(CreateColoredView(new Color("#FF6200"), 200));
            Add(CreateColoredView(new Color("#A40404"), 300));
        }

        private View CreateColoredView(Color color, float width, Vector4 cornerRadius = null)
        {
            View view = new View()
            {
                Size = new Size(width, 8).SpToPx(),
                BackgroundColor = color,
            };

            if (cornerRadius != null)
            {
                view.CornerRadius = cornerRadius;
            }

            return view;
        }
    }
}
