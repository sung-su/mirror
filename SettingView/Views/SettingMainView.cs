using Tizen.NUI;
using Tizen.NUI.Components;
using SettingView.TextResources;
using Tizen.NUI.Binding;
using Tizen.NUI.BaseComponents;
using SettingView.Common;
using SettingView.ViewModels;
using SettingView.Models;
using System.Windows.Input;
using Tizen;
using System;
using SettingCore;

namespace SettingView.Views
{


    static class ImageViewBindings
    {
        public static BindingProperty<ImageView, string> ResourceUrlProperty { get; } = new BindingProperty<ImageView, string>
        {
            Setter = (v, value) => v.ResourceUrl = value,
        };
    }

    static class BackgroundColorPropertyBindings
    {
        public static BindingProperty<View, string> BackgroundColorProperty { get; } = new BindingProperty<View, string>
        {
            Setter = (v, value) =>
            {
                try
                {
                    Color color = new Color(value);
                    v.BackgroundColor = color;
                }
                catch(Exception ex)
                {
                    Logger.Debug("Error setting BackgroundColor: {ex.Message}");
                }
            },
        };
    }
    static class ButtonBindings
    {
        public static BindingProperty<Button, bool> IsSelectedProperty { get; } = new BindingProperty<Button, bool>
        {
            Setter = (v, value) => v.IsSelected = value,
        };

        public static BindingProperty<Button, ICommand> CommandProperty { get; } = new BindingProperty<Button, ICommand>
        {
            Setter = (v, value) => v.Command = value,
        };

        public static BindingProperty<Button, bool> IsEnabledProperty { get; } = new BindingProperty<Button, bool>
        {
            Setter = (v, value) => v.IsEnabled = value,
        };
    }

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

                var itemSession = new BindingSession<GadgetInfoModel>();
                item.BindingContextChanged += (sender, e) =>
                {
                    if (item.BindingContext is GadgetInfoModel model)
                    {
                        try
                        {
                            itemSession.ViewModel = model;
                        }
                        catch (Exception ex)
                        {
                            Logger.Debug(ex.Message);
                        }
                    }
                };
                try
                {
                    item.TitleLabel.SetBinding(itemSession, TextLabelBindings.TextProperty, "Title");
                    item.IconView.SetBinding(itemSession, ImageViewBindings.ResourceUrlProperty, "IconPath");
                    item.IconBackground.SetBinding(itemSession, BackgroundColorPropertyBindings.BackgroundColorProperty, "IconColorHex");

                    item.SetBinding(itemSession, MainMenuItemLayoutBindings.ItemSelectCommandProperty, "GadgetSelectCommand");
                }

                catch (Exception ex)
                {
                    Logger.Debug(ex.Message);
                }
                return item;
            });
        }

        public void UpdateWindowSize()
        {
            Size2D = NUIApplication.GetDefaultWindow().Size;
        }
    }
}
