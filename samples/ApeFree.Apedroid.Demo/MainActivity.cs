using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.Core.Widget;
using ApeFree.ApeDialogs.Core;
using ApeFree.ApeDialogs.Settings;
using ApeFree.ApeDialogs.Theme;
using ApeFree.Apedroid.Core;
using ApeFree.Apedroid.Dialogs.Adapters;
using ApeFree.Apedroid.Dialogs.Extensions;
using ApeFree.Apedroid.Dialogs.Providers;
using Google.Android.Material.Button;

namespace ApeFree.Apedroid.Demo
{
    [Activity(Label = "Apedroid Dialogs Showcase", MainLauncher = true, Theme = "@android:style/Theme.Material.Light.NoActionBar")]
    public class MainActivity : Activity
    {
        private TextView _tvLog = null!;

        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            ActivityTracker.Init(Application);

            var rootLayout = new LinearLayout(this)
            {
                Orientation = Orientation.Vertical
            };
            rootLayout.SetPadding(this.DpToPx(16), this.DpToPx(16), this.DpToPx(16), this.DpToPx(16));

            // 头部标题
            var tvHeader = new TextView(this)
            {
                Text = "Apedroid 19 种全量标准 Dialog 演示",
                TextSize = 20,
                Typeface = Typeface.DefaultBold
            };
            tvHeader.SetTextColor(Color.Rgb(28, 27, 31));
            tvHeader.Gravity = GravityFlags.Center;
            rootLayout.AddView(tvHeader, new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent));

            // 结果日志卡片
            _tvLog = new TextView(this)
            {
                Text = "操作结果将在此处实时显示...",
                TextSize = 13,
                Gravity = GravityFlags.CenterVertical
            };
            _tvLog.SetTextColor(Color.Rgb(50, 80, 160));
            _tvLog.SetBackgroundColor(Color.Rgb(240, 244, 255));
            int logPad = this.DpToPx(10);
            _tvLog.SetPadding(logPad, logPad, logPad, logPad);
            var logParams = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
            logParams.SetMargins(0, this.DpToPx(12), 0, this.DpToPx(12));
            rootLayout.AddView(_tvLog, logParams);

            // 按钮滚动区域
            var scrollView = new NestedScrollView(this);
            var buttonContainer = new LinearLayout(this) { Orientation = Orientation.Vertical };

            // 1. 基础信息类
            AddCategoryHeader(buttonContainer, "1. 基础信息类 (Information)");
            AddDemoButton(buttonContainer, "01. MessageDialog (消息提示)", async () =>
            {
                var r = await this.ShowMessageDialogAsync("系统通知", "这是一条标准的消息提示，告知用户当前操作成功完成。");
                LogResult("MessageDialog", r);
            });
            AddDemoButton(buttonContainer, "02. PromptDialog (询问确认)", async () =>
            {
                var r = await this.ShowPromptDialogAsync("确认删除", "确定要永久删除该记录吗？此操作无法撤销。");
                LogResult("PromptDialog", r);
            });
            AddDemoButton(buttonContainer, "03. TextDialog (长文本协议)", async () =>
            {
                string license = "【软件最终用户许可协议】\n\n1. 授权范围：Apedroid 是遵循开源许可协议的现代化 .NET Android 库。\n2. 使用限制：您可自由在商业或非商业项目中集成使用。\n3. 免责条款：作者在法律允许的最大范围内不对任何连带责任承担赔偿。\n\n感谢您选择 ApeFree 开源生态！\n(本段文本支持长按自由选取复制)";
                var r = await this.ShowTextDialogAsync("用户协议", license);
                LogResult("TextDialog", r);
            });
            AddDemoButton(buttonContainer, "04. ToastDialog (定时轻提示)", async () =>
            {
                var r = await this.ShowToastAsync("操作已提交，将在 2 秒后自动消失…", TimeSpan.FromSeconds(2));
                LogResult("ToastDialog", r);
            });

