using Android.Content;
using Android.OS;
using Android.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ApeFree.Apedroid.Permission
{
    /// <summary>
    /// 自启动权限
    /// </summary>
    public class AutomaticStartupPermission
    {
        private static Dictionary<string, string[]> GetManagerpageUriDictionary()
        {
            Dictionary<string, string[]> dict = new Dictionary<string, string[]>();

            // 小米 & 红米 & 黑鲨 & POCO
            dict["xiaomi"] = dict["redmi"] = dict["blackshark"] = new string[] {
                    "com.miui.securitycenter/com.miui.permcenter.autostart.AutoStartManagementActivity",
                    "com.miui.securitycenter",

                    "com.miui.securitycenter/com.miui.powercenter.PowerSettings",
                    "com.miui.securitycenter/com.miui.powercenter.PowerSettingsActivity",
                    "com.miui.powerkeeper/.ui.HiddenAppsContainerManagementActivity",
                    "com.miui.powerkeeper/.ui.HiddenAppsConfigActivity",
                    "com.miui.powerkeeper/.ui.HiddenAppsConfigActivityAlias",
                    "com.miui.powerkeeper/.ui.HiddenAppsContainerManagementActivityAlias",
                    "com.miui.powerkeeper/.ui.HiddenAppsConfigActivity2",
                    "com.miui.powerkeeper/.ui.HiddenAppsConfigActivityAlias2",
                    "com.miui.powerkeeper/.ui.HiddenAppsContainerManagementActivityAlias2",
                    "com.miui.powerkeeper/.ui.HiddenAppsConfigActivity3",
                    "com.miui.powerkeeper/.ui.HiddenAppsConfigActivityAlias3",
                    "com.miui.powerkeeper/.ui.HiddenAppsContainerManagementActivityAlias3",
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
            dict["huawei"] = dict["honor"] = new string[] {
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
                    "com.iqoo.secure",
                    "com.vivo.permissionmanager",

                    "com.vivo.permissionmanager/.activity.PurviewTabActivity",
                    "com.vivo.abeui/.manager.VivoAutoLaunchManagerActivity",
                    "com.iqoo.secure/.ui.phoneoptimize.AddWhiteListActivity",
                    "com.iqoo.secure/.safeguard.SoftPermissionDetailActivity",
            };
            // 魅族
            dict["meizu"] = new string[] {
                    "com.meizu.safe/.permission.SmartBGActivity",
                    "com.meizu.safe/.permission.PermissionMainActivity",
                    "com.meizu.safe"
            };
            // OPPO
            dict["oppo"] = new string[] {
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
                    "com.coloros.safecenter/.startupapp.StartupAppListActivityForExternal",
                    "com.coloros.safecenter/.startupapp.StartupAppListActivityForLauncher",
                    "com.coloros.safecenter/.startupapp.StartupAppListActivityForIcon",
                    "com.coloros.safecenter/.startupapp.StartupAppListActivityForOneKey",
                    "com.coloros.safecenter/.startupapp.StartupAppListActivityForOneKeyForExternal",
                    "com.coloros.safecenter/.startupapp.StartupAppListActivityForOneKeyForLauncher",
                    "com.coloros.safecenter/.startupapp.StartupAppListActivityForOneKeyForIcon",
                    "com.coloros.safecenter/.PermissionTopActivity",
                    "com.coloros.safecenter/.permission.startupapp.StartupAppListActivityForExternal",
                    "com.coloros.safecenter/.permission.startupapp.StartupAppListActivityForLauncher",
                    "com.coloros.safecenter/.permission.startupapp.StartupAppListActivityForIcon",
                    "com.coloros.safecenter/.startupapp.StartupAppListActivityForExternal",
                    "com.coloros.safecenter/.startupapp.StartupAppListActivityForLauncher",
                    "com.coloros.safecenter/.startupapp.StartupAppListActivityForIcon",
                    "com.coloros.safecenter/.startupapp.StartupAppListActivityForOneKey",
                    "com.coloros.safecenter/.startupapp.StartupAppListActivityForOneKeyForExternal",
                    "com.coloros.safecenter/.startupapp.StartupAppListActivityForOneKeyForLauncher",
                    "com.coloros.safecenter/.startupapp.StartupAppListActivityForOneKeyForIcon",
                    "com.coloros.safecenter/.permission.topactivity.PermissionTopActivity",
                    "com.coloros.safecenter/.permission.startupapp.StartupAppListActivity",
                    "com.coloros.safecenter/.permission.startupapp.StartupAppListActivityForExternal",
                    "com.coloros.safecenter/.permission.startupapp.StartupAppListActivityForLauncher",
                    "com.coloros.safecenter/.permission.startupapp.StartupAppListActivityForIcon",
                    "com.coloros.safecenter/.startupapp.StartupAppListActivityForExternal",
                    "com.coloros.safecenter/.startupapp.StartupAppListActivityForLauncher",
                    "com.coloros.safecenter/.startupapp.StartupAppListActivityForIcon",
                    "com.coloros.safecenter/.startupapp.StartupAppListActivityForOneKey",
                    "com.coloros.safecenter/.startupapp.StartupAppListActivityForOneKeyForExternal",
                    "com.coloros.safecenter/.startupapp.StartupAppListActivityForOneKeyForLauncher",
                    "com.coloros.safecenter/.startupapp.StartupAppListActivityForOneKeyForIcon",
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
            dict["f"] = new string[] {
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
            // 努比亚
            dict["nubia"] = new string[] {
                "com.nubia.security/com.nubia.security.autoStart.AutoStartManagerActivity",
                "com.nubia.security/com.nubia.security.autoStart.AutoStartManagerActivity2",
                "com.nubia.security/.autoStart.AutoStartManagerActivity",
                "com.nubia.security/.autoStart.AutoStartManagerActivity2",
                "com.nubia.security/.autoStart.AutoStartSettingsActivity",
                "com.nubia.security",
                "com.nubia.powermaster",
                "com.nubia.powermaster/.ui.PowerMasterActivity",
                "com.nubia.powercenter",
                "com.nubia.powercenter/.ui.PowerCenterActivity",
            };
            // LG
            dict["lg"] = new string[] {
                "com.lge.powermanager/.ui.PwrSavingModeActivity",
                "com.lge.powersavingmode/.ui.PwrSavingModeActivity",
                "com.lge.batterydrainageoptimizer/.ui.BatteryDrainageOptimizerActivity",
                "com.lge.ips/.lgips.LGIPSActivity",
                "com.lge.ips/.lgips.LGIPSMainActivity",
                "com.lge.ips/.lgips.LGIPSActivityForTab",
            };
            // 其他品牌
            dict["other"] = new string[] {
                "com.android.settings/.Settings$BatterySaverSettingsActivity",
                "com.android.settings/.Settings$PowerUsageSummaryActivity",
                "com.android.settings/.Settings$HighPowerApplicationsActivity",
                "com.android.settings/.Settings$BatterySaverScheduleActivity",
                "com.android.settings/.Settings$BatterySaverScheduleRulesActivity",
                "com.android.settings/.Settings$BatterySaverSettingsActivity",
                "com.android.settings/.Settings$BatterySaverScheduleDetailActivity",
                "com.android.settings/.Settings$BatterySaverScheduleRuleSettingsActivity",
                "com.android.settings/.Settings$BatterySaverScheduleAddRuleActivity",
                "com.android.settings/.Settings$BatterySaverScheduleAddStartTimeActivity",
                "com.android.settings/.Settings$BatterySaverScheduleAddEndTimeActivity",
                "com.android.settings/.Settings$BatterySaverScheduleDaySelectionActivity",
                "com.android.settings/.Settings$BatterySaverScheduleRepeatSelectionActivity",
                "com.android.settings/.Settings$BatterySaverScheduleUpcomingRuleActivity",
                "com.android.settings/.Settings$BatterySaverScheduleSettingsActivity",
                "com.android.settings/.Settings$BatterySaverScheduleRuleListActivity",
                "com.android.settings/.Settings$BatteryOptimizationActivity",
                "com.android.settings/.Settings$BatterySaverSettingsActivity",
                "com.android.settings/.Settings$HighPowerApplicationsActivity",
                "com.android.settings/.Settings$AppAndNotificationBatteryUsageActivity",
                "com.android.settings/.Settings$BatterySaverScheduleRuleAddStartTimeActivity",
                "com.android.settings/.Settings$BatterySaverScheduleRuleAddEndTimeActivity",
                "com.android.settings/.Settings$BatterySaverScheduleRuleDaySelectionActivity",
                "com.android.settings/.Settings$BatterySaverScheduleRuleRepeatSelectionActivity",
                "com.android.settings/.Settings$BatterySaverScheduleRuleSettingsActivity",
                "com.android.settings/.Settings$BatterySaverScheduleRuleListActivity",
                "com.android.settings/.Settings$BatterySaverScheduleUpcomingRuleActivity",
                "com.android.settings/.Settings$BatterySaverScheduleSettingsActivity",
                "com.android.settings/.Settings$BatterySaverScheduleAddRuleActivity",
                "com.android.settings/.Settings$BatterySaverScheduleAddStartTimeActivity",
                "com.android.settings/.Settings$BatterySaverScheduleAddEndTimeActivity",
                "com.android.settings/.Settings$BatterySaverScheduleDaySelectionActivity",
                "com.android.settings/.Settings$BatterySaverScheduleRepeatSelectionActivity",
                "com.android.settings/.Settings$BatterySaverScheduleRuleAddStartTimeActivity",
                "com.android.settings/.Settings$BatterySaverScheduleRuleAddEndTimeActivity",
                "com.android.settings/.Settings$BatterySaverScheduleRuleDaySelectionActivity",
                "com.android.settings/.Settings$BatterySaverScheduleRuleRepeatSelectionActivity",
                "com.android.settings/.Settings$BatterySaverScheduleRuleSettingsActivity",
                "com.android.settings/.Settings$BatterySaverScheduleRuleListActivity",
                "com.android.settings/.Settings$BatterySaverScheduleUpcomingRuleActivity",
                "com.android.settings/.Settings$BatterySaverScheduleSettingsActivity",
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
            List<string> uris = new List<string>();

            // 获取设备的产品型号名称
            var brand = Build.Brand.ToLower();

            // 所有品牌的自启动URI信息
            var dictUri = GetManagerpageUriDictionary();


            // 判断是否获取成功
            if (!string.IsNullOrWhiteSpace(brand) && dictUri.ContainsKey(brand))
            {
                // 保存与产品名称对应的管理界面URI
                uris.AddRange(dictUri[brand]);
            }

            // 保存通用的管理界面URI
            uris.AddRange(dictUri["other"]);

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