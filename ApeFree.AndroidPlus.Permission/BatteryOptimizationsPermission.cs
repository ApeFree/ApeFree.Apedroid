using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.Annotations;
using System.Text;

namespace ApeFree.AndroidPlus.Permission
{

    /// <summary>
    /// 电池优化管理
    /// 需要在AndroidManifest.xml中添加权限[android.permission.REQUEST_IGNORE_BATTERY_OPTIMIZATIONS]
    /// </summary>
    public class BatteryOptimizationsPermission
    {
        /// <summary>
        /// 应用是否在白名单中
        /// 即：电池优化是否已忽略当前应用
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        [RequiresApi(Api = (int)BuildVersionCodes.M)]
        public static bool IsIgnoringBatteryOptimizations(Context context)
        {
            bool isIgnoring = false;
            PowerManager powerManager = (PowerManager)context.GetSystemService(Context.PowerService);
            if (powerManager != null)
            {
                isIgnoring = powerManager.IsIgnoringBatteryOptimizations(context.PackageName);
            }
            return isIgnoring;
        }

        /// <summary>
        /// 申请加入电池优化
        /// </summary>
        /// <param name="context"></param>
        [RequiresApi(Api = (int)BuildVersionCodes.M)]
        public static void RequestIgnoreBatteryOptimizations(Context context)
        {
            Intent intent = new Intent(Android.Provider.Settings.ActionRequestIgnoreBatteryOptimizations);
            intent.SetData(Android.Net.Uri.Parse("package:" + context.PackageName));
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