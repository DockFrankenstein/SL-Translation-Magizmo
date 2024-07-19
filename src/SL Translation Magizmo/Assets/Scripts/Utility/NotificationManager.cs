using qASIC;
using qASIC.Options;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Project
{
    public class NotificationManager : MonoBehaviour
    {
        //[EditorButton(nameof(DebugNotification), activityType: ButtonActivityType.OnPlayMode)]

        [Option("notif_duration")]
        public static float DefaultDuration { get; set; } = 5f;

        private void OnEnable()
        {
            qApplication.QasicInstance.RegisteredObjects.Register(this);
        }

        private void OnDisable()
        {
            qApplication.QasicInstance.RegisteredObjects.Deregister(this);
        }

        public List<Notification> Notifications { get; private set; } = new List<Notification>();
        public event Action<Notification> OnNewNotification;

        public void Notify(string message) =>
            Notify(message, DefaultDuration);

        public void Notify(string message, float duration)
        {
            var notification = new Notification(message, duration);
            Notifications.Add(notification);
            OnNewNotification?.Invoke(notification);
        }

        void DebugNotification()
        {
            Notify("This is a debug notification");
        }

        public class Notification
        {
            public Notification(string message, float duration)
            {
                this.message = message;
                this.duration = duration;
            }

            public string message;
            public float duration;
        }
    }
}