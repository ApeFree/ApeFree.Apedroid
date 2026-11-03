using System;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Text;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using ApeFree.ApeDialogs.Settings;
using ApeFree.Apedroid.Dialogs.Adapters;
using ApeFree.Apedroid.Dialogs.Core;
using ApeFree.Apedroid.Dialogs.Hosting;
using Google.Android.Material.TextField;

namespace ApeFree.Apedroid.Dialogs.Views
{
    /// <summary>
    /// 文本输入对话框，支持单行/多行、字数限制与水印提示。
    /// </summary>
    public class ApedroidInputDialog : ApedroidDialog<string>
    {
        private readonly InputDialogSettings _settings;
        private readonly TextInputLayout _inputLayout;
        private readonly TextInputEditText _editText;

        public ApedroidInputDialog(InputDialogSettings settings, Context? context = null)
            : base(settings, context)
        {
            _settings = settings;

            var ctx = Context!;
            _inputLayout = new TextInputLayout(ctx)
            {
                BoxBackgroundMode = TextInputLayout.BoxBackgroundOutline,
                Hint = string.IsNullOrWhiteSpace(settings.Content) ? "请输入内容" : settings.Content
            };
            _inputLayout.SetBoxCornerRadii(ctx.DpToPx(12), ctx.DpToPx(12), ctx.DpToPx(12), ctx.DpToPx(12));

            _editText = new TextInputEditText(_inputLayout.Context)
            {
                Text = settings.DefaultText ?? string.Empty
            };
            _editText.SetTextColor(Color.Rgb(28, 27, 31));
            _editText.TextSize = 16;

            if (settings.IsMultiline)
            {
                _editText.InputType = InputTypes.ClassText | InputTypes.TextFlagMultiLine;
                _editText.SetMinLines(3);
                _editText.SetMaxLines(6);
                _editText.Gravity = GravityFlags.Top | GravityFlags.Start;
            }
            else
            {
                _editText.InputType = InputTypes.ClassText;
                _editText.SetSingleLine(true);
                _editText.ImeOptions = ImeAction.Done;
                _editText.EditorAction += (s, e) =>
                {
                    if (e.ActionId == ImeAction.Done)
                    {
                        OnConfirmRequested();
                    }
                };
            }

            if (settings.MaximumLength > 0)
            {
                _inputLayout.CounterEnabled = true;
                _inputLayout.CounterMaxLength = settings.MaximumLength;
                _editText.SetFilters(new IInputFilter[] { new InputFilterLengthFilter(settings.MaximumLength) });
            }

            _inputLayout.AddView(_editText);
            Host.SetContentView(_inputLayout);

            AddDefaultOptions();
        }

        public override OptionButton AddOption(DialogOption dialogOption)
        {
            var button = base.AddOption(dialogOption);
            if (ReferenceEquals(dialogOption, _settings.ConfirmOption))
            {
                button.Click += (_, _) => TrySubmit(_editText.Text ?? string.Empty);
            }
            else if (ReferenceEquals(dialogOption, _settings.ClearOption))
            {
                button.Click += (_, _) =>
                {
                    _editText.Text = string.Empty;
                    _editText.RequestFocus();
                };
            }
            return button;
        }

        protected override void OnConfirmRequested() => TrySubmit(_editText.Text ?? string.Empty);
    }

    /// <summary>
    /// 密码输入对话框，支持密码掩码与眼睛图标切换可见性。
    /// </summary>
    public class ApedroidPasswordDialog : ApedroidDialog<string>
    {
        private readonly PasswordDialogSettings _settings;
        private readonly TextInputLayout _inputLayout;
        private readonly TextInputEditText _editText;

        public ApedroidPasswordDialog(PasswordDialogSettings settings, Context? context = null)
            : base(settings, context)
        {
            _settings = settings;

            var ctx = Context!;
            _inputLayout = new TextInputLayout(ctx)
            {
                BoxBackgroundMode = TextInputLayout.BoxBackgroundOutline,
                Hint = string.IsNullOrWhiteSpace(settings.Content) ? "请输入密码" : settings.Content,
                EndIconMode = TextInputLayout.EndIconPasswordToggle
            };
            _inputLayout.SetBoxCornerRadii(ctx.DpToPx(12), ctx.DpToPx(12), ctx.DpToPx(12), ctx.DpToPx(12));

            _editText = new TextInputEditText(_inputLayout.Context)
            {
                InputType = InputTypes.ClassText | InputTypes.TextVariationPassword,
                Text = settings.DefaultText ?? string.Empty
            };
            _editText.SetTextColor(Color.Rgb(28, 27, 31));
            _editText.SetSingleLine(true);
            _editText.ImeOptions = ImeAction.Done;
            _editText.EditorAction += (s, e) =>
            {
                if (e.ActionId == ImeAction.Done)
                {
                    OnConfirmRequested();
                }
            };

            if (settings.MaximumLength > 0)
            {
                _inputLayout.CounterEnabled = true;
                _inputLayout.CounterMaxLength = settings.MaximumLength;
                _editText.SetFilters(new IInputFilter[] { new InputFilterLengthFilter(settings.MaximumLength) });
            }

            _inputLayout.AddView(_editText);
            Host.SetContentView(_inputLayout);

            AddDefaultOptions();
        }

        public override OptionButton AddOption(DialogOption dialogOption)
        {
            var button = base.AddOption(dialogOption);
            if (ReferenceEquals(dialogOption, _settings.ConfirmOption))
            {
                button.Click += (_, _) => TrySubmit(_editText.Text ?? string.Empty);
            }
            else if (ReferenceEquals(dialogOption, _settings.ClearOption))
            {
                button.Click += (_, _) =>
                {
                    _editText.Text = string.Empty;
                    _editText.RequestFocus();
                };
            }
            return button;
        }

        protected override void OnConfirmRequested() => TrySubmit(_editText.Text ?? string.Empty);
    }
}
