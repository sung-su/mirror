using Tizen.NUI;
using Tizen.NUI.Components;
using SettingView.TextResources;
using Tizen.NUI.Binding;
using Tizen.NUI.BaseComponents;
using SettingView.Common;

namespace SettingView.Views
{
    class SettingMainView : CollectionView
    {
        private static Extents HeaderPadding = new Extents(16, 16, 8, 8);
        private static Extents HeaderMargin = new Extents(22, 22, 10, 0);
        private const int HeaderHeight = 64;

        public SettingMainView() : base()
        {
            ThemeChangeSensitive = true;
            CornerRadius = 26.SpToPx();
            UpdateWindowSize();

            ItemsLayouter = new LinearLayouter();
            ScrollingDirection = Direction.Vertical;
            SelectionMode = ItemSelectionMode.Single;
            BackgroundColor = AppConstants.BackgroundColor;

            Header = GetHeader();
            ItemTemplate = GetItemTemplate();
        }

        private RecyclerViewItem GetHeader()
        {
            DefaultTitleItem allAppTitle = new DefaultTitleItem()
            {
                ThemeChangeSensitive = true,
                Margin = HeaderMargin.SpToPx(),
                Padding = HeaderPadding.SpToPx(),
                HeightSpecification = HeaderHeight.SpToPx(),
                WidthResizePolicy = ResizePolicyType.FillToParent,
            };
            allAppTitle.Label.PixelSize = 24.SpToPx();
            allAppTitle.Label.FontFamily = "BreezeSans";
            allAppTitle.Label.Text = Resources.IDS_ST_OPT_SETTINGS;
            allAppTitle.Label.TextColor = AppConstants.TextColor;
            return allAppTitle;
        }

        private DataTemplate GetItemTemplate()
        {
            return new DataTemplate(() =>
            {
                MainMenuItemLayout item = new MainMenuItemLayout();
                item.TitleLabel.SetBinding(TextLabel.TextProperty, "Title");
                item.IconView.SetBinding(ImageView.ResourceUrlProperty, "IconPath");
                item.IconBackground.SetBinding(View.BackgroundColorProperty, "IconColorHex");
                item.SetBinding(MainMenuItemLayout.ItemSelectCommandProperty, "GadgetSelectCommand");
                return item;
            });
        }

        public void UpdateWindowSize()
        {
            Size2D = NUIApplication.GetDefaultWindow().Size;
        }
    }
}
