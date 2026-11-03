using System;
using Android.App;
using Android.Content;
using Android.OS;

namespace ApeFree.Apedroid.Core
{
    /// <summary>
    /// 全局 Activity 生命周期追踪器与运行时上下文助手。
    /// 自动跟踪前台活跃 Activity，为无 Context 弹窗及主线程调度提供支撑。
    /// </summary>
    public static class ActivityTracker
    {
        private static WeakReference<Activity>? _currentActivityRef;
        private static Application? _application;
        private static readonly Handler _mainHandler = new(Looper.MainLooper);

        /// <summary>
        /// 获取当前顶层活跃的 Activity（若已销毁或不在前台则可能为 null）。
        /// </summary>
        public static Activity? CurrentActivity
        {
            get
            {
                if (_currentActivityRef != null && _currentActivityRef.TryGetTarget(out var activity))
                {
                    if (!activity.IsFinishing && !activity.IsDestroyed)
                    {
                        return activity;
                    }
                }
                return null;
            }
            internal set
            {
                _currentActivityRef = value != null ? new WeakReference<Activity>(value) : null;
            }
        }

        /// <summary>
        /// 获取已注册的全局 Application 实例。
        /// </summary>
        public static Application? Application => _application;

        /// <summary>
        /// 初始化全局 Activity 追踪。建议在自定义 Application.OnCreate 中调用。
        /// </summary>
        /// <param name="app">Android Application 实例</param>
        public static void Init(Application app)
        {
            if (_application != null)
            {
                return;
            }

            _application = app ?? throw new ArgumentNullException(nameof(app));
            app.RegisterActivityLifecycleCallbacks(new LifecycleCallbacks());
        }

        /// <summary>
        /// 确保操作在 Android 主（UI）线程上执行。
        /// </summary>
        /// <param name="action">待执行逻辑</param>
        public static void RunOnUiThread(Action action)
        {
            if (action == null) return;

            if (Looper.MyLooper() == Looper.MainLooper)
            {
                action();
            }
            else
            {
                _mainHandler.Post(action);
            }
        }

        private sealed class LifecycleCallbacks : Java.Lang.Object, Application.IActivityLifecycleCallbacks
        {
            public void OnActivityCreated(Activity activity, Bundle? savedInstanceState)
            {
                CurrentActivity = activity;
            }

            public void OnActivityStarted(Activity activity)
            {
                CurrentActivity = activity;
            }

            public void OnActivityResumed(Activity activity)
            {
                CurrentActivity = activity;
            }

            public void OnActivityPaused(Activity activity)
            {
                if (ReferenceEquals(CurrentActivity, activity))
                {
                    // 保持引用，直到下一个 Activity Resume
                }
            }

            public void OnActivityStopped(Activity activity)
            {
                if (ReferenceEquals(CurrentActivity, activity))
                {
                    CurrentActivity = null;
                }
            }

            public void OnActivitySaveInstanceState(Activity activity, Bundle outState) { }

            public void OnActivityDestroyed(Activity activity)
            {
                if (ReferenceEquals(CurrentActivity, activity))
                {
                    CurrentActivity = null;
                }
            }
        }
    }
}
