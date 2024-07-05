using UnityEngine;
using System;
using System.Linq;

namespace qASIC
{
    public static class qApplication
    {
        public static qInstance QasicInstance { get; private set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Initialize()
        {
            QasicInstance = new qInstance(new RemoteAppInfo()
            {
                projectName = Application.productName,
                version = Application.version,
                engine = "Unity",
                engineVersion = Application.unityVersion,
            });

            QasicInstance.Start();

            qDebug.OnLog += QDebug_OnLog;

            Application.quitting += OnApplicationQuit;
        }

        private static void QDebug_OnLog(qLog obj)
        {
            var color = new Color(obj.color.red / 255f, obj.color.green / 255f, obj.color.blue / 255f, obj.color.alpha / 255f);
            Debug.Log($"<color=#{ColorUtility.ToHtmlStringRGBA(color)}>{obj.message}</color>");
        }

        static void OnApplicationQuit()
        {
            QasicInstance?.Stop();
        }
    }
}