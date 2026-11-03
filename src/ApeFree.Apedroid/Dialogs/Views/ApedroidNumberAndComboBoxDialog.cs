using System;
using System.Globalization;
using System.Linq;
using Android.Content;
using Android.Graphics;
using Android.Text;
using Android.Views;
using Android.Widget;
using ApeFree.ApeDialogs.Settings;
using ApeFree.Apedroid.Dialogs.Adapters;
using ApeFree.Apedroid.Dialogs.Core;
using ApeFree.Apedroid.Dialogs.Hosting;
using Google.Android.Material.Button;
using Google.Android.Material.TextField;

namespace ApeFree.Apedroid.Dialogs.Views
{
    /// <summary>
    /// 数值步进输入对话框，支持加减按钮步进、范围限制与小数位数设定。
    /// </summary>
    public class ApedroidNumberInputDialog : ApedroidDialog<decimal>
    {
        private readonly NumberInputDialogSettings _settings;
        private readonly EditText _etValue;
        private decimal _currentValue;

        public ApedroidNumberInputDialog(NumberInputDialogSettings settings, Context? context = null)
            : base(settings, context)
        {
            _settings = settings;
            _currentValue = Math.Max(settings.Minimum, Math.Min(settings.Maximum, settings.DefaultValue));

            var ctx = Context!;
            var container = new LinearLayout(ctx)
            {
                Orientation = Orientation.Horizontal
            };
            container.SetGravity(GravityFlags.Center);

            var btnMinus = new MaterialButton(ctx)
            {
                Text = "-",
                TextSize = 20,
                CornerRadius = ctx.DpToPx(12)
            };
            btnMinus.LayoutParameters = new ViewGroup.LayoutParams(ctx.DpToPx(52), ctx.DpToPx(48));

            _etValue = new EditText(ctx)
            {
                InputType = InputTypes.ClassNumber | InputTypes.NumberFlagDecimal | InputTypes.NumberFlagSigned,
                Gravity = GravityFlags.Center,
                TextSize = 18
            };
            _etValue.SetTextColor(Color.Rgb(28, 27, 31));
            UpdateText();

            var etParams = new LinearLayout.LayoutParams(0, ctx.DpToPx(48), 1.0f);
            etParams.SetMargins(ctx.DpToPx(8), 0, ctx.DpToPx(8), 0);
            _etValue.LayoutParameters = etParams;

            var btnPlus = new MaterialButton(ctx)
            {
                Text = "+",
                TextSize = 20,
                CornerRadius = ctx.DpToPx(12)
            };
            btnPlus.LayoutParameters = new ViewGroup.LayoutParams(ctx.DpToPx(52), ctx.DpToPx(48));

            btnMinus.Click += (s, e) =>
            {
                SyncFromText();
                _currentValue = Math.Max(_settings.Minimum, _currentValue - _settings.Increment);
                UpdateText();
            };

            btnPlus.Click += (s, e) =>
            {
                SyncFromText();
                _currentValue = Math.Min(_settings.Maximum, _currentValue + _settings.Increment);
                UpdateText();
            };

            container.AddView(btnMinus);
            container.AddView(_etValue);
            container.AddView(btnPlus);

            Host.SetContentView(container);
            AddDefaultOptions();
        }

        private void SyncFromText()
        {
            if (decimal.TryParse(_etValue.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out var val))
            {
                _currentValue = Math.Max(_settings.Minimum, Math.Min(_settings.Maximum, val));
            }
        }

        private void UpdateText()
        {
            string format = _settings.DecimalPlaces > 0 ? $"F{_settings.DecimalPlaces}" : "F0";
            _etValue.Text = _currentValue.ToString(format, CultureInfo.InvariantCulture);
            _etValue.SetSelection(_etValue.Text.Length);
        }

        public override OptionButton AddOption(DialogOption dialogOption)
        {
            var button = base.AddOption(dialogOption);
            if (ReferenceEquals(dialogOption, _settings.ConfirmOption))
            {
                button.Click += (_, _) => OnConfirmRequested();
            }
            return button;
        }

        protected override void OnConfirmRequested()
        {
            SyncFromText();
            TrySubmit(_currentValue);
        }
    }

    /// <summary>
    /// 可编辑/不可编辑下拉输入对话框（Exposed Dropdown Menu）。
    /// </summary>
    public class ApedroidComboBoxInputDialog : ApedroidDialog<string>
    {
        private readonly ComboBoxInputDialogSettings _settings;
        private readonly MaterialAutoCompleteTextView _autoCompleteTextView;

        public ApedroidComboBoxInputDialog(ComboBoxInputDialogSettings settings, Context? context = null)
            : base(settings, context)
        {
            _settings = settings;

            var ctx = Context!;
            var textInputLayout = new TextInputLayout(ctx)
            {
                BoxBackgroundMode = TextInputLayout.BoxBackgroundOutline,
                Hint = string.IsNullOrWhiteSpace(settings.Content) ? "请选择或输入" : settings.Content
            };
            textInputLayout.SetBoxCornerRadii(ctx.DpToPx(12), ctx.DpToPx(12), ctx.DpToPx(12), ctx.DpToPx(12));

            _autoCompleteTextView = new MaterialAutoCompleteTextView(textInputLayout.Context)
            {
                TextSize = 16
            };
            _autoCompleteTextView.SetTextColor(Color.Rgb(28, 27, 31));

            var items = settings.Items?.ToList() ?? new System.Collections.Generic.List<string>();
            var adapter = new ArrayAdapter<string>(ctx, Android.Resource.Layout.SimpleDropDownItem1Line, items);
            _autoCompleteTextView.Adapter = adapter;

            if (!settings.IsEditable)
            {
                _autoCompleteTextView.Focusable = false;
                _autoCompleteTextView.Clickable = true;
            }

            _autoCompleteTextView.Text = settings.DefaultText ?? (items.Count > 0 ? items[0] : string.Empty);

            textInputLayout.AddView(_autoCompleteTextView);
            Host.SetContentView(textInputLayout);

            AddDefaultOptions();
        }

        public override OptionButton AddOption(DialogOption dialogOption)
        {
            var button = base.AddOption(dialogOption);
            if (ReferenceEquals(dialogOption, _settings.ConfirmOption))
            {
                button.Click += (_, _) => OnConfirmRequested();
            }
            return button;
        }

        protected override void OnConfirmRequested()
        {
            TrySubmit(_autoCompleteTextView.Text ?? string.Empty);
        }
    }
}
