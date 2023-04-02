using SettingCore;
using SettingCore.Views;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;

namespace SettingMainGadget.About
{
    public class AboutScalableGadget : SettingCore.MenuGadget
    {
        public override string ProvideTitle() => "Scalable UI for Developers";

        protected override View OnCreate()
        {
            base.OnCreate();

            var content = new ScrollableBase()
            {
                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = LayoutParamPolicies.MatchParent,
                ScrollingDirection = ScrollableBase.Direction.Vertical,
                HideScrollbar = false,
                ThemeChangeSensitive = true,
                Layout = new LinearLayout()
                {
                    LinearOrientation = LinearLayout.Orientation.Vertical,
                },
            };

            var scalable = new (string, string)[]
            {
                ("Scaling Factor", $"{GraphicsTypeManager.Instance.ScalingFactor}"),
                ("DPI", $"{GraphicsTypeManager.Instance.Dpi}"),
                ("Scaled DPI", $"{GraphicsTypeManager.Instance.ScaledDpi}"),
                ("Baseline DPI", $"{GraphicsTypeManager.Instance.BaselineDpi}"),
                ("Density", $"{GraphicsTypeManager.Instance.Density}"),
                ("Scaled Density", $"{GraphicsTypeManager.Instance.ScaledDensity}"),
                ("100dp scaled is displayed as:", $"{GraphicsTypeManager.Instance.ConvertScriptToPixel("100dp")}px"),
                ("100sp scaled is displayed as:", $"{GraphicsTypeManager.Instance.ConvertScriptToPixel("100sp")}px"),
            };
            foreach (var (title, value) in scalable)
            {
                var row = TextListItem.CreatePrimaryTextItemWithSecondaryText(title, value);
                content.Add(row);
            }

            var hideButton = new Button()
            {
                Text = "Hide menu: Scalable UI for Developers",
                WidthSpecification = LayoutParamPolicies.MatchParent,
            };
            hideButton.TextLabel.PixelSize = 24.SpToPx();
            hideButton.TextLabel.Margin = new Extents(0, 0, 16, 16).SpToPx();
            hideButton.Clicked += (s, e) =>
            {
                GadgetManager.Instance.ChangeMenuPathOrder(MainMenuProvider.About_ScalableUI, -30);
                NavigateBack();
            };
            content.Add(hideButton);

            return content;
        }
    }
}
