using System;
using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using ApeFree.ApeDialogs.Settings;
using ApeFree.Apedroid.Dialogs.Adapters;
using ApeFree.Apedroid.Dialogs.Core;
using ApeFree.Apedroid.Dialogs.Hosting;
using ApeFree.Apedroid.Dialogs.Saf;
using Google.Android.Material.Button;
using Google.Android.Material.TextField;

namespace ApeFree.Apedroid.Dialogs.Views
{
    /// <summary>
    /// 打开文件对话框，整合 Android 系统级 SAF 存储选择器与路径预览。
    /// </summary>
    public class ApedroidOpenFileDialog : ApedroidDialog<IReadOnlyList<string>>
    {
        private readonly OpenFileDialogSettings _settings;
        private readonly TextInputEditText _etPath;
        private readonly List<string> _selectedPaths = new();

        public ApedroidOpenFileDialog(OpenFileDialogSettings settings, Context? context = null)
            : base(settings, context)
        {
            _settings = settings;

            var ctx = Context!;
            var container = new LinearLayout(ctx)
            {
                Orientation = Orientation.Vertical
            };

            var btnBrowse = new MaterialButton(ctx)
            {
                Text = settings.MultiSelect ? "浏览选择文件 (可多选)" : "浏览选择文件",
                CornerRadius = ctx.DpToPx(12)
            };
            btnBrowse.SetBackgroundColor(Color.Rgb(230, 235, 245));
            btnBrowse.SetTextColor(Color.Rgb(30, 80, 180));
            var btnParams = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ctx.DpToPx(46)
            );
            btnParams.SetMargins(0, 0, 0, ctx.DpToPx(10));
            container.AddView(btnBrowse, btnParams);

            var textLayout = new TextInputLayout(ctx)
            {
                BoxBackgroundMode = TextInputLayout.BoxBackgroundOutline,
                Hint = "已选文件路径或 URI"
            };
            textLayout.SetBoxCornerRadii(ctx.DpToPx(8), ctx.DpToPx(8), ctx.DpToPx(8), ctx.DpToPx(8));

            _etPath = new TextInputEditText(textLayout.Context)
            {
                Text = settings.InitialPath ?? string.Empty
            };
            _etPath.SetTextColor(Color.Rgb(28, 27, 31));
            textLayout.AddView(_etPath);
            container.AddView(textLayout);

            btnBrowse.Click += async (s, e) =>
            {
                try
                {
                    var picked = await SafBridge.PickFilesAsync(ctx, settings.MultiSelect, "*/*");
                    if (picked != null && picked.Count > 0)
                    {
                        _selectedPaths.Clear();
                        _selectedPaths.AddRange(picked);
                        _etPath.Text = string.Join("\n", picked);
                    }
                }
                catch (Exception ex)
                {
                    Toast.MakeText(ctx, "文件选择异常: " + ex.Message, ToastLength.Short)?.Show();
                }
            };

            Host.SetContentView(container);
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
            if (_selectedPaths.Count > 0)
            {
                TrySubmit(_selectedPaths);
                return;
            }

