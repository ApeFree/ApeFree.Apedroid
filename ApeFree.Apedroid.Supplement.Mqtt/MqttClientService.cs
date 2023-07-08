using Android.Content;
using Android.Runtime;
using Android.Util;
using ApeFree.Apedroid.Core.App;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Options;
using MQTTnet.Exceptions;
using System;
using System.Threading.Tasks;
using System.Timers;

namespace ApeFree.Apedroid.Supplement.Mqtt
{
    [Android.App.Service]
    public class MqttClientService : Service
    {
        public const string BROADCAST_KEYWORD_EVENT_MQTT_CONNECTED = "Connected";
        public const string BROADCAST_KEYWORD_EVENT_MQTT_DISCONNECTED = "Disconnected";
        public const string BROADCAST_KEYWORD_EVENT_MQTT_MESSAGE_RECEIVED = "MessageReceived";
        public const string BROADCAST_KEYWORD_EVENT_MQTT_CONNECT_FAILED = "ConnectFailed";
        public const string BROADCAST_KEYWORD_FIELD_EVENT = "Event";
        public const string BROADCAST_KEYWORD_FIELD_TOPIC = "Topic";
        public const string BROADCAST_KEYWORD_FIELD_PAYLOAD = "Payload";
        public const string BROADCAST_KEYWORD_FIELD_DATA = "Data";

        private readonly object _connectLocker = new object();

        public static IMqttClient Client => MqttClientSingleton.Instance.Client;
        public static bool IsConnected => Client != null && Client.IsConnected;
        public static IMqttClientOptions Options { get => MqttClientSingleton.Instance.Options; set => MqttClientSingleton.Instance.Options = value; }

        private Task currentTask;
        public static int ReconnectInterval { get; set; } = 5000;
        protected Timer MqttReconnectTimer { get; private set; }

        // MQTT连接成功
        public class MqttConnectedEventArgs : ServiceEventArgs
        {
            public MqttClientConnectedEventArgs Args { get; }
            public MqttConnectedEventArgs(Context context, MqttClientConnectedEventArgs args) : base(context)
            {
                Args = args;
            }
        }
        public delegate void MqttConnectedHandler(MqttClientService sender, MqttConnectedEventArgs e);
        public static event MqttConnectedHandler MqttConnected;

        // MQTT断开连接
        public class MqttDisconnectedEventArgs : ServiceEventArgs
        {
            public MqttClientDisconnectedEventArgs Args { get; }
            public MqttDisconnectedEventArgs(Context context, MqttClientDisconnectedEventArgs args) : base(context) => Args = args;
        }
        public delegate void MqttDisconnectedHandler(MqttClientService sender, MqttDisconnectedEventArgs e);
        public static event MqttDisconnectedHandler MqttDisconnected;

        // MQTT连接失败
        public class MqttConnectFailedEventArgs : ServiceEventArgs
        {
            public Exception Exception { get; }
            public MqttConnectFailedEventArgs(Context context, Exception exception = null) : base(context)
            {
                Exception = exception;
            }
        }
        public delegate void MqttConnectFailedHandler(MqttClientService sender, MqttConnectFailedEventArgs e);
        public static event MqttConnectFailedHandler MqttConnectFailed;

        // MQTT收到消息
        public class MqttApplicationMessageReceivedEventArgs : ServiceEventArgs
        {
            public MQTTnet.MqttApplicationMessageReceivedEventArgs Args { get; }
            public MqttApplicationMessageReceivedEventArgs(Context context, MQTTnet.MqttApplicationMessageReceivedEventArgs args) : base(context) => Args = args;
        }
        public delegate void MqttApplicationMessageReceivedHandler(MqttClientService sender, MqttApplicationMessageReceivedEventArgs e);
        public static event MqttApplicationMessageReceivedHandler MqttApplicationMessageReceived;


        public override void OnCreate()
        {
            base.OnCreate();

            MqttReconnectTimer = new Timer();
            MqttReconnectTimer.Interval = ReconnectInterval;
            MqttReconnectTimer.Elapsed += MqttReconnectTimer_Elapsed;
            MqttReconnectTimer.AutoReset = true;
        }

        private void MqttReconnectTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                MqttReconnectTimer.Stop();

