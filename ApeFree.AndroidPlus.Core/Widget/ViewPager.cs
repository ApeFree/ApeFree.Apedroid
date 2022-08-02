using Android.Runtime;
using Android.Util;

namespace ApeFree.AndroidPlus.Core.Widget
{
    public class ViewPager : AndroidX.ViewPager.Widget.ViewPager
    {
        public ViewPager(Android.Content.Context context) : base(context) { }
        public ViewPager(Android.Content.Context context, IAttributeSet attrs) : base(context, attrs) { }
        protected ViewPager(System.IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) { }

        /// <summary>
        /// 当前是否处于首页
        /// </summary>
        public bool IsStartPage => CurrentItem <= 0;

        /// <summary>
        /// 当前是否处于尾页
        /// </summary>
        public bool IsLastPage => CurrentItem >= Adapter.Count-1;

        public void JumpToNext(bool smoothScroll = true)
        {
            SetCurrentItem(CurrentItem + 1, smoothScroll);
        }

        public void JumpToLast(bool smoothScroll = true)
        {
            SetCurrentItem(CurrentItem - 1, smoothScroll);
        }

        public void Jump(int pageIndex, bool smoothScroll = true)
        {
            if (pageIndex < 0 || pageIndex >= Adapter.Count)
                return;

            SetCurrentItem(pageIndex, smoothScroll);
        }
    }
}