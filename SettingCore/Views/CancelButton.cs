using Tizen.NUI;
using Tizen.NUI.BaseComponents;

namespace SettingCore.Views
{
    public class CancelButton : BaseComponent
    {
        private readonly ThemeColor BackgroundColors = new ThemeColor(Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent);
        private readonly ThemeColor IconColors = new ThemeColor(new Color("#17234D"), new Color("#FDFDFD"), new Color("#FF6200"), new Color("#FF8A00"));

        private readonly ImageView icon;
        private readonly ImageVisual iconVisual;

        public CancelButton() : base()
        {
            BackgroundColor = BackgroundColors.Normal;
            CornerRadius = 8f.SpToPx();
            Size = new Size(32, 32).SpToPx();

            iconVisual = new ImageVisual
            {
                MixColor = IconColors.Normal,
                URL = System.IO.Path.Combine(Tizen.Applications.Application.Current.DirectoryInfo.Resource, "cancel-text-icon.svg"),
                FittingMode = FittingModeType.ScaleToFill,
            };
            icon = new ImageView
            {
                Image = iconVisual.OutputVisualMap,
                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = LayoutParamPolicies.MatchParent,
            };
            Add(icon);

            ThemeManager.ThemeChanged += (s, e) => { OnChangeSelected(false); };
        }

        public override void OnChangeSelected(bool selected)
        {
            if (selected)
            {
                iconVisual.MixColor = IconColors.Selected;
                icon.Image = iconVisual.OutputVisualMap;

                BackgroundColor = BackgroundColors.Selected;
            }
            else
            {
                iconVisual.MixColor = IconColors.Normal;
                icon.Image = iconVisual.OutputVisualMap;

                BackgroundColor = BackgroundColors.Normal;
            }
        }
    }
}
