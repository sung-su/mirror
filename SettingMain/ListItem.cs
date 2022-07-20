using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SettingMain
{
    public class ListItem
    {
        private readonly int ItemType = 0;
        private readonly string Path = null;
        private readonly string Label = null;
        private readonly string Description = null;

        public ListItem(int type, string path, string label, string description = "")
        {
            ItemType = type;
            Path = path;
            Label = label;
            Description = description;
        }


        public int GetItemType()
        {
            return ItemType;
        }
        public string GetPath()
        {
            return Path;
        }

        public string GetLabel()
        {
            return Label;
        }

        public string GetDescription()
        {
            return Description;
        }
    };
}
