using System;
using System.Collections.Generic;
using Android.Content;
using ApeFree.ApeDialogs;
using ApeFree.ApeDialogs.Core;
using ApeFree.ApeDialogs.Settings;
using ApeFree.ApeDialogs.Theme;
using ApeFree.Apedroid.Dialogs.Views;

namespace ApeFree.Apedroid.Dialogs.Providers
{
    /// <summary>
    /// ApeDialogs 的 Android 原生 Material 3 实现提供器。
    /// 支持上下文为 <see cref="Context"/>（Activity 或应用 Context），
    /// 当 Context 为空时自动回退为当前活跃的 Top Activity。
    /// </summary>
    public class ApedroidDialogProvider : DialogProvider<Context>
    {
        private static readonly Lazy<ApedroidDialogProvider> _lazyDefault = new(() => new ApedroidDialogProvider());

        /// <summary>
        /// 默认单例提供器实例。
        /// </summary>
        public static ApedroidDialogProvider Default => _lazyDefault.Value;

        private readonly IDialogTheme _theme;

        public ApedroidDialogProvider() : this(DefaultTheme.Instance)
        {
        }

        public ApedroidDialogProvider(IDialogTheme theme)
        {
            _theme = theme ?? throw new ArgumentNullException(nameof(theme));
        }

        /// <inheritdoc/>
        public override IDialogTheme Theme => _theme;

        /// <inheritdoc/>
        public override IDialog<bool> CreateMessageDialog(MessageDialogSettings settings, Context? context = null)
            => new ApedroidMessageDialog(settings, context);

        /// <inheritdoc/>
        public override IDialog<bool> CreatePromptDialog(PromptDialogSettings settings, Context? context = null)
            => new ApedroidPromptDialog(settings, context);

        /// <inheritdoc/>
        public override IDialog<string> CreateInputDialog(InputDialogSettings settings, Context? context = null)
            => new ApedroidInputDialog(settings, context);

        /// <inheritdoc/>
        public override IDialog<string> CreatePasswordDialog(PasswordDialogSettings settings, Context? context = null)
            => new ApedroidPasswordDialog(settings, context);

        /// <inheritdoc/>
        public override IDialog<T> CreateSelectionDialog<T>(SelectionDialogSettings<T> settings, IEnumerable<T> collection, T defaultSelectedItem = default!, Context? context = null)
            => new ApedroidSelectionDialog<T>(settings, collection, defaultSelectedItem, context);

        /// <inheritdoc/>
        public override IDialog<IReadOnlyList<T>> CreateMultipleSelectionDialog<T>(MultipleSelectionDialogSettings<T> settings, IEnumerable<T> collection, IEnumerable<T>? defaultSelectedItems = null, Context? context = null)
            => new ApedroidMultipleSelectionDialog<T>(settings, collection, defaultSelectedItems, context);

        /// <inheritdoc/>
        public override IDialog<DateTime> CreateDateTimeDialog(DateTimeDialogSettings settings, Context? context = null)
            => new ApedroidDateTimeDialog(settings, context);

        /// <inheritdoc/>
        public override IDialog<DataEntrySheet> CreateDataEntrySheetDialog(DataEntrySheet sheet, DataEntrySheetDialogSettings settings, Context? context = null)
            => new ApedroidDataEntrySheetDialog(sheet, settings, context);

        /// <inheritdoc/>
        public override IDialog<IReadOnlyList<string>> CreateOpenFileDialog(OpenFileDialogSettings settings, Context? context = null)
            => new ApedroidOpenFileDialog(settings, context);

        /// <inheritdoc/>
        public override IDialog<IReadOnlyList<string>> CreateOpenFolderDialog(OpenFolderDialogSettings settings, Context? context = null)
            => new ApedroidOpenFolderDialog(settings, context);

        /// <inheritdoc/>
        public override IDialog<decimal> CreateNumberInputDialog(NumberInputDialogSettings settings, Context? context = null)
            => new ApedroidNumberInputDialog(settings, context);

        /// <inheritdoc/>
        public override IAdjustDialog<decimal> CreateSliderDialog(SliderDialogSettings settings, Context? context = null)
            => new ApedroidSliderDialog(settings, context);

        /// <inheritdoc/>
        public override IAdjustDialog<ValueRange<decimal>> CreateRangeSliderDialog(RangeSliderDialogSettings settings, Context? context = null)
            => new ApedroidRangeSliderDialog(settings, context);

        /// <inheritdoc/>
        public override IProgressDialog CreateProgressDialog(ProgressDialogSettings settings, Context? context = null)
            => new ApedroidProgressDialog(settings, context);

        /// <inheritdoc/>
        public override IDialog<bool> CreateToastDialog(ToastDialogSettings settings, Context? context = null)
            => new ApedroidToastDialog(settings, context);

        /// <inheritdoc/>
        public override IDialog<bool> CreateTextDialog(TextDialogSettings settings, Context? context = null)
            => new ApedroidTextDialog(settings, context);

        /// <inheritdoc/>
        public override IDialog<ApeColor> CreateColorDialog(ColorDialogSettings settings, Context? context = null)
            => new ApedroidColorDialog(settings, context);

        /// <inheritdoc/>
        public override IDialog<string> CreateSaveFileDialog(SaveFileDialogSettings settings, Context? context = null)
            => new ApedroidSaveFileDialog(settings, context);

        /// <inheritdoc/>
        public override IDialog<string> CreateComboBoxInputDialog(ComboBoxInputDialogSettings settings, Context? context = null)
            => new ApedroidComboBoxInputDialog(settings, context);
    }
}
