using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Android.Content;
using Android.Graphics;
using Android.Text;
using Android.Views;
using Android.Widget;
using AndroidX.Core.Widget;
using ApeFree.ApeDialogs.Core;
using ApeFree.ApeDialogs.Settings;
using ApeFree.Apedroid.Dialogs.Adapters;
using ApeFree.Apedroid.Dialogs.Core;
using ApeFree.Apedroid.Dialogs.Hosting;
using Google.Android.Material.CheckBox;
using Google.Android.Material.RadioButton;
using Google.Android.Material.TextField;

namespace ApeFree.Apedroid.Dialogs.Views
{
    /// <summary>
    /// Android 动态表单视图组件。
    /// 根据 DataEntrySheet 中的字段列表动态生成并双向同步表单控件。
    /// </summary>
    public class DataEntryView : NestedScrollView
    {
        private readonly LinearLayout _container;
        private readonly List<Action> _saveActions = new();
        private DataEntrySheet? _sheet;

        public DataEntryView(Context context) : base(context)
        {
            ScrollbarFadingEnabled = false;

            _container = new LinearLayout(context)
            {
                Orientation = Orientation.Vertical
            };
            AddView(_container, new LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent
            ));
        }

        public void BindSheet(DataEntrySheet sheet)
        {
            _sheet = sheet ?? throw new ArgumentNullException(nameof(sheet));
            _container.RemoveAllViews();
            _saveActions.Clear();

            var ctx = Context!;

            foreach (var field in sheet.Fields)
            {
                var fieldCard = new LinearLayout(ctx)
                {
                    Orientation = Orientation.Vertical
                };
                var cardParams = new LinearLayout.LayoutParams(
                    ViewGroup.LayoutParams.MatchParent,
                    ViewGroup.LayoutParams.WrapContent
                );
                cardParams.SetMargins(0, 0, 0, ctx.DpToPx(12));
                fieldCard.LayoutParameters = cardParams;

                // 字段标题
                var tvLabel = new TextView(ctx)
                {
                    Text = field.Title,
                    TextSize = 14,
                    Typeface = Typeface.DefaultBold
                };
                tvLabel.SetTextColor(Color.Rgb(60, 60, 60));
                var labelParams = new LinearLayout.LayoutParams(
                    ViewGroup.LayoutParams.MatchParent,
                    ViewGroup.LayoutParams.WrapContent
                );
                labelParams.SetMargins(0, 0, 0, ctx.DpToPx(4));
                fieldCard.AddView(tvLabel, labelParams);

                // 字段编辑视图
                var editorView = CreateEditorView(ctx, field);
                if (editorView != null)
                {
                    fieldCard.AddView(editorView);
                }

                _container.AddView(fieldCard);
            }
        }

