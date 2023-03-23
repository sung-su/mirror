using System;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;

namespace SettingCore.Views
{
    public class SliderListItem : BaseComponent
    {
        private readonly ThemeColor TrackColor = new ThemeColor(new Color("#FF6200"), new Color("#FF8A00"), new Color("#FF6200"), new Color("#FF8A00"));
        private readonly ThemeColor BgTrackColor = new ThemeColor(new Color(1.0f, 0.37f, 0.0f, 0.1f), new Color(1.0f, 0.37f, 0.0f, 0.1f), new Color("#FF6200"), new Color("#FF8A00"));

        public Slider Slider = null;

        public string IconPath
        {
            get => icon.ResourceUrl;
            set
            {
                if(!String.IsNullOrEmpty(value))
                {
                    icon.ResourceUrl = value;
                }
            }
        }

        private ImageView icon = null;
        private TextLabel primary = null;

        public SliderListItem(string primaryText, string iconpath, float curvalue)
            : base()
        {
            WidthSpecification = LayoutParamPolicies.MatchParent;
            HeightSpecification = LayoutParamPolicies.WrapContent;
            LeaveRequired = true;
            AccessibilityHighlightable = true;

            Layout = new LinearLayout()
            {
                LinearOrientation = LinearLayout.Orientation.Vertical,
            };

            primary = new TextLabel
            {
                AccessibilityHidden = true,
                ThemeChangeSensitive = true,
                Text = primaryText,
                Margin = new Extents(16, 0, 0, 0).SpToPx(),
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

            icon = new ImageView(iconpath)
            {
                Size2D = new Size2D(32, 32).SpToPx(),
                Margin = new Extents(0, 5, 0, 0).SpToPx(),
            };

            Slider = new Slider()
            {
                WidthResizePolicy = ResizePolicyType.FillToParent,
                Direction = Slider.DirectionType.Horizontal,
                TrackThickness = (uint)8.SpToPx(),
                ThumbSize = new Size(24, 24).SpToPx(),
                MinValue = 0,
                MaxValue = 1.0f,
                CurrentValue = curvalue,
                BgTrackColor = BgTrackColor.Normal,
                SlidedTrackColor = TrackColor.Normal
            };

            sliderView.Add(icon);
            sliderView.Add(Slider);

            Add(primary);
            Add(sliderView);

            ThemeManager.ThemeChanged += ThemeManager_ThemeChanged;
        }

        private void ThemeManager_ThemeChanged(object sender, ThemeChangedEventArgs e)
        {
            Slider.BgTrackColor = BgTrackColor.Normal;
            Slider.SlidedTrackColor = TrackColor.Normal;
        }

        protected override string AccessibilityGetName()
        {
            return $"{primary.Text}, {(int)(Slider.CurrentValue / Slider.MaxValue * 100)} percent";
        }
    }
}
