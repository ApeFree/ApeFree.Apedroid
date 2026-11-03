using System;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Views;
using ApeFree.ApeDialogs.Settings;
using ApeFree.ApeDialogs.Theme;
using ApeFree.Apedroid.Dialogs.Adapters;
using Google.Android.Material.Button;

namespace ApeFree.Apedroid.Dialogs.Hosting
{
    /// <summary>
    /// 对话框选项按钮控件，遵循 Material 3 设计规范并支持 ApeDialogs 语义标签配色。
    /// </summary>
    public class OptionButton : MaterialButton
    {
        public DialogOption Option { get; }

        public OptionButton(Context context, DialogOption option, IDialogTheme? theme = null)
            : base(context)
        {
            Option = option ?? throw new ArgumentNullException(nameof(option));

            Text = option.Text;
            TextAlignment = TextAlignment.Center;
            CornerRadius = context.DpToPx(12);
            SetAllCaps(false);

            ApplyThemeStyle(theme ?? DefaultTheme.Instance);

            var layoutParams = new ViewGroup.MarginLayoutParams(
                ViewGroup.LayoutParams.WrapContent,
                context.DpToPx(44)
            );
            int marginHorizontal = context.DpToPx(4);
            layoutParams.SetMargins(marginHorizontal, 0, marginHorizontal, 0);
            LayoutParameters = layoutParams;

            int paddingH = context.DpToPx(16);
            SetPadding(paddingH, 0, paddingH, 0);
        }

        /// <summary>
        /// 根据 DialogOptionTag 与主题应用背景和文字配色
        /// </summary>
        public void ApplyThemeStyle(IDialogTheme theme)
        {
            var apeColor = theme.GetColor(Option.Tag);
            var bgColor = apeColor.ToColor();

            if (apeColor.A == 0)
            {
                // 透明/无特定色，使用 Material Outlined / Tonal 默认方案
                SetBackgroundColor(Color.Transparent);
                StrokeWidth = Context.DpToPx(1);
                StrokeColor = ColorStateList.ValueOf(Color.Rgb(180, 180, 180));
                SetTextColor(Color.Rgb(33, 33, 33));
            }
            else
            {
                // 实体填充色
                BackgroundTintList = ColorStateList.ValueOf(bgColor);
                var textColor = bgColor.GetContrastTextColor();
                SetTextColor(textColor);
                StrokeWidth = 0;
            }
        }
    }
}