        private View? CreateEditorView(Context ctx, SheetField field)
        {
            switch (field.FieldType)
            {
                case FieldType.Text:
                case FieldType.HexBytes:
                case FieldType.FilePath:
                case FieldType.PicturePath:
                    {
                        var textLayout = new TextInputLayout(ctx)
                        {
                            BoxBackgroundMode = TextInputLayout.BoxBackgroundOutline
                        };
                        textLayout.SetBoxCornerRadii(ctx.DpToPx(8), ctx.DpToPx(8), ctx.DpToPx(8), ctx.DpToPx(8));

                        var editText = new TextInputEditText(textLayout.Context)
                        {
                            Text = field.GetRawValue()?.ToString() ?? string.Empty
                        };
                        editText.SetTextColor(Color.Rgb(28, 27, 31));
                        textLayout.AddView(editText);

                        _saveActions.Add(() => field.SetRawValue(editText.Text));
                        return textLayout;
                    }

                case FieldType.Password:
                    {
                        var textLayout = new TextInputLayout(ctx)
                        {
                            BoxBackgroundMode = TextInputLayout.BoxBackgroundOutline,
                            EndIconMode = TextInputLayout.EndIconPasswordToggle
                        };
                        textLayout.SetBoxCornerRadii(ctx.DpToPx(8), ctx.DpToPx(8), ctx.DpToPx(8), ctx.DpToPx(8));

                        var editText = new TextInputEditText(textLayout.Context)
                        {
                            InputType = InputTypes.ClassText | InputTypes.TextVariationPassword,
                            Text = field.GetRawValue()?.ToString() ?? string.Empty
                        };
                        editText.SetTextColor(Color.Rgb(28, 27, 31));
                        textLayout.AddView(editText);

                        _saveActions.Add(() => field.SetRawValue(editText.Text));
                        return textLayout;
                    }

                case FieldType.Number:
                    {
                        var textLayout = new TextInputLayout(ctx)
                        {
                            BoxBackgroundMode = TextInputLayout.BoxBackgroundOutline
                        };
                        textLayout.SetBoxCornerRadii(ctx.DpToPx(8), ctx.DpToPx(8), ctx.DpToPx(8), ctx.DpToPx(8));

                        var editText = new TextInputEditText(textLayout.Context)
                        {
                            InputType = InputTypes.ClassNumber | InputTypes.NumberFlagDecimal | InputTypes.NumberFlagSigned,
                            Text = field.GetRawValue()?.ToString() ?? "0"
                        };
                        editText.SetTextColor(Color.Rgb(28, 27, 31));
                        textLayout.AddView(editText);

                        _saveActions.Add(() =>
                        {
                            if (decimal.TryParse(editText.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out var num))
                            {
                                field.SetRawValue(num);
                            }
                        });
                        return textLayout;
                    }

                case FieldType.ComboBox:
                    {
                        var textLayout = new TextInputLayout(ctx)
                        {
                            BoxBackgroundMode = TextInputLayout.BoxBackgroundOutline
                        };
                        textLayout.SetBoxCornerRadii(ctx.DpToPx(8), ctx.DpToPx(8), ctx.DpToPx(8), ctx.DpToPx(8));

                        var autoTv = new MaterialAutoCompleteTextView(textLayout.Context);
                        autoTv.SetTextColor(Color.Rgb(28, 27, 31));

                        if (field is ComboBoxField cbField)
                        {
                            var stringItems = cbField.Items?.Select(x => x?.ToString() ?? string.Empty).ToList() ?? new List<string>();
                            var adapter = new ArrayAdapter<string>(ctx, Android.Resource.Layout.SimpleDropDownItem1Line, stringItems);
                            autoTv.Adapter = adapter;
                            autoTv.Text = cbField.Value?.ToString() ?? string.Empty;
                        }

                        textLayout.AddView(autoTv);
                        _saveActions.Add(() => field.SetRawValue(autoTv.Text));
                        return textLayout;
                    }

                case FieldType.SingleChoice:
                    {
                        var radioGroup = new RadioGroup(ctx) { Orientation = Orientation.Vertical };
                        if (field is SingleChoiceField scField && scField.Items != null)
                        {
                            var selectedObj = scField.Value;
                            foreach (var item in scField.Items)
                            {
                                var rb = new MaterialRadioButton(ctx)
                                {
                                    Id = Android.Views.View.GenerateViewId(),
                                    Text = item?.ToString() ?? string.Empty,
                                    Checked = Equals(item, selectedObj)
                                };
                                var captured = item;
                                rb.CheckedChange += (s, e) =>
                                {
                                    if (e.IsChecked)
                                    {
                                        scField.SetRawValue(captured);
                                    }
                                };
                                radioGroup.AddView(rb);
                            }
                        }
                        return radioGroup;
                    }

                case FieldType.MultipleChoice:
                    {
                        var multiBox = new LinearLayout(ctx) { Orientation = Orientation.Vertical };
                        if (field is MultipleChoiceField mcField && mcField.Items != null)
                        {
                            var selectedList = mcField.Value as IEnumerable<object> ?? Array.Empty<object>();
                            var selectedSet = new HashSet<object>(selectedList);
                            var checkBoxes = new List<(MaterialCheckBox Cb, object Item)>();

                            foreach (var item in mcField.Items)
                            {
                                var cb = new MaterialCheckBox(ctx)
                                {
                                    Text = item?.ToString() ?? string.Empty,
                                    Checked = selectedSet.Contains(item)
                                };
                                checkBoxes.Add((cb, item));
                                multiBox.AddView(cb);
                            }

                            _saveActions.Add(() =>
                            {
                                var chosen = checkBoxes.Where(x => x.Cb.Checked).Select(x => x.Item).ToList();
                                mcField.SetRawValue(chosen);
                            });
                        }
                        return multiBox;
                    }

                case FieldType.DateTime:
                    {
                        var btnDate = new Button(ctx)
                        {
                            Text = field.GetRawValue() is DateTime dt ? dt.ToString("yyyy-MM-dd HH:mm") : DateTime.Now.ToString("yyyy-MM-dd HH:mm")
                        };
                        var currentDate = field.GetRawValue() is DateTime d ? d : DateTime.Now;

                        btnDate.Click += (s, e) =>
                        {
                            var dpd = new Android.App.DatePickerDialog(ctx, (sender, args) =>
                            {
                                currentDate = new DateTime(args.Year, args.Month + 1, args.DayOfMonth, currentDate.Hour, currentDate.Minute, 0);
                                btnDate.Text = currentDate.ToString("yyyy-MM-dd HH:mm");
                            }, currentDate.Year, currentDate.Month - 1, currentDate.Day);
                            dpd.Show();
                        };

                        _saveActions.Add(() => field.SetRawValue(currentDate));
                        return btnDate;
                    }

                default:
                    return null;
            }
        }

        /// <summary>
        /// 将所有 UI 控件中的最新输入同步回 SheetField 中
        /// </summary>
        public void SaveAllValues()
        {
            foreach (var action in _saveActions)
            {
                action();
            }
        }
    }

    /// <summary>
    /// 多字段动态表单对话框，自动渲染复杂业务表单并驱动联动校验。
    /// </summary>
    public class ApedroidDataEntrySheetDialog : ApedroidDialog<DataEntrySheet>
    {
        private readonly DataEntrySheetDialogSettings _settings;
        private readonly DataEntrySheet _sheet;
        private readonly DataEntryView _dataEntryView;

        public ApedroidDataEntrySheetDialog(
            DataEntrySheet sheet,
            DataEntrySheetDialogSettings settings,
            Context? context = null)
            : base(settings, context)
        {
            _sheet = sheet ?? throw new ArgumentNullException(nameof(sheet));
            _settings = settings;

            var ctx = Context!;
            _dataEntryView = new DataEntryView(ctx);
            _dataEntryView.BindSheet(sheet);

            var scrollParams = new ViewGroup.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                Math.Min(ctx.DpToPx(380), ViewGroup.LayoutParams.WrapContent)
            );
            _dataEntryView.LayoutParameters = scrollParams;

            Host.SetContentView(_dataEntryView);
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
            _dataEntryView.SaveAllValues();

            // 针对每个字段执行校验
            foreach (var field in _sheet.Fields)
            {
                var vr = field.Validate();
                if (!vr.IsSuccess)
                {
                    OnValidationFailed(vr);
                    return;
                }
            }

            TrySubmit(_sheet);
        }
    }
}
