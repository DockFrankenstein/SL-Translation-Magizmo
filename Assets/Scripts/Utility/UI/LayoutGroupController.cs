using UnityEngine.UI;
using UnityEngine;
using System;

namespace Project.Utility.UI
{
    public class LayoutGroupController : MonoBehaviour
    {
        private static event Action _OnRefresh;

        [SerializeField] RectTransform target;

        [Label("Auto refresh")]
        [SerializeField] bool refreshOnAwake;
        [SerializeField] bool refreshOnStart;
        [SerializeField] bool refreshOnEnable;

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

            if (refreshOnAwake)
                RefreshSingle();
        }

        private void Start()
        {
            if (refreshOnStart)
                RefreshSingle();
        }

        private void OnEnable()
        {
            if (refreshOnEnable)
                RefreshSingle();
        }

        private void OnDisable()
        {
            _OnRefresh -= HandleRefresh;
        }

        void HandleRefresh()
        {
            RefreshSingle();
        }

        public void RefreshSingle()
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(target);
        }
    }
}