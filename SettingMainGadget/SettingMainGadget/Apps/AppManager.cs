using SettingCore;
using System;
using System.Collections.Generic;
using System.Linq;
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
                Logger.Warn($"Cann't get application running context: {ex.Message}");
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
    }
}