            // 2. 输入与表单类
            AddCategoryHeader(buttonContainer, "2. 输入与表单类 (Input & Forms)");
            AddDemoButton(buttonContainer, "05. InputDialog (文本输入)", async () =>
            {
                var r = await this.ShowInputDialogAsync("修改昵称", "请输入您的用户名：", "ApeFree Developer");
                LogResult("InputDialog", r);
            });
            AddDemoButton(buttonContainer, "06. PasswordDialog (密码输入)", async () =>
            {
                var r = await this.ShowPasswordDialogAsync("安全验证", "请输入您的登录密码：");
                LogResult("PasswordDialog", r);
            });
            AddDemoButton(buttonContainer, "07. NumberInputDialog (数值步进)", async () =>
            {
                var r = await this.ShowNumberInputDialogAsync("设置购买数量", defaultValue: 5, min: 1, max: 99);
                LogResult("NumberInputDialog", r);
            });
            AddDemoButton(buttonContainer, "08. ComboBoxInputDialog (可编辑下拉)", async () =>
            {
                var settings = new ComboBoxInputDialogSettings
                {
                    Title = "选择应用主题模式",
                    Content = "从预设项选择或自定义名称：",
                    Items = new[] { "Follow System", "Material Light", "Material Dark", "High Contrast" },
                    DefaultText = "Follow System",
                    IsEditable = true
                };
                var r = await ApedroidDialogProvider.Default.CreateComboBoxInputDialog(settings, this).ShowAsync();
                LogResult("ComboBoxInputDialog", r);
            });
            AddDemoButton(buttonContainer, "09. DataEntrySheetDialog (复杂动态表单)", async () =>
            {
                var sheet = new DataEntrySheet();
                sheet.AddField(new TextField { Title = "服务器主机", Value = "192.168.1.100" });
                sheet.AddField(new NumberField { Title = "端口号", Value = 8080 });
                sheet.AddField(new PasswordField { Title = "连接密钥", Value = "SecretToken123" });
                sheet.AddField(new ComboBoxField { Title = "传输协议", Items = new[] { "HTTP", "HTTPS", "WebSocket", "gRPC" }, Value = "HTTPS" });
                sheet.AddField(new DateTimeField { Title = "计划执行时间", Value = DateTime.Now.AddDays(1) });

                var r = await this.ShowDataEntrySheetDialogAsync("服务配置表单", sheet);
                LogResult("DataEntrySheetDialog", r);
            });

            // 3. 数据选择类
            AddCategoryHeader(buttonContainer, "3. 数据选择类 (Selection)");
            AddDemoButton(buttonContainer, "10. SelectionDialog (单选列表)", async () =>
            {
                var cities = new[] { "北京 (Beijing)", "上海 (Shanghai)", "广州 (Guangzhou)", "深圳 (Shenzhen)", "成都 (Chengdu)", "杭州 (Hangzhou)" };
                var r = await this.ShowSelectionDialogAsync("选择所属城市", cities, "成都 (Chengdu)");
                LogResult("SelectionDialog", r);
            });
            AddDemoButton(buttonContainer, "11. MultipleSelectionDialog (多选列表)", async () =>
            {
                var tags = new[] { "C#", "Android", ".NET 10", "Material Design", "ApeFree", "Kotlin", "MAUI" };
                var r = await this.ShowMultipleSelectionDialogAsync("选择技能标签", tags, new[] { "C#", ".NET 10", "ApeFree" });
                LogResult("MultipleSelectionDialog", r);
            });
            AddDemoButton(buttonContainer, "12. DateTimeDialog (日期与时间)", async () =>
            {
                var r = await this.ShowDateTimeDialogAsync("预约时间", DateTime.Now, DateTimePrecision.DateTime);
                LogResult("DateTimeDialog", r);
            });
            AddDemoButton(buttonContainer, "13. ColorDialog (调色板与拾色器)", async () =>
            {
                var r = await this.ShowColorDialogAsync("自定义主色调", ApeColor.DodgerBlue);
                LogResult("ColorDialog", r);
            });

