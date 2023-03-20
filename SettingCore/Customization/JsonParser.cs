using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SettingCore.Customization
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
            foreach (var JsonLine in jsonString.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
            {
                Match commentMatch = commentRegex.Match(JsonLine);
                Match custMatch = custRegex.Match(JsonLine);
                if (!commentMatch.Success)
                {
                    jsonStringWithoutComments.AppendLine(JsonLine);
                }
                else if (custMatch.Success)
                {
                    jsonStringWithoutComments.AppendLine(custMatch.Value);
                }
            }
            return jsonStringWithoutComments.ToString();
        }

        public static IEnumerable<MenuCustomizationItem> sortByDepth(IEnumerable<MenuCustomizationItem> items)
        {
            List<MenuCustomizationItem> sorted = new List<MenuCustomizationItem>();
            int depth = 1;
            while (sorted.Count() != items.Count())
            {
                var depthItems = items.Where(item => getMenuItemDepth(item.MenuPath) == depth);
                sorted.AddRange(depthItems);
                depth++;
            }

            return sorted;
        }

        public static IEnumerable<MenuCustomizationItem> SortByNesting(IEnumerable<MenuCustomizationItem> items)
        {
            List<MenuCustomizationItem> sorted = new List<MenuCustomizationItem>();
            IEnumerable<MenuCustomizationItem> collection = items.Where(item => getMenuItemDepth(item.MenuPath) == 1);
            sorted.AddRange(collection);
            foreach (var mainMenuItem in collection)
            {
                addChildren(mainMenuItem, items, sorted);
            }

            return sorted;
        }

        public static string GenerateComments(string jsonString)
        {
            Regex regex = new Regex(customizationPattern);
            StringBuilder jsonStringWithComments = new StringBuilder();
            string lastButOneMenu = "without";
            foreach (var JsonLine in jsonString.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
            {
                Match match = regex.Match(JsonLine);
                if (match.Success)
                {
                    string menuPath = match.Groups[1].Value;
                    string currentLastButOneMenu = getLastButOneMenu(menuPath);
                    if (currentLastButOneMenu != lastButOneMenu)
                    {
                        if (jsonStringWithComments.Length > 2)
                        {
                            jsonStringWithComments.AppendLine(string.Empty);
                        }
                        jsonStringWithComments.AppendLine($"{generateComment(menuPath)}");
                    }
                    lastButOneMenu = currentLastButOneMenu;
                }
                jsonStringWithComments.AppendLine($"{JsonLine}");
            }
            return jsonStringWithComments.ToString();
        }

        private static void addChildren(MenuCustomizationItem item, IEnumerable<MenuCustomizationItem> items, List<MenuCustomizationItem> sorted)
        {
            var itemDepth = getMenuItemDepth(item.MenuPath);
            IEnumerable<MenuCustomizationItem> directChildren = items.Where(currItem => checkIfIsDirectChild(item, currItem, itemDepth));
            sorted.AddRange(directChildren);
            foreach (var menuItem in directChildren)
            {
                addChildren(menuItem, items, sorted);
            }
        }

        private static int getMenuItemDepth(string menuItem)
        {
            return menuItem.Split('.').Length;
        }

        private static bool checkIfIsDirectChild(MenuCustomizationItem item, MenuCustomizationItem currItem, int itemDepth)
        {
            return item.MenuPath == string.Join(".", currItem.MenuPath.Split('.').SkipLast(1));
        }

        private static object generateComment(string menuPath)
        {
            StringBuilder comment = new StringBuilder("\t// group menu: Main");
            var splitted = menuPath.Split('.');

            if (splitted.Length > 1)
            {
                foreach (var menuItem in menuPath.Split('.').SkipLast(1))
                {
                    comment.Append($" > { char.ToUpper(menuItem[0]) + menuItem.Substring(1)}");
                }
            }
            return comment.ToString();
        }

        private static string getLastButOneMenu(string menuPath)
        {
            var splitted = menuPath.Split('.');
            return splitted.Length < 2 ? string.Empty : splitted.Reverse().Skip(1).First();
        }
    }
}
