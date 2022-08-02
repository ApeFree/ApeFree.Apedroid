using Android.Content;
using Android.OS;
using Android.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ApeFree.AndroidPlus.Permission
{
    /// <summary>
    /// 自启动权限
    /// </summary>
    public class AutomaticStartupPermission
    {
        private static Dictionary<string, string[]> GetManagerpageUriDictionary()
        {
            Dictionary<string, string[]> dict = new Dictionary<string, string[]>();

            // 小米 & 红米 & 黑鲨
            dict["Xiaomi"] = dict["Redmi"] = dict["blackshark"] = new string[] {
                    "com.miui.securitycenter/com.miui.permcenter.autostart.AutoStartManagementActivity",
                    "com.miui.securitycenter"
            };
            // 三星
            dict["samsung"] = new string[] {
                    "com.samsung.android.sm_cn/com.samsung.android.sm.ui.ram.AutoRunActivity",
                    "com.samsung.android.sm_cn/com.samsung.android.sm.ui.appmanagement.AppManagementActivity",
                    "com.samsung.android.sm_cn/com.samsung.android.sm.ui.cstyleboard.SmartManagerDashBoardActivity",
                    "com.samsung.android.sm_cn/.ui.ram.RamActivity",
                    "com.samsung.android.sm_cn/.app.dashboard.SmartManagerDashBoardActivity",

                    "com.samsung.android.sm/com.samsung.android.sm.ui.ram.AutoRunActivity",
                    "com.samsung.android.sm/com.samsung.android.sm.ui.appmanagement.AppManagementActivity",
                    "com.samsung.android.sm/com.samsung.android.sm.ui.cstyleboard.SmartManagerDashBoardActivity",
                    "com.samsung.android.sm/.ui.ram.RamActivity",
                    "com.samsung.android.sm/.app.dashboard.SmartManagerDashBoardActivity",

                    "com.samsung.android.lool/com.samsung.android.sm.ui.battery.BatteryActivity",
                    "com.samsung.android.sm_cn",
                    "com.samsung.android.sm"
            };
            // 华为 & 荣耀
            dict["HUAWEI"] = dict["honor"] = new string[] {
                    "com.huawei.systemmanager/.startupmgr.ui.StartupNormalAppListActivity",
                    "com.huawei.systemmanager/.appcontrol.activity.StartupAppControlActivity",
                    "com.huawei.systemmanager/.optimize.process.ProtectActivity",
                    "com.huawei.systemmanager/.optimize.bootstart.BootStartActivity",
                    "com.huawei.systemmanager"
            };
            // VIVO
            dict["vivo"] = new string[] {
                    "com.iqoo.secure/.ui.phoneoptimize.BgStartUpManager",
                    "com.iqoo.secure/.safeguard.PurviewTabActivity",
                    "com.vivo.permissionmanager/.activity.BgStartUpManagerActivity",
                    // "com.iqoo.secure/.ui.phoneoptimize.AddWhiteListActivity", //这是白名单, 不是自启动
                    "com.iqoo.secure",
                    "com.vivo.permissionmanager"
            };
            // 魅族
            dict["Meizu"] = new string[] {
                    "com.meizu.safe/.permission.SmartBGActivity",
                    "com.meizu.safe/.permission.PermissionMainActivity",
                    "com.meizu.safe"
            };
            // OPPO
            dict["OPPO"] = new string[] {
                    "com.coloros.safecenter/.startupapp.StartupAppListActivity",
                    "com.coloros.safecenter/.permission.startup.StartupAppListActivity",
                    "com.oppo.safe/.permission.startup.StartupAppListActivity",
                    "com.coloros.oppoguardelf/com.coloros.powermanager.fuelgaue.PowerUsageModelActivity",
                    "com.color.safecenter/.permission.PermissionTopActivity",
                    "com.color.safecenter/.permission.startup.StartupAppListActivity",
                    "com.coloros.safecenter/com.coloros.privacypermissionsentry.PermissionTopActivity",
                    "com.coloros.safecenter",
                    "com.color.safecenter",
                    "com.oppo.safe",
                    "com.coloros.oppoguardelf",
                    "com.coloros.safecenter/.startupapp.AssociateStartActivity",
            };
            // 一加
            dict["oneplus"] = new string[] {
                    "com.oneplus.security/.chainlaunch.view.ChainLaunchAppListActivity",
                    "com.oneplus.security"
            };
            // 乐视
            dict["letv"] = new string[] {
                    "com.letv.android.letvsafe/.AutobootManageActivity",
                    "com.letv.android.letvsafe/.BackgroundAppManageActivity",
                    "com.letv.android.letvsafe"
            };
            // 中兴
            dict["zte"] = new string[] {
                    "com.zte.heartyservice/.autorun.AppAutoRunManager",
                    "com.zte.heartyservice"
            };
            // 金立
            dict["F"] = new string[] {
                    "com.gionee.softmanager/.MainActivity",
                    "com.gionee.softmanager"
            };
            // 锤子
            dict["smartisanos"] = new string[] {
                    "com.smartisanos.security/.invokeHistory.InvokeHistoryActivity",
                    "com.smartisanos.security"
            };
            // 360
            dict["360"] = dict["ulong"] = new string[] {
                    "com.yulong.android.coolsafe/.ui.activity.autorun.AutoRunListActivity",
                    "com.yulong.android.coolsafe"
            };
            // 联想
            dict["lenovo"] = new string[] {
                    "com.lenovo.security/.purebackground.PureBackgroundActivity",
                    "com.lenovo.security"
            };
            // HTC
            dict["htc"] = new string[] {
                    "com.htc.pitroad/.landingpage.activity.LandingPageActivity",
                    "com.htc.pitroad"
            };
            // 华硕
            dict["asus"] = new string[] {
                    "com.asus.mobilemanager/.MainActivity",
                    "com.asus.mobilemanager"
            };
            // 酷派
            dict["coolpad"] = dict["YuLong"] = new string[] {
                    "com.yulong.android.softmanager/.SpeedupActivity",
                    "com.yulong.android.security/com.yulong.android.seccenter.tabbarmain",
                    "com.yulong.android.security"
            };
            return dict;
        }

        /// <summary>
        /// 打开自启动管理界面
        /// </summary>
        /// <param name="context"></param>
        /// <exception cref="Exception"></exception>
        public static void StartAutomaticStartupManagementActivity(Context context)
        {
            // 获取设备的产品型号名称
            var brand = Build.Brand.ToLower();

            // 判断是否获取成功
            if (string.IsNullOrWhiteSpace(brand))
                throw new Exception("无法获取设备的产品品牌名称");

            // 获取与产品名称对应的管理界面URI字符串数组
            var uris = GetManagerpageUriDictionary().FirstOrDefault(kv => kv.Key.ToLower() == brand).Value;

            // 判断是否支持当前产品品牌的自动跳转
            if (uris == null)
                throw new Exception($"不支持当前产品品牌[{brand}]的设备跳转至自启动管理界面");

            // 跳转进入品牌对应的自启动管理界面
            foreach (string act in uris)
            {
                try
                {
                    Intent intent;
                    if (act.Contains("/"))
                    {
                        intent = new Intent();
                        intent.AddFlags(ActivityFlags.NewTask);
                        ComponentName componentName = ComponentName.UnflattenFromString(act);
                        intent.SetComponent(componentName);
                    }
                    else
                    {
                        intent = context.PackageManager.GetLaunchIntentForPackage(act);
                    }
                    context.StartActivity(intent);
                    return;
                }
                catch (Exception)
                {
                    Log.Debug("AutomaticStart", $"自启动管理界面路径[{brand}] uri:{act}");
                }
            }

            // 未匹配到有效的自启动管理界面路径
            throw new Exception($"当前设备[{brand}]未匹配到有效的自启动管理界面路径");
        }
    }
}