using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Widget;
using AndroidX.Core.Widget;
using ApeFree.ApeDialogs.Settings;
using ApeFree.ApeDialogs.Theme;
using ApeFree.Apedroid.Dialogs.Adapters;
using ApeFree.Apedroid.Dialogs.Core;
using ApeFree.Apedroid.Dialogs.Hosting;
using Google.Android.Material.Slider;

namespace ApeFree.Apedroid.Dialogs.Views
{
    /// <summary>
    /// 日期与时间选择对话框，支持 Date、Time 及完整 DateTime 精度。
    /// </summary>
    public class ApedroidDateTimeDialog : ApedroidDialog<DateTime>
    {
        private readonly DateTimeDialogSettings _settings;
        private readonly DatePicker? _datePicker;
        private readonly TimePicker? _timePicker;

        public ApedroidDateTimeDialog(DateTimeDialogSettings settings, Context? context = null)
            : base(settings, context)
        {
            _settings = settings;

            var ctx = Context!;
            var container = new LinearLayout(ctx)
            {
                Orientation = Orientation.Vertical
            };

            var defaultDt = settings.DefaultDateTime;

            if (settings.Precision == DateTimePrecision.Date || settings.Precision == DateTimePrecision.DateTime)
            {
                _datePicker = new DatePicker(ctx)
                {
                    CalendarViewShown = false
                };
                _datePicker.Init(defaultDt.Year, defaultDt.Month - 1, defaultDt.Day, null);
                container.AddView(_datePicker, new LinearLayout.LayoutParams(
                    ViewGroup.LayoutParams.MatchParent,
                    ViewGroup.LayoutParams.WrapContent
                ));
            }

            if (settings.Precision == DateTimePrecision.Time || settings.Precision == DateTimePrecision.DateTime)
            {
                _timePicker = new TimePicker(ctx)
                {
                    Hour = defaultDt.Hour,
                    Minute = defaultDt.Minute
                };
                _timePicker.SetIs24HourView(Java.Lang.Boolean.True);
                container.AddView(_timePicker, new LinearLayout.LayoutParams(
                    ViewGroup.LayoutParams.MatchParent,
                    ViewGroup.LayoutParams.WrapContent
                ));
            }

            Host.SetContentView(container);
            AddDefaultOptions();
        }

        private DateTime GetSelectedDateTime()
        {
            int year = _datePicker?.Year ?? _settings.DefaultDateTime.Year;
            int month = (_datePicker?.Month ?? (_settings.DefaultDateTime.Month - 1)) + 1;
            int day = _datePicker?.DayOfMonth ?? _settings.DefaultDateTime.Day;

            int hour = _timePicker?.Hour ?? _settings.DefaultDateTime.Hour;
            int minute = _timePicker?.Minute ?? _settings.DefaultDateTime.Minute;

            return new DateTime(year, month, day, hour, minute, 0);
        }

        public override OptionButton AddOption(DialogOption dialogOption)
        {
            var button = base.AddOption(dialogOption);
            if (ReferenceEquals(dialogOption, _settings.ConfirmOption))
            {
                button.Click += (_, _) => OnConfirmRequested();
            }
            else if (ReferenceEquals(dialogOption, _settings.CurrentTimeOption))
            {
                button.Click += (_, _) =>
                {
                    var now = DateTime.Now;
                    if (_datePicker != null)
                    {
                        _datePicker.UpdateDate(now.Year, now.Month - 1, now.Day);
                    }
                    if (_timePicker != null)
                    {
                        _timePicker.Hour = now.Hour;
                        _timePicker.Minute = now.Minute;
                    }
                };
            }
            return button;
        }

        protected override void OnConfirmRequested() => TrySubmit(GetSelectedDateTime());
    }

    /// <summary>
    /// 颜色选择对话框，包含实时色块预览、RGB/Alpha 调节滑块与快捷预设调色盘。
    /// </summary>
    public class ApedroidColorDialog : ApedroidDialog<ApeColor>
    {
        private readonly ColorDialogSettings _settings;
        private readonly View _previewBox;
        private readonly TextView _tvHex;
        private readonly Slider _sliderR;
        private readonly Slider _sliderG;
        private readonly Slider _sliderB;
        private readonly Slider? _sliderA;

        private byte _r;
        private byte _g;
        private byte _b;
        private byte _a;

        public ApedroidColorDialog(ColorDialogSettings settings, Context? context = null)
            : base(settings, context)
        {
            _settings = settings;
            _r = settings.DefaultColor.R;
            _g = settings.DefaultColor.G;
            _b = settings.DefaultColor.B;
            _a = settings.AllowAlpha ? settings.DefaultColor.A : (byte)255;

            var ctx = Context!;
            var container = new LinearLayout(ctx)
            {
                Orientation = Orientation.Vertical
            };

            // 1. 颜色实时预览框与 HEX 标签
            var previewRow = new LinearLayout(ctx)
            {
                Orientation = Orientation.Horizontal
            };
            previewRow.SetGravity(GravityFlags.CenterVertical);

            _previewBox = new View(ctx);
            var boxDrawable = new GradientDrawable();
            boxDrawable.SetCornerRadius(ctx.DpToPx(8));
            _previewBox.Background = boxDrawable;
            var boxParams = new LinearLayout.LayoutParams(ctx.DpToPx(60), ctx.DpToPx(36));
            previewRow.AddView(_previewBox, boxParams);

            _tvHex = new TextView(ctx)
            {
                TextSize = 16,
                Typeface = Typeface.DefaultBold
            };
            _tvHex.SetTextColor(Color.Rgb(28, 27, 31));
            var hexParams = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.WrapContent,
                ViewGroup.LayoutParams.WrapContent
            );
            hexParams.SetMargins(ctx.DpToPx(16), 0, 0, 0);
            previewRow.AddView(_tvHex, hexParams);

