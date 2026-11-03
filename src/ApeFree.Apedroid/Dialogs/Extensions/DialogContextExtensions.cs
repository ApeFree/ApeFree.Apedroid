using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Android.Content;
using ApeFree.ApeDialogs.Core;
using ApeFree.ApeDialogs.Settings;
using ApeFree.ApeDialogs.Theme;
using ApeFree.Apedroid.Dialogs.Providers;

namespace ApeFree.Apedroid.Dialogs.Extensions
{
    /// <summary>
    /// Android Context 与全局弹窗的便捷扩展方法集合。
    /// </summary>
    public static class DialogContextExtensions
    {
        private static ApedroidDialogProvider Provider => ApedroidDialogProvider.Default;

        #region 全局无 Context 捷径（自动回退到 Top Activity）

        /// <summary>
        /// 全局弹出消息对话框
        /// </summary>
        public static Task<DialogResult<bool>> ShowMessageAsync(string title, string content, Action<MessageDialogSettings>? configure = null, CancellationToken cancellationToken = default)
        {
            var settings = new MessageDialogSettings { Title = title, Content = content };
            configure?.Invoke(settings);
            return Provider.CreateMessageDialog(settings).ShowAsync(cancellationToken);
        }

        /// <summary>
        /// 全局弹出询问确认对话框 (Yes/No)
        /// </summary>
        public static Task<DialogResult<bool>> ShowPromptAsync(string title, string content, Action<PromptDialogSettings>? configure = null, CancellationToken cancellationToken = default)
        {
            var settings = new PromptDialogSettings { Title = title, Content = content };
            configure?.Invoke(settings);
            return Provider.CreatePromptDialog(settings).ShowAsync(cancellationToken);
        }

        /// <summary>
        /// 全局弹出单行/多行文本输入对话框
        /// </summary>
        public static Task<DialogResult<string>> ShowInputAsync(string title, string content = "", string defaultText = "", Action<InputDialogSettings>? configure = null, CancellationToken cancellationToken = default)
        {
            var settings = new InputDialogSettings { Title = title, Content = content, DefaultText = defaultText };
            configure?.Invoke(settings);
            return Provider.CreateInputDialog(settings).ShowAsync(cancellationToken);
        }

        /// <summary>
        /// 全局弹出密码输入对话框
        /// </summary>
        public static Task<DialogResult<string>> ShowPasswordAsync(string title, string content = "", Action<PasswordDialogSettings>? configure = null, CancellationToken cancellationToken = default)
        {
            var settings = new PasswordDialogSettings { Title = title, Content = content };
            configure?.Invoke(settings);
            return Provider.CreatePasswordDialog(settings).ShowAsync(cancellationToken);
        }

        /// <summary>
        /// 全局弹出 Toast 轻提示
        /// </summary>
        public static Task<DialogResult<bool>> ShowToastAsync(string content, TimeSpan? duration = null, CancellationToken cancellationToken = default)
        {
            var settings = new ToastDialogSettings
            {
                Title = string.Empty,
                Content = content,
                Duration = duration ?? TimeSpan.FromSeconds(2.5)
            };
            return Provider.CreateToastDialog(settings).ShowAsync(cancellationToken);
        }

        /// <summary>
        /// Context 弹出 Toast 轻提示
        /// </summary>
        public static Task<DialogResult<bool>> ShowToastAsync(this Context context, string content, TimeSpan? duration = null, CancellationToken cancellationToken = default)
        {
            var settings = new ToastDialogSettings
            {
                Title = string.Empty,
                Content = content,
                Duration = duration ?? TimeSpan.FromSeconds(2.5)
            };
            return Provider.CreateToastDialog(settings, context).ShowAsync(cancellationToken);
        }

        #endregion

        #region Context 扩展方法

        /// <summary>
        /// 弹出消息对话框
        /// </summary>
        public static Task<DialogResult<bool>> ShowMessageDialogAsync(this Context context, string title, string content, Action<MessageDialogSettings>? configure = null, CancellationToken cancellationToken = default)
        {
            var settings = new MessageDialogSettings { Title = title, Content = content };
            configure?.Invoke(settings);
            return Provider.CreateMessageDialog(settings, context).ShowAsync(cancellationToken);
        }

        /// <summary>
        /// 弹出询问提示对话框
        /// </summary>
        public static Task<DialogResult<bool>> ShowPromptDialogAsync(this Context context, string title, string content, Action<PromptDialogSettings>? configure = null, CancellationToken cancellationToken = default)
        {
            var settings = new PromptDialogSettings { Title = title, Content = content };
            configure?.Invoke(settings);
            return Provider.CreatePromptDialog(settings, context).ShowAsync(cancellationToken);
        }

