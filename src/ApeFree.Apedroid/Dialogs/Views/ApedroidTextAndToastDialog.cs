using System;
using Android.Content;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using AndroidX.Core.Widget;
using ApeFree.ApeDialogs.Settings;
using ApeFree.Apedroid.Dialogs.Adapters;
using ApeFree.Apedroid.Dialogs.Core;
using ApeFree.Apedroid.Dialogs.Hosting;

namespace ApeFree.Apedroid.Dialogs.Views
{
    /// <summary>
    /// 长文本与协议查看对话框，支持多段落流畅滚动与文字长按复制。
    /// </summary>
    public class ApedroidTextDialog : ApedroidDialog<bool>
    {
        private readonly TextDialogSettings _settings;

        public ApedroidTextDialog(TextDialogSettings settings, Context? context = null)
            : base(settings, context)
        {
            _settings = settings;

            var ctx = Context!;
            var scrollView = new NestedScrollView(ctx)
            {
                VerticalScrollBarEnabled = false,
                HorizontalScrollBarEnabled = false
            };

            var tvText = new TextView(ctx)
            {
                Text = settings.Text ?? string.Empty,
                TextSize = 14
            };
            tvText.SetTextColor(Color.Rgb(40, 40, 40));
            tvText.SetTextIsSelectable(true);
            tvText.SetLineSpacing(ctx.DpToPx(3), 1.2f);

            var scrollParams = new ViewGroup.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                Math.Min(ctx.DpToPx(320), ViewGroup.LayoutParams.WrapContent)
            );
            scrollView.LayoutParameters = scrollParams;
            scrollView.AddView(tvText);

            Host.SetContentView(scrollView);
            AddDefaultOptions();
        }

        public override OptionButton AddOption(DialogOption dialogOption)
        {
            var button = base.AddOption(dialogOption);
            if (ReferenceEquals(dialogOption, _settings.CloseOption))
            {
                button.Click += (_, _) => TrySubmit(true);
            }
            return button;
        }

        protected override void OnConfirmRequested() => TrySubmit(true);
    }

    /// <summary>
    /// 轻提示弹窗（Toast 卡片），按 Duration 自动倒计时退出，亦支持点击确认。
    /// </summary>
    public class ApedroidToastDialog : ApedroidDialog<bool>
    {
        public ApedroidToastDialog(ToastDialogSettings settings, Context? context = null)
            : base(settings, context)
        {
            AddDefaultOptions();
        }

        public override OptionButton AddOption(DialogOption dialogOption)
        {
            var button = base.AddOption(dialogOption);
            button.Click += (_, _) => TrySubmit(true);
            return button;
        }

        protected override void OnConfirmRequested() => TrySubmit(true);
    }
}
