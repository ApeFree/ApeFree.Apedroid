using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Widget;
using ApeFree.ApeDialogs.Theme;
using ApeFree.Apedroid.Core;
using ApeFree.Apedroid.Dialogs.Adapters;

namespace ApeFree.Apedroid.Dialogs.Hosting
{
    /// <summary>
    /// Material 3 现代弹窗宿主。
    /// 承载大圆角卡片、顶部醒目指示条、标题、说明正文、动态内容区与操作按钮栏。
    /// </summary>
    public class ApedroidDialogHost : IDisposable
    {
        private readonly Context _context;
        private readonly Dialog _dialog;
        private readonly LinearLayout _rootLayout;
        private readonly View _reminderBar;
        private readonly TextView _tvTitle;
        private readonly TextView _tvContent;
        private readonly FrameLayout _contentContainer;
        private readonly LinearLayout _buttonBar;
        private readonly HorizontalScrollView _buttonScrollView;
        private readonly List<OptionButton> _options = new();
        private bool _isDisposed;

        public event EventHandler? Shown;
        public event EventHandler? Dismissed;
        public event EventHandler? Canceled;

        public Dialog InnerDialog => _dialog;

        public string Title
        {
            get => _tvTitle.Text ?? string.Empty;
            set
            {
                ActivityTracker.RunOnUiThread(() =>
                {
                    _tvTitle.Text = value;
                    _tvTitle.Visibility = string.IsNullOrWhiteSpace(value) ? ViewStates.Gone : ViewStates.Visible;
                });
            }
        }

        public string Content
        {
            get => _tvContent.Text ?? string.Empty;
            set
            {
                ActivityTracker.RunOnUiThread(() =>
                {
                    _tvContent.Text = value;
                    _tvContent.Visibility = string.IsNullOrWhiteSpace(value) ? ViewStates.Gone : ViewStates.Visible;
                });
            }
        }

        public ApedroidDialogHost(Context context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dialog = new Dialog(context);

            // 根卡片容器
            _rootLayout = new LinearLayout(context)
            {
                Orientation = Orientation.Vertical,
                Elevation = context.DpToPx(8)
            };

            // 28dp 大圆角卡片背景
            var cardDrawable = new GradientDrawable();
            cardDrawable.SetCornerRadius(context.DpToPx(28));
            bool isNight = (context.Resources?.Configuration?.UiMode & Android.Content.Res.UiMode.NightMask) == Android.Content.Res.UiMode.NightYes;
            cardDrawable.SetColor(isNight ? Color.Rgb(32, 33, 37) : Color.Rgb(250, 250, 252));
            _rootLayout.Background = cardDrawable;

            // 1. 顶部醒目标签条 (Reminder Bar)
            _reminderBar = new View(context)
            {
                Visibility = ViewStates.Gone
            };
            var reminderBarParams = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                context.DpToPx(4)
            );
            _rootLayout.AddView(_reminderBar, reminderBarParams);

            // 2. 标题
            _tvTitle = new TextView(context)
            {
                TextSize = 20,
                Typeface = Typeface.DefaultBold,
                Visibility = ViewStates.Gone
            };
            _tvTitle.SetTextColor(isNight ? Color.Rgb(242, 242, 247) : Color.Rgb(28, 27, 31));
            var titleParams = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent
            );
            int marginHoriz = context.DpToPx(24);
            int topPadding = context.DpToPx(20);
            titleParams.SetMargins(marginHoriz, topPadding, marginHoriz, context.DpToPx(8));
            _rootLayout.AddView(_tvTitle, titleParams);

