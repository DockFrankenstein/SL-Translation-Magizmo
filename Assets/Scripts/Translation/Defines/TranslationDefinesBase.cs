﻿using UnityEngine;
using Project.Translation.Data;

namespace Project.Translation.Defines
{
    public abstract class TranslationDefinesBase : ScriptableObject
    {
        public string fileName;

        public abstract string[] GetDefines();

        public abstract void Import(AppFile file, string txt);
        public abstract string Export(AppFile file);
    }
}
