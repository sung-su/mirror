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

namespace Setting.Menu
{
    public class StorageGadget : MainMenuGadget
    {
        public override Color ProvideIconColor() => new Color(IsLightTheme ? "#7F2982" : "#922F95");

        public override string ProvideIconPath() => GetResourcePath("storage.svg");

        public override string ProvideTitle() => Resources.IDS_ST_BODY_DEVICE_STORAGE;

        private const string VconfCardStatus = "memory/sysman/mmc";

        private bool isLightTheme => ThemeManager.PlatformThemeId == "org.tizen.default-light-theme";

        private Sections sections = new Sections();
        private View content;

        private TextWithIconListItem appsItem;
        private TextWithIconListItem cacheItem;
        private TextWithIconListItem systemItem;

        private StorageIndicator storageIndicator;

        private double nonSystemSpace = 0;

        protected override View OnCreate()
        {
            base.OnCreate();

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

            CreateView();

            var calculating = StartCalculatingAppCacheSize();

            content.Relayout += (s, e) =>
            {
                if (calculating.IsCompleted)
                {
                    storageIndicator.Update();
                }
            };

            return content;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        private void CreateView()
        {
            sections.RemoveAllSectionsFromView(content);

            // Internal storage
            var internalUsageItem = new TextHeaderListItem("Device storage usage"); // TODO : add translation to Resources
            sections.Add(MainMenuProvider.Storage_InternalUsage, internalUsageItem);

            GetStorageStatus(out int InternalCount, out double InternalTotal, out int ExternalCount, out double InternalAvailable, out double ExternalTotal, out double ExternalAvailable);
            GetMediaInfo(out double sizeImage, out double sizeVideo, out double sizeAudio);
            GetMiscInfo(out double sizeMisc);

            nonSystemSpace = InternalTotal - InternalAvailable - sizeImage - sizeVideo - sizeAudio - sizeMisc;

            if (InternalCount > 0)
            {
                var usedItem = TextListItem.CreatePrimaryTextItem(GetMediaSizeString(InternalTotal - InternalAvailable));
                sections.Add(MainMenuProvider.Storage_Used, usedItem);

                storageIndicator = new StorageIndicator(InternalTotal);
                storageIndicator.AddItem("apps", new Color("#FFC700"), 0);
                storageIndicator.AddItem("images", new Color("#FF8A00"), sizeImage);
                storageIndicator.AddItem("video", new Color("#FF6200"), sizeVideo);
                storageIndicator.AddItem("audio", new Color("#A40404"), sizeAudio);
                storageIndicator.AddItem("misc", new Color("#28262B"), sizeMisc);
                storageIndicator.AddItem("cache", new Color("#3641FA"), 0);
                storageIndicator.AddItem("system", new Color("#17234D"), 0);
                sections.Add(MainMenuProvider.Storage_UsageIndicator, storageIndicator);

                var totalItem = TextListItem.CreatePrimaryTextItemWithSecondaryText($"{Resources.IDS_ST_HEADER_TOTAL_SPACE}:", GetMediaSizeString(InternalTotal));
                sections.Add(MainMenuProvider.Storage_TotalInternal, totalItem);

                var freeItem = TextListItem.CreatePrimaryTextItemWithSecondaryText("Free space:", GetMediaSizeString(InternalAvailable)); // TODO : add translation to Resources
                sections.Add(MainMenuProvider.Storage_FreeInternal, freeItem);
            }

            var usageSummary = new View()
            {
                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = LayoutParamPolicies.WrapContent,
                Layout = new LinearLayout()
                {
                    LinearOrientation = LinearLayout.Orientation.Vertical,
                },
            };

            appsItem = new TextWithIconListItem(Resources.IDS_SM_TMBODY_APPS_ABB, new Color("#FFC700"), subText: Resources.IDS_SM_SBODY_CALCULATING_ING);
            appsItem.Clicked += (s, e) =>
            {
                // TODO : add apps info gadget 
            };
            usageSummary.Add(appsItem);

            var imageItem = new TextWithIconListItem(Resources.IDS_ST_BODY_IMAGES, new Color("#FF8A00"), subText: GetMediaSizeString(sizeImage));
            imageItem.Clicked += (s, e) =>
            {
                // TODO : add media files info gadget 
            };
            usageSummary.Add(imageItem);

            var videoItem = new TextWithIconListItem(Resources.IDS_ST_BODY_VIDEOS, new Color("#FF6200"), subText: GetMediaSizeString(sizeVideo));
            videoItem.Clicked += (s, e) =>
            {
                // TODO : add media files info gadget 
            };
            usageSummary.Add(videoItem);

            var audioItem = new TextWithIconListItem(Resources.IDS_ST_BODY_AUDIO, new Color("#A40404"), subText: GetMediaSizeString(sizeAudio));
            audioItem.Clicked += (s, e) =>
            {
                // TODO : add media files info gadget 
            };
            usageSummary.Add(audioItem);

            // TODO : add documents item

            var miscItem = new TextWithIconListItem(Resources.IDS_ST_BODY_MISCELLANEOUS_FILES, new Color("#28262B"), subText: GetMediaSizeString(sizeMisc));
            miscItem.Clicked += (s, e) =>
            {
                // TODO : add miscellaneous info gadget 
            };
            usageSummary.Add(miscItem);

            cacheItem = new TextWithIconListItem(Resources.IDS_ST_BODY_CACHED_DATA_ABB, new Color("#3641FA"), subText: Resources.IDS_SM_SBODY_CALCULATING_ING);
            cacheItem.Clicked += (s, e) =>
            {
                ShowCachePopup();
            };
            usageSummary.Add(cacheItem);            
            
            systemItem = new TextWithIconListItem(Resources.IDS_ST_BODY_SYSTEM_STORAGE, new Color("#17234D"), subText: Resources.IDS_SM_SBODY_CALCULATING_ING);
            usageSummary.Add(systemItem);

            sections.Add(MainMenuProvider.Storage_UsageSummary, usageSummary);

            // External storage
            var externalUsageItem = new TextHeaderListItem("External storage usage"); // TODO : add translation to Resources
            sections.Add(MainMenuProvider.Storage_ExternalUsage, externalUsageItem);

            var externalStorage = new View()
            {
                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = LayoutParamPolicies.WrapContent,
                Layout = new LinearLayout()
                {
                    LinearOrientation = LinearLayout.Orientation.Vertical,
                },
            };

            if (ExternalCount > 0)
            {
                // VCONFKEY_SYSMAN_MMC_STATUS
                // 0 : VCONFKEY_SYSMAN_MMC_REMOVED
                // 1 : VCONFKEY_SYSMAN_MMC_MOUNTED
                // 2 : VCONFKEY_SYSMAN_MMC_INSERTED_NOT_MOUNTED
                if (!Tizen.Vconf.TryGetInt(VconfCardStatus, out int status))
                {
                    Logger.Warn($"could not get value for {VconfCardStatus}");
                }

                var totalItem = TextListItem.CreatePrimaryTextItemWithSecondaryText($"{Resources.IDS_ST_HEADER_TOTAL_SPACE}:", GetMediaSizeString(ExternalTotal));
                externalStorage.Add(totalItem);

                var freeItem = TextListItem.CreatePrimaryTextItemWithSecondaryText("Free space:", GetMediaSizeString(ExternalAvailable)); // TODO : add translation to Resources
                externalStorage.Add(freeItem);

                var unmount = TextListItem.CreatePrimaryTextItem(Resources.IDS_ST_BODY_UNMOUNT_SD_CARD);
                unmount.Clicked += (s, e) =>
                {
                    // TODO : add popup with unmount functionality (storage_request_unmount_mmc)
                };
                externalStorage.Add(unmount);

                var format = TextListItem.CreatePrimaryTextItem(Resources.IDS_ST_BODY_FORMAT_SD_CARD);
                format.Clicked += (s, e) =>
                {
                    // TODO : add popup with format card functionality (storage_request_format_mmc)
                };
                externalStorage.Add(format);
            }
            else
            {
                var item = TextListItem.CreatePrimaryTextItemWithSubText("No SD card", Resources.IDS_ST_BODY_INSERT_SD_CARD); // TODO : add translation to Resources
                item.IsEnabled = false;
                externalStorage.Add(item);
            }
            sections.Add(MainMenuProvider.Storage_ExternalStorage, externalStorage);

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

            var defaultSettingsItem = new TextHeaderListItem("Storage usage"); // TODO : add translation to Resources
            var storageLocationItem = TextListItem.CreatePrimaryTextItemWithSubText("Default storage settings", "Device/Storage"); // FIXME : sub text should be dynamic & add translation to Resources
            storageLocationItem.Clicked += (s, e) =>
            {
                NavigateTo(MainMenuProvider.Storage_DefaultSettings);
            };
            defaultSettings.Add(defaultSettingsItem);
            defaultSettings.Add(storageLocationItem);
            sections.Add(MainMenuProvider.Storage_DefaultSettings, defaultSettings);

            var customization = GetCustomization().OrderBy(c => c.Order);
            Logger.Debug($"customization: {customization.Count()}");
            foreach (var cust in customization)
            {
                string visibility = cust.IsVisible ? "visible" : "hidden";
                Logger.Verbose($"Customization: {cust.MenuPath} - {visibility} - {cust.Order}");
                if (cust.IsVisible && sections.TryGetValue(cust.MenuPath, out View row))
                {
                    content.Add(row);
                }
            }
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
            var sizeInfo = await PackageManager.GetTotalSizeInformationAsync();

            long sizeApp = sizeInfo.AppSize;
            long sizeCache = sizeInfo.CacheSize;

            if (appsItem != null)
            {
                appsItem.SubText = GetMediaSizeString(sizeApp);
            }
            if (cacheItem != null)
            {
                cacheItem.SubText = GetMediaSizeString(sizeCache);
            }
            if (systemItem != null)
            {
                systemItem.SubText = GetMediaSizeString(nonSystemSpace - sizeApp - sizeCache);
            }

            storageIndicator.SizeInfoList.Where(x => x.Name == "apps").FirstOrDefault()?.SetSize(sizeApp);
            storageIndicator.SizeInfoList.Where(x => x.Name == "cache").FirstOrDefault()?.SetSize(sizeCache);
            storageIndicator.SizeInfoList.Where(x => x.Name == "system").FirstOrDefault()?.SetSize(nonSystemSpace - sizeApp - sizeCache);

            storageIndicator.Update();
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
            var content = new View()
            {
                BackgroundColor = isLightTheme ? new Color("#FAFAFA") : new Color("#16131A"),
                SizeWidth = 690.SpToPx(),
                HeightSpecification = LayoutParamPolicies.WrapContent,
                Layout = new FlexLayout()
                {
                    Justification = FlexLayout.FlexJustification.Center,
                    Direction = FlexLayout.FlexDirection.Column, 
                    Alignment = FlexLayout.AlignmentType.Center,
                },
            };

            //title text
            var textTitle = new TextLabel("Clear cache data") // TODO : add translation to Resources
            {
                FontFamily = "BreezeSans",
                PixelSize = 24.SpToPx(),
                Margin = new Extents(0, 0, 24, 16).SpToPx(),
            };
            FlexLayout.SetFlexAlignmentSelf(textTitle, FlexLayout.AlignmentType.Center);

            content.Add(textTitle);

            // main text
            var textSubTitle = new TextLabel("Cached data will be cleared for all apps.") // TODO : add translation to Resources
            {
                FontFamily = "BreezeSans",
                PixelSize = 24.SpToPx(),
                Ellipsis = true,
                Margin = new Extents(32, 32, 0, 40).SpToPx(),
            };
            FlexLayout.SetFlexAlignmentSelf(textSubTitle, FlexLayout.AlignmentType.FlexStart);
            content.Add(textSubTitle);

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
                Text = "Clear", // TODO : add translation to Resources
                Size = new Size(252, 48).SpToPx(),
                Margin = new Extents(61, 32, 0, 32).SpToPx(),
            };
            clearButton.Clicked += (s, e) => 
            {
                PackageManager.ClearAllCacheDirectory();
                // TODO : update cache info

                NUIApplication.GetDefaultWindow().GetDefaultNavigator().Pop();
            };

            var cancelButton = new Button("Tizen.NUI.Components.Button.Outlined")
            {
                WidthResizePolicy = ResizePolicyType.FitToChildren,
                HeightResizePolicy = ResizePolicyType.FitToChildren,
                Text = Resources.IDS_ST_BUTTON_CANCEL,
                Size = new Size(252, 48).SpToPx(),
                Margin = new Extents(32, 61, 0, 32).SpToPx(),
            };

            cancelButton.Clicked += (s, e) => 
            { 
                NUIApplication.GetDefaultWindow().GetDefaultNavigator().Pop(); 
            };

            buttons.Add(cancelButton);
            buttons.Add(clearButton);

            content.Add(buttons);

            RoundedDialogPage.ShowDialog(content);
        }

        private void OnMediaInfoUpdated(object sender, MediaInfoUpdatedEventArgs args)
        {
            Logger.Debug($"MediaInfo updated: Id = {args.Id}, Operation = {args.OperationType}");
        }

        private void OnFolderUpdated(object sender, FolderUpdatedEventArgs args)
        {
            Logger.Debug($"Folder updated: Id = {args.Id}, Operation = {args.OperationType}");
        }

        protected override void OnCustomizationUpdate(IEnumerable<MenuCustomizationItem> items)
        {
            Logger.Verbose($"{nameof(DisplayGadget)} got customization with {items.Count()} items. Recreating view.");
            CreateView();
        }
    }
}
