/*
 *  Copyright (c) 2022 Samsung Electronics Co., Ltd All Rights Reserved
 *
 *  Licensed under the Apache License, Version 2.0 (the "License");
 *  you may not use this file except in compliance with the License.
 *  You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License
 */

using System;
using System.Collections.Generic; // for Dictionary
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;
using Tizen.Applications;

using Tizen.System;
using Tizen.Content.MediaContent;

using System.Threading;
using System.Threading.Tasks;

using SettingAppTextResopurces.TextResources;

namespace SettingMain
{
    class SettingContent_Storage : SettingContent_Base
    {
        View mContent;

        DefaultLinearItem mAppsItem;
        DefaultLinearItem mCacheItem;

        private const string LogTag = "NUI";

        public SettingContent_Storage()
            : base()
        {
            mTitle = Resources.IDS_ST_BODY_DEVICE_STORAGE;
            mContent = null;

            mAppsItem = null;
            mCacheItem = null;
        }

        protected override View CreateContent(Window window)
        {
            // Content of the page which scrolls items vertically.
            var content = new ScrollableBase()
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
            mContent = content;

            DefaultLinearItem item;

            GetStorageStatus(out int InternalCount, out double InternalTotal, out int ExternalCount, out double InternalAvailable, out double ExternalTotal, out double ExternalAvailable);

            if (InternalCount > 0)
            {
                string mainText = "Internal Storage";
                string subText = Resources.IDS_ST_BODY_USED + " : " + GetMediaSizeString(InternalTotal - InternalAvailable) + ", "
                               + Resources.IDS_ST_HEADER_TOTAL_SPACE + " : " + GetMediaSizeString(InternalTotal) + ", "
                               + Resources.IDS_SM_BODY_FREE_M_MEMORY_ABB + " : " + GetMediaSizeString(InternalAvailable);

                item = SettingItemCreator.CreateItemWithCheck(mainText, subText);
                content.Add(item);
            }
            if (ExternalCount > 0)
            {
                string mainText = "External Storage";
                string subText = Resources.IDS_ST_BODY_USED + " : " + GetMediaSizeString(ExternalTotal - ExternalAvailable) + ", "
                               + Resources.IDS_ST_HEADER_TOTAL_SPACE + " : " + GetMediaSizeString(ExternalTotal) + ", "
                               + Resources.IDS_SM_BODY_FREE_M_MEMORY_ABB + " : " + GetMediaSizeString(ExternalAvailable);

                item = SettingItemCreator.CreateItemWithCheck(mainText, subText);
                content.Add(item);
            }

            content.Add(SettingItemCreator.CreateItemStatic(""));

            GetMediaInfo(out double sizeImage, out double sizeVideo, out double sizeAudio);
            item = SettingItemCreator.CreateItemWithIcon(Resources.IDS_ST_BODY_IMAGES, resPath + SETTING_ICON_PATH_CFG + "storage/12_images_normal.png", GetMediaSizeString(sizeImage));
            if (item != null)
            {
                content.Add(item);
            }
            item = SettingItemCreator.CreateItemWithIcon(Resources.IDS_ST_BODY_VIDEOS, resPath + SETTING_ICON_PATH_CFG + "storage/12_videos_normal.png", GetMediaSizeString(sizeVideo));
            if (item != null)
            {
                content.Add(item);
            }
            item = SettingItemCreator.CreateItemWithIcon(Resources.IDS_ST_BODY_AUDIO, resPath + SETTING_ICON_PATH_CFG + "storage/12_audio_normal.png", GetMediaSizeString(sizeAudio));
            if (item != null)
            {
                content.Add(item);
            }

            GetMiscInfo(out double sizeMisc);
            item = SettingItemCreator.CreateItemWithIcon(Resources.IDS_ST_BODY_MISCELLANEOUS_FILES, resPath + SETTING_ICON_PATH_CFG + "storage/12_misc_normal.png", GetMediaSizeString(sizeMisc));
            if (item != null)
            {
                item.Clicked += (o, e) =>
                {
                    //RequestWidgetPush("applist@org.tizen.cssettings");
                };
                content.Add(item);
            }

            item = SettingItemCreator.CreateItemWithIcon(Resources.IDS_SM_TMBODY_APPS_ABB, resPath + SETTING_ICON_PATH_CFG + "storage/12_apps_normal.png", Resources.IDS_SM_SBODY_CALCULATING_ING);
            mAppsItem = item;
            if (item != null)
            {
                item.Clicked += (o, e) =>
                {
                    //RequestWidgetPush("applist@org.tizen.cssettings");
                };
                content.Add(item);
            }

            item = SettingItemCreator.CreateItemWithIcon(Resources.IDS_ST_BODY_CACHED_DATA_ABB, resPath + SETTING_ICON_PATH_CFG + "storage/12_cached_data_normal.png", Resources.IDS_SM_SBODY_CALCULATING_ING);
            mCacheItem = item;
            if (item != null)
            {
                item.Clicked += (o, e) =>
                {
                    var dialog = new AlertDialog()
                    {
                        Title = Resources.IDS_ST_HEADER_CLEAR_CACHE_ABB,
                        Message = Resources.IDS_ST_POP_ALL_THE_CACHE_DATA_WILL_BE_CLEARED,
                    };

                    var buttonCancel = new Button()
                    {
                        Text = Resources.IDS_ST_BUTTON_CANCEL,
                    };
                    buttonCancel.Clicked += (abo, abe) =>
                    {
                        RequestWidgetPop();
                    };
                    var buttonOK = new Button()
                    {
                        Text = Resources.IDS_ST_BUTTON_OK,
                    };
                    buttonOK.Clicked += (abo, abe) =>
                    {
                        PackageManager.ClearAllCacheDirectory();
                        RequestWidgetPop();
                    };

                    dialog.Actions = new View[] { buttonCancel, buttonOK };
                    window.Add(dialog);
                };
                content.Add(item);
            }


            /* Default storage */
            content.Add(SettingItemCreator.CreateItemStatic(""));

            /* "Default storage locations" item with status */
            item = SettingItemCreator.CreateItemWithCheck(Resources.IDS_SM_HEADER_DEFAULT_STORAGE_SETTINGS_ABB);
            if (item != null)
            {
                item.Clicked += (o, e) =>
                {
                    RequestWidgetPush("defaultstorage@org.tizen.cssettings");
                };
                content.Add(item);
            }


            AppendSDInfo(content);


#if false
	ad->usb_otg_status = SETTING_STORAGE_USB_OTG_REMOVE;
	storage_init_USB(ad);
#endif



            StartCalculatingAppCacheSize();


            return content;
        }



