using SettingCore.TextResources;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;

namespace SettingCore.Views
{
    public class BackButton : BaseComponent
    {
        private readonly ThemeColor BackgroundColors = new ThemeColor(new Color("#FAFAFACC"), new Color("#16131ACC"), new Color("#FBE2D2"), new Color("#2D2B30"));
        private readonly ThemeColor IconColors = new ThemeColor(new Color("#17234D"), new Color("#FDFDFD"), new Color("#FF6200"), new Color("#FF8A00"));

        private readonly ImageView icon;
        private readonly ImageVisual iconVisual;

        public BackButton() : base()
        {
            AccessibilityRole = Role.PushButton;
            BackgroundColor = BackgroundColors.Normal;
            CornerRadius = 8f.SpToPx();
            Size = new Size(40, 40).SpToPx();

            iconVisual = new ImageVisual
            {
                MixColor = IconColors.Normal,
                URL = System.IO.Path.Combine(Tizen.Applications.Application.Current.DirectoryInfo.Resource, "back-button-icon.svg"),
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

        protected override string AccessibilityGetName()
        {
            return Resources.IDS_ST_BUTTON_BACK;
        }
    }
}
