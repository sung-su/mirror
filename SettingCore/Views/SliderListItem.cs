using System;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;

namespace SettingCore.Views
{
    public class SliderListItem : BaseComponent
    {
        private readonly ThemeColor TrackColors = new ThemeColor(new Color("#FF6200"), new Color("#FF8A00"), Color.Transparent, Color.Transparent, new Color("#CACACA"), new Color("#CACACA"));
        private readonly ThemeColor BgTrackColors = new ThemeColor(new Color(1.0f, 0.37f, 0.0f, 0.1f), new Color(1.0f, 0.37f, 0.0f, 0.1f), Color.Transparent, Color.Transparent);
        private readonly ThemeColor TextColors = new ThemeColor(new Color("#090E21"), new Color("#FDFDFD"), Color.Transparent, Color.Transparent, new Color("#CACACA"), new Color("#666666"));

        public Slider Slider { get; private set; }

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
                BgTrackColor = BgTrackColors.Normal,
                SlidedTrackColor = TrackColors.Normal
            };

            sliderView.Add(icon);
            sliderView.Add(Slider);

            Add(primary);
            Add(sliderView);

            ThemeManager.ThemeChanged += ThemeManager_ThemeChanged;
        }

        public override void OnDisabledStateChanged(bool isEnabled)
        {
            Slider.IsEnabled = isEnabled;

            if (isEnabled)
            {
                primary.TextColor = TextColors.Normal;

                Slider.BgTrackColor = BgTrackColors.Normal;
                Slider.SlidedTrackColor = TrackColors.Normal;
            }
            else
            {
                Slider.CurrentValue = Slider.MaxValue;

                primary.TextColor = TextColors.Disabled;

                Slider.BgTrackColor = BgTrackColors.Disabled;
                Slider.SlidedTrackColor = TrackColors.Disabled;
            }
        }

        private void ThemeManager_ThemeChanged(object sender, ThemeChangedEventArgs e)
        {
            OnDisabledStateChanged(IsEnabled);
        }

        protected override string AccessibilityGetName()
        {
            return $"{primary.Text}, {(int)(Slider.CurrentValue / Slider.MaxValue * 100)} percent";
        }
    }
}
