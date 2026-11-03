using Android.Content;
using ApeFree.ApeDialogs.Settings;
using ApeFree.Apedroid.Dialogs.Core;
using ApeFree.Apedroid.Dialogs.Hosting;

namespace ApeFree.Apedroid.Dialogs.Views
{
    /// <summary>
    /// 基础消息对话框，提供单确认按钮的信息提示。
    /// </summary>
    public class ApedroidMessageDialog : ApedroidDialog<bool>
    {
        private readonly MessageDialogSettings _settings;

        public ApedroidMessageDialog(MessageDialogSettings settings, Context? context = null)
            : base(settings, context)
        {
            _settings = settings;
            AddDefaultOptions();
        }

        public override OptionButton AddOption(DialogOption dialogOption)
        {
            var button = base.AddOption(dialogOption);
            if (ReferenceEquals(dialogOption, _settings.ConfirmOption))
            {
                button.Click += (_, _) => TrySubmit(true);
            }
            return button;
        }

        protected override void OnConfirmRequested() => TrySubmit(true);
    }

    /// <summary>
    /// 二元提示对话框（Yes / No 或 确认 / 取消）。
    /// </summary>
    public class ApedroidPromptDialog : ApedroidDialog<bool>
    {
        private readonly PromptDialogSettings _settings;

        public ApedroidPromptDialog(PromptDialogSettings settings, Context? context = null)
            : base(settings, context)
        {
            _settings = settings;
            AddDefaultOptions();
        }

        public override OptionButton AddOption(DialogOption dialogOption)
        {
            var button = base.AddOption(dialogOption);
            if (ReferenceEquals(dialogOption, _settings.PositiveOption))
            {
                button.Click += (_, _) => TrySubmit(true);
            }
            else if (ReferenceEquals(dialogOption, _settings.NegativeOption))
            {
                button.Click += (_, _) => TrySubmit(false);
            }
            return button;
        }

        protected override void OnConfirmRequested() => TrySubmit(true);
    }
}
