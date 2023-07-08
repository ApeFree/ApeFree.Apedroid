using Java.Lang;
using System;

namespace Android.Content
{
    public static class TypeExtension
    {
        public static Class ToJavaClass(this Type type) => Class.FromType(type);
        public static Class GetJavaType(this object obj) => obj.GetType().ToJavaClass();
    }
}