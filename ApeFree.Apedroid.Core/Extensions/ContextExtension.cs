using Android.App;
using Java.Lang;

namespace Android.Content
{
    public static class ContextExtension
    {

        public static ActivityManager GetActivityManager(this Context context)
        {
            return (ActivityManager)context.GetSystemService(Context.ActivityService);
        }

        public static void RestartPackage(this Context context)
        {
            GetActivityManager(context).RestartPackage(context.PackageName);
        }

        public static void KillProcesses(this Context context)
        {
            JavaSystem.Exit(0);
            // GetActivityManager(context).KillBackgroundProcesses(context.PackageName);
        }
    }
}