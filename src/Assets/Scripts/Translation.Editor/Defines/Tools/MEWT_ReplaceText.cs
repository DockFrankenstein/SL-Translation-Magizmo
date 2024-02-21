using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Project.Editor.Translation.Defines.Tools
{
    public class MEWT_ReplaceText : MultiEntryWindowTool
    {
        public override string Name => "Replace Text";

        public enum TargetType
        {
            Id,
            DisplayName,
        };

        TargetType _type;
        string _fromText;
        string _toText;
        bool _useRegex;

        public override void Initialize()
        {
            _type = TargetType.DisplayName;
            _fromText = string.Empty;
            _toText = string.Empty;
            _useRegex = false;
        }

        public override void OnGUI()
        {
            _type = (TargetType)EditorGUILayout.EnumPopup("Target Type", _type);
            _useRegex = EditorGUILayout.Toggle("Use Regex", _useRegex);

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Find What");
            _fromText = EditorGUILayout.TextArea(_fromText, GUILayout.Height(100f));

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Replace With");
            _toText = EditorGUILayout.TextArea(_toText, GUILayout.Height(100f));

            EditorGUILayout.Space();

            if (GUILayout.Button("Replace", GUILayout.Height(36f)))
                Replace();
        }

        void Replace()
        {
            foreach (var line in Window.asset.lines)
            {
                foreach (var field in line.fields)
                {
                    switch (_type)
                    {
                        case TargetType.Id:
                            field.id = ReplaceText(field.id);
                            break;
                        case TargetType.DisplayName:
                            field.displayName = ReplaceText(field.displayName);
                            break;
                    }
                }
            }

            Window.SetAssetDirty();

            string ReplaceText(string text)
            {
                if (_useRegex)
                    return Regex.Replace(text, _fromText, _toText);

                return text.Replace(_fromText, _toText);
            }
        }
    }
}