        private static void GetStorageStatus(out int InternalCount, out double InternalTotal, out int ExternalCount, out double InternalAvailable, out double ExternalTotal, out double ExternalAvailable)
        {
            InternalCount = ExternalCount = 0;
            InternalTotal = ExternalTotal = 0.0;
            InternalAvailable = ExternalAvailable = 0.0;

            IEnumerator<Tizen.System.Storage> storages = StorageManager.Storages.GetEnumerator();
            while (storages.MoveNext())
            {
                Tizen.System.Storage storage = storages.Current;
                if (storage.StorageType == Tizen.System.StorageArea.Internal || storage.StorageType == Tizen.System.StorageArea.ExtendedInternal)
                {
                    InternalCount++;
                    InternalTotal += storage.TotalSpace;
                    InternalAvailable += storage.AvailableSpace;
                }
                else if (storage.StorageType == Tizen.System.StorageArea.External)
                {
                    ExternalCount++;
                    ExternalTotal += storage.TotalSpace;
                    ExternalAvailable += storage.AvailableSpace;
                }
            }

        }

        private void StartCalculatingAppCacheSize()
        {
            Task<PackageSizeInformation> task = PackageManager.GetTotalSizeInformationAsync();
            task.ContinueWith(x =>
            {
                PackageSizeInformation sizeinfo = x.Result;
                long sizeApp = sizeinfo.AppSize;
                long sizeCache = sizeinfo.CacheSize;
                Tizen.Log.Debug("NUI", "GetAppCacheSize() Task complete!!,  " + sizeApp.ToString() + ", " + sizeCache.ToString());

                if (mAppsItem)
                {
                    mAppsItem.SubText = GetMediaSizeString((double)sizeApp);
                    mAppsItem.Hide();
                    mAppsItem.Show();
                }
                if (mCacheItem) {
                    mCacheItem.SubText = GetMediaSizeString((double)sizeCache);
                    mAppsItem.Hide();
                    mCacheItem.Show();
                }
            });
        }

