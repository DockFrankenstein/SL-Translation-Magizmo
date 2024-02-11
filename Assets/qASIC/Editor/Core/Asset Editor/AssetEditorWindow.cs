using qASIC.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace qASIC.EditorTools.AssetEditor
{
    public abstract class AssetEditorWindow<TWindow, TAsset> : EditorWindow where TWindow : AssetEditorWindow<TWindow, TAsset>
        where TAsset : ScriptableObject
    {
        [SerializeField] Texture2D icon;

        [SerializeField] public TAsset asset;
        [NonSerialized] protected bool initialized;

        private static Dictionary<TAsset, List<TWindow>> _openedWindows = new Dictionary<TAsset, List<TWindow>>();
        
        /// <summary>Title of the window that will be displayed on the top.</summary>
        public abstract string WindowTitle { get; }

        /// <summary>Prefix for every key used to save SessionState or EditorPrefs. Keep the prefix lowercase for consistency.</summary>
        public abstract string PrefsKeyPrefix { get; }
        public virtual Vector2 MinWindowSize => new Vector2(512f, 256f);
        public virtual Vector2 MaxWindowSize => new Vector2(4000f, 4000f);

        #region Creating
        public static TWindow CreateWindow()
        {
            var window = CreateWindow<TWindow>();

            window.minSize = window.MinWindowSize;
            window.maxSize = window.MaxWindowSize;

            window.UpdateWindowTitle();
            window.OnCreateWindow();
            window.Show();

            return window;
        }

        public static void OpenAsset(TAsset asset)
        {
            if (_openedWindows.ContainsKey(asset) && _openedWindows[asset].Count > 0)
            {
                _openedWindows[asset][0].Show();
                return;
            }

            var window = CreateWindow();
            window.asset = asset;
            window.UpdateWindowTitle();
        }

        public void UpdateWindowTitle()
        {
            titleContent = new GUIContent($"{(_isDirty == true ? "*" : "")}{(asset ? asset.name : WindowTitle)}", icon);
        }

        protected virtual void OnCreateWindow()
        {

        }
        #endregion

        #region Initialization
        protected void InitializeIfRequired()
        {
            if (!initialized)
                Initialize();
        }

        protected virtual void Initialize()
        {
            initialized = true;
            EnsureWindowIsRegistered();
        }
        #endregion

        protected virtual void OnGUI()
        {
            InitializeIfRequired();
        }

        protected virtual void OnDestroy()
        {
            if (asset != null &&
                _openedWindows.ContainsKey(asset) && 
                _openedWindows[asset].Contains(this as TWindow))
                _openedWindows[asset].Remove(this as TWindow);
        }

        private void EnsureWindowIsRegistered()
        {
            if (asset == null)
                return;

            if (!_openedWindows.ContainsKey(asset))
                _openedWindows.Add(asset, new List<TWindow>());

            _openedWindows[asset].Add(this as TWindow);
        }

        protected static TWindow[] GetWindows() =>
            Resources.FindObjectsOfTypeAll<TWindow>();

        protected static TWindow GetWindow() =>
            GetWindows()
                .FirstOrDefault();

        #region Saving
        private string prefsKey_autoSave => $"{PrefsKeyPrefix}_autosave";
        private string prefskey_autoSaveTimeLimit => $"{PrefsKeyPrefix}_autosave_timelimit";

        private bool? prefs_autoSave = null;
        public bool Prefs_AutoSave
        {
            get
            {
                if (prefs_autoSave == null)
                    prefs_autoSave = EditorPrefs.GetBool(prefsKey_autoSave, true);

                return prefs_autoSave ?? false;
            }
            set
            {
                if (value == Prefs_AutoSave)
                    return;

                EditorPrefs.SetBool(prefsKey_autoSave, value);

                if (IsDirty)
                    Save();

                _lastSaveTime = (float)EditorApplication.timeSinceStartup;
                _waitForAutoSave = false;

                prefs_autoSave = value;
            }
        }

        private float? prefs_autoSaveTimeLimit = null;
        public float Prefs_AutoSaveTimeLimit
        {
            get
            {
                if (prefs_autoSaveTimeLimit == null)
                    prefs_autoSaveTimeLimit = EditorPrefs.GetFloat(prefskey_autoSaveTimeLimit, 0f);

                return prefs_autoSaveTimeLimit ?? 0f;
            }
            set
            {
                if (value == prefs_autoSaveTimeLimit)
                    return;

                value = Mathf.Max(0f, value);
                EditorPrefs.SetFloat(prefskey_autoSaveTimeLimit, value);

                prefs_autoSaveTimeLimit = value;
            }
        }

        bool _waitForAutoSave = false;
        public double TimeToAutoSave => _lastSaveTime - EditorApplication.timeSinceStartup;

        private float _lastSaveTime = float.NegativeInfinity;

        public bool CanAutoSave() =>
            _lastSaveTime < EditorApplication.timeSinceStartup;

        public void TEMP() =>
            Debug.Log(_waitForAutoSave);

        public float TimeSinceLastSave =>
            (float)EditorApplication.timeSinceStartup - _lastSaveTime;

        private bool? _isDirty = null;
        public bool IsDirty
        {
            get
            {
                if (asset == null)
                    return false;

                if (_isDirty == null)
                    _isDirty = EditorUtility.GetDirtyCount(asset) != 0;

                return _isDirty ?? false;
            }
        }

        public void SetAssetDirty()
        {
            _isDirty = true;
            EditorUtility.SetDirty(asset);
            UpdateWindowTitle();

            if (!Prefs_AutoSave) return;

            if (!CanAutoSave())
            {
                _waitForAutoSave = true;
                return;
            }

            Save();
        }

        public virtual void Save()
        {
            _lastSaveTime = (float)EditorApplication.timeSinceStartup;
            _waitForAutoSave = false;
            _isDirty = false;
            UpdateWindowTitle();
        }

        public void DiscardAssetChanges()
        {
            _isDirty = false;
            var unmodifiedJson = SessionState.GetString(UnmodifiedVersionKey, JsonUtility.ToJson(asset));
            var unmodified = JsonUtility.FromJson<TAsset>(unmodifiedJson);

            if (unmodified == null || asset == unmodified)
                return;

            AssetDatabase.SaveAssets();
            EditorUtility.ClearDirty(asset);
            asset = unmodified;
            DeleteUnmodifiedVersion();

            UpdateWindowTitle();
            OnDiscardAssetChanges();
        }

        protected virtual void OnDiscardAssetChanges()
        {

        }
        #endregion

        #region Unmodified Version
        private string UnmodifiedVersionKey => $"{PrefsKeyPrefix}_asset";

        private void SaveUnmodifiedVersion(TAsset asset) =>
            SessionState.SetString(UnmodifiedVersionKey, JsonUtility.ToJson(asset));

        private void DeleteUnmodifiedVersion() =>
            SessionState.EraseString(UnmodifiedVersionKey);
        #endregion

        #region GUI
        protected void DrawTreeView(TreeView tree)
        {
            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
            Rect rect = GUILayoutUtility.GetLastRect();
            tree?.OnGUI(rect);
        }
        #endregion
    }
}