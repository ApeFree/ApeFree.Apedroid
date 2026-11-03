using System;
using Android.Content;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using ApeFree.ApeDialogs.Core;
using ApeFree.ApeDialogs.Settings;
using ApeFree.Apedroid.Core;
using ApeFree.Apedroid.Dialogs.Adapters;
using ApeFree.Apedroid.Dialogs.Core;
using ApeFree.Apedroid.Dialogs.Hosting;
using Google.Android.Material.ProgressIndicator;

namespace ApeFree.Apedroid.Dialogs.Views
{
    /// <summary>
    /// 进度指示对话框，支持确定型线性进度与不确定型动画，提供动态消息更新。
    /// </summary>
    public class ApedroidProgressDialog : ApedroidDialog<bool>, IProgressDialog
    {
        private readonly ProgressDialogSettings _settings;
        private readonly LinearProgressIndicator _progressIndicator;
        private readonly TextView _tvMessage;
        private readonly TextView _tvPercent;

        public bool IsIndeterminate => _settings.IsIndeterminate;

        public ApedroidProgressDialog(ProgressDialogSettings settings, Context? context = null)
            : base(settings, context)
        {
            _settings = settings;

            var ctx = Context!;
            var container = new LinearLayout(ctx)
            {
                Orientation = Orientation.Vertical
            };

            _tvMessage = new TextView(ctx)
            {
                Text = settings.InitialMessage ?? "正在处理，请稍候…",
                TextSize = 14
            };
            _tvMessage.SetTextColor(Color.Rgb(60, 60, 60));
            var messageParams = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent
            );
            messageParams.SetMargins(0, 0, 0, ctx.DpToPx(10));
            container.AddView(_tvMessage, messageParams);

            _progressIndicator = new LinearProgressIndicator(ctx)
            {
                Indeterminate = settings.IsIndeterminate,
                TrackCornerRadius = ctx.DpToPx(4),
                TrackThickness = ctx.DpToPx(8)
            };

            if (!settings.IsIndeterminate)
            {
                _progressIndicator.Max = 100;
                _progressIndicator.Progress = (int)Math.Max(0, Math.Min(100, settings.InitialProgress * 100));
            }

            var progressParams = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent
            );
            progressParams.SetMargins(0, 0, 0, ctx.DpToPx(6));
            container.AddView(_progressIndicator, progressParams);

            _tvPercent = new TextView(ctx)
            {
                TextSize = 12,
                Gravity = GravityFlags.End,
                Visibility = settings.IsIndeterminate ? ViewStates.Gone : ViewStates.Visible
            };
            _tvPercent.SetTextColor(Color.Rgb(120, 120, 120));
            if (!settings.IsIndeterminate)
            {
                _tvPercent.Text = $"{_progressIndicator.Progress}%";
            }
            container.AddView(_tvPercent, new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent
            ));

            Host.SetContentView(container);
            AddDefaultOptions();
        }

        public void UpdateProgress(double progress, string? message = null)
        {
            ActivityTracker.RunOnUiThread(() =>
            {
                var clamped = Math.Max(0.0, Math.Min(1.0, progress));
                if (!IsIndeterminate)
                {
                    int val = (int)(clamped * 100);
                    _progressIndicator.Progress = val;
                    _tvPercent.Text = $"{val}%";
                }

                if (!string.IsNullOrEmpty(message))
                {
                    _tvMessage.Text = message;
                }

                if (clamped >= 1.0)
                {
                    TrySubmit(true);
                }
            });
        }

        public override OptionButton AddOption(DialogOption dialogOption)
        {
            var button = base.AddOption(dialogOption);
            button.Click += (_, _) =>
            {
                if (ReferenceEquals(dialogOption, _settings.CancelOption))
                {
                    Cancel();
                }
            };
            return button;
        }

        protected override void OnConfirmRequested() => TrySubmit(true);
    }
}
