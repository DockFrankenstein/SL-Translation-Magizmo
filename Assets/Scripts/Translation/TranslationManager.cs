using UnityEngine;
using Project.Translation.Data;
using Project.Translation.Defines;
using System;

namespace Project.Translation
{
    public class TranslationManager : MonoBehaviour
    {
        public TranslationVersion[] versions;

        public AppFile file;

        private string _selectedItem = null;
        public string SelectedItem 
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                OnSelectionChange?.Invoke(value);
            }
        }

        public event Action<string> OnSelectionChange;

        public TranslationVersion CurrentVersion =>
            versions.Length == 0 ?
            null :
            versions[versions.Length - 1];

        private void Awake()
        {
            file = AppFile.Create(CurrentVersion);
        }
    }
}