                if (currentTask.Status == TaskStatus.Running || currentTask.Status == TaskStatus.WaitingToRun)
                    return;
                // currentTask?.Wait();
                Connect();
            }
            catch (Exception) { }
            finally
            {
                //if (Client == null || !Client.IsConnected)
                    MqttReconnectTimer.Start();
            }
        }

        [return: GeneratedEnum]
        public override Android.App.StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] Android.App.StartCommandFlags flags, int startId)
        {
            var result = base.OnStartCommand(intent, flags, startId);
            Connect();
            return result;
        }

        public override void OnDestroy()
        {
            Disconnect();
            base.OnDestroy();
        }

        protected virtual Task OnMqttConnected(MqttClientConnectedEventArgs e)
        {
            Log.Debug(GetType().Name, $"{BROADCAST_KEYWORD_EVENT_MQTT_CONNECTED}::{e.ConnectResult.ReasonString}");

            // 停止自动重连
            MqttReconnectTimer.Stop();

            // 如果连接成功但Option没有存在全局，则进行保存;
            if(Options != Client.Options)
            {
                Options = Client.Options;
            }

            // 发布广播
            SendBroadcast(intent =>
            {
                intent.PutExtra(BROADCAST_KEYWORD_FIELD_EVENT, BROADCAST_KEYWORD_EVENT_MQTT_CONNECTED);
            });
            // 执行事件
            SafelyPerformDelegateCallback(() => MqttConnected?.Invoke(this, new MqttConnectedEventArgs(this, e)));

            return Task.CompletedTask;
        }

        protected virtual Task OnMqttDisconnected(MqttClientDisconnectedEventArgs e)
        {
            Log.Debug(GetType().Name, $"{BROADCAST_KEYWORD_EVENT_MQTT_DISCONNECTED}::{e.Reason}");

            // 启动自动重连
            MqttReconnectTimer.Start();
            // 发布广播
            SendBroadcast(intent =>
            {
                intent.PutExtra(BROADCAST_KEYWORD_FIELD_EVENT, BROADCAST_KEYWORD_EVENT_MQTT_DISCONNECTED);
            });
            // 执行事件
            SafelyPerformDelegateCallback(() => MqttDisconnected?.Invoke(this, new MqttDisconnectedEventArgs(this, e)));

            return Task.CompletedTask;
        }

        protected virtual Task OnMqttApplicationMessageReceived(MQTTnet.MqttApplicationMessageReceivedEventArgs e)
        {
            Log.Debug(GetType().Name, $"{BROADCAST_KEYWORD_EVENT_MQTT_MESSAGE_RECEIVED}::{e.ApplicationMessage?.Topic}");

            // 发布广播
            SendBroadcast(intent =>
            {
                intent.PutExtra(BROADCAST_KEYWORD_FIELD_EVENT, BROADCAST_KEYWORD_EVENT_MQTT_MESSAGE_RECEIVED);
                intent.PutExtra(BROADCAST_KEYWORD_FIELD_TOPIC, e.ApplicationMessage.Topic);
                intent.PutExtra(BROADCAST_KEYWORD_FIELD_PAYLOAD, e.ApplicationMessage.Payload);
            });
            // 执行事件
            SafelyPerformDelegateCallback(() => MqttApplicationMessageReceived?.Invoke(this, new MqttApplicationMessageReceivedEventArgs(this, e)));

            return Task.CompletedTask;
        }

        protected virtual void OnMqttConnectFailed(Exception ex)
        {
            Log.Debug(GetType().Name, $"{BROADCAST_KEYWORD_EVENT_MQTT_CONNECT_FAILED}::{ex.Message}");

            // 启动自动重连
            MqttReconnectTimer.Start();
            // 发布广播
            SendBroadcast(intent =>
            {
                intent.PutExtra(BROADCAST_KEYWORD_FIELD_EVENT, BROADCAST_KEYWORD_EVENT_MQTT_CONNECT_FAILED);
                if (ex.GetType() == typeof(MqttCommunicationException))
                {
                    intent.PutExtra(BROADCAST_KEYWORD_FIELD_DATA, ((MqttCommunicationException)ex).Message);
                }
            });
            // 执行事件
            SafelyPerformDelegateCallback(() => MqttConnectFailed?.Invoke(this, new MqttConnectFailedEventArgs(this, ex)));
        }



        protected void Connect()
        {
            Connect(Options);
        }

        protected void Connect(IMqttClientOptions options)
        {
            lock (_connectLocker)
            {
                if (Client != null && Client.IsConnected) return;
                if (options == null) return;

                // if (Client == null)
                {
                    MqttClientSingleton.Instance.Client = new MqttFactory().CreateMqttClient();

                    Client.UseConnectedHandler(OnMqttConnected);
                    Client.UseDisconnectedHandler(OnMqttDisconnected);
                    Client.UseApplicationMessageReceivedHandler(OnMqttApplicationMessageReceived);
                }

                try
                {
                    currentTask = Client.ConnectAsync(options);
                }
                catch (Exception ex)
                {
                    if (ex.GetType().Name.StartsWith("Mqtt"))
                    {
                        OnMqttConnectFailed(ex);
                    }
                    else
                    {
#if DEBUG
                        throw ex;
#endif
                    }
                }
            }
        }

        private void Disconnect()
        {
            lock (_connectLocker)
            {
                if (Client!=null && Client.IsConnected)
                    currentTask = Client.DisconnectAsync();

                MqttReconnectTimer.Stop();
                MqttReconnectTimer.Dispose();

                StopSelf();
            }
        }

        /// <summary>
        /// 安全执行
        /// 避免事件回调的实现执行异常导致崩溃
        /// </summary>
        /// <param name="action"></param>
        protected void SafelyPerformDelegateCallback(Action action)
        {
            try { action.Invoke(); }
            catch (Exception ex)
            {
#if DEBUG
                throw ex;
#endif
            }
        }

        protected void SendBroadcast(Action<Intent> action)
        {
            Intent intent = new Intent(this.GetType().FullName);
            action.Invoke(intent);
            SendBroadcast(intent);
        }
    }

    internal sealed class MqttClientSingleton
    {
        private static readonly Lazy<MqttClientSingleton> Instancelock = new Lazy<MqttClientSingleton>(() => new MqttClientSingleton());
        public static MqttClientSingleton Instance => Instancelock.Value;
        private MqttClientSingleton() { }

        public IMqttClient Client { get; set; }
        public IMqttClientOptions Options { get; internal set; }
    }
}
