using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ApeFree.AndroidPlus.Core.Content
{
    /// <summary>
    /// 基于SharedPreferences实现的对象存储器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract partial class ObjectStore<T>
    {
        public T Value { get; set; }
        protected Context Context { get; }
        protected string StoreName { get;}

        public static readonly Dictionary<string, ObjectStore<T>> _ObjectStores;

        static ObjectStore()
        {
            _ObjectStores = new Dictionary<string, ObjectStore<T>>();
        }

        public ObjectStore(Context context, string storeName, T defaultValue)
        {
            Context = context;
            StoreName = storeName;
            Value = defaultValue;

            // TODO: 检查是否有重复名称的存储器
            _ObjectStores[storeName] = this;

            Load();
        }
    }

    public abstract partial class ObjectStore<T>
    {
        /// <summary>
        /// 加载数据
        /// </summary>
        protected virtual void Load()
        {
            Value = ReadFromSharedPreferences(Context, StoreName, Value);
        }

        /// <summary>
        /// 保存数据
        /// </summary>
        public virtual void Save()
        {
            SaveInSharedPreferences(Context, StoreName, Value);
        }

        /// <summary>
        /// 释放存储器
        /// </summary>
        public void Release()
        {
            if (_ObjectStores != null && _ObjectStores.ContainsKey(StoreName))
            {
                _ObjectStores.Remove(StoreName);
            }
        }
    }

    public abstract partial class ObjectStore<T>
    {
        /// <summary>
        /// 获取唯一的存储器
        /// </summary>
        /// <typeparam name="P">继承于ObjectStore的存储器类型</typeparam>
        /// <param name="storeName">存储文件名</param>
        /// <param name="createStoreHandle">存储器对象构造回调</param>
        /// <returns></returns>
        public static P GetStore<P>(string storeName, Func<P> createStoreHandle) where P : ObjectStore<T>
        {
            if (!_ObjectStores.ContainsKey(storeName))
            {
                var store = createStoreHandle.Invoke();
                _ObjectStores[storeName] = store;
                return store;
            }
            else
            {
                return (P)_ObjectStores[storeName];
            }
        }
    }

    public abstract partial class ObjectStore<T>
    {
        /// <summary>
        /// Json序列化设置
        /// </summary>
        private static JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore, NullValueHandling = NullValueHandling.Ignore };

        /// <summary>
        /// 将数据序列化后保存至SharedPreferences中
        /// </summary>
        /// <param name="context"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        protected virtual void SaveInSharedPreferences(Context context, string name, object value)
        {
            var sp = context.GetSharedPreferences(name, FileCreationMode.Private);
            var editor = sp.Edit();
            editor.PutString("json", JsonConvert.SerializeObject(value, _jsonSerializerSettings));
            editor.Commit();
        }

        /// <summary>
        /// 将数据反序列化成存储器对象
        /// </summary>
        /// <typeparam name="P"></typeparam>
        /// <param name="context"></param>
        /// <param name="name"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        protected virtual P ReadFromSharedPreferences<P>(Context context, string name, P defaultValue)
        {
            var sp = context.GetSharedPreferences(name, FileCreationMode.Private);
            try
            {
                if (sp.Contains("json"))
                {
                    string json = sp.GetString("json", null);
                    return JsonConvert.DeserializeObject<P>(json);
                }
                else
                {
                    SaveInSharedPreferences(context, name, defaultValue);
                    return defaultValue;
                }
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }
    }
}