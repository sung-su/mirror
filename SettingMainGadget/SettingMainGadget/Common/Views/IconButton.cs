using Tizen.NUI.BaseComponents;
using Tizen.NUI;
using Tizen;

using SettingCore.Views;

namespace SettingMainGadget.Common.Views
{
    public class IconButton : BaseComponent
    {
        private readonly ThemeColor BackgroundColors = new ThemeColor(new Color("#FAFAFACC"), new Color("#16131ACC"), new Color("#FBE2D2"), new Color("#2D2B30"));
        private readonly ThemeColor IconColors = new ThemeColor(new Color("#17234D"), new Color("#FDFDFD"), new Color("#FF6200"), new Color("#FF8A00"));

        private readonly ImageView icon;
        private readonly ImageVisual iconVisual;

        public IconButton(string iconpath) : base()
        {
            AccessibilityRole = Role.PushButton;
            AccessibilityHighlightable = true;
            BackgroundColor = Color.Transparent;
            CornerRadius = 8f.SpToPx();
            Size = new Size(48, 48).SpToPx();

            iconVisual = new ImageVisual
            {
                MixColor = IconColors.Normal,
                URL = iconpath,
                FittingMode = FittingModeType.ScaleToFill,
            };

            icon = new ImageView
            {
                Image = iconVisual.OutputVisualMap,
                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = LayoutParamPolicies.MatchParent,
            };
            Add(icon);

            ThemeManager.ThemeChanged += ThemeManager_ThemeChanged;
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

        private void ThemeManager_ThemeChanged(object sender, ThemeChangedEventArgs e)
        {
            OnChangeSelected(false);
        }

        protected override void Dispose(bool disposing)
        {
            ThemeManager.ThemeChanged -= ThemeManager_ThemeChanged;
            base.Dispose(disposing);
        }
    }
}
