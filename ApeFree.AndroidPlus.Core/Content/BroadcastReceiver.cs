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

namespace ApeFree.AndroidPlus.Core.Services
{
    [BroadcastReceiver(Enabled = true)]
    public class BroadcastReceiver : Android.Content.BroadcastReceiver
    {
        public delegate void ReceivedHandler(Context context, Intent intent);
        public event ReceivedHandler Received;

        public BroadcastReceiver() { }
        public BroadcastReceiver(ReceivedHandler receivedHandler)
        {
            Received += receivedHandler;
        }

        public override void OnReceive(Context context, Intent intent)
        {
            Received?.Invoke(context, intent);
        }
    }
}