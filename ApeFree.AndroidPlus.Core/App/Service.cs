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
    public class Service : Android.App.Service
    {
        public override IBinder OnBind(Intent intent)
        {
            return new Binder(this);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
        }

        public class ServiceEventArgs : Lang.EventArgs
        {
            public ServiceEventArgs(Context context = null) : base(context) { }
        }

        public class Binder : Android.OS.Binder
        {
            public object Tag { get; }
            public Android.App.Service Service { get; }
            public Binder(Android.App.Service service) => Service = service;
            public Binder(Android.App.Service service, object tag) : this(service) => Tag = tag;
        }
    }

    public class Service<T> : Service
    {
        public T Instance { get; private set; }

        public delegate T CreateInstanceHandler(Service<T> sender, EventArgs e);
        public delegate void StartCommandHandler(Service<T> sender, StartCommandEventArgs e);

        public CreateInstanceHandler CreateInstance;
        public StartCommandHandler StartCommand;

        public Service() { }

        public Service(CreateInstanceHandler createInstanceHandler)
        {
            CreateInstance = createInstanceHandler;
        }

        public Service(CreateInstanceHandler createInstanceHandler, StartCommandHandler startCommandHandler) : this(createInstanceHandler)
        {
            StartCommand = startCommandHandler;
        }


        public override void OnCreate()
        {
            base.OnCreate();
            if (CreateInstance != null)
                Instance = CreateInstance.Invoke(this, new EventArgs());
        }

        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            StartCommand?.Invoke(this, new StartCommandEventArgs(intent, flags, startId));
            return base.OnStartCommand(intent, flags, startId);
        }

        public override IBinder OnBind(Intent intent)
        {
            return new Binder<T>(this, Instance);
        }

        public class Binder<P> : Binder
        {
            public P Instance { get; }

            public Binder(Android.App.Service service, P instance, object tag = null) : base(service, tag)
            {
                Instance = instance;
            }
        }

        public class StartCommandEventArgs : EventArgs
        {
            public Intent Intent { get; }
            public StartCommandFlags Flags { get; }
            public int StartId { get; }
            public StartCommandEventArgs(Intent intent, StartCommandFlags flags, int startId)
            {
                Intent = intent;
                Flags = flags;
                StartId = startId;
            }
        }
    }

}