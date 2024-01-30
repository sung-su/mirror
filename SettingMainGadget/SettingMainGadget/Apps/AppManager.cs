using SettingCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Tizen.Applications;

namespace SettingMainGadget.Apps
{
    public static class AppManager
    {
        public static Package CurrentApp { get; set; }

        public static ApplicationRunningContext GetRunningContext()
        {
            try
            {
                var appContext = new ApplicationRunningContext(CurrentApp.Id);
                return appContext;
            }
            catch (Exception ex)
            {
                Logger.Warn($"Can't get application running context: {ex.Message}");
                return null;
            }
        }

        public static string GetSizeString(double size)
        {
            string[] suffixes = { "Bytes", "KB", "MB", "GB" };
            int counter = 0;

            while (Math.Round(size / 1024, 2) >= 1)
            {
                size = size / 1024;
                counter++;
            }

            return string.Format("{0:0.##} {1}", size, suffixes[counter]);
        }

        public static List<ApplicationInfo> GetApplicationsInfoByCategory(string category)
        {
            var allPackages = PackageManager.GetPackages().ToList();
            var listInfo = new List<ApplicationInfo>();

            foreach (var package in allPackages)
            {
                listInfo.Add(new ApplicationInfo(package.Id));
            }

            return listInfo.Where(a => a.Categories.Contains(category)).ToList();
        }

        public class ApplicationItemInfo : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;
            public string AppId { get; }
            public string PackageId { get; set; }
            public string Name { get; set; }
            public string IconPath { get; set; }
            public long AppSize { get; set; }
            public System.DateTime LastLaunchTime { get; set; }

            private string size;
            public string SizeToDisplay
            {
                get => size;
                set
                {
                    if (value != size)
                    {
                        size = value;
                        RaisePropertyChanged(nameof(SizeToDisplay));
                    }
                }
            }

            public ApplicationItemInfo(string appid, string name, string iconPath, string size)
            {
                AppId = appid;
                Name = name;
                IconPath = iconPath;
                SizeToDisplay = size;
            }

            /// <summary>
            /// Raises PropertyChanged event.
            /// </summary>
            protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
