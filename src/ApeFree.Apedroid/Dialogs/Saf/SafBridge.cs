using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Net;
using Android.OS;
using ApeFree.Apedroid.Core;

namespace ApeFree.Apedroid.Dialogs.Saf
{
    /// <summary>
    /// Android 存储访问框架 (Storage Access Framework) 异步桥接器。
    /// 支持免手动重写 OnActivityResult 即可直接 await 系统级文件/目录选择结果。
    /// </summary>
    public static class SafBridge
    {
        internal static TaskCompletionSource<IReadOnlyList<string>>? CurrentFilesTcs;
        internal static TaskCompletionSource<string?>? CurrentSaveFileTcs;

        /// <summary>
        /// 异步打开文件选择器
        /// </summary>
        public static Task<IReadOnlyList<string>> PickFilesAsync(Context context, bool multiSelect = false, string? mimeType = "*/*")
        {
            var tcs = new TaskCompletionSource<IReadOnlyList<string>>(TaskCreationOptions.RunContinuationsAsynchronously);
            CurrentFilesTcs = tcs;

            var intent = new Intent(context, typeof(SafBridgeActivity));
            intent.PutExtra("action_type", "open_file");
            intent.PutExtra("multi_select", multiSelect);
            intent.PutExtra("mime_type", string.IsNullOrWhiteSpace(mimeType) ? "*/*" : mimeType);
            intent.AddFlags(ActivityFlags.NewTask);

            context.StartActivity(intent);
            return tcs.Task;
        }

        /// <summary>
        /// 异步打开文件保存器
        /// </summary>
        public static Task<string?> CreateFileAsync(Context context, string? defaultFileName = null, string? mimeType = "*/*")
        {
            var tcs = new TaskCompletionSource<string?>(TaskCreationOptions.RunContinuationsAsynchronously);
            CurrentSaveFileTcs = tcs;

            var intent = new Intent(context, typeof(SafBridgeActivity));
            intent.PutExtra("action_type", "save_file");
            intent.PutExtra("default_name", defaultFileName ?? "file");
            intent.PutExtra("mime_type", string.IsNullOrWhiteSpace(mimeType) ? "*/*" : mimeType);
            intent.AddFlags(ActivityFlags.NewTask);

            context.StartActivity(intent);
            return tcs.Task;
        }

        /// <summary>
        /// 异步打开文件夹选择器
        /// </summary>
        public static Task<IReadOnlyList<string>> PickFolderAsync(Context context)
        {
            var tcs = new TaskCompletionSource<IReadOnlyList<string>>(TaskCreationOptions.RunContinuationsAsynchronously);
            CurrentFilesTcs = tcs;

            var intent = new Intent(context, typeof(SafBridgeActivity));
            intent.PutExtra("action_type", "open_folder");
            intent.AddFlags(ActivityFlags.NewTask);

            context.StartActivity(intent);
            return tcs.Task;
        }
    }

    /// <summary>
    /// 用于代理 SAF 交互的透明隐形 Activity
    /// </summary>
    [Activity(Theme = "@android:style/Theme.Translucent.NoTitleBar", ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)]
    public class SafBridgeActivity : Activity
    {
        private const int RequestCodeOpenFile = 1001;
        private const int RequestCodeSaveFile = 1002;
        private const int RequestCodeOpenFolder = 1003;

        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            string? actionType = Intent?.GetStringExtra("action_type");
            try
            {
                if (actionType == "open_file")
                {
                    bool multiSelect = Intent?.GetBooleanExtra("multi_select", false) ?? false;
                    string mimeType = Intent?.GetStringExtra("mime_type") ?? "*/*";

                    var intent = new Intent(Intent.ActionOpenDocument);
                    intent.AddCategory(Intent.CategoryOpenable);
                    intent.SetType(mimeType);
                    intent.PutExtra(Intent.ExtraAllowMultiple, multiSelect);
                    StartActivityForResult(intent, RequestCodeOpenFile);
                }
                else if (actionType == "save_file")
                {
                    string defaultName = Intent?.GetStringExtra("default_name") ?? "file";
                    string mimeType = Intent?.GetStringExtra("mime_type") ?? "*/*";

                    var intent = new Intent(Intent.ActionCreateDocument);
                    intent.AddCategory(Intent.CategoryOpenable);
                    intent.SetType(mimeType);
                    intent.PutExtra(Intent.ExtraTitle, defaultName);
                    StartActivityForResult(intent, RequestCodeSaveFile);
                }
                else if (actionType == "open_folder")
                {
                    var intent = new Intent(Intent.ActionOpenDocumentTree);
                    StartActivityForResult(intent, RequestCodeOpenFolder);
                }
                else
                {
                    Finish();
                }
            }
            catch (Exception ex)
            {
                SafBridge.CurrentFilesTcs?.TrySetException(ex);
                SafBridge.CurrentSaveFileTcs?.TrySetException(ex);
                Finish();
            }
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent? data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            try
            {
                if (resultCode == Result.Ok && data != null)
                {
                    if (requestCode == RequestCodeOpenFile)
                    {
                        var results = new List<string>();
                        if (data.ClipData != null)
                        {
                            for (int i = 0; i < data.ClipData.ItemCount; i++)
                            {
                                var uri = data.ClipData.GetItemAt(i)?.Uri;
                                if (uri != null) results.Add(uri.ToString());
                            }
                        }
                        else if (data.Data != null)
                        {
                            results.Add(data.Data.ToString());
                        }
                        SafBridge.CurrentFilesTcs?.TrySetResult(results);
                    }
                    else if (requestCode == RequestCodeSaveFile)
                    {
                        var uri = data.Data?.ToString();
                        SafBridge.CurrentSaveFileTcs?.TrySetResult(uri);
                    }
                    else if (requestCode == RequestCodeOpenFolder)
                    {
                        var results = new List<string>();
                        if (data.Data != null)
                        {
                            results.Add(data.Data.ToString());
                        }
                        SafBridge.CurrentFilesTcs?.TrySetResult(results);
                    }
                }
                else
                {
                    // 用户取消
                    SafBridge.CurrentFilesTcs?.TrySetResult(Array.Empty<string>());
                    SafBridge.CurrentSaveFileTcs?.TrySetResult(null);
                }
            }
            catch (Exception ex)
            {
                SafBridge.CurrentFilesTcs?.TrySetException(ex);
                SafBridge.CurrentSaveFileTcs?.TrySetException(ex);
            }
            finally
            {
                Finish();
            }
        }
    }
}