        public void VconfChanged_SDCardStatus(IntPtr node, IntPtr userData)
        {

            int mmcStatus = Vconf.GetInt("memory/sysman/mmc");

            // AppendSDInfo(mContent);
        }

        void AppendSDInfo(View content)
        {
            if(content == null) return;

            //VCONFKEY_SYSMAN_MMC_STATUS
            int mmc_mode = Vconf.GetInt("memory/sysman/mmc");

            Tizen.Log.Debug("NUI", string.Format("mmc_mode: "+ mmc_mode.ToString()));




#if false
            content.Add(SettingItemCreator.CreateItemStatic(Resources.IDS_ST_BODY_SD_CARD));

            item = SettingItemCreator.CreateItemWithCheck(Resources.IDS_ST_BODY_MOUNT_SD_CARD, null);
            if (item != null)
            {
                item.Clicked += (o, e) =>
                {
                };
                content.Add(item);
            }

#if false
            if (-1 == mmc_mode)
                mmc_mode = VCONFKEY_SYSMAN_MMC_REMOVED;

            if (VCONFKEY_SYSMAN_MMC_REMOVED == mmc_mode)
                storage_SD_info_removed(ad);
            if (VCONFKEY_SYSMAN_MMC_INSERTED_NOT_MOUNTED == mmc_mode)
                storage_SD_info_inserted_not_mounted(ad);
            if (VCONFKEY_SYSMAN_MMC_MOUNTED == mmc_mode)
                storage_SD_info_portable_mounted(ad);
            if (VCONFKEY_SYSMAN_MMC_EXTENDEDINTERNAL_MOUNTED == mmc_mode)
                storage_SD_info_extended_mounted(ad);
#endif

#endif

        }