        /// <summary>
        /// 弹出文本输入对话框
        /// </summary>
        public static Task<DialogResult<string>> ShowInputDialogAsync(this Context context, string title, string content = "", string defaultText = "", Action<InputDialogSettings>? configure = null, CancellationToken cancellationToken = default)
        {
            var settings = new InputDialogSettings { Title = title, Content = content, DefaultText = defaultText };
            configure?.Invoke(settings);
            return Provider.CreateInputDialog(settings, context).ShowAsync(cancellationToken);
        }

        /// <summary>
        /// 弹出密码输入对话框
        /// </summary>
        public static Task<DialogResult<string>> ShowPasswordDialogAsync(this Context context, string title, string content = "", Action<PasswordDialogSettings>? configure = null, CancellationToken cancellationToken = default)
        {
            var settings = new PasswordDialogSettings { Title = title, Content = content };
            configure?.Invoke(settings);
            return Provider.CreatePasswordDialog(settings, context).ShowAsync(cancellationToken);
        }

        /// <summary>
        /// 弹出单选对话框
        /// </summary>
        public static Task<DialogResult<T>> ShowSelectionDialogAsync<T>(this Context context, string title, IEnumerable<T> collection, T defaultSelected = default!, Action<SelectionDialogSettings<T>>? configure = null, CancellationToken cancellationToken = default)
        {
            var settings = new SelectionDialogSettings<T> { Title = title };
            configure?.Invoke(settings);
            return Provider.CreateSelectionDialog(settings, collection, defaultSelected, context).ShowAsync(cancellationToken);
        }

        /// <summary>
        /// 弹出多选对话框
        /// </summary>
        public static Task<DialogResult<IReadOnlyList<T>>> ShowMultipleSelectionDialogAsync<T>(this Context context, string title, IEnumerable<T> collection, IEnumerable<T>? defaultSelected = null, Action<MultipleSelectionDialogSettings<T>>? configure = null, CancellationToken cancellationToken = default)
        {
            var settings = new MultipleSelectionDialogSettings<T> { Title = title };
            configure?.Invoke(settings);
            return Provider.CreateMultipleSelectionDialog(settings, collection, defaultSelected, context).ShowAsync(cancellationToken);
        }

        /// <summary>
        /// 弹出数值输入对话框
        /// </summary>
        public static Task<DialogResult<decimal>> ShowNumberInputDialogAsync(this Context context, string title, decimal defaultValue = 0, decimal min = 0, decimal max = 100, Action<NumberInputDialogSettings>? configure = null, CancellationToken cancellationToken = default)
        {
            var settings = new NumberInputDialogSettings
            {
                Title = title,
                DefaultValue = defaultValue,
                Minimum = min,
                Maximum = max
            };
            configure?.Invoke(settings);
            return Provider.CreateNumberInputDialog(settings, context).ShowAsync(cancellationToken);
        }

        /// <summary>
        /// 弹出颜色选择对话框
        /// </summary>
        public static Task<DialogResult<ApeColor>> ShowColorDialogAsync(this Context context, string title, ApeColor defaultColor = default, Action<ColorDialogSettings>? configure = null, CancellationToken cancellationToken = default)
        {
            var settings = new ColorDialogSettings { Title = title, DefaultColor = defaultColor };
            configure?.Invoke(settings);
            return Provider.CreateColorDialog(settings, context).ShowAsync(cancellationToken);
        }

        /// <summary>
        /// 弹出日期时间选择对话框
        /// </summary>
        public static Task<DialogResult<DateTime>> ShowDateTimeDialogAsync(this Context context, string title, DateTime? defaultDt = null, DateTimePrecision precision = DateTimePrecision.DateTime, Action<DateTimeDialogSettings>? configure = null, CancellationToken cancellationToken = default)
        {
            var settings = new DateTimeDialogSettings
            {
                Title = title,
                DefaultDateTime = defaultDt ?? DateTime.Now,
                Precision = precision
            };
            configure?.Invoke(settings);
            return Provider.CreateDateTimeDialog(settings, context).ShowAsync(cancellationToken);
        }

        /// <summary>
        /// 弹出长文本查看对话框
        /// </summary>
        public static Task<DialogResult<bool>> ShowTextDialogAsync(this Context context, string title, string text, Action<TextDialogSettings>? configure = null, CancellationToken cancellationToken = default)
        {
            var settings = new TextDialogSettings { Title = title, Text = text };
            configure?.Invoke(settings);
            return Provider.CreateTextDialog(settings, context).ShowAsync(cancellationToken);
        }

        /// <summary>
        /// 弹出动态表单对话框
        /// </summary>
        public static Task<DialogResult<DataEntrySheet>> ShowDataEntrySheetDialogAsync(this Context context, string title, DataEntrySheet sheet, Action<DataEntrySheetDialogSettings>? configure = null, CancellationToken cancellationToken = default)
        {
            var settings = new DataEntrySheetDialogSettings { Title = title };
            configure?.Invoke(settings);
            return Provider.CreateDataEntrySheetDialog(sheet, settings, context).ShowAsync(cancellationToken);
        }

        #endregion
    }
}
