using qASIC;
using UnityEditor;
using UnityEngine;
using Unity.EditorCoroutines.Editor;

namespace Project.AutoUpdate.Editor.Windows
{
    public class AutoUpdaterTester : EditorWindow
    {
        [MenuItem("Window/Project/Auto Updater Tester")]
        public static void OpenWindow()
        {
            var window = CreateWindow<AutoUpdaterTester>("Auto Updater Tester");
            window.Show();
        }

        [SerializeField] AutoUpdater updater; 

        private void OnGUI()
        {
            if (updater == null)
            {
                updater = new AutoUpdater()
                {
                    CurrentVersion = Application.version,
                    ResultPath = $"{Application.dataPath}/Download.zip"
                };
            }

            EditorGUILayout.LabelField("Version", EditorStyles.boldLabel);

            updater.CurrentVersion = EditorGUILayout.TextField("Current Version", updater.CurrentVersion ?? string.Empty);
            using (new GUILayout.HorizontalScope())
            {
                updater.NewVersion = EditorGUILayout.TextField("New Version", updater.NewVersion);

                if (GUILayout.Button("Fetch", GUILayout.Width(100f)))
                    this.StartCoroutine(updater.GetVersion());
            }

            EditorGUILayout.LabelField("File", EditorStyles.boldLabel);
            updater.TargetFileName = EditorGUILayout.TextField("Target File Name", updater.TargetFileName);
            updater.ResultPath = EditorGUILayout.TextField("Result Path", updater.ResultPath);

            EditorGUILayout.Space();

            if (GUILayout.Button("Reset download"))
                updater.IsDownloading = false;

            GUILayout.FlexibleSpace();

            using (new EditorGUI.DisabledGroupScope(updater.IsDownloading))
            {
                if (GUILayout.Button("Download"))
                    this.StartCoroutine(updater.DownloadUpdate());
            }
        }
    }
}