            container.AddView(previewRow);

            // 2. 快捷预设色盘
            var presets = settings.PresetColors?.Count > 0
                ? settings.PresetColors
                : new List<ApeColor>
                {
                    ApeColor.Red, ApeColor.Orange, ApeColor.Yellow, ApeColor.Green,
                    ApeColor.Blue, ApeColor.DodgerBlue, ApeColor.Black, ApeColor.Gray, ApeColor.White
                };

            var presetScroll = new HorizontalScrollView(ctx)
            {
                HorizontalScrollBarEnabled = false
            };
            var presetRow = new LinearLayout(ctx) { Orientation = Orientation.Horizontal };
            foreach (var preset in presets)
            {
                var swatch = new View(ctx);
                var swatchDrawable = new GradientDrawable();
                swatchDrawable.SetCornerRadius(ctx.DpToPx(6));
                swatchDrawable.SetColor(preset.ToColor());
                swatchDrawable.SetStroke(ctx.DpToPx(1), Color.Rgb(200, 200, 200));
                swatch.Background = swatchDrawable;

                var swatchParams = new LinearLayout.LayoutParams(ctx.DpToPx(28), ctx.DpToPx(28));
                swatchParams.SetMargins(0, ctx.DpToPx(10), ctx.DpToPx(8), ctx.DpToPx(10));
                swatch.LayoutParameters = swatchParams;

                swatch.Click += (s, e) =>
                {
                    _r = preset.R;
                    _g = preset.G;
                    _b = preset.B;
                    _sliderR.Value = _r;
                    _sliderG.Value = _g;
                    _sliderB.Value = _b;
                    UpdatePreview();
                };

                presetRow.AddView(swatch);
            }
            presetScroll.AddView(presetRow);
            container.AddView(presetScroll);

            // 3. RGB 滑块组
            _sliderR = CreateColorSlider(ctx, "R", _r, Color.Rgb(230, 50, 50), val => { _r = (byte)val; UpdatePreview(); }, container);
            _sliderG = CreateColorSlider(ctx, "G", _g, Color.Rgb(40, 160, 40), val => { _g = (byte)val; UpdatePreview(); }, container);
            _sliderB = CreateColorSlider(ctx, "B", _b, Color.Rgb(40, 90, 220), val => { _b = (byte)val; UpdatePreview(); }, container);

            if (settings.AllowAlpha)
            {
                _sliderA = CreateColorSlider(ctx, "A", _a, Color.Rgb(120, 120, 120), val => { _a = (byte)val; UpdatePreview(); }, container);
            }

            UpdatePreview();

            Host.SetContentView(container);
            AddDefaultOptions();
        }

        private Slider CreateColorSlider(Context ctx, string label, byte initialValue, Color tintColor, Action<float> onValChanged, LinearLayout parent)
        {
            var row = new LinearLayout(ctx)
            {
                Orientation = Orientation.Horizontal
            };
            row.SetGravity(GravityFlags.CenterVertical);

            var tv = new TextView(ctx)
            {
                Text = label,
                TextSize = 14,
                Typeface = Typeface.DefaultBold
            };
            tv.SetTextColor(tintColor);
            row.AddView(tv, new LinearLayout.LayoutParams(ctx.DpToPx(20), ViewGroup.LayoutParams.WrapContent));

            var slider = new Slider(ctx)
            {
                ValueFrom = 0f,
                ValueTo = 255f,
                StepSize = 1f,
                Value = initialValue
            };

            slider.AddOnChangeListener(new SliderListener((s, val, fromUser) => onValChanged(val)));

            var sliderParams = new LinearLayout.LayoutParams(0, ViewGroup.LayoutParams.WrapContent, 1.0f);
            row.AddView(slider, sliderParams);

            parent.AddView(row);
            return slider;
        }

        private void UpdatePreview()
        {
            var currentColor = Color.Argb(_a, _r, _g, _b);
            var bg = (GradientDrawable)_previewBox.Background!;
            bg.SetColor(currentColor);
            bg.SetStroke(Context!.DpToPx(1), Color.Rgb(200, 200, 200));

            _tvHex.Text = _settings.AllowAlpha
                ? $"#{_a:X2}{_r:X2}{_g:X2}{_b:X2}"
                : $"#{_r:X2}{_g:X2}{_b:X2}";
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
            TrySubmit(new ApeColor(_a, _r, _g, _b));
        }

        private class SliderListener : Java.Lang.Object, Slider.IOnChangeListener
        {
            private readonly Action<Slider, float, bool> _callback;

            public SliderListener(Action<Slider, float, bool> callback)
            {
                _callback = callback;
            }

            public void OnValueChange(Slider slider, float value, bool fromUser)
            {
                _callback(slider, value, fromUser);
            }
        }
    }
}
