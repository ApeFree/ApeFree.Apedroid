using System;
using System.Globalization;
using Android.Content;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using ApeFree.ApeDialogs.Core;
using ApeFree.ApeDialogs.Settings;
using ApeFree.Apedroid.Dialogs.Adapters;
using ApeFree.Apedroid.Dialogs.Core;
using ApeFree.Apedroid.Dialogs.Hosting;
using Google.Android.Material.Slider;

namespace ApeFree.Apedroid.Dialogs.Views
{
    /// <summary>
    /// 单滑块实时调节对话框，支持推流与实时标签更新。
    /// </summary>
    public class ApedroidSliderDialog : ApedroidAdjustDialog<decimal>
    {
        private readonly SliderDialogSettings _settings;
        private readonly Slider _slider;
        private readonly TextView _tvValue;
        private decimal _currentValue;

        public override decimal Value => _currentValue;

        public ApedroidSliderDialog(SliderDialogSettings settings, Context? context = null)
            : base(settings, context)
        {
            _settings = settings;
            _currentValue = Math.Max(settings.Minimum, Math.Min(settings.Maximum, settings.DefaultValue));

            var ctx = Context!;
            var container = new LinearLayout(ctx)
            {
                Orientation = Orientation.Vertical
            };

            _tvValue = new TextView(ctx)
            {
                TextSize = 16,
                Typeface = Typeface.DefaultBold,
                Visibility = settings.ShowValueLabel ? ViewStates.Visible : ViewStates.Gone
            };
            _tvValue.Gravity = GravityFlags.Center;
            _tvValue.SetTextColor(Color.Rgb(28, 27, 31));
            UpdateLabel();
            container.AddView(_tvValue, new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent
            ));

            _slider = new Slider(ctx)
            {
                ValueFrom = (float)settings.Minimum,
                ValueTo = (float)Math.Max((float)settings.Minimum + 0.001f, (float)settings.Maximum),
                Value = (float)_currentValue
            };

            if (settings.TickFrequency > 0 && settings.TickFrequency <= (settings.Maximum - settings.Minimum))
            {
                _slider.StepSize = (float)settings.TickFrequency;
            }

            _slider.AddOnChangeListener(new SliderChangeListener((s, val, fromUser) =>
            {
                _currentValue = (decimal)val;
                UpdateLabel();
                RaiseValueChanged(_currentValue);
            }));

            var sliderParams = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent
            );
            sliderParams.SetMargins(0, ctx.DpToPx(8), 0, ctx.DpToPx(8));
            container.AddView(_slider, sliderParams);

            Host.SetContentView(container);
            AddDefaultOptions();
        }

        private void UpdateLabel()
        {
            _tvValue.Text = _currentValue.ToString("0.##", CultureInfo.InvariantCulture);
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

        protected override void OnConfirmRequested() => TrySubmit(_currentValue);

        private class SliderChangeListener : Java.Lang.Object, Slider.IOnChangeListener
        {
            private readonly Action<Slider, float, bool> _callback;

            public SliderChangeListener(Action<Slider, float, bool> callback)
            {
                _callback = callback;
            }

            public void OnValueChange(Slider slider, float value, bool fromUser)
            {
                _callback(slider, value, fromUser);
            }
        }
    }

    /// <summary>
    /// 双滑块区间范围调节对话框。
    /// </summary>
    public class ApedroidRangeSliderDialog : ApedroidAdjustDialog<ValueRange<decimal>>
    {
        private readonly RangeSliderDialogSettings _settings;
        private readonly RangeSlider _rangeSlider;
        private readonly TextView _tvValue;
        private ValueRange<decimal> _currentRange;

        public override ValueRange<decimal> Value => _currentRange;

        public ApedroidRangeSliderDialog(RangeSliderDialogSettings settings, Context? context = null)
            : base(settings, context)
        {
            _settings = settings;
            _currentRange = new ValueRange<decimal>(
                Math.Max(settings.Minimum, Math.Min(settings.Maximum, settings.DefaultStart)),
                Math.Max(settings.Minimum, Math.Min(settings.Maximum, settings.DefaultEnd))
            );

            var ctx = Context!;
            var container = new LinearLayout(ctx)
            {
                Orientation = Orientation.Vertical
            };

            _tvValue = new TextView(ctx)
            {
                TextSize = 16,
                Typeface = Typeface.DefaultBold,
                Visibility = settings.ShowValueLabel ? ViewStates.Visible : ViewStates.Gone
            };
            _tvValue.Gravity = GravityFlags.Center;
            _tvValue.SetTextColor(Color.Rgb(28, 27, 31));
            UpdateLabel();
            container.AddView(_tvValue, new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent
            ));

            _rangeSlider = new RangeSlider(ctx)
            {
                ValueFrom = (float)settings.Minimum,
                ValueTo = (float)Math.Max((float)settings.Minimum + 0.001f, (float)settings.Maximum)
            };

            if (settings.TickFrequency > 0 && settings.TickFrequency <= (settings.Maximum - settings.Minimum))
            {
                _rangeSlider.StepSize = (float)settings.TickFrequency;
            }

            _rangeSlider.SetValues(Java.Lang.Float.ValueOf((float)_currentRange.Start), Java.Lang.Float.ValueOf((float)_currentRange.End));

            _rangeSlider.AddOnChangeListener(new RangeSliderChangeListener((s, val, fromUser) =>
            {
                var values = s.Values;
                if (values != null && values.Count >= 2)
                {
                    var start = (decimal)(float)values[0];
                    var end = (decimal)(float)values[1];
                    _currentRange = new ValueRange<decimal>(start, end);
                    UpdateLabel();
                    RaiseValueChanged(_currentRange);
                }
            }));

            var sliderParams = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent
            );
            sliderParams.SetMargins(0, ctx.DpToPx(8), 0, ctx.DpToPx(8));
            container.AddView(_rangeSlider, sliderParams);

            Host.SetContentView(container);
            AddDefaultOptions();
        }

        private void UpdateLabel()
        {
            _tvValue.Text = $"[{_currentRange.Start:0.##} ~ {_currentRange.End:0.##}]";
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

        protected override void OnConfirmRequested() => TrySubmit(_currentRange);

        private class RangeSliderChangeListener : Java.Lang.Object, RangeSlider.IOnChangeListener
        {
            private readonly Action<RangeSlider, float, bool> _callback;

            public RangeSliderChangeListener(Action<RangeSlider, float, bool> callback)
            {
                _callback = callback;
            }

            public void OnValueChange(RangeSlider slider, float value, bool fromUser)
            {
                _callback(slider, value, fromUser);
            }
        }
    }
}
