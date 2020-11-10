using Android.App;
using Android.Content;
using Android.Support.V4.App;
using Firebase.Messaging;
using System.Collections.Generic;
using Barber.Droid;
using Android.Graphics;
using Xamarin.Forms;
using Barber.Views;
using System;

namespace FCMClient
{
    [Service]
    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
    public class MyFirebaseMessagingService : FirebaseMessagingService
    {
        public override void OnMessageReceived(RemoteMessage message)
        {
            var body = message.GetNotification().Body;
            var rcvTitle = message.GetNotification().Title;
            /*var title = rcvTitle[0];
            var type = rcvTitle[1];*/
            SendNotification(rcvTitle, body, message.Data);

            MessagingCenter.Send<Object>(this, "NewNotification");
        }

        void SendNotification(string messageTitle, string messageBody, IDictionary<string, string> data)
        {
            var intent = new Intent(this, typeof(MainActivity));
            intent.AddFlags(ActivityFlags.ClearTop);
            foreach (var key in data.Keys)
            {
                intent.PutExtra(key, data[key]);
            }

            var pendingIntent = PendingIntent.GetActivity(this,
                                                          MainActivity.NOTIFICATION_ID,
                                                          intent,
                                                          PendingIntentFlags.OneShot);

            var notificationBuilder = new NotificationCompat.Builder(this, MainActivity.CHANNEL_ID)
                                      .SetSmallIcon(Resource.Drawable.logo)
                                      .SetLargeIcon(BitmapFactory.DecodeResource(Resources, Resource.Drawable.ic_notification))
                                      .SetContentTitle(messageTitle)
                                      .SetContentText(messageBody)
                                      .SetAutoCancel(true)
                                      .SetContentIntent(pendingIntent);

            var notificationManager = NotificationManagerCompat.From(this);
            notificationManager.Notify(MainActivity.NOTIFICATION_ID, notificationBuilder.Build());
        }
    }
}