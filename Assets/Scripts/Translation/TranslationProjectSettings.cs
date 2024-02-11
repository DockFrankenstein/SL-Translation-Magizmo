using UnityEngine;
using Project.Translation.Defines;
using System.Collections.Generic;
using UnityEngine.Windows;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Project.Translation
{
    public class TranslationProjectSettings : ScriptableObject
    {
        const string PATH = "Project Settings";

        public List<TranslationVersion> translationVersions = new List<TranslationVersion>();

        public static TranslationProjectSettings _instance;
        public static TranslationProjectSettings Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Resources.Load<TranslationProjectSettings>($"{PATH}/Translation");

#if UNITY_EDITOR
                    if (_instance == null)
                    {
                        _instance = CreateInstance<TranslationProjectSettings>();

                        Directory.CreateDirectory($"{Application.dataPath}/Resources/{PATH}");

                        AssetDatabase.CreateAsset(_instance, $"Assets/Resources/{PATH}/Translation.asset");
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                        Debug.Log("Created New Translation Settings");
                    }
#endif
                }

                return _instance;
            }
        }
    }
}