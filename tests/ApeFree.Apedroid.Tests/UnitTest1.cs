using System;
using System.Collections.Generic;
using Android.Graphics;
using ApeFree.ApeDialogs;
using ApeFree.ApeDialogs.Settings;
using ApeFree.ApeDialogs.Theme;
using ApeFree.Apedroid.Dialogs.Adapters;
using ApeFree.Apedroid.Dialogs.Providers;
using Xunit;

namespace ApeFree.Apedroid.Tests
{
    public class ApedroidTests
    {
        [Fact]
        public void ColorConversion_Roundtrip_Succeeds()
        {
            var original = ApeColor.FromArgb(200, 100, 150, 220);
            var androidColor = original.ToColor();

            Assert.Equal(original.A, androidColor.A);
            Assert.Equal(original.R, androidColor.R);
            Assert.Equal(original.G, androidColor.G);
            Assert.Equal(original.B, androidColor.B);

            var roundtrip = androidColor.ToApeColor();
            Assert.Equal(original, roundtrip);
        }

        [Fact]
        public void ContrastTextColor_BrightBackground_ReturnsDarkText()
        {
            var whiteBg = Color.White;
            var textColor = whiteBg.GetContrastTextColor();

            // 浅色背景应返回深色字体
            Assert.Equal(Color.Rgb(33, 33, 33), textColor);
        }

        [Fact]
        public void ContrastTextColor_DarkBackground_ReturnsWhiteText()
        {
            var blackBg = Color.Black;
            var textColor = blackBg.GetContrastTextColor();

            // 深色背景应返回白色字体
            Assert.Equal(Color.White, textColor);
        }

        [Fact]
        public void FontConversion_CreatesValidTypeface()
        {
            var font = new DialogFont("sans-serif", 16f, FontWeight.Bold, true);
            var typeface = font.ToTypeface();

            Assert.NotNull(typeface);
        }

        [Fact]
        public void DialogProvider_ImplementsStandardInterface()
        {
            var provider = ApedroidDialogProvider.Default;

            Assert.NotNull(provider);
            Assert.IsAssignableFrom<IDialogProvider<Android.Content.Context>>(provider);
            Assert.NotNull(provider.Theme);
        }

        [Fact]
        public void DataEntrySheet_AddFieldAndValidate_Works()
        {
            var sheet = new DataEntrySheet();
            var field1 = new TextField { Title = "用户名", Value = "admin" };
            var field2 = new NumberField { Title = "年龄", Value = 25 };

            sheet.AddField(field1);
            sheet.AddField(field2);

            Assert.Equal(2, sheet.Fields.Count);
            Assert.Equal("admin", sheet["用户名"]?.GetRawValue());
            Assert.Equal(25m, sheet["年龄"]?.GetRawValue());

            var v1 = field1.Validate();
            var v2 = field2.Validate();
            Assert.True(v1.IsSuccess);
            Assert.True(v2.IsSuccess);
        }
    }
}
