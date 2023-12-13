using SettingCore.TextResources;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;

namespace SettingCore.Views
{
    public class SpeechRateRow : BaseComponent
    {
        private readonly ThemeColor TrackColors = new ThemeColor(new Color("#FF6200"), new Color("#FF8A00"), Color.Transparent, Color.Transparent, new Color("#CACACA"), new Color("#CACACA"));
        private readonly ThemeColor BgTrackColors = new ThemeColor(new Color(1.0f, 0.37f, 0.0f, 0.1f), new Color(1.0f, 0.37f, 0.0f, 0.1f), Color.Transparent, Color.Transparent);
        private readonly ThemeColor TextColors = new ThemeColor(new Color("#090E21"), new Color("#FDFDFD"), Color.Transparent, Color.Transparent, new Color("#CACACA"), new Color("#666666"));

        private ImageView leftIcon;
        private ImageView rightIcon;
        private TextLabel primary;

        private string leftImageUrl => Tools.IsLightTheme ? "walking-icon-light.svg" : "walking-icon-dark.svg";
        private string rightImageUrl => Tools.IsLightTheme ? "running-icon-light.svg" : "running-icon-dark.svg";
        public Slider Slider { get; private set; }

        public SpeechRateRow()
        {
            Layout = new LinearLayout()
            {
                LinearOrientation = LinearLayout.Orientation.Vertical,
            };

            primary = new TextLabel
            {
                AccessibilityHidden = true,
                ThemeChangeSensitive = true,
                Text = Resources.IDS_ST_HEADER_SPEECH_RATE_ABB,
                Margin = new Extents(16, 0, 16, 0).SpToPx(),
                PixelSize = 24.SpToPx(),
            };

            var sliderView = new View
            {
                WidthSpecification = LayoutParamPolicies.MatchParent,
                Margin = new Extents(16, 16, 16, 16).SpToPx(),
                Layout = new LinearLayout()
                {
                    LinearOrientation = LinearLayout.Orientation.Horizontal,
                    VerticalAlignment = VerticalAlignment.Center,
                },
            };

            leftIcon = new ImageView(GetResourcePath(leftImageUrl))
            {
                Size2D = new Size2D(48, 48).SpToPx(),
                Margin = new Extents(16, 0, 8, 8).SpToPx(),
            };

            rightIcon = new ImageView(GetResourcePath(rightImageUrl))
            {
                Size2D = new Size2D(48, 48).SpToPx(),
                Margin = new Extents(0, 16, 8, 8).SpToPx(),
            };

            Slider = new Slider()
            {
                WidthResizePolicy = ResizePolicyType.FillToParent,
                Direction = Slider.DirectionType.Horizontal,
                TrackThickness = (uint)8.SpToPx(),
                ThumbSize = new Size(24, 24).SpToPx(),
                BgTrackColor = BgTrackColors.Normal,
                SlidedTrackColor = TrackColors.Normal,

                IsDiscrete = true,
                DiscreteValue = 1,
            };

            sliderView.Add(leftIcon);
            sliderView.Add(Slider);
            sliderView.Add(rightIcon);

            Add(primary);
            Add(sliderView);

            ThemeManager.ThemeChanged += ThemeManager_ThemeChanged;
        }

        private void ThemeManager_ThemeChanged(object sender, ThemeChangedEventArgs e)
        {
            primary.TextColor = TextColors.Normal;

            Slider.BgTrackColor = BgTrackColors.Normal;
            Slider.SlidedTrackColor = TrackColors.Normal;

            leftIcon.ResourceUrl = GetResourcePath(leftImageUrl);
            rightIcon.ResourceUrl = GetResourcePath(rightImageUrl);
        }

        private static string GetResourcePath(string relativePath) => System.IO.Path.Combine(Tizen.Applications.Application.Current.DirectoryInfo.Resource, relativePath);
        protected override string AccessibilityGetName()
        {
            return $"{primary.Text}, {(int)(Slider.CurrentValue / Slider.MaxValue * 100)} %";
        }
    }
}
