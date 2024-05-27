using System;
using UnityEngine;

namespace Project
{
    [AddComponentMenu("")]
    public class DelayExecute : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod]
        static void Initialize()
        {
            var obj = new GameObject("Execute Next Frame", typeof(DelayExecute));
            DontDestroyOnLoad(obj);
        }

        public static void NextFrame(Action action)
        {
            OnNextFrame += action;
        }

        static event Action OnNextFrame;

        private void Update()
        {
            OnNextFrame?.Invoke();
        }
    }
}