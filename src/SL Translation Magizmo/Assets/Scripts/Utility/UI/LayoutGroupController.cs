using UnityEngine.UI;
using UnityEngine;
using System;
using qASIC.Internal;

namespace Project.Utility.UI
{
    public class LayoutGroupController : MonoBehaviour
    {
        private static event Action _OnRefresh;
        private static event Action _OnRefreshNextFrame;

        [SerializeField] RectTransform target;

        [Label("Auto refresh")]
        [SerializeField] bool refreshOnAwake;
        [SerializeField] bool refreshOnStart;
        [SerializeField] bool refreshOnEnable;

        bool _refreshNextFrame;

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

        private void LateUpdate()
        {
            if (_refreshNextFrame)
            {
                _refreshNextFrame = false;
                RefreshSingle();
            }
        }

        private void OnEnable()
        {
            _OnRefresh += HandleRefresh;
            _OnRefreshNextFrame += HandleRefreshNextFrame;

            if (refreshOnEnable)
                RefreshSingle();
        }

        private void OnDisable()
        {
            _OnRefresh -= HandleRefresh;
            _OnRefreshNextFrame -= HandleRefreshNextFrame;
        }
        #endregion

        void HandleRefresh()
        {
            RefreshSingle();
        }

        void HandleRefreshNextFrame()
        {
            RefreshSingleNextFrame();
        }

        public void RefreshSingle()
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(target);
        }

        public void RefreshSingleNextFrame()
        {
            _refreshNextFrame = true;
        }

        public static void Refresh()
        {
            _OnRefresh?.Invoke();
        }

        public static void RefreshNextFrame()
        {
            _OnRefreshNextFrame?.Invoke();
        }
    }
}