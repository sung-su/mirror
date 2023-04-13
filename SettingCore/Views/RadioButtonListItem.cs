using System;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;

namespace SettingCore.Views
{
    public class RadioButtonListItem : BaseComponent
    {
        private readonly ThemeColor BackgroundColors = new ThemeColor(Color.Transparent, Color.Transparent, new Color("#FF6400").WithAlpha(0.16f), new Color("#FFFFFF").WithAlpha(0.16f));
        private readonly ThemeColor TextColors = new ThemeColor(new Color("#090E21"), new Color("#FDFDFD"), new Color("#FF6200"), new Color("#FF8A00"), new Color("#CACACA"), new Color("#666666"));

        public RadioButton RadioButton { get; private set; }

        private bool isLightTheme => ThemeManager.PlatformThemeId == "org.tizen.default-light-theme";

        public RadioButtonListItem(string text)
            :base()
        {
            AccessibilityRole = Role.RadioButton;

            Layout = new LinearLayout()
            {
                LinearOrientation = LinearLayout.Orientation.Vertical,
                VerticalAlignment = VerticalAlignment.Center,
            };

            var style_light = ThemeManager.GetStyle("Tizen.NUI.Components.RadioButton") as ButtonStyle;
            style_light.Text.PixelSize = 24.SpToPx();
            style_light.Text.ThemeChangeSensitive = false;
            style_light.Icon.Size = new Size(48, 48).SpToPx();
            style_light.Icon.ResourceUrl = new Selector<string>
            {
                Normal = GetIconPath("Rb_normal.svg"),
                Selected = GetIconPath("Rb_selected.svg"),
                Disabled = GetIconPath("Rb_normal_disabled.svg"),
                DisabledSelected = GetIconPath("Rb_selected_disabled.svg"),
            };

            var style_dark = new ButtonStyle(style_light);
            style_dark.Icon.ResourceUrl = new Selector<string>
            {
                Normal = GetIconPath("Rb_normal_dark.svg"),
                Selected = GetIconPath("Rb_selected_dark.svg"),
                Disabled = GetIconPath("Rb_normal_disabled_dark.svg"),
                DisabledSelected = GetIconPath("Rb_selected_disabled_dark.svg"),
            };

            RadioButton = new RadioButton(isLightTheme ? style_light : style_dark)
            {
                Text = text,
                AccessibilityHidden = true,
                Margin = new Extents(24, 0, 0, 0).SpToPx(),
            };

            RadioButton.ControlStateChangedEvent += (s, e) =>
            {
                OnChangeSelected(e.CurrentState.Equals(ControlState.Pressed));
            };

            Clicked += (s, e) =>
            {
                if (!RadioButton.IsSelected)
                {
                    RadioButton.IsSelected = true;
                }
            };

            Add(RadioButton);

            ThemeManager.ThemeChanged += (s, e) =>
            {
                RadioButton.ApplyStyle(isLightTheme ? style_light : style_dark);
                OnChangeSelected(false);
            };

            Relayout += RadioButtonListItem_Relayout;
        }

        private void RadioButtonListItem_Relayout(object sender, EventArgs e)
        {
            if (!IsEnabled) OnDisabledStateChanged(false);

            Relayout -= RadioButtonListItem_Relayout;
        }

        public override void OnChangeSelected(bool selected)
        {
            if (selected)
            {
                base.BackgroundColor = BackgroundColors.Selected;

                RadioButton.TextColor = TextColors.Selected;
            }
            else
            {
                base.BackgroundColor = BackgroundColors.Normal;

                RadioButton.TextColor = TextColors.Normal;
            }
        }

        public override void OnDisabledStateChanged(bool isEnabled)
        {
            RadioButton.IsEnabled = isEnabled;

            if (isEnabled)
            {
                RadioButton.TextColor = TextColors.Normal;
            }
            else
            {
                RadioButton.TextColor = TextColors.Disabled;
            }
        }

        private string GetIconPath(string name)
        {
            return System.IO.Path.Combine(Tizen.Applications.Application.Current.DirectoryInfo.Resource, $"radiobutton/{name}");
        }

        protected override string AccessibilityGetName() => RadioButton.Text;

        protected override AccessibilityStates AccessibilityCalculateStates()
        {
            var states = base.AccessibilityCalculateStates();

            states[AccessibilityState.Checked] = RadioButton.IsSelected;

            return states;
        }
    }
}
