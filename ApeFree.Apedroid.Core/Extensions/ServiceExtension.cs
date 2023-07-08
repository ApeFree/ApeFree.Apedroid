using Android.Content.PM;
using Android.OS;
using Java.Lang;

namespace Android.Content
{
    /// <summary>
    /// 服务扩展
    /// </summary>
    public static class ServiceExtension
    {
        /// <summary>
        /// 绑定服务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="serviceConnection"></param>
        /// <returns></returns>
        public static bool BindService<T>(this Context context, IServiceConnection serviceConnection, Bind flag = Bind.AutoCreate)
        {
            var intent = new Intent(context, typeof(T).ToJavaClass());
            return context.BindService(intent, serviceConnection, flag);
        }

        /// <summary>
        /// 启动服务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="args"></param>
        public static void StartService<T>(this Context context, Bundle args = null) where T : Android.App.Service
        {
            var intent = new Intent(context, typeof(T).ToJavaClass());
            if (args != null)
            {
                intent.PutExtras(args);
            }
            context.StartService(intent);
        }

        /// <summary>
        /// 启动前台服务
        /// 在 Android 8.0 及更高版本上，它将使用该StartForegroundService方法，否则将使用较旧的StartService方法。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="args"></param>
        public static void StartForegroundService<T>(this Context context, Bundle args = null) where T : Android.App.Service
        {
            var intent = new Intent(context, typeof(T).ToJavaClass());
            if (args != null)
            {
                intent.PutExtras(args);
            }

            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O)
            {
                context.StartForegroundService(intent);
            }
            else
            {
                context.StartService(intent);
            }
        }

        

        /// <summary>
        /// 停止服务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        public static void StopService<T>(this Context context)
        {
            var intent = new Intent(context, typeof(T).ToJavaClass());
            context.StopService(intent);
        }

        /// <summary>
        /// 重新绑定服务
        /// 把应用的T实现类disable再enable，即可触发系统rebind操作
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        public static void RebindService<T>(this Context context)
        {
            context.DisabledService<T>();
            context.EnabledService<T>();
        }

        /// <summary>
        /// 禁用服务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        public static void DisabledService<T>(this Context context)
        {
            context.PackageManager.SetComponentEnabledSetting(
                    new ComponentName(context, Class.FromType(typeof(T)).Name),
                    ComponentEnabledState.Disabled, ComponentEnableOption.DontKillApp);
        }

        /// <summary>
        /// 启用服务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        public static void EnabledService<T>(this Context context)
        {
            context.PackageManager.SetComponentEnabledSetting(
                    new ComponentName(context, Class.FromType(typeof(T)).Name),
                    ComponentEnabledState.Enabled, ComponentEnableOption.DontKillApp);
        }
    }
}