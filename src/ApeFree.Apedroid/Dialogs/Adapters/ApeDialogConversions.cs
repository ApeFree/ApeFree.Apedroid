using System;
using Android.Content;
using Android.Graphics;
using Android.Util;
using ApeFree.ApeDialogs.Theme;

namespace ApeFree.Apedroid.Dialogs.Adapters
{
    /// <summary>
    /// ApeDialogs 与 Android 原生图形属性相互转换的工具集。
    /// </summary>
    public static class ApeDialogConversions
    {
        /// <summary>
        /// 将 ApeColor 转换为 Android 原生 Color。
        /// </summary>
        public static Color ToColor(this ApeColor apeColor)
        {
            return new Color(apeColor.R, apeColor.G, apeColor.B, apeColor.A);
        }

        /// <summary>
        /// 将 Android 原生 Color 转换为 ApeColor。
        /// </summary>
        public static ApeColor ToApeColor(this Color color)
        {
            return ApeColor.FromArgb(color.A, color.R, color.G, color.B);
        }

        /// <summary>
        /// 根据背景色自动计算高对比度的前景色（黑色或白色）。
        /// </summary>
        public static Color GetContrastTextColor(this Color bg)
        {
            // 亮度计算公式 (Luminance = 0.299*R + 0.587*G + 0.114*B)
            double luminance = (0.299 * bg.R + 0.587 * bg.G + 0.114 * bg.B) / 255.0;
            return luminance > 0.55 ? Color.Rgb(33, 33, 33) : Color.White;
        }

        /// <summary>
        /// 将 dp 单位转为 px 物理像素。
        /// </summary>
        public static int DpToPx(this Context context, float dp)
        {
            var metrics = context.Resources?.DisplayMetrics ?? Android.Content.Res.Resources.System.DisplayMetrics!;
            return (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, dp, metrics);
        }

        /// <summary>
        /// 将 px 物理像素转为 dp。
        /// </summary>
        public static float PxToDp(this Context context, int px)
        {
            var metrics = context.Resources?.DisplayMetrics ?? Android.Content.Res.Resources.System.DisplayMetrics!;
            return px / metrics.Density;
        }

        /// <summary>
        /// 获取 DialogFont 对应的 Typeface。
        /// </summary>
        public static Typeface ToTypeface(this DialogFont font)
        {
            var style = TypefaceStyle.Normal;
            if (font.Weight == FontWeight.Bold && font.Italic)
            {
                style = TypefaceStyle.BoldItalic;
            }
            else if (font.Weight == FontWeight.Bold)
            {
                style = TypefaceStyle.Bold;
            }
            else if (font.Italic)
            {
                style = TypefaceStyle.Italic;
            }

            if (string.IsNullOrWhiteSpace(font.Family))
            {
                return Typeface.Create(Typeface.Default, style);
            }

            return Typeface.Create(font.Family, style);
        }
    }
}
