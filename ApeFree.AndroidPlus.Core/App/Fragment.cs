using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ApeFree.AndroidPlus.Core.App
{
    public class Fragment : AndroidX.Fragment.App.Fragment
    {
        public FragmentStatus Status { get; private set; }

        public class FragmentEventArgs : Lang.EventArgs { }

        public class FragmentAttachEventArgs : FragmentEventArgs { }
        public delegate void FragmentAttachHandler(Fragment sender, FragmentAttachEventArgs e);
        public event FragmentAttachHandler FragmentAttach;

        public class FragmentCreateEventArgs : FragmentEventArgs { public Bundle SavedInstanceState { get; internal set; } }
        public delegate void FragmentCreateHandler(Fragment sender, FragmentCreateEventArgs e);
        public event FragmentCreateHandler FragmentCreate;

        public class FragmentCreateViewEventArgs : FragmentEventArgs { public LayoutInflater Inflater { get; internal set; } public ViewGroup Container { get; internal set; } public Bundle SavedInstanceState { get; internal set; } public View View { get; internal set; } }
        public delegate void FragmentCreateViewHandler(Fragment sender, FragmentCreateViewEventArgs e);
        public event FragmentCreateViewHandler FragmentCreateView;

        public class FragmentDestroyEventArgs : FragmentEventArgs { }
        public delegate void FragmentDestroyHandler(Fragment sender, FragmentDestroyEventArgs e);
        public event FragmentDestroyHandler FragmentDestroy;

        public class FragmentDestroyViewEventArgs : FragmentEventArgs { }
        public delegate void FragmentDestroyViewHandler(Fragment sender, FragmentDestroyViewEventArgs e);
        public event FragmentDestroyViewHandler FragmentDestroyView;

        public class FragmentDetachEventArgs : FragmentEventArgs { }
        public delegate void FragmentDetachHandler(Fragment sender, FragmentDetachEventArgs e);
        public event FragmentDetachHandler FragmentDetach;

        public class FragmentPauseEventArgs : FragmentEventArgs { }
        public delegate void FragmentPauseHandler(Fragment sender, FragmentPauseEventArgs e);
        public event FragmentPauseHandler FragmentPause;

        public class FragmentResumeEventArgs : FragmentEventArgs { }
        public delegate void FragmentResumeHandler(Fragment sender, FragmentResumeEventArgs e);
        public event FragmentResumeHandler FragmentResume;

        public class FragmentStartEventArgs : FragmentEventArgs { }
        public delegate void FragmentStartHandler(Fragment sender, FragmentStartEventArgs e);
        public event FragmentStartHandler FragmentStart;

        public class FragmentStopEventArgs : FragmentEventArgs { public LayoutInflater Inflater { get; internal set; } public ViewGroup Container { get; internal set; } public Bundle SavedInstanceState { get; internal set; } }
        public delegate void FragmentStopHandler(Fragment sender, FragmentStopEventArgs e);
        public event FragmentStopHandler FragmentStop;

        public override void OnAttach(Context context)
        {
            base.OnAttach(context);
            Status = FragmentStatus.Attach;
            FragmentAttach?.Invoke(this, new FragmentAttachEventArgs() { Context = context });
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Status = FragmentStatus.Create;
            FragmentCreate?.Invoke(this, new FragmentCreateEventArgs() { Context = Context, SavedInstanceState = savedInstanceState });
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = base.OnCreateView(inflater, container, savedInstanceState);
            Status = FragmentStatus.CreateView;
            FragmentCreateView?.Invoke(this, new FragmentCreateViewEventArgs() { Context = Context, View = view });
            return view;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            Status = FragmentStatus.Destroy;
            FragmentDestroy?.Invoke(this, new FragmentDestroyEventArgs() { Context = Context });
        }

        public override void OnDestroyView()
        {
            base.OnDestroyView();
            Status = FragmentStatus.DestroyView;
            FragmentDestroyView?.Invoke(this, new FragmentDestroyViewEventArgs() { Context = Context });
        }

        public override void OnDetach()
        {
            base.OnDetach();
            Status = FragmentStatus.Detach;
            FragmentDetach?.Invoke(this, new FragmentDetachEventArgs() { Context = Context });
        }

        public override void OnPause()
        {
            base.OnPause();
            Status = FragmentStatus.Pause;
            FragmentPause?.Invoke(this, new FragmentPauseEventArgs() { Context = Context });
        }

        public override void OnResume()
        {
            base.OnResume();
            Status = FragmentStatus.Resume;
            FragmentResume?.Invoke(this, new FragmentResumeEventArgs() { Context = Context });
        }

        public override void OnStart()
        {
            base.OnStart();
            Status = FragmentStatus.Start;
            FragmentStart?.Invoke(this, new FragmentStartEventArgs() { Context = Context });
        }

        public override void OnStop()
        {
            base.OnStop();
            Status = FragmentStatus.Stop;
            FragmentStop?.Invoke(this, new FragmentStopEventArgs() { Context = Context });
        }

        public enum FragmentStatus : byte
        {
            Attach,
            Create,
            CreateView,
            Start,
            Resume,
            Pause,
            Stop,
            DestroyView,
            Destroy,
            Detach,
        }
    }
}