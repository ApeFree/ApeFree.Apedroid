using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Android.Views
{
    public static class ViewGroupExtension
    {
        /// <summary>
        /// 获取所有子视图列表
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        public static List<View> GetViews(this ViewGroup view)
        {
            var views = new List<View>();
            var count = view.ChildCount;
            for (int i = 0; i < count; ++i)
            {
                views.Add(view.GetChildAt(i));
            }
            return views;
        }

        /// <summary>
        /// 获取指定类型的子视图列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="view"></param>
        /// <returns></returns>
        public static List<T> GetViews<T>(this ViewGroup view) where T : View
        {
            var type = typeof(T);
            return view.GetViews().Where(v => type.IsAssignableFrom(v.GetType())).Cast<T>().ToList();
        }
    }
}