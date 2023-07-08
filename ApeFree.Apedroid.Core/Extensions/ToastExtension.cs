using Android.OS;
using Android.Widget;
using System;

namespace Android.Content
{
    public static class ToastExtension
    {
        /// <summary>
        /// 弹出显示Toast
        /// </summary>
        /// <param name="context"></param>
        /// <param name="text"></param>
        /// <param name="length"></param>
        public static void ShowToast(this Context context, string text, ToastLength length = ToastLength.Short)
        {
            context.ModifyInUI(() => Toast.MakeText(context, text, length).Show());
        }

        //public static void ShowToast(this Context context, string text, ToastLength length = ToastLength.Short)
        //{
        //    bool useLooper = Looper.MyLooper() != Looper.MainLooper;

        //    if (useLooper) Looper.Prepare();
        //    Toast.MakeText(context, text, length).Show();
        //    if (useLooper) Looper.Loop();
        //}

        public static void ModifyInUI(this Context context, Action action)
        {
            bool useLooper = Looper.MyLooper() != Looper.MainLooper;

            if (useLooper) Looper.Prepare();
            action.Invoke();
            if (useLooper) Looper.Loop();
        }
    }
}