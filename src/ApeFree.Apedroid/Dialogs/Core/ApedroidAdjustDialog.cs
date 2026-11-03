using System;
using Android.Content;
using ApeFree.ApeDialogs.Core;
using ApeFree.ApeDialogs.Settings;
using ApeFree.ApeDialogs.Theme;
using ApeFree.Apedroid.Core;
using ApeFree.Apedroid.Dialogs.Adapters;
using ApeFree.Apedroid.Dialogs.Hosting;

namespace ApeFree.Apedroid.Dialogs.Core
{
    /// <summary>
    /// Apedroid 实时调节类 Dialog 基类（如单滑块、双滑块范围调节等）。
    /// </summary>
    /// <typeparam name="TResult">结果数据类型</typeparam>
    public abstract class ApedroidAdjustDialog<TResult> : BaseAdjustDialog<ApedroidDialogHost, OptionButton, Context, TResult>
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

        protected ApedroidAdjustDialog(DialogSettings<TResult> settings, Context? context = null)
            : base(settings, ResolveContext(context), null)
        {
            _theme = settings.Theme ?? DefaultTheme.Instance;

            var ctx = Context ?? throw new InvalidOperationException("无法获取有效的 Android Context。");
            View = new ApedroidDialogHost(ctx);

            Host.Title = settings.Title;
            Host.Content = settings.Content ?? string.Empty;
            Host.SetCancelable(settings.Cancelable);

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
            if (context != null) return context;
            return ActivityTracker.CurrentActivity ?? (Context)ActivityTracker.Application!;
        }

        public override OptionButton AddOption(DialogOption dialogOption)
        {
            if (dialogOption == null)
            {
                throw new ArgumentNullException(nameof(dialogOption));
            }

            var btn = new OptionButton(Context!, dialogOption, _theme);
            btn.Click += (_, _) => dialogOption.Action?.Invoke(this);

            Host.AddOptionButton(btn);
            return btn;
        }

        public override void ClearOptions() { }

        protected override void ShowHandler() => Host.Show();

        protected override void DismissHandler() => Host.Dismiss();

        protected virtual void OnConfirmRequested() { }
    }
}