            var text = _etPath.Text?.Trim();
            if (!string.IsNullOrEmpty(text))
            {
                var paths = text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                TrySubmit(paths);
            }
            else
            {
                TrySubmit(Array.Empty<string>());
            }
        }
    }

    /// <summary>
    /// 保存文件对话框，整合 SAF 文档创建与目标文件名确认。
    /// </summary>
    public class ApedroidSaveFileDialog : ApedroidDialog<string>
    {
        private readonly SaveFileDialogSettings _settings;
        private readonly TextInputEditText _etPath;
        private string? _chosenUri;

        public ApedroidSaveFileDialog(SaveFileDialogSettings settings, Context? context = null)
            : base(settings, context)
        {
            _settings = settings;

            var ctx = Context!;
            var container = new LinearLayout(ctx)
            {
                Orientation = Orientation.Vertical
            };

            var btnBrowse = new MaterialButton(ctx)
            {
                Text = "选择保存目标位置 (SAF)",
                CornerRadius = ctx.DpToPx(12)
            };
            btnBrowse.SetBackgroundColor(Color.Rgb(230, 235, 245));
            btnBrowse.SetTextColor(Color.Rgb(30, 80, 180));
            var btnParams = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ctx.DpToPx(46)
            );
            btnParams.SetMargins(0, 0, 0, ctx.DpToPx(10));
            container.AddView(btnBrowse, btnParams);

            var textLayout = new TextInputLayout(ctx)
            {
                BoxBackgroundMode = TextInputLayout.BoxBackgroundOutline,
                Hint = "保存文件名或目标 URI"
            };
            textLayout.SetBoxCornerRadii(ctx.DpToPx(8), ctx.DpToPx(8), ctx.DpToPx(8), ctx.DpToPx(8));

            _etPath = new TextInputEditText(textLayout.Context)
            {
                Text = settings.DefaultFileName ?? string.Empty
            };
            _etPath.SetTextColor(Color.Rgb(28, 27, 31));
            textLayout.AddView(_etPath);
            container.AddView(textLayout);

            btnBrowse.Click += async (s, e) =>
            {
                try
                {
                    string filename = _etPath.Text?.Trim() ?? "new_file";
                    var uri = await SafBridge.CreateFileAsync(ctx, filename, "*/*");
                    if (!string.IsNullOrEmpty(uri))
                    {
                        _chosenUri = uri;
                        _etPath.Text = uri;
                    }
                }
                catch (Exception ex)
                {
                    Toast.MakeText(ctx, "创建文件异常: " + ex.Message, ToastLength.Short)?.Show();
                }
            };

            Host.SetContentView(container);
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
            var res = _chosenUri ?? _etPath.Text?.Trim() ?? string.Empty;
            TrySubmit(res);
        }
    }

    /// <summary>
    /// 打开文件夹对话框，整合 SAF DocumentTree 目录授权与树形查看。
    /// </summary>
    public class ApedroidOpenFolderDialog : ApedroidDialog<IReadOnlyList<string>>
    {
        private readonly OpenFolderDialogSettings _settings;
        private readonly TextInputEditText _etPath;
        private readonly List<string> _selectedPaths = new();

        public ApedroidOpenFolderDialog(OpenFolderDialogSettings settings, Context? context = null)
            : base(settings, context)
        {
            _settings = settings;

            var ctx = Context!;
            var container = new LinearLayout(ctx)
            {
                Orientation = Orientation.Vertical
            };

            var btnBrowse = new MaterialButton(ctx)
            {
                Text = "选择授权目录 (SAF)",
                CornerRadius = ctx.DpToPx(12)
            };
            btnBrowse.SetBackgroundColor(Color.Rgb(230, 235, 245));
            btnBrowse.SetTextColor(Color.Rgb(30, 80, 180));
            var btnParams = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ctx.DpToPx(46)
            );
            btnParams.SetMargins(0, 0, 0, ctx.DpToPx(10));
            container.AddView(btnBrowse, btnParams);

            var textLayout = new TextInputLayout(ctx)
            {
                BoxBackgroundMode = TextInputLayout.BoxBackgroundOutline,
                Hint = "已选文件夹 URI 或路径"
            };
            textLayout.SetBoxCornerRadii(ctx.DpToPx(8), ctx.DpToPx(8), ctx.DpToPx(8), ctx.DpToPx(8));

            _etPath = new TextInputEditText(textLayout.Context)
            {
                Text = settings.InitialPath ?? string.Empty
            };
            _etPath.SetTextColor(Color.Rgb(28, 27, 31));
            textLayout.AddView(_etPath);
            container.AddView(textLayout);

            btnBrowse.Click += async (s, e) =>
            {
                try
                {
                    var picked = await SafBridge.PickFolderAsync(ctx);
                    if (picked != null && picked.Count > 0)
                    {
                        _selectedPaths.Clear();
                        _selectedPaths.AddRange(picked);
                        _etPath.Text = string.Join("\n", picked);
                    }
                }
                catch (Exception ex)
                {
                    Toast.MakeText(ctx, "目录选择异常: " + ex.Message, ToastLength.Short)?.Show();
                }
            };

            Host.SetContentView(container);
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
            if (_selectedPaths.Count > 0)
            {
                TrySubmit(_selectedPaths);
                return;
            }

            var text = _etPath.Text?.Trim();
            if (!string.IsNullOrEmpty(text))
            {
                TrySubmit(new[] { text });
            }
            else
            {
                TrySubmit(Array.Empty<string>());
            }
        }
    }
}
