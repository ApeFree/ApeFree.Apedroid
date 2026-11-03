using System;
using Android.Content;
using Android.Widget;
using ApeFree.ApeDialogs.Core;
using ApeFree.ApeDialogs.Settings;
using ApeFree.ApeDialogs.Theme;
using ApeFree.Apedroid.Core;
using ApeFree.Apedroid.Dialogs.Adapters;
using ApeFree.Apedroid.Dialogs.Hosting;

namespace ApeFree.Apedroid.Dialogs.Core
{
    /// <summary>
    /// Apedroid Dialog 实现基类。
    /// 统一管理 ApedroidDialogHost、选项按钮装配、智能上下文回退与生命周期事件。
    /// </summary>
    /// <typeparam name="TResult">结果数据类型</typeparam>
    public class ApedroidDialog<TResult> : BaseDialog<ApedroidDialogHost, OptionButton, Context, TResult>
    {
        private readonly IDialogTheme _theme;

        public ApedroidDialogHost Host => View!;

        public override string Title
        {
            get => Host?.Title ?? string.Empty;
            set
            {
                if (Host != null)
                {
                    Host.Title = value;
                }
            }
        }

        public override string Content
        {
            get => Host?.Content ?? string.Empty;
            set
            {
                if (Host != null)
                {
                    Host.Content = value;
                }
            }
        }

        public ApedroidDialog(DialogSettings<TResult> settings, Context? context = null)
            : base(settings, ResolveContext(context), null)
        {
            _theme = settings.Theme ?? DefaultTheme.Instance;

            var ctx = Context ?? throw new InvalidOperationException("无法获取有效的 Android Context。请确保传入 Activity 或已通过 ActivityTracker 追踪到当前前台 Activity。");
            View = new ApedroidDialogHost(ctx);

            Host.Title = settings.Title;
            Host.Content = settings.Content ?? string.Empty;
            Host.SetCancelable(settings.Cancelable);

            // 应用提醒色 (ReminderColor)
            var reminderApeColor = settings.ReminderColor ?? _theme.ReminderColor;
            if (reminderApeColor.A > 0)
            {
                Host.SetReminderColor(reminderApeColor.ToColor());
            }

            Host.Shown += (_, _) => RaiseShown();
            Host.Dismissed += (_, _) =>
            {
                if (IsShown)
                {
                    Cancel();
                }
            };
            Host.Canceled += (_, _) => Cancel();
        }

        private static Context ResolveContext(Context? context)
        {
            if (context != null)
            {
                return context;
            }

            var current = ActivityTracker.CurrentActivity;
            if (current != null)
            {
                return current;
            }

            return ActivityTracker.Application!;
        }

        /// <inheritdoc/>
        public override OptionButton AddOption(DialogOption dialogOption)
        {
            if (dialogOption == null)
            {
                throw new ArgumentNullException(nameof(dialogOption));
            }

            var btn = new OptionButton(Context!, dialogOption, _theme);
            btn.Click += (_, _) =>
            {
                dialogOption.Action?.Invoke(this);
            };

            Host.AddOptionButton(btn);
            return btn;
        }

        /// <inheritdoc/>
        public override void ClearOptions()
        {
            // 由 Host 管理按钮重置
        }

        /// <inheritdoc/>
        protected override void ShowHandler()
        {
            Host.Show();
        }

        /// <inheritdoc/>
        protected override void DismissHandler()
        {
            Host.Dismiss();
        }

        /// <inheritdoc/>
        protected override void OnValidationFailed(ValidationResult result)
        {
            base.OnValidationFailed(result);

            ActivityTracker.RunOnUiThread(() =>
            {
                string message = result.ErrorMessage ?? "输入无效，请检查后重试";
                Toast.MakeText(Context, message, ToastLength.Short)?.Show();
            });
        }

        /// <summary>
        /// 触发提交动作（子类快捷调用）
        /// </summary>
        protected virtual void OnConfirmRequested()
        {
        }
    }
}
