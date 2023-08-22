using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SFB;
using TMPro;

namespace Project.UI
{
    public class PathField : MonoBehaviour
    {
        public TMP_InputField inputField;

        [Label("Settings")]
        public string extension;

        public string Path
        {
            get => inputField == null ? string.Empty : inputField.text;
            set
            {
                if (inputField != null)
                    inputField.text = value;
            }
        }

        public void OpenFileBrowser()
        {
            var paths = string.IsNullOrWhiteSpace(extension) ?
                StandaloneFileBrowser.OpenFolderPanel("", Path, false) :
                StandaloneFileBrowser.OpenFilePanel("", Path, extension, false);

            if (paths.Length > 0)
                Path = paths[0];    
        }
    }
}