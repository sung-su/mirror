using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SettingCore
{
    internal static class JsonParser
    {
        private static readonly string customizationPattern = @"^\s*""\s*([a-z.]*)\s*""\s*:\s*[-0-9]*\s*[,]?\s*";
        private static readonly string commentPattern = @"\s*//.*$";
        public static string RemoveComments(string jsonString)
        {
            Regex commentRegex = new Regex(commentPattern);
            Regex custRegex = new Regex(customizationPattern);
            StringBuilder jsonStringWithoutComments = new StringBuilder();
            foreach (var jsonLine in jsonString.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
            {
                Match commentMatch = commentRegex.Match(jsonLine);
                if (!commentMatch.Success)
                {
                    jsonStringWithoutComments.AppendLine(jsonLine);
                }
                else
                {
                    Match custMatch = custRegex.Match(jsonLine);
                    if (custMatch.Success)
                    {
                        jsonStringWithoutComments.AppendLine(custMatch.Value);
                    }
                }
            }
            return jsonStringWithoutComments.ToString();
        }

        public static IEnumerable<MenuCustomizationItem> SortByNesting(IEnumerable<MenuCustomizationItem> items)
        {
            List<MenuCustomizationItem> sorted = new List<MenuCustomizationItem>();
            IEnumerable<MenuCustomizationItem> mainMenus = items.Where(item => getMenuItemDepth(item.MenuPath) == 1);
            sorted.AddRange(mainMenus);
            foreach (var mainMenuItem in mainMenus)
            {
                var children = addChildren(mainMenuItem, items);
                sorted.AddRange(children);
            }

            return sorted;
        }

        public static string GenerateComments(string jsonString)
        {
            Regex regex = new Regex(customizationPattern);
            StringBuilder jsonStringWithComments = new StringBuilder();
            string lastMenuGroup = "without";
            foreach (var JsonLine in jsonString.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
            {
                Match match = regex.Match(JsonLine);
                if (match.Success)
                {
                    string menuPath = match.Groups[1].Value;
                    string currentMenuGroup = getMenuGroup(menuPath);
                    if (currentMenuGroup != lastMenuGroup)
                    {
                        if (jsonStringWithComments.Length > 2)
                        {
                            jsonStringWithComments.AppendLine(string.Empty);
                        }
                        jsonStringWithComments.AppendLine($"{generateComment(menuPath)}");
                    }
                    lastMenuGroup = currentMenuGroup;
                }
                jsonStringWithComments.AppendLine($"{JsonLine}");
            }
            return jsonStringWithComments.ToString();
        }

        private static List<MenuCustomizationItem> addChildren(MenuCustomizationItem item, IEnumerable<MenuCustomizationItem> items)
        {
            List<MenuCustomizationItem> sorted = new List<MenuCustomizationItem>();
            IEnumerable<MenuCustomizationItem> directChildren = items.Where(currItem => checkIfIsDirectChild(item, currItem));
            sorted.AddRange(directChildren);
            foreach (var menuItem in directChildren)
            {
                var children = addChildren(menuItem, items);
                sorted.AddRange(children);
            }

            return sorted;
        }

        private static int getMenuItemDepth(string menuItem)
        {
            return menuItem.Split('.').Length;
        }

        private static bool checkIfIsDirectChild(MenuCustomizationItem item, MenuCustomizationItem currItem)
        {
            return item.MenuPath == string.Join(".", currItem.MenuPath.Split('.').SkipLast(1));
        }

        private static object generateComment(string menuPath)
        {
            StringBuilder comment = new StringBuilder("\t// group menu: main");
            var splitted = menuPath.Split('.');

            if (splitted.Length > 1)
            {
                foreach (var menuItem in splitted.SkipLast(1))
                {
                    comment.Append($" > { menuItem }");
                }
            }
            return comment.ToString();
        }

        private static string getMenuGroup(string menuPath)
        {
            return menuPath.Split('.').Reverse().Skip(1).FirstOrDefault() ?? string.Empty;
        }
    }
}
