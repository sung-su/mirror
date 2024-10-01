using SettingMainGadget.TextResources;
using SettingCore;
using SettingCore.Views;
using SettingMainGadget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tizen.Applications;
using Tizen.Content.MediaContent;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;
using Tizen.System;
using Tizen;
using SettingMainGadget.Common;

namespace Setting.Menu
{
    public class StorageGadget : MainMenuGadget
    {
        public override Color ProvideIconColor() => new Color(IsLightTheme ? "#7F2982" : "#922F95");

        public override string ProvideIconPath() => GetResourcePath("storage.svg");

        public override string ProvideTitle() => NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_DEVICE_STORAGE));

        private const string VconfCardStatus = "memory/sysman/mmc";

        private bool isLightTheme => ThemeManager.PlatformThemeId == "org.tizen.default-light-theme";

        private View content;
        private View externalStorage;

        private AlertDialog cachedDataPopupDialog;

        private TextWithIconListItem appsItem;
        private TextWithIconListItem cacheItem;
        private TextWithIconListItem systemItem;

        private StorageIndicator storageIndicator;

        private double nonSystemSpace = 0;
        private double internalTotal;
        private int internalCount;
        private int externalCount;
        private double internalAvailable;
        private double externalTotal;
        private double externalAvailable;
        private double sizeImage;
        private double sizeVideo;
        private double sizeAudio;
        private double sizeMisc;
        private IDictionary<string, Action> sectionViews;
        private Task initialSizeCalculation;
        private PackageSizeInformation sizeInfo;

        protected override View OnCreate()
        {
            base.OnCreate();

            sectionViews = new Dictionary<string, Action>(StringComparer.OrdinalIgnoreCase)
            {
                { MainMenuProvider.Storage_InternalUsage, InternalUsageView},
                { MainMenuProvider.Storage_Used, UsedView},
                { MainMenuProvider.Storage_UsageIndicator, UsageIndicatorView},
                { MainMenuProvider.Storage_TotalInternal, TotalInternalView},
                { MainMenuProvider.Storage_FreeInternal, FreeInternalView},
                { MainMenuProvider.Storage_UsageSummary, UsageSummaryView},
                { MainMenuProvider.Storage_ExternalUsage, ExternalUsageView},
                { MainMenuProvider.Storage_ExternalStorage, ExternalStorageView},
                { MainMenuProvider.Storage_DefaultSettings, DefaultSettingsView},
            };

            content = new ScrollableBase()
            {
                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = LayoutParamPolicies.MatchParent,
                ScrollingDirection = ScrollableBase.Direction.Vertical,
                HideScrollbar = false,
                Layout = new LinearLayout()
                {
                    LinearOrientation = LinearLayout.Orientation.Vertical,
                },
            };

            initialSizeCalculation = StartCalculatingAppCacheSize();

            content.Relayout += (s, e) =>
            {
                if (initialSizeCalculation.IsCompleted)
                {
                    storageIndicator.Update();
                }
                ContentRelayout();
            };

            CreateView();
            Vconf.Notify(VconfCardStatus, OnCardStatusChanged);

            AppAttributes.DefaultWindow.Resized += (o, e) =>
            {
                ContentRelayout();
            };
            return content;
        }

        private void ContentRelayout()
        {
            if(cachedDataPopupDialog != null && cachedDataPopupDialog.IsOnWindow)
            {
                RemoveCachedDataPopup();
                ShowCachePopup();
            }
        }

        protected override void OnDestroy()
        {
            Vconf.Ignore(VconfCardStatus, OnCardStatusChanged);

            base.OnDestroy();
        }

        private void PrepareData()
        {
            GetStorageStatus(out internalCount, out internalTotal, out externalCount, out internalAvailable, out externalTotal, out externalAvailable);
            GetMediaInfo(out sizeImage, out sizeVideo, out sizeAudio);
            GetMiscInfo(out sizeMisc);

            nonSystemSpace = internalTotal - internalAvailable - sizeImage - sizeVideo - sizeAudio - sizeMisc;
        }

        private async void CreateView()
        {
            PrepareData();
            _ = Task.Run(async () =>
            {
                var customization = GetCustomization().OrderBy(c => c.Order);
                Logger.Debug($"customization: {customization.Count()}");
                foreach (var cust in customization)
                {
                    await CoreApplication.Post(() =>
                    {
                        string visibility = cust.IsVisible ? "visible" : "hidden";
                        Logger.Verbose($"Customization: {cust.MenuPath} - {visibility} - {cust.Order}");
                        if (cust.IsVisible && sectionViews.TryGetValue(cust.MenuPath, out Action action))
                        {
                            action();
                        }

                        return true;
                    });
                }
            });
        }

        private void InternalUsageView()
        {
            // Internal storage
            var internalUsageItem = new TextHeaderListItem(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_DEVICE_STORAGE_USAGE)));
            content.Add(internalUsageItem);
        }

        private void UsedView()
        {
            var usedItem = TextListItem.CreatePrimaryTextItemWithSecondaryText($"{NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_USED))}", GetMediaSizeString(internalTotal - internalAvailable));
            content.Add(usedItem);
        }

        private void UsageIndicatorView()
        {
            if (internalCount > 0)
            {
                storageIndicator = new StorageIndicator(internalTotal);
                storageIndicator.AddItem("apps", new Color("#FFC700"), 0);
                storageIndicator.AddItem("images", new Color("#FF8A00"), sizeImage);
                storageIndicator.AddItem("video", new Color("#FF6200"), sizeVideo);
                storageIndicator.AddItem("audio", new Color("#A40404"), sizeAudio);
                storageIndicator.AddItem("misc", new Color("#28262B"), sizeMisc);
                storageIndicator.AddItem("cache", new Color("#3641FA"), 0);
                storageIndicator.AddItem("system", new Color("#17234D"), 0);
                content.Add(storageIndicator);
            }
        }

        private void TotalInternalView()
        {
            if (internalCount > 0)
            {
                var totalItem = TextListItem.CreatePrimaryTextItemWithSecondaryText($"{NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_HEADER_TOTAL_SPACE))}", GetMediaSizeString(internalTotal));
                content.Add(totalItem);
            }
        }

        private void FreeInternalView()
        {
            if (internalCount > 0)
            {
                var freeItem = TextListItem.CreatePrimaryTextItemWithSecondaryText(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_HEADER_FREE_SPACE)), GetMediaSizeString(internalAvailable));
                content.Add(freeItem);
            }
        }

        private void UsageSummaryView()
        {
            var usageSummary = new View()
            {
                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = LayoutParamPolicies.WrapContent,
                Layout = new LinearLayout()
                {
                    LinearOrientation = LinearLayout.Orientation.Vertical,
                },
            };

            appsItem = new TextWithIconListItem(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_SM_TMBODY_APPS_ABB)), new Color("#FFC700"), subText: NUIGadgetResourceManager.GetString(nameof(Resources.IDS_SM_SBODY_CALCULATING_ING)));
            appsItem.Clicked += (s, e) =>
            {
                NavigateTo(MainMenuProvider.Storage_Apps);
            };
            usageSummary.Add(appsItem);

            var imageItem = new TextWithIconListItem(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_IMAGES)), new Color("#FF8A00"), subText: GetMediaSizeString(sizeImage));
            imageItem.Clicked += (s, e) =>
            {
                // TODO : add media files info gadget
            };
            usageSummary.Add(imageItem);

            var videoItem = new TextWithIconListItem(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_VIDEOS)), new Color("#FF6200"), subText: GetMediaSizeString(sizeVideo));
            videoItem.Clicked += (s, e) =>
            {
                // TODO : add media files info gadget
            };
            usageSummary.Add(videoItem);

            var audioItem = new TextWithIconListItem(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_AUDIO)), new Color("#A40404"), subText: GetMediaSizeString(sizeAudio));
            audioItem.Clicked += (s, e) =>
            {
                // TODO : add media files info gadget
            };
            usageSummary.Add(audioItem);

            // TODO : add documents item

            var miscItem = new TextWithIconListItem(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_MISCELLANEOUS_FILES)), new Color("#28262B"), subText: GetMediaSizeString(sizeMisc));
            miscItem.Clicked += (s, e) =>
            {
                // TODO : add miscellaneous info gadget
            };
            usageSummary.Add(miscItem);

            cacheItem = new TextWithIconListItem(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_CACHED_DATA_ABB)), new Color("#3641FA"), subText: NUIGadgetResourceManager.GetString(nameof(Resources.IDS_SM_SBODY_CALCULATING_ING)));
            cacheItem.Clicked += (s, e) =>
            {
                ShowCachePopup();
            };
            usageSummary.Add(cacheItem);

            systemItem = new TextWithIconListItem(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_SYSTEM_STORAGE)), new Color("#17234D"), subText: NUIGadgetResourceManager.GetString(nameof(Resources.IDS_SM_SBODY_CALCULATING_ING)));
            usageSummary.Add(systemItem);

            content.Add(usageSummary);

            if (initialSizeCalculation.IsCompleted)
            {
                UpdateSizeInfo();
            }
        }

        private void ExternalUsageView()
        {
            // External storage
            var externalUsageItem = new TextHeaderListItem(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_DEVICE_SD_STORAGE_USAGE)));
            content.Add(externalUsageItem);
        }

        private void ExternalStorageView()
        {
            externalStorage = new View()
            {
                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = LayoutParamPolicies.WrapContent,
                Layout = new LinearLayout()
                {
                    LinearOrientation = LinearLayout.Orientation.Vertical,
                },
            };

            if (externalStorage)
            {
                var totalItem = TextListItem.CreatePrimaryTextItemWithSecondaryText($"{NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_HEADER_TOTAL_SPACE))}", GetMediaSizeString(externalTotal));
                externalStorage.Add(totalItem);

                var freeItem = TextListItem.CreatePrimaryTextItemWithSecondaryText($"{NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_HEADER_FREE_SPACE))}", GetMediaSizeString(externalAvailable));
                externalStorage.Add(freeItem);

                var unmount = TextListItem.CreatePrimaryTextItem(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_UNMOUNT_SD_CARD)));
                unmount.Clicked += (s, e) =>
                {
                    // TODO : add popup with unmount functionality (storage_request_unmount_mmc)
                };
                externalStorage.Add(unmount);

                var format = TextListItem.CreatePrimaryTextItem(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_FORMAT_SD_CARD)));
                format.Clicked += (s, e) =>
                {
                    // TODO : add popup with format card functionality (storage_request_format_mmc)
                };
                externalStorage.Add(format);
            }
            else
            {
                var item = TextListItem.CreatePrimaryTextItemWithSubText(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_NO_SD_CARD)), NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_INSERT_SD_CARD)));
                item.IsEnabled = false;
                externalStorage.Add(item);
            }
            content.Add(externalStorage);
        }

        private void DefaultSettingsView()
        {
            // Default storage
            var defaultSettings = new View()
            {
                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = LayoutParamPolicies.WrapContent,
                Layout = new LinearLayout()
                {
                    LinearOrientation = LinearLayout.Orientation.Vertical,
                },
            };

            var defaultSettingsItem = new TextHeaderListItem(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_SM_HEADER_DEFAULT_STORAGE_SETTINGS_ABB)));
            var storageLocationItem = TextListItem.CreatePrimaryTextItemWithSubText(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_SM_HEADER_DEFAULT_STORAGE_LOC_ABB)), NUIGadgetResourceManager.GetString(nameof(Resources.IDS_SM_HEADER_DEVICE_STORAGE_ABB)));
            storageLocationItem.Clicked += (s, e) =>
            {
                NavigateTo(MainMenuProvider.Storage_DefaultSettings);
            };
            defaultSettings.Add(defaultSettingsItem);
            defaultSettings.Add(storageLocationItem);
            content.Add(defaultSettings);
        }

        private void GetStorageStatus(out int InternalCount, out double InternalTotal, out int ExternalCount, out double InternalAvailable, out double ExternalTotal, out double ExternalAvailable)
        {
            InternalCount = ExternalCount = 0;
            InternalTotal = ExternalTotal = 0.0;
            InternalAvailable = ExternalAvailable = 0.0;

            var storages = StorageManager.Storages.ToList();

            foreach (var storage in storages)
            {
                if (storage.StorageType == StorageArea.Internal || storage.StorageType == StorageArea.ExtendedInternal)
                {
                    InternalCount++;
                    InternalTotal += storage.TotalSpace;
                    InternalAvailable += storage.AvailableSpace;
                }
                else if (storage.StorageType == StorageArea.External)
                {
                    ExternalCount++;
                    ExternalTotal += storage.TotalSpace;
                    ExternalAvailable += storage.AvailableSpace;
                }
            }

            Logger.Debug("Storage space : ");
            Logger.Debug($"     - InternalTotal : {GetMediaSizeString(InternalTotal)}");
            Logger.Debug($"     - InternalAvailable : {GetMediaSizeString(InternalAvailable)}");

            if (ExternalCount > 0)
            {
                Logger.Debug($"     - ExternalTotal : {GetMediaSizeString(ExternalTotal)} - {ExternalTotal}");
                Logger.Debug($"     - ExternalAvailable : {GetMediaSizeString(ExternalAvailable)} - {ExternalAvailable}");
            }
        }

        private string GetMediaSizeString(double size)
        {
            string[] suffixes = { "Bytes", "KB", "MB", "GB"};
            int counter = 0;

            while (Math.Round(size / 1024, 2) >= 1)
            {
                size = size / 1024;
                counter++;
            }

            return string.Format("{0:0.##} {1}", size, suffixes[counter]);
        }

        private async Task StartCalculatingAppCacheSize()
        {
            sizeInfo = await PackageManager.GetTotalSizeInformationAsync();
            UpdateSizeInfo();
        }

        private void UpdateSizeInfo()
        {
            long sizeApp = sizeInfo.AppSize;
            long sizeCache = sizeInfo.CacheSize;
            bool indicatorValueChanged = false;

            if (appsItem != null)
            {
                appsItem.SubText = GetMediaSizeString(sizeApp);

                var apps = storageIndicator.SizeInfoList.FirstOrDefault(x => x.Name == "apps");
                if (apps != null)
                {
                    apps.SizeInfo = sizeApp;
                    indicatorValueChanged = true;
                }
            }
            if (cacheItem != null)
            {
                cacheItem.SubText = GetMediaSizeString(sizeCache);

                var cache = storageIndicator.SizeInfoList.FirstOrDefault(x => x.Name == "cache");
                if (cache != null)
                {
                    cache.SizeInfo = sizeCache;
                    indicatorValueChanged = true;
                }
            }
            if (systemItem != null)
            {
                systemItem.SubText = GetMediaSizeString(nonSystemSpace - sizeApp - sizeCache);

                var system = storageIndicator.SizeInfoList.FirstOrDefault(x => x.Name == "system");
                if (system != null)
                {
                    system.SizeInfo = (float)(nonSystemSpace - sizeApp - sizeCache);
                    indicatorValueChanged = true;
                }
            }

            if (indicatorValueChanged)
            {
                storageIndicator.Update();
            }
        }

        private void GetMediaInfo(out double sizeImage, out double sizeVideo, out double sizeAudio)
        {
            sizeImage = sizeVideo = sizeAudio = 0;

            var mediaDatabase = new MediaDatabase();
            mediaDatabase.Connect();
            MediaDatabase.MediaInfoUpdated += OnMediaInfoUpdated;
            MediaDatabase.FolderUpdated += OnFolderUpdated;

            MediaInfoCommand mediaInfoCmd = new MediaInfoCommand(mediaDatabase);

            var selectArguments = new SelectArguments()
            {
                /* 0-image, 1-video, 2-sound, 3-music, 4-other
                cond = "((MEDIA_TYPE < 4) AND (MEDIA_STORAGE_TYPE=0))";
                FilterExpression = "((MEDIA_TYPE < 4) AND (MEDIA_STORAGE_TYPE=0))",
                SortOrder = "{MediaInfoColumns.DisplayName} COLLATE NOCASE DESC"
                FilterExpression = $"{MediaInfoColumns.DisplayName} LIKE '%%.png'" */
                FilterExpression = $"{MediaInfoColumns.MediaType}={(int)Media​Type.Image} OR {MediaInfoColumns.MediaType}={(int)Media​Type.Video} OR {MediaInfoColumns.MediaType}={(int)Media​Type.Music} OR {MediaInfoColumns.MediaType}={(int)Media​Type.Sound}",
            };

            using (var mediaDataReader = mediaInfoCmd.SelectMedia(selectArguments))
            {
                while (mediaDataReader.Read())
                {
                    var mediaInfo = mediaDataReader.Current;

                    switch (mediaInfo.MediaType)
                    {
                        case MediaType.Image:
                            ImageInfo imageInfo = mediaInfo as ImageInfo;
                            sizeImage += imageInfo.FileSize;
                            break;

                        case MediaType.Video:
                            VideoInfo videoInfo = mediaInfo as VideoInfo;
                            sizeVideo += videoInfo.FileSize;
                            break;

                        case MediaType.Sound:
                        case MediaType.Music:
                            AudioInfo audioInfo = mediaInfo as AudioInfo;
                            sizeAudio += audioInfo.FileSize;
                            break;

                        default:
                            Logger.Warn($"Invalid Type : {mediaInfo.MediaType}");
                            break;
                    }
                }
            }

            Logger.Debug("Total Size : ");
            Logger.Debug($"     - Image : {GetMediaSizeString(sizeImage)}");
            Logger.Debug($"     - Video : {GetMediaSizeString(sizeVideo)}" );
            Logger.Debug($"     - Audio : {GetMediaSizeString(sizeAudio)}");

            MediaDatabase.MediaInfoUpdated -= OnMediaInfoUpdated;
            MediaDatabase.FolderUpdated -= OnFolderUpdated;
            mediaDatabase.Disconnect();
        }

        private void GetMiscInfo(out double sizeMisc)
        {
            sizeMisc = 0;

            var mediaDatabase = new MediaDatabase();
            mediaDatabase.Connect();
            MediaDatabase.MediaInfoUpdated += OnMediaInfoUpdated;
            MediaDatabase.FolderUpdated += OnFolderUpdated;

            MediaInfoCommand mediaInfoCmd = new MediaInfoCommand(mediaDatabase);

            var selectArguments = new SelectArguments()
            {
                FilterExpression = $"{MediaInfoColumns.MediaType}!={(int)Media​Type.Image} AND {MediaInfoColumns.MediaType}!={(int)Media​Type.Video} AND {MediaInfoColumns.MediaType}!={(int)Media​Type.Music} AND {MediaInfoColumns.MediaType}!={(int)Media​Type.Sound}",
            };

            using (var mediaDataReader = mediaInfoCmd.SelectMedia(selectArguments))
            {
                while (mediaDataReader.Read())
                {
                    sizeMisc += mediaDataReader.Current.FileSize;
                }
            }

            Logger.Debug($"Misc size: {GetMediaSizeString(sizeMisc)}");

            MediaDatabase.MediaInfoUpdated -= OnMediaInfoUpdated;
            MediaDatabase.FolderUpdated -= OnFolderUpdated;
            mediaDatabase.Disconnect();
        }

        private void ShowCachePopup()
        {
            //title text
            var textTitle = new TextLabel(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_CLEAR_CACHE_DATA)))
            {
                FontFamily = "BreezeSans",
                PixelSize = 24.SpToPx(),
                Margin = new Extents(0, 0, 24, 16).SpToPx(),
            };

            View contentArea = new View()
            {
                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = LayoutParamPolicies.WrapContent,
                BackgroundColor = Color.Transparent,
                Layout = new LinearLayout()
                {
                    LinearOrientation = LinearLayout.Orientation.Vertical,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Top,
                    Margin = new Extents(0, 0, 8, 16),
                }
            };

            // main text
            var textSubTitle = new TextLabel()
            {
                StyleName = "LabelText",
                ThemeChangeSensitive = true,
                PixelSize = 18.SpToPx(),
                FontFamily = "BreezeSans",
                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = LayoutParamPolicies.WrapContent,
                HorizontalAlignment = HorizontalAlignment.Center,
                Text = NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_CACHED_DATA_WILL_BE_CLEARED)),
                MultiLine = true,
            };

            contentArea.Add(textSubTitle);

            // buttons
            View buttons = new View()
            {
                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = LayoutParamPolicies.WrapContent,
                Layout = new FlexLayout()
                {
                    Direction = FlexLayout.FlexDirection.Row,
                    Justification = FlexLayout.FlexJustification.SpaceBetween,
                },
            };

            var clearButton = new Button()
            {
                WidthResizePolicy = ResizePolicyType.FitToChildren,
                HeightResizePolicy = ResizePolicyType.FitToChildren,
                Text = NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_CLEAR)),
                Size = AppAttributes.PopupActionButtonSize,
                Margin = AppAttributes.PopupActionButtonMargin,
            };
            clearButton.Clicked += (s, e) =>
            {
                PackageManager.ClearAllCacheDirectory();
                // TODO : update cache info

                RemoveCachedDataPopup();
            };

            var cancelButton = new Button("Tizen.NUI.Components.Button.Outlined")
            {
                WidthResizePolicy = ResizePolicyType.FitToChildren,
                HeightResizePolicy = ResizePolicyType.FitToChildren,
                Text = NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BUTTON_CANCEL)),
                Size = AppAttributes.PopupActionButtonSize,
                Margin = AppAttributes.PopupActionButtonMargin,
            };

            buttons.Add(cancelButton);
            buttons.Add(clearButton);

            cachedDataPopupDialog = new AlertDialog()
            {
                ThemeChangeSensitive = true,
                StyleName = "Dialogs",
                WidthSpecification = AppAttributes.IsPortrait ? AppAttributes.WindowWidth - 64.SpToPx() : (int)(AppAttributes.WindowWidth * 0.7f),
                HeightSpecification = LayoutParamPolicies.WrapContent,
                Layout = new LinearLayout()
                {
                    Padding = new Extents(80, 80, 40, 40).SpToPx(),
                    Margin = new Extents(32, 32, 0, 0).SpToPx(),
                    LinearOrientation = LinearLayout.Orientation.Vertical,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Top,
                },
                TitleContent = textTitle,
                Content = contentArea,
                ActionContent = buttons,
                BoxShadow = AppAttributes.PopupBoxShadow,
            };

            AppAttributes.DefaultWindow.Add(cachedDataPopupDialog);

            cancelButton.Clicked += (o, e) =>
            {
                RemoveCachedDataPopup();
            };
        }

        private void RemoveCachedDataPopup()
        {
            if (cachedDataPopupDialog != null)
            {
                AppAttributes.DefaultWindow.Remove(cachedDataPopupDialog);
                cachedDataPopupDialog.Hide();
                cachedDataPopupDialog.Dispose();
            }
        }

        private void OnMediaInfoUpdated(object sender, MediaInfoUpdatedEventArgs args)
        {
            Logger.Debug($"MediaInfo updated: Id = {args.Id}, Operation = {args.OperationType}");
        }

        private void OnFolderUpdated(object sender, FolderUpdatedEventArgs args)
        {
            Logger.Debug($"Folder updated: Id = {args.Id}, Operation = {args.OperationType}");
        }

        private void OnCardStatusChanged(string key, Type type, dynamic arg)
        {
            // update content when card was removed (0) or mounted (1)
            if (arg == 0 || arg == 1)
            {
                for (int i = (int)externalStorage.ChildCount - 1; i >= 0; --i)
                {
                    View child = externalStorage.GetChildAt((uint)i);

                    if (child == null)
                    {
                        continue;
                    }

                    externalStorage.Remove(child);
                    child.Dispose();
                }

                if (arg > 0)
                {
                    GetStorageStatus(out int InternalCount, out double InternalTotal, out int ExternalCount, out double InternalAvailable, out double ExternalTotal, out double ExternalAvailable);

                    var totalItem = TextListItem.CreatePrimaryTextItemWithSecondaryText($"{NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_HEADER_TOTAL_SPACE))}:", GetMediaSizeString(ExternalTotal));
                    externalStorage.Add(totalItem);

                    var freeItem = TextListItem.CreatePrimaryTextItemWithSecondaryText($"{NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_HEADER_FREE_SPACE))}:", GetMediaSizeString(ExternalAvailable));
                    externalStorage.Add(freeItem);

                    var unmount = TextListItem.CreatePrimaryTextItem(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_UNMOUNT_SD_CARD)));
                    unmount.Clicked += (s, e) =>
                    {
                        // TODO : add popup with unmount functionality (storage_request_unmount_mmc)
                    };
                    externalStorage.Add(unmount);

                    var format = TextListItem.CreatePrimaryTextItem(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_FORMAT_SD_CARD)));
                    format.Clicked += (s, e) =>
                    {
                        // TODO : add popup with format card functionality (storage_request_format_mmc)
                    };
                    externalStorage.Add(format);
                }
                else
                {
                    var item = TextListItem.CreatePrimaryTextItemWithSubText(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_NO_SD_CARD)), NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_INSERT_SD_CARD)));
                    item.IsEnabled = false;
                    externalStorage.Add(item);
                }
            }
        }

        protected override void OnCustomizationUpdate(IEnumerable<MenuCustomizationItem> items)
        {
            Logger.Verbose($"{nameof(DisplayGadget)} got customization with {items.Count()} items. Recreating view.");
            CreateView();
        }
    }
}
