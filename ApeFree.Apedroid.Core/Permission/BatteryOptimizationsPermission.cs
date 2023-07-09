using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Text;

namespace ApeFree.Apedroid.Permission
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
        // [RequiresApi(Api = (int)BuildVersionCodes.M)]
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
        // [RequiresApi(Api = (int)BuildVersionCodes.M)]
        public static void RequestIgnoreBatteryOptimizations(Context context)
        {
            Intent intent = new Intent(Android.Provider.Settings.ActionRequestIgnoreBatteryOptimizations);
            intent.SetData(Android.Net.Uri.Parse("package:" + context.PackageName));
            context.StartActivity(intent);
        }
    }
}