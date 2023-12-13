using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tizen.NUI.BaseComponents;

namespace SettingCore
{
    public class Sections
    {
        private Dictionary<string, View> sections = new Dictionary<string, View>();
        private List<Section> sectionList = new List<Section>();

        public IEnumerable<View> Views => sections.Values;

        public bool Add(string menuPath, View view)
        {
            try
            {
                menuPath = menuPath.ToLowerInvariant();
                sections.Add(menuPath, view);
                return true;
            }
            catch (ArgumentNullException)
            {
                Logger.Warn("Cannot add view with null menuPath to Sections.");
            }
            catch (ArgumentException)
            {
                Logger.Warn($"Cannot add view, because menuPath '{menuPath}' already exists in Sections.");
            }
            return false;
        }

        public void Add(string menuPath, Action action, Action init = null)
        {
            menuPath = menuPath.ToLowerInvariant();
            sectionList.Add(new Section
            {
                MenuPath = menuPath,
                CreateItem = action,
                Init = init
            });
        }

        public Section GetSection(string menuPath)
        {
            return sectionList.Where(a => a.MenuPath == menuPath).FirstOrDefault();
        }

        public void InitSections()
        {
            var initList = sectionList.Where(a => a.Init != null).ToList();
            foreach (var section in initList) 
            {
                section.InitComplete = Task.Run(() => section.Init);
            }
        }

        public void Clear()
        {
            sectionList.Clear();
        }

        public bool TryGetValue(string menuPath, out View view)
        {
            try
            {
                return sections.TryGetValue(menuPath, out view);
            }
            catch (ArgumentNullException)
            {
                Logger.Warn("Cannot get view with null menuPath from Sections.");
                view = null;
                return false;
            }
        }

        public void RemoveAllSectionsFromView(View parentView)
        {
            foreach (var view in sections.Values)
            {
                try
                {
                    parentView.Remove(view);
                }
                catch (InvalidOperationException)
                {
                    Logger.Warn($"Parent view does not contain child view.");
                }
            }
            sections.Clear();
        }

        public class Section
        {
            public string MenuPath { get; set; }
            public Action CreateItem { get; set; } 
            public Action Init { get; set; } 
            public Task InitComplete { get; set; }
        }
    }
}
