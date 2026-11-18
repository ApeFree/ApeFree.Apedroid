using System;
using System.Collections.Generic;
using Android.Graphics;
using System.Linq;
using Android.Content;
using Android.Views;
using Android.Widget;
using AndroidX.Core.Widget;
using ApeFree.ApeDialogs.Settings;
using ApeFree.Apedroid.Dialogs.Adapters;
using ApeFree.Apedroid.Dialogs.Core;
using ApeFree.Apedroid.Dialogs.Hosting;
using Google.Android.Material.CheckBox;
using Google.Android.Material.RadioButton;

namespace ApeFree.Apedroid.Dialogs.Views
{
    /// <summary>
    /// 单选列表对话框，采用 Material 单选按钮组渲染。
    /// </summary>
    /// <typeparam name="T">选项元素类型</typeparam>
    public class ApedroidSelectionDialog<T> : ApedroidDialog<T>
    {
        private readonly SelectionDialogSettings<T> _settings;
        private readonly List<T> _items;
        private T _selectedItem;

        public ApedroidSelectionDialog(
            SelectionDialogSettings<T> settings,
            IEnumerable<T> collection,
            T defaultSelectedItem = default!,
            Context? context = null)
            : base(settings, context)
        {
            _settings = settings;
            _items = collection?.ToList() ?? new List<T>();
            _selectedItem = defaultSelectedItem;

            var ctx = Context!;
            var scrollView = new NestedScrollView(ctx)
            {
                VerticalScrollBarEnabled = false,
                HorizontalScrollBarEnabled = false
            };

            var radioGroup = new RadioGroup(ctx)
            {
                Orientation = Orientation.Vertical
            };

            for (int i = 0; i < _items.Count; i++)
            {
                var item = _items[i];
                var displayText = settings.ItemDisplayConverter != null
                    ? settings.ItemDisplayConverter(item)
                    : item?.ToString() ?? string.Empty;

                var rb = new MaterialRadioButton(ctx)
                {
                    Id = Android.Views.View.GenerateViewId(),
                    Text = displayText,
                    TextSize = 15
                };
                bool isNight = (ctx.Resources?.Configuration?.UiMode & Android.Content.Res.UiMode.NightMask) == Android.Content.Res.UiMode.NightYes;
                rb.SetTextColor(isNight ? Color.Rgb(240, 240, 245) : Color.Rgb(28, 27, 31));
                int paddingV = ctx.DpToPx(8);
                rb.SetPadding(ctx.DpToPx(8), paddingV, ctx.DpToPx(8), paddingV);

                if (Equals(item, defaultSelectedItem))
                {
                    rb.Checked = true;
                    _selectedItem = item;
                }

                int index = i;
                rb.CheckedChange += (s, e) =>
                {
                    if (e.IsChecked)
                    {
                        _selectedItem = _items[index];
                    }
                };

                radioGroup.AddView(rb);
            }

            var scrollParams = new ViewGroup.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                Math.Min(ctx.DpToPx(300), ViewGroup.LayoutParams.WrapContent)
            );
            scrollView.LayoutParameters = scrollParams;
            scrollView.AddView(radioGroup);

            Host.SetContentView(scrollView);
            AddDefaultOptions();
        }

        public override OptionButton AddOption(DialogOption dialogOption)
        {
            var button = base.AddOption(dialogOption);
            if (ReferenceEquals(dialogOption, _settings.ConfirmOption))
            {
                button.Click += (_, _) => TrySubmit(_selectedItem);
            }
            return button;
        }

        protected override void OnConfirmRequested() => TrySubmit(_selectedItem);
    }

    /// <summary>
    /// 多选列表对话框，采用 Material 复选框列表渲染。
    /// </summary>
    /// <typeparam name="T">选项元素类型</typeparam>
    public class ApedroidMultipleSelectionDialog<T> : ApedroidDialog<IReadOnlyList<T>>
    {
        private readonly MultipleSelectionDialogSettings<T> _settings;
        private readonly List<T> _items;
        private readonly List<MaterialCheckBox> _checkBoxes = new();

        public ApedroidMultipleSelectionDialog(
            MultipleSelectionDialogSettings<T> settings,
            IEnumerable<T> collection,
            IEnumerable<T>? defaultSelectedItems = null,
            Context? context = null)
            : base(settings, context)
        {
            _settings = settings;
            _items = collection?.ToList() ?? new List<T>();

            var defaultSet = defaultSelectedItems != null
                ? new HashSet<T>(defaultSelectedItems)
                : new HashSet<T>();

            var ctx = Context!;
            var scrollView = new NestedScrollView(ctx)
            {
                VerticalScrollBarEnabled = false,
                HorizontalScrollBarEnabled = false
            };

            var layout = new LinearLayout(ctx)
            {
                Orientation = Orientation.Vertical
            };

            for (int i = 0; i < _items.Count; i++)
            {
                var item = _items[i];
                var displayText = settings.ItemDisplayConverter != null
                    ? settings.ItemDisplayConverter(item)
                    : item?.ToString() ?? string.Empty;

                var cb = new MaterialCheckBox(ctx)
                {
                    Text = displayText,
                    TextSize = 15,
                    Checked = defaultSet.Contains(item)
                };
                bool isNight = (ctx.Resources?.Configuration?.UiMode & Android.Content.Res.UiMode.NightMask) == Android.Content.Res.UiMode.NightYes;
                cb.SetTextColor(isNight ? Color.Rgb(240, 240, 245) : Color.Rgb(28, 27, 31));
                int paddingV = ctx.DpToPx(8);
                cb.SetPadding(ctx.DpToPx(8), paddingV, ctx.DpToPx(8), paddingV);

                _checkBoxes.Add(cb);
                layout.AddView(cb);
            }

            var scrollParams = new ViewGroup.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                Math.Min(ctx.DpToPx(300), ViewGroup.LayoutParams.WrapContent)
            );
            scrollView.LayoutParameters = scrollParams;
            scrollView.AddView(layout);

            Host.SetContentView(scrollView);
            AddDefaultOptions();
        }

        private List<T> GetSelectedItems()
        {
            var result = new List<T>();
            for (int i = 0; i < _items.Count; i++)
            {
                if (_checkBoxes[i].Checked)
                {
                    result.Add(_items[i]);
                }
            }
            return result;
        }

        public override OptionButton AddOption(DialogOption dialogOption)
        {
            var button = base.AddOption(dialogOption);
            if (ReferenceEquals(dialogOption, _settings.ConfirmOption))
            {
                button.Click += (_, _) => TrySubmit(GetSelectedItems());
            }
            return button;
        }

        protected override void OnConfirmRequested() => TrySubmit(GetSelectedItems());
    }
}
