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

        #region Unity
        private void Reset()
        {
            target = GetComponent<RectTransform>();
        }

        private void Awake()
        {
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
            _OnRefresh += HandleRefresh;

            if (refreshOnEnable)
                RefreshSingle();
        }

        private void OnDisable()
        {
            _OnRefresh -= HandleRefresh;
        }
        #endregion

        void HandleRefresh()
        {
            RefreshSingle();
        }

        public void RefreshSingle()
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(target);
        }

        public static void Refresh()
        {
            _OnRefresh?.Invoke();
        }
    }
}