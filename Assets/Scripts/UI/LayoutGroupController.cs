using UnityEngine.UI;
using UnityEngine;
using System;

namespace Project.UI
{
    public class LayoutGroupController : MonoBehaviour
    {
        private static event Action _OnRefresh;

        [SerializeField] RectTransform target;

        private void Reset()
        {
            target = GetComponent<RectTransform>();
        }

        public static void Refresh()
        {
            _OnRefresh?.Invoke();
        }

        private void Awake()
        {
            _OnRefresh += HandleRefresh;
        }

        void HandleRefresh()
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(target);
        }
    }
}