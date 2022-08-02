using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Interop;
using Java.Lang;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ApeFree.AndroidPlus.Core.Lang
{
    public class Bean<T> : Java.Lang.Object
    {
        public T Entity { get; set; }
        public object Tag { get; set; }

        public Bean() { }

        public Bean(T entity)
        {
            Entity = entity;
        }

        public Bean(T entity, object tag)
        {
            Entity = entity;
            Tag = tag;
        }

    }

    public static class BeanExtension
    {
        public static List<Bean<T>> ToBeans<T>(this IEnumerable<T> data, object tag = null)
        {
            return data.Select(bean => bean.ToBean(tag)).ToList();
        }

        public static Bean<T> ToBean<T>(this T data, object tag = null)
        {
            return new Bean<T> { Entity = data, Tag = tag };
        }
    }

    public class EventArgs : System.EventArgs
    {
        public Context Context { get; internal set; }
        public EventArgs(Context context = null)
        {
            Context = context;
        }
    }

}