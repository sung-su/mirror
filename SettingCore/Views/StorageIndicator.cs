using System.Collections.Generic;
using System.Linq;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;

namespace SettingCore.Views
{
    public class StorageIndicator : View
    {
        public class IndicatorItem
        {
            public string Name { get; set; }
            public Color Color { get; set; }
            public double SizeInfo { get; set; }
            public float Width { get; set; }

            public IndicatorItem(string name, Color color, double sizeInfo)
            {
                Name = name;
                Color = color;
                SizeInfo = sizeInfo;
            }
        }

        private const int duration = 500;
        private double totalSize;
        private Animation animation;      

        public List<IndicatorItem> SizeInfoList = new List<IndicatorItem>();

        public StorageIndicator(double totalSize)
        {
            this.totalSize = totalSize;

            Layout = new LinearLayout()
            {
                LinearOrientation = LinearLayout.Orientation.Horizontal,
                VerticalAlignment = VerticalAlignment.Center,
            };

            WidthSpecification = LayoutParamPolicies.MatchParent;
            BackgroundColor = new Color("#83868F").WithAlpha(0.1f);
            Margin = new Extents(40, 40, 24, 24).SpToPx();
            SizeHeight = 8.SpToPx();
            CornerRadius = 4.SpToPx();
        }

        public void AddItem(string name, Color color, double size)
        {
            SizeInfoList.Add(new IndicatorItem(name, color, size));
        }

        public void Update()
        {
            if (animation != null)
            {
                return;
            }

            SizeInfoList = new List<IndicatorItem>(SizeInfoList.OrderByDescending(a => a.SizeInfo).ToList());

            foreach (var item in SizeInfoList)
            {
                item.Width = (float)(item.SizeInfo / totalSize * SizeWidth);
            }

            RemoveChildren();
            AddItems();
        }

        private void AddItems()
        {
            animation = new Animation(duration);

            for (int i = SizeInfoList.Count - 1; i >= 0; i--)
            {
                var coloredView = CreateColoredView(SizeInfoList[i].Color, i == 0);
                Add(coloredView);

                animation.AnimateTo(coloredView, "SizeWidth", SizeInfoList[i].Width, 0, duration, new AlphaFunction(AlphaFunction.BuiltinFunctions.EaseOut));

                if (i > 0)
                {
                    animation.AnimateTo(coloredView, "PositionX", SizeInfoList.GetRange(0, i).Select(x => x.Width).Sum(), 0, duration, new AlphaFunction(AlphaFunction.BuiltinFunctions.EaseOut));
                }
            }

            animation.Play();
            animation.Finished += (s, e) => 
            { 
                animation.Dispose();
            };
        }

        private View CreateColoredView(Color color, bool first)
        {
            View view = new View()
            {
                SizeHeight = 8.SpToPx(),
                BackgroundColor = color,
                CornerRadius = first ? new Vector4(4.SpToPx(), 0, 0, 4.SpToPx()) : 0,
            };

            return view;
        }

        private void RemoveChildren()
        {
            for (int i = (int)ChildCount - 1; i >= 0; --i)
            {
                View child = GetChildAt((uint)i);

                if (child == null)
                {
                    continue;
                }

                Remove(child);
                child.Dispose();
            }
        }
    }
}