            // 3. 内容正文
            _tvContent = new TextView(context)
            {
                TextSize = 15,
                Visibility = ViewStates.Gone
            };
            _tvContent.SetTextColor(isNight ? Color.Rgb(174, 174, 178) : Color.Rgb(73, 69, 79));
            _tvContent.SetLineSpacing(context.DpToPx(3), 1.15f);
            var contentParams = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent
            );
            contentParams.SetMargins(marginHoriz, 0, marginHoriz, context.DpToPx(12));
            _rootLayout.AddView(_tvContent, contentParams);

            // 4. 动态内容区 (ContentView slot)
            _contentContainer = new FrameLayout(context)
            {
                Visibility = ViewStates.Gone
            };
            var slotParams = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent
            );
            slotParams.SetMargins(marginHoriz, 0, marginHoriz, context.DpToPx(16));
            _rootLayout.AddView(_contentContainer, slotParams);

            // 5. 按钮操作栏 (水平滚动容纳多个按钮)
            _buttonScrollView = new HorizontalScrollView(context)
            {
                HorizontalScrollBarEnabled = false
            };
            _buttonBar = new LinearLayout(context)
            {
                Orientation = Orientation.Horizontal
            };
            _buttonBar.SetGravity(GravityFlags.End | GravityFlags.CenterVertical);
            var buttonBarParams = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent
            );
            _buttonScrollView.AddView(_buttonBar, new ViewGroup.LayoutParams(
                ViewGroup.LayoutParams.WrapContent,
                ViewGroup.LayoutParams.WrapContent
            ));

            var scrollContainerParams = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent
            );
            scrollContainerParams.SetMargins(context.DpToPx(16), 0, context.DpToPx(16), context.DpToPx(18));
            _rootLayout.AddView(_buttonScrollView, scrollContainerParams);

            // 配置 Android Dialog 窗口属性
            _dialog.RequestWindowFeature((int)WindowFeatures.NoTitle);
            _dialog.SetContentView(_rootLayout);

            if (_dialog.Window != null)
            {
                _dialog.Window.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));
                _dialog.Window.SetDimAmount(0.45f);
                _dialog.Window.SetSoftInputMode(SoftInput.AdjustResize);

                // 计算弹窗宽度：屏幕宽度的 88%~92%，最大宽度限制 560dp
                var metrics = context.Resources?.DisplayMetrics ?? Android.Content.Res.Resources.System.DisplayMetrics!;
                int screenWidth = metrics.WidthPixels;
                int maxDialogWidth = context.DpToPx(560);
                int dialogWidth = Math.Min((int)(screenWidth * 0.88f), maxDialogWidth);
                _dialog.Window.SetLayout(dialogWidth, ViewGroup.LayoutParams.WrapContent);
            }

            _dialog.ShowEvent += (s, e) => Shown?.Invoke(this, EventArgs.Empty);
            _dialog.DismissEvent += (s, e) => Dismissed?.Invoke(this, EventArgs.Empty);
            _dialog.CancelEvent += (s, e) => Canceled?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// 设置顶部醒目标记条颜色 (Reminder Color)
        /// </summary>
        public void SetReminderColor(Color color)
        {
            if (color.A == 0)
            {
                _reminderBar.Visibility = ViewStates.Gone;
                return;
            }

            _reminderBar.SetBackgroundColor(color);
            _reminderBar.Visibility = ViewStates.Visible;
        }

        /// <summary>
        /// 挂载自定义动态内容视图
        /// </summary>
        public void SetContentView(View? view)
        {
            ActivityTracker.RunOnUiThread(() =>
            {
                _contentContainer.RemoveAllViews();
                if (view != null)
                {
                    _contentContainer.AddView(view, new FrameLayout.LayoutParams(
                        ViewGroup.LayoutParams.MatchParent,
                        ViewGroup.LayoutParams.WrapContent
                    ));
                    _contentContainer.Visibility = ViewStates.Visible;
                }
                else
                {
                    _contentContainer.Visibility = ViewStates.Gone;
                }
            });
        }

        /// <summary>
        /// 添加选项按钮
        /// </summary>
        public void AddOptionButton(OptionButton button)
        {
            if (button == null) return;
            _options.Add(button);
            _buttonBar.AddView(button);
        }

        /// <summary>
        /// 设置是否允许外部或物理返回键取消
        /// </summary>
        public void SetCancelable(bool cancelable)
        {
            _dialog.SetCancelable(cancelable);
            _dialog.SetCanceledOnTouchOutside(cancelable);
        }

        /// <summary>
        /// 展示弹窗
        /// </summary>
        public void Show()
        {
            ActivityTracker.RunOnUiThread(() =>
            {
                if (!_dialog.IsShowing)
                {
                    _dialog.Show();
                }
            });
        }

        /// <summary>
        /// 关闭弹窗
        /// </summary>
        public void Dismiss()
        {
            ActivityTracker.RunOnUiThread(() =>
            {
                if (_dialog.IsShowing)
                {
                    _dialog.Dismiss();
                }
            });
        }

        public void Dispose()
        {
            if (_isDisposed) return;
            _isDisposed = true;

            ActivityTracker.RunOnUiThread(() =>
            {
                try
                {
                    if (_dialog.IsShowing)
                    {
                        _dialog.Dismiss();
                    }
                    _dialog.Dispose();
                }
                catch { }
            });
        }
    }
}
