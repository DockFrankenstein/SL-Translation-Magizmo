using System;
using UnityEngine;

namespace qASIC.Input.Update
{
    public static class InputUpdateManager
    {
        public static event Action OnUpdate;
        public static event Action OnPostUpdate;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        {
            new GameObject("[qASIC] Input Update", typeof(InputBehaviorUpdate), typeof(AddToDontDestroy));
        }

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
        private static void EditorInitialize()
        {
            UnityEditor.EditorApplication.update += () =>
            {
                if (!Application.isPlaying)
                    TriggerUpdate();
            };
        }
#endif

        public static void TriggerUpdate()
        {
            OnUpdate?.Invoke();
            OnPostUpdate?.Invoke();
        }
    }
}
