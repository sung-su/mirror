using System.Windows.Input;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;
using Tizen.NUI.Binding;
using SettingCore.Views;

namespace SettingView.Views
{
    public class MainMenuItemLayout : RecyclerViewItem
    {
        private readonly ThemeColor TextColors = new ThemeColor(new Color("#090E21"), new Color("#FDFDFD"), new Color("#FF6200"), new Color("#FF8A00"));

        public ImageView IconView { get; private set; }
        public View IconBackground { get; private set; }
        public TextLabel TitleLabel { get; private set; }

        public static readonly BindableProperty ItemSelectCommandProperty = BindableProperty.Create(
            nameof(MainMenuItemSelectCommand),typeof(ICommand), typeof(SettingMainView), null,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                var instance = (MainMenuItemLayout)bindable;
                if (oldValue != newValue)
                {
                    instance.mainMenuItemSelectCommand = (ICommand)newValue;
                }
            },
            defaultValueCreator: (bindable) => ((MainMenuItemLayout)bindable).mainMenuItemSelectCommand);

        private ICommand mainMenuItemSelectCommand;
        public ICommand MainMenuItemSelectCommand
        {
            get => (ICommand)GetValue(ItemSelectCommandProperty);
            set => SetValue(ItemSelectCommandProperty, value);
        }

        public MainMenuItemLayout() : base()
        {
            BackgroundColor = Color.Transparent;
            WidthSpecification = LayoutParamPolicies.MatchParent;

            Layout = new LinearLayout
            {
                LinearOrientation = LinearLayout.Orientation.Horizontal,
                VerticalAlignment = VerticalAlignment.Center,
            };

            IconBackground = new View
            {
                CornerRadius = 5.SpToPx(),
                Size = new Size(32, 32).SpToPx(),
                Margin = new Extents(16, 16, 16, 16).SpToPx(),
            };
            IconView = new ImageView
            {
                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = LayoutParamPolicies.MatchParent,
            };
            IconBackground.Add(IconView);

            TitleLabel = new TextLabel
            {
                AccessibilityHidden = true,
                PixelSize = 24.SpToPx(),
                TextColor = TextColors.Normal,
                Margin = new Extents(16, 16, 16, 16).SpToPx(),
            };
            FlexLayout.SetFlexGrow(TitleLabel, 1);

            Add(IconBackground);
            Add(TitleLabel);

            AccessibilityRole = Role.MenuItem;

            TouchEvent += (s, e) =>
            {
                if (e.Touch.GetState(0) == PointStateType.Up)
                {
                    ItemClicked();
                }
                return true;
            };

            AccessibilityActivated += (s, e) =>
            {
                ItemClicked();
            };

            ThemeManager.ThemeChanged += ThemeManager_ThemeChanged;
        }

        private void ItemClicked()
        {
            MainMenuItemSelectCommand.Execute(null);
        }

        public void UpdateItem(string title, Color iconBackgroundColor)
        {
            TitleLabel.Text = title;
            IconBackground.BackgroundColor = iconBackgroundColor;
        }

        private void ThemeManager_ThemeChanged(object sender, ThemeChangedEventArgs e)
        {
        }

        protected override string AccessibilityGetName()
        {
            return TitleLabel.Text;
        }

        protected override void Dispose(bool disposing)
        {
            ThemeManager.ThemeChanged -= ThemeManager_ThemeChanged;
            base.Dispose(disposing);
        }
    }
}
