using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ApeFree.Apedroid.Core.Widget
{
    /// <summary>
    /// 适配器
    /// </summary>
    /// <typeparam name="T">数据实体类（JavaBean）</typeparam>
    /// <typeparam name="V">视图类型</typeparam>
    public class Adapter<T, V> : BaseAdapter where T : Java.Lang.Object where V : View
    {
        public IEnumerable<T> Data { get; private set; }
        public int LayoutId { get; }
        public Context Context { get; private set; }

        public Action<T, V, int> LoadDataHandler;

        //构造方法
        public Adapter(Context context, IEnumerable<T> data, int layoutId)
        {
            Context = context;
            Data = data;
            LayoutId = layoutId;
        }
        //数量
        public override int Count => Data.Count();

        //对象
        public override Java.Lang.Object GetItem(int position)
        {
            return Data.ElementAt(position);
        }

        //获取对象的ID
        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            if (convertView == null)
            {
                convertView = View.Inflate(Context, LayoutId, null);
            }

            LoadDataHandler?.Invoke(Data.ElementAt(position), (V)convertView, position);

            return convertView;
        }
    }
}