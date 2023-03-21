using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace SettingCore.Customization
{
    internal class FileStorage
    {
        private static readonly FileStorage instance = new FileStorage();
        public static FileStorage Instance => instance;

        private System.IO.FileSystemWatcher fsw;

        public event Action Changed;
        public event Action Lost;

        private readonly string InitialFileName;
        private readonly string CurrentFileName;
        private readonly string DirectoryPath;

        public static string InitialFilePath { get; private set; }
        public static string CurrentFilePath { get; private set; }

        private FileStorage()
        {
            InitialFileName = "initial.json";
            CurrentFileName = "current.json";
            DirectoryPath = Tizen.Applications.Application.Current.DirectoryInfo.Data;
            InitialFilePath = System.IO.Path.Combine(DirectoryPath, InitialFileName);
            CurrentFilePath = System.IO.Path.Combine(DirectoryPath, CurrentFileName);
        }

        private void OnFileChanged(object sender, System.IO.FileSystemEventArgs e)
        {
            InvokeChangedEventHandler(e.ChangeType);
        }

        private void OnFileRenamed(object sender, System.IO.RenamedEventArgs e)
        {
            InvokeLostEventHandler(e.ChangeType);
        }

        private void OnFileDeleted(object sender, System.IO.FileSystemEventArgs e)
        {
            InvokeLostEventHandler(e.ChangeType);
        }

        private void InvokeChangedEventHandler(System.IO.WatcherChangeTypes changeType)
        {
            Logger.Warn($"Invoking {changeType}");

            Tizen.Applications.CoreApplication.Post(() =>
            {
                var handler = Changed;
                handler?.Invoke();
            });
        }

        private async void InvokeLostEventHandler(System.IO.WatcherChangeTypes changeType)
        {
            Logger.Warn($"Invoking {changeType}");

            await System.Threading.Tasks.Task.Delay(200);
            bool exists = System.IO.File.Exists(CurrentFilePath);
            if (exists)
            {
                Logger.Verbose($"{CurrentFilePath} still exist, so may have been overwritten...");
                return;
            }

            Tizen.Applications.CoreApplication.Post(() =>
            {
                var handler = Lost;
                handler?.Invoke();
            });
        }

        public void StartMonitoring()
        {
            if (fsw != null)
            {
                Logger.Warn("File watching already enabled.");
                return;
            }

            try
            {
                fsw = new System.IO.FileSystemWatcher(DirectoryPath, CurrentFileName);
                fsw.Changed += OnFileChanged;
                fsw.Renamed += OnFileRenamed;
                fsw.Deleted += OnFileDeleted;
                fsw.EnableRaisingEvents = true;
                Logger.Warn("File watching enabled.");
            }
            catch (Exception ex)
            {
                Logger.Warn($"{ex}");

                fsw.Dispose();
                fsw = null;
            }
        }

        public void StopMonitoring()
        {
            if (fsw == null)
            {
                Logger.Warn("File watching already disabled.");
                return;
            }

            try
            {
                fsw.Changed -= OnFileChanged;
                fsw.Renamed -= OnFileRenamed;
                fsw.Deleted -= OnFileDeleted;
                fsw.EnableRaisingEvents = false;
            }
            catch (Exception ex)
            {
                Logger.Warn($"{ex}");
            }
            finally
            {
                fsw.Dispose();
                fsw = null;
                Logger.Warn("File watching disabled.");
            }
        }

        public static IEnumerable<MenuCustomizationItem> ReadFromFile(string filepath)
        {
            Logger.Verbose($"Reading customization from file.");

            bool exists = System.IO.File.Exists(filepath);
            if (!exists)
            {
                Logger.Verbose($"Customization file does not exists.");
                return null;
            }

            try
            {
                string text = System.IO.File.ReadAllText(filepath);
                text = JsonParser.RemoveComments(text);
                var dict = JsonSerializer.Deserialize<Dictionary<string, int>>(text);

                var items = dict.Select(pair => new MenuCustomizationItem(pair.Key.ToLowerInvariant(), pair.Value));

                StringBuilder sb = new StringBuilder("Read from file:" + Environment.NewLine);
                foreach (var item in items)
                {
                    sb.AppendLine($"{item}");
                }
                Logger.Verbose($"{sb}");

                return items;
            }
            catch (Exception exc)
            {
                Logger.Warn($"Could not parse file ({exc.Message}).");
                return null;
            }
        }

        public static void WriteToFile(IEnumerable<MenuCustomizationItem> items, string filepath)
        {
            Logger.Verbose($"Writing customization to file.");

            items = JsonParser.SortByNesting(items);
            var pairs = items.Select(i => new KeyValuePair<string, int>(i.MenuPath, i.Order));
            var dict = new Dictionary<string, int>(pairs);

            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(dict, options);
            jsonString = JsonParser.GenerateComments(jsonString);
            try
            {
                System.IO.File.Create(filepath).Dispose();
                System.IO.File.WriteAllText(filepath, jsonString);
            }
            catch (Exception exc)
            {
                Logger.Warn($"Could not write customization to file: {exc}");
            }
        }
    }
}
