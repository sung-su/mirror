using System;
using System.Collections.Generic;
using Tizen.NUI.BaseComponents;

namespace SettingCore
{
    public class Sections
    {
        private Dictionary<string, View> sections = new Dictionary<string, View>();

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
    }
}
