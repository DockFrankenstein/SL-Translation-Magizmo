using Project.Utility.UI;
using qASIC;
using qASIC.Options;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Project.GUI
{
    public class NotificationGUI : MonoBehaviour
    {
        [Label("Manager")]
        [SerializeField] NotificationManager manager;

        [Label("Text")]
        [SerializeField] TMP_Text notificationTemplate;
        [SerializeField] Transform notificationParent;

        [Option("notif_fadeout_duration")]
        static float FadeoutDuration { get; set; } = 1f;

        Queue<TMP_Text> textPool = new Queue<TMP_Text>();

        List<TMP_Text> activeNotifications = new List<TMP_Text>();

        private void Reset()
        {
            manager = GetComponent<NotificationManager>();
        }

        private void Awake()
        {
            notificationTemplate.gameObject.SetActive(false);

            manager.OnNewNotification += Manager_OnNewNotification;
        }

        private void OnEnable()
        {
            qApplication.QasicInstance.RegisteredObjects.Register(this);
        }

        private void OnDisable()
        {
            qApplication.QasicInstance.RegisteredObjects.Deregister(this);
        }

        private void Manager_OnNewNotification(NotificationManager.Notification notification)
        {
            var text = textPool.Count != 0 ? textPool.Dequeue() : null;

            if (text == null)
                text = Instantiate(notificationTemplate);

            text.transform.SetParent(notificationParent);
            text.transform.SetSiblingIndex(notificationParent.childCount - 1);
            text.text = notification.message;
            var color = text.color;
            color.a = 1f;
            text.color = color;
            text.gameObject.SetActive(true);

            StartCoroutine(AnimateText(text, notification.duration));
            LayoutGroupController.Refresh();
        }

        IEnumerator AnimateText(TMP_Text text, float duration)
        {
            yield return new WaitForSeconds(duration);

            float t = 0f;
            while (t < FadeoutDuration)
            {
                yield return null;
                t += Time.deltaTime;
                var color = text.color;
                color.a = Mathf.Lerp(1f, 0f, t / FadeoutDuration);
                text.color = color;
            }

            text.gameObject.SetActive(false);
            activeNotifications.Remove(text);
            textPool.Enqueue(text);

            LayoutGroupController.Refresh();
        }
    }
}