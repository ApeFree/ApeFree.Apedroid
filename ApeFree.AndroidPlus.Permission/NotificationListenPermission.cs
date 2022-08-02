using Android.Content;
using Android.OS;
using AndroidX.Core.App;

namespace ApeFree.AndroidPlus.Permission
{
    /// <summary>
    /// 通知使用(监听)权
    /// </summary>
    public class NotificationListenPermission
    {
        /// <summary>
        /// 是否拥有通知使用权
        /// 检测通知监听服务是否被授权
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static bool IsNotificationListenerEnabled(Context context)
        {
            var packageNames = NotificationManagerCompat.GetEnabledListenerPackages(context);
            if (packageNames.Contains(context.PackageName))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 打开通知监听设置页面
        /// </summary>
        /// <param name="context"></param>
        public static void StartNotificationListenSettingsActivity(Context context)
        {
            Intent intent;
            if (Build.VERSION.SdkInt >= BuildVersionCodes.LollipopMr1)
            {
                intent = new Intent(Android.Provider.Settings.ActionNotificationListenerSettings);
            }
            else
            {
                intent = new Intent("android.settings.ACTION_NOTIFICATION_LISTENER_SETTINGS");
            }
            context.StartActivity(intent);
        }
    }

    //public class Brand
    //{
    //    public static bool IsHuaweiOrHonor() => Build.Brand != null && (Build.Brand.ToLower().Equals("huawei") || Build.Brand.ToLower().Equals("honor"));

    //    public static bool IsXiaomi() => Build.Brand != null && Build.Brand.ToLower().Equals("xiaomi");

    //    public static bool IsOPPO() => Build.Brand != null && Build.Brand.ToLower().Equals("oppo");

    //    public static bool IsVIVO() => Build.Brand != null && Build.Brand.ToLower().Equals("vivo");

    //    public static bool IsMeizu() => Build.Brand != null && Build.Brand.ToLower().Equals("meizu");

    //    public static bool IsSamsung() => Build.Brand != null && Build.Brand.ToLower().Equals("samsung");

    //    public static bool IsLeTV() => Build.Brand != null && Build.Brand.ToLower().Equals("letv");

    //    public static bool IsLenovo() => Build.Brand != null && Build.Brand.ToLower().Equals("lenovo");

    //    public static bool IsSmartisan() => Build.Brand != null && Build.Brand.ToLower().Equals("smartisan");
    //}
}