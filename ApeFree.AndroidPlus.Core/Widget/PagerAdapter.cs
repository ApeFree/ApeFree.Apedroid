using Android.Views;
using Java.Lang;
using System.Collections.Generic;
using System.Linq;

namespace ApeFree.AndroidPlus.Core.Widget
{
    public class PagerAdapter : AndroidX.ViewPager.Widget.PagerAdapter
    {

        public List<View> Views { get; set; }

        public PagerAdapter(IEnumerable<View> views) => Views = views.ToList();

        public PagerAdapter(params View[] views) => Views = views.ToList();

        public override int Count => Views != null ? Views.Count() : 0;

        public override bool IsViewFromObject(View view, Object @object)
        {
            return view == @object;
        }

        public override void DestroyItem(ViewGroup container, int position, Object @object)
        {
            container.RemoveView(Views[position]);
        }

        public override Object InstantiateItem(ViewGroup container, int position)
        {
            container.AddView(Views[position]);
            return Views[position];
        }
    }
}