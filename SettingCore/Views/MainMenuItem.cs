using Tizen.NUI;
using Tizen.NUI.BaseComponents;

namespace SettingCore.Views
{
    public class MainMenuItem : BaseComponent
    {
        private readonly ThemeColor BackgroundColors = new ThemeColor(Color.Transparent, Color.Transparent, new Color("#FF6400").WithAlpha(0.16f), Color.White.WithAlpha(0.16f));
        private readonly ThemeColor TextColors = new ThemeColor(new Color("#090E21"), new Color("#FDFDFD"), new Color("#FF6200"), new Color("#FF8A00"));
        private readonly ThemeColor IconColors = new ThemeColor(Color.White, Color.Black, Color.White, Color.Black);
        private readonly ThemeColor IconBackgroundColors;

        private readonly ImageView icon;
        private readonly ImageVisual iconVisual;
        private readonly TextLabel titleTextLabel;

        public MainMenuItem(string iconPath, Color iconBackgroungColor, string title) : base()
        {
            IconBackgroundColors = new ThemeColor(iconBackgroungColor, iconBackgroungColor, iconBackgroungColor, iconBackgroungColor);

            Layout = new LinearLayout
            {
                LinearOrientation = LinearLayout.Orientation.Horizontal,
                VerticalAlignment = VerticalAlignment.Center,
            };

            var iconBackground = new View
            {
                CornerRadius = 5.SpToPx(),
                BackgroundColor = IconBackgroundColors.Normal,
                Size = new Size(32, 32).SpToPx(),
                Margin = new Extents(16, 16, 16, 16).SpToPx(),
            };
            iconVisual = new ImageVisual
            {
                MixColor = IconColors.Normal,
                URL = iconPath,
                FittingMode = FittingModeType.ScaleToFill,
            };
            icon = new ImageView
            {
                Image = iconVisual.OutputVisualMap,
                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = LayoutParamPolicies.MatchParent,
            };
            iconBackground.Add(icon);

            titleTextLabel = new TextLabel
            {
                AccessibilityHidden = true,
                Text = title,
                PixelSize = 24.SpToPx(),
                TextColor = TextColors.Normal,
                Margin = new Extents(16, 16, 16, 16).SpToPx(),
            };
            FlexLayout.SetFlexGrow(titleTextLabel, 1);

            Add(iconBackground);
            Add(titleTextLabel);

            AccessibilityRole = Role.MenuItem;

            ThemeManager.ThemeChanged += ThemeManager_ThemeChanged;
        }

        public override void OnChangeSelected(bool selected)
        {
            if (selected)
            {
                BackgroundColor = BackgroundColors.Selected;

                iconVisual.MixColor = IconColors.Selected;
                icon.Image = iconVisual.OutputVisualMap;

                titleTextLabel.TextColor = TextColors.Selected;
            }
            else
            {
                BackgroundColor = BackgroundColors.Normal;

                iconVisual.MixColor = IconColors.Normal;
                icon.Image = iconVisual.OutputVisualMap;

                titleTextLabel.TextColor = TextColors.Normal;
            }
        }

        private void ThemeManager_ThemeChanged(object sender, ThemeChangedEventArgs e)
        {
            if (this != null) // handle exception NUI's native dali object is already disposed.
            {
                OnChangeSelected(false);
            }
        }

        protected override string AccessibilityGetName()
        {
            return titleTextLabel.Text;
        }

        protected override void Dispose(bool disposing)
        {
            ThemeManager.ThemeChanged -= ThemeManager_ThemeChanged;
            base.Dispose(disposing);
        }
    }
}