            // 4. 调节控制类
            AddCategoryHeader(buttonContainer, "4. 调节控制与过程类 (Adjust & Progress)");
            AddDemoButton(buttonContainer, "14. SliderDialog (单滑块实时推流)", async () =>
            {
                var settings = new SliderDialogSettings
                {
                    Title = "屏幕亮度调节",
                    Content = "实时拖动滑块查看数值变化：",
                    Minimum = 0,
                    Maximum = 100,
                    DefaultValue = 65,
                    TickFrequency = 5
                };
                settings.OnValueChanged = v =>
                {
                    ActivityTracker.RunOnUiThread(() => _tvLog.Text = $"[Slider 实时推流] 当前值: {v}");
                };
                var r = await ApedroidDialogProvider.Default.CreateSliderDialog(settings, this).ShowAsync();
                LogResult("SliderDialog", r);
            });
            AddDemoButton(buttonContainer, "15. RangeSliderDialog (双滑块区间选择)", async () =>
            {
                var settings = new RangeSliderDialogSettings
                {
                    Title = "价格区间筛选",
                    Content = "请拖动两个滑块设定预算区间：",
                    Minimum = 0,
                    Maximum = 500,
                    DefaultStart = 100,
                    DefaultEnd = 350,
                    TickFrequency = 10
                };
                settings.OnValueChanged = range =>
                {
                    ActivityTracker.RunOnUiThread(() => _tvLog.Text = $"[RangeSlider 实时推流] 范围: {range.Start} ~ {range.End}");
                };
                var r = await ApedroidDialogProvider.Default.CreateRangeSliderDialog(settings, this).ShowAsync();
                LogResult("RangeSliderDialog", r);
            });
            AddDemoButton(buttonContainer, "16. ProgressDialog (进度条加载)", async () =>
            {
                var settings = new ProgressDialogSettings
                {
                    Title = "正在下载系统资源包",
                    InitialMessage = "准备就绪，即将开始…",
                    IsIndeterminate = false,
                    Cancelable = true
                };
                var progressDialog = ApedroidDialogProvider.Default.CreateProgressDialog(settings, this);

                // 启动后台模拟任务
                _ = Task.Run(async () =>
                {
                    for (int i = 0; i <= 100; i += 10)
                    {
                        await Task.Delay(250);
                        progressDialog.UpdateProgress(i / 100.0, $"正在下载: 数据包 #{i / 10} / 10");
                    }
                });

                var r = await progressDialog.ShowAsync();
                LogResult("ProgressDialog", r);
            });

            // 5. 存储与系统交互类 (SAF)
            AddCategoryHeader(buttonContainer, "5. 存储与系统交互类 (SAF)");
            AddDemoButton(buttonContainer, "17. OpenFileDialog (打开系统文件)", async () =>
            {
                var settings = new OpenFileDialogSettings
                {
                    Title = "导入外部数据文件",
                    MultiSelect = true
                };
                var r = await ApedroidDialogProvider.Default.CreateOpenFileDialog(settings, this).ShowAsync();
                LogResult("OpenFileDialog", r);
            });
            AddDemoButton(buttonContainer, "18. SaveFileDialog (保存文件目标)", async () =>
            {
                var settings = new SaveFileDialogSettings
                {
                    Title = "导出报表为文件",
                    DefaultFileName = "Report_2026.csv"
                };
                var r = await ApedroidDialogProvider.Default.CreateSaveFileDialog(settings, this).ShowAsync();
                LogResult("SaveFileDialog", r);
            });
            AddDemoButton(buttonContainer, "19. OpenFolderDialog (授权目录树)", async () =>
            {
                var settings = new OpenFolderDialogSettings
                {
                    Title = "选择应用工作备份目录"
                };
                var r = await ApedroidDialogProvider.Default.CreateOpenFolderDialog(settings, this).ShowAsync();
                LogResult("OpenFolderDialog", r);
            });

            scrollView.AddView(buttonContainer);
            rootLayout.AddView(scrollView, new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, 0, 1.0f));

            SetContentView(rootLayout);
        }

        private void AddCategoryHeader(LinearLayout container, string title)
        {
            var tv = new TextView(this)
            {
                Text = title,
                TextSize = 15,
                Typeface = Typeface.DefaultBold
            };
            tv.SetTextColor(Color.Rgb(100, 100, 100));
            var lp = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
            lp.SetMargins(0, this.DpToPx(14), 0, this.DpToPx(6));
            container.AddView(tv, lp);
        }

        private void AddDemoButton(LinearLayout container, string text, Action onClick)
        {
            var btn = new MaterialButton(this)
            {
                Text = text,
                CornerRadius = this.DpToPx(10)
            };
            btn.SetAllCaps(false);
            btn.SetBackgroundColor(Color.Rgb(245, 245, 248));
            btn.SetTextColor(Color.Rgb(28, 27, 31));
            btn.StrokeWidth = this.DpToPx(1);
            btn.StrokeColor = Android.Content.Res.ColorStateList.ValueOf(Color.Rgb(220, 220, 225));

            var lp = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, this.DpToPx(44));
            lp.SetMargins(0, this.DpToPx(3), 0, this.DpToPx(3));
            btn.LayoutParameters = lp;

            btn.Click += (s, e) => onClick();
            container.AddView(btn);
        }

        private void LogResult<T>(string dialogName, DialogResult<T> result)
        {
            ActivityTracker.RunOnUiThread(() =>
            {
                string statusText = !result.IsCanceled ? "已确认 (Confirmed)" : "已取消 (Canceled)";
                string valText = !result.IsCanceled ? (result.Value is IEnumerable<string> list ? string.Join(", ", list) : result.Value?.ToString() ?? "null") : "无";
                _tvLog.Text = $"[{dialogName}] 状态: {statusText}\n返回值: {valText}";
            });
        }
    }
}