        private void GetMediaInfo(out double sizeImage, out double sizeVideo, out double sizeAudio)
        {
            sizeImage = sizeVideo = sizeAudio = 0;

            var mediaDatabase = new MediaDatabase();
            mediaDatabase.Connect();
            MediaDatabase.MediaInfoUpdated += OnMediaInfoUpdated;
            MediaDatabase.FolderUpdated += OnFolderUpdated;


            MediaInfoCommand mediaInfoCmd = new MediaInfoCommand(mediaDatabase);

            //////////////////////////////
            var selectArguments = new SelectArguments()
            {
                /*0-image, 1-video, 2-sound, 3-music, 4-other*/
                //cond = "((MEDIA_TYPE < 4) AND (MEDIA_STORAGE_TYPE=0))";
                FilterExpression = $"{MediaInfoColumns.MediaType}={(int)Media​Type.Image} OR {MediaInfoColumns.MediaType}={(int)Media​Type.Video} OR {MediaInfoColumns.MediaType}={(int)Media​Type.Music} OR {MediaInfoColumns.MediaType}={(int)Media​Type.Sound}",
                //FilterExpression = "((MEDIA_TYPE < 4) AND (MEDIA_STORAGE_TYPE=0))",
                //SortOrder = "{MediaInfoColumns.DisplayName} COLLATE NOCASE DESC"
            };

//            double sizeBook = 0;

            using (var mediaDataReader = mediaInfoCmd.SelectMedia(selectArguments))
            {
                while (mediaDataReader.Read())
                {
                    var mediaInfo = mediaDataReader.Current;

                    Tizen.Log.Info(LogTag, $"Id={mediaInfo.Id}, Name={mediaInfo.DisplayName}, Path={mediaInfo.Path}");

                    switch (mediaInfo.MediaType)
                    {
                        case MediaType.Image:
                            ImageInfo imageInfo = mediaInfo as ImageInfo;
                            sizeImage += imageInfo.FileSize;
                            Tizen.Log.Info(LogTag, mediaInfo.MediaType.ToString() + ", Size : " + imageInfo.FileSize);
                            break;

                        case MediaType.Video:
                            VideoInfo videoInfo = mediaInfo as VideoInfo;
                            sizeVideo += videoInfo.FileSize;
                            Tizen.Log.Info(LogTag, mediaInfo.MediaType.ToString() + ", Size : " + videoInfo.FileSize);
                            break;

                        case MediaType.Sound:
                        case MediaType.Music:
                            AudioInfo audioInfo = mediaInfo as AudioInfo;
                            sizeAudio += audioInfo.FileSize;
                            Tizen.Log.Info(LogTag, mediaInfo.MediaType.ToString() + ", Size : " + audioInfo.FileSize);
                            break;

                        default:
                            Tizen.Log.Info(LogTag, "Invalid Type : ", mediaInfo.MediaType.ToString());
                            break;
                    }
                }
            }


            Tizen.Log.Info(LogTag, "Total Size : ");
            Tizen.Log.Info(LogTag, "     - Image : " + sizeImage);
            Tizen.Log.Info(LogTag, "     - Video : " + sizeVideo);
            Tizen.Log.Info(LogTag, "     - Audio : " + sizeAudio);
            //////////////////////////////

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

            //////////////////////////////
            var selectArguments = new SelectArguments()
            {
                /*0-image, 1-video, 2-sound, 3-music, 4-other*/
                FilterExpression = $"{MediaInfoColumns.MediaType}!={(int)Media​Type.Image} AND {MediaInfoColumns.MediaType}!={(int)Media​Type.Video} AND {MediaInfoColumns.MediaType}!={(int)Media​Type.Music} AND {MediaInfoColumns.MediaType}!={(int)Media​Type.Sound}",
                //FilterExpression = "((MEDIA_TYPE=4) AND (MEDIA_STORAGE_TYPE=0))",
                //SortOrder = "{MediaInfoColumns.DisplayName} COLLATE NOCASE DESC",
            };

            using (var mediaDataReader = mediaInfoCmd.SelectMedia(selectArguments))
            {
                while (mediaDataReader.Read())
                {
                    var mediaInfo = mediaDataReader.Current;

                    Tizen.Log.Info(LogTag, $"Id={mediaInfo.Id}, Name={mediaInfo.DisplayName}, Path={mediaInfo.Path}, Size={mediaInfo.FileSize}");

                    sizeMisc += mediaInfo.FileSize;

                }
            }


            Tizen.Log.Info(LogTag, "Total Size : ");
            Tizen.Log.Info(LogTag, "     - Misc : " + sizeMisc);
            //////////////////////////////

            MediaDatabase.MediaInfoUpdated -= OnMediaInfoUpdated;
            MediaDatabase.FolderUpdated -= OnFolderUpdated;
            mediaDatabase.Disconnect();

        }

        void OnMediaInfoUpdated(object sender, MediaInfoUpdatedEventArgs args)
        {
            Tizen.Log.Info(LogTag, $"MediaInfo updated: Id = {args.Id}, Operation = {args.OperationType}");
        }

        void OnFolderUpdated(object sender, FolderUpdatedEventArgs args)
        {
            Tizen.Log.Info(LogTag, $"Folder updated: Id = {args.Id}, Operation = {args.OperationType}");
        }


        private static string GetMediaSizeString(double size)
        {
            size = size / 1024;
            if (size < 1024.0)
                return string.Format("{0:0.0}", size) + " KB";

            size = size / 1024;
            if (size < 1024.0)
                return string.Format("{0:0.0}", size) + " MB";

            size = size / 1024;
            return string.Format("{0:0.0}", size) + " GB";
        }




        protected override void OnCreate(string contentInfo, Window window)
        {
            base.OnCreate(contentInfo, window);

            Vconf.NotifyKeyChanged("memory/sysman/mmc", VconfChanged_SDCardStatus);
        }

        protected override void OnTerminate(string contentInfo, TerminationType type)
        {
            Vconf.IgnoreKeyChanged("memory/sysman/mmc", VconfChanged_SDCardStatus);

            base.OnTerminate(contentInfo, type);
        }
    }
}
