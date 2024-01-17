using SettingMainGadget.TextResources;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Binding;
using Tizen.NUI.Components;
using Tizen.System;
using static SettingMainGadget.DateTime.DateTimeTimezoneManager;

namespace Setting.Menu.DateTime
{
    public class DateTimeSetTimezoneGadget : SettingCore.MenuGadget
    {
        public override string ProvideTitle() => NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_TIME_ZONE));

        private View content = null;
        private List<TimeZone> timeZones;
        private Loading loadingIndicator;
        private CollectionView collectionView;

        protected override View OnCreate()
        {
            base.OnCreate();

            content = new View
            {
                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = LayoutParamPolicies.MatchParent,
                Layout = new LinearLayout()
                {
                    LinearOrientation = LinearLayout.Orientation.Vertical,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                },
            };

            CreateView();

            return content;
        }

        private async void CreateView()
        {
            var installedAppsContent = new View()
            {
                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = LayoutParamPolicies.MatchParent,
                Layout = new LinearLayout()
                {
                    LinearOrientation = LinearLayout.Orientation.Vertical,
                },
            };

            await LoadData();

            content.Add(SearchView());

            collectionView = new CollectionView()
            {
                ItemsLayouter = new LinearLayouter(),
                ItemTemplate = new DataTemplate(() =>
                {
                    DefaultLinearItem item = new DefaultLinearItem()
                    {
                        WidthSpecification = LayoutParamPolicies.MatchParent,
                    };
                    item.Label.SetBinding(TextLabel.TextProperty, "DisplayName");
                    item.Label.HorizontalAlignment = HorizontalAlignment.Begin;

                    return item;
                }),
                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = LayoutParamPolicies.MatchParent,
                ScrollingDirection = ScrollableBase.Direction.Vertical,
                SelectionMode = ItemSelectionMode.SingleAlways,
            };
            collectionView.ItemsSource = timeZones;

            var currentTimeZone = timeZones.Where(x => x.Info.Id == SystemSettings.LocaleTimeZone).FirstOrDefault();
            if(currentTimeZone != null)
            {
                collectionView.SelectedItem = currentTimeZone;
            }

            collectionView.SelectionChanged += (s, e) =>
            {
                var timeZone = e.CurrentSelection.FirstOrDefault() as TimeZone;

                if(timeZone != null)
                {
                    SetTimezone(timeZone.Info.Id);
                    NavigateBack();
                }
            };

            content.Add(collectionView);
        }

        private async Task LoadData()
        {
            loadingIndicator = new Loading();
            loadingIndicator.Play();
            content.Add(loadingIndicator);

            await Task.Run(() =>
            {
                timeZones = GetTimeZones();
                timeZones = timeZones.OrderBy(a => a.City).ThenBy(x => x.Continent).ToList();
            });

            loadingIndicator?.Stop();
            loadingIndicator?.Unparent();
            loadingIndicator?.Dispose();
        }

        private View SearchView()
        {
            View searchView = new View()
            {
                BackgroundColor = IsLightTheme ? new Color("#FAFAFA") : new Color("#1D1A21"),
                WidthSpecification = LayoutParamPolicies.MatchParent,
                WidthResizePolicy = ResizePolicyType.FillToParent,
                Layout = new LinearLayout()
                {
                    LinearOrientation = LinearLayout.Orientation.Vertical,
                },
                Margin = new Extents(16, 16, 0, 2).SpToPx(),
                SizeHeight = 49.SpToPx(),
            };

            var textFieldView = new View()
            {
                WidthSpecification = LayoutParamPolicies.MatchParent,
                WidthResizePolicy = ResizePolicyType.FillToParent,
                Layout = new FlexLayout()
                {
                    Justification = FlexLayout.FlexJustification.SpaceBetween,
                    Direction = FlexLayout.FlexDirection.Row,
                    ItemsAlignment = FlexLayout.AlignmentType.Center
                },
            };

            var iconVisual = new ImageVisual
            {
                MixColor = IsLightTheme ? new Color("#17234D") : new Color("#FF8A00"),
                URL = GetResourcePath("search.svg"),
                FittingMode = FittingModeType.ScaleToFill,
            };

            var icon = new ImageView
            {
                Image = iconVisual.OutputVisualMap,
                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = LayoutParamPolicies.MatchParent,
                Size = new Size(32, 32).SpToPx(),
            };

            var textField = new TextField
            {
                PlaceholderText = NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_SEARCH)),
                BackgroundColor = IsLightTheme ? new Color("#FAFAFA") : new Color("#1D1A21"),
                PlaceholderTextColor = IsLightTheme ? new Color("#CACACA") : new Color("#666666"),
                WidthResizePolicy = ResizePolicyType.FillToParent,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Extents(16, 0, 0, 0).SpToPx(),
                SizeHeight = 48.SpToPx(),
                PixelSize = 24.SpToPx(),
                EnableCursorBlink = true,
            };

            var input = textField.GetInputMethodContext();
            input.Activate();

            textField.TouchEvent += (s, e) =>
            {
                var state = input.GetInputPanelState();

                if (state == InputMethodContext.State.Hide)
                {
                    input.Activate();
                }

                return false;
            };

            textField.TextChanged += (s, e) =>
            {
                var filtered = timeZones.Where(a => a.DisplayName.Contains(textField.Text)).ToList();
                collectionView.ItemsSource = filtered;
            };

            View separator = new View
            {
                BackgroundColor = IsLightTheme ? new Color("#FF6200") : new Color("#FF8A00"),
                WidthResizePolicy = ResizePolicyType.FillToParent,
                SizeHeight = 1.SpToPx(),
            };

            textFieldView.Add(textField);
            textFieldView.Add(icon);

            searchView.Add(textFieldView);
            searchView.Add(separator);

            return searchView;
        }
    }
}
