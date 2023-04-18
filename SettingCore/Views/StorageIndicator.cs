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
            public string Name { get; private set; }
            public Color Color { get; private set; }
            public double SizeInfo { get; private set; }
            public float Width { get; private set; }

            public IndicatorItem(string name, Color color, double sizeInfo)
            {
                Name = name;
                Color = color;
                SizeInfo = sizeInfo;
            }

            public void SetWidth(float width)
            {
                Width = width;
            }

            public void SetSize(double sizeInfo)
            {
                SizeInfo = sizeInfo;
            }
        }

        private const int duration = 1000;
        private double totalSize;

        private Animation animation;      
        private List<IndicatorItem> sizeInfoList = new List<IndicatorItem>();

        public List<IndicatorItem> SizeInfoList
        {
            get => sizeInfoList; 
            set 
            { 
                if(sizeInfoList != value) 
                {
                    sizeInfoList = value;
                }
            }
        }

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
            sizeInfoList.Add(new IndicatorItem(name, color, size));
        }

        public void Update()
        {
            if (animation != null)
            {
                return;
            }

            sizeInfoList = new List<IndicatorItem>(sizeInfoList.OrderByDescending(a => a.SizeInfo).ToList());

            Calculation();
            RemoveChildren(this);
            AddItems();
        }

        private void AddItems()
        {
            animation = new Animation(duration);

            for (int i = sizeInfoList.Count - 1; i >= 0; i--)
            {
                var coloredView = CreateColoredView(sizeInfoList[i].Color, sizeInfoList[i].Equals(sizeInfoList.First()));
                Add(coloredView);

                animation.AnimateTo(coloredView, "SizeWidth", sizeInfoList[i].Width, 0, duration, new AlphaFunction(AlphaFunction.BuiltinFunctions.EaseOut));

                if (i > 0)
                {
                    animation.AnimateTo(coloredView, "PositionX", sizeInfoList.GetRange(0, i).Select(x => x.Width).Sum(), 0, duration, new AlphaFunction(AlphaFunction.BuiltinFunctions.EaseOut));
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

        private void RemoveChildren(View parent)
        {
            if (parent == null)
            {
                return;
            }

            int maxChild = (int)parent.ChildCount;
            for (int i = maxChild - 1; i >= 0; --i)
            {
                View child = parent.GetChildAt((uint)i);

                if (child == null)
                {
                    continue;
                }

                RemoveChildren(child);
                parent.Remove(child);
                child.Dispose();
            }
        }

        private void Calculation()
        {
            foreach (var item in sizeInfoList)
            {
                item.SetWidth((float)(item.SizeInfo / totalSize * SizeWidth));
            }
        }
    }
}
