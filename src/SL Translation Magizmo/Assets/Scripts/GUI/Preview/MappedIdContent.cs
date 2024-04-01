using Project.Translation;
using System;
using UnityEngine;

namespace Project.GUI.Preview
{
    public abstract class MappedIdContent : MonoBehaviour
    {
        [EditorButton(nameof(AssignToEveryId))]
        [SerializeField] [Hide] string s;

        public virtual Type ContextType => null;

        public virtual string[] GetContent(GetContentArgs args, object context) =>
            new string[] { "" };

        public struct GetContentArgs
        {
            public TranslationManager manager;
            public string id;
            public string[] normalContent;
            public string defaultContent;
        }

        void AssignToEveryId()
        {
            var entry = GetComponent<PreviewEntry>();
            entry.mainId.content = this;
            for (int i = 0; i < entry.otherIds.Length; i++)
                entry.otherIds[i].content = this;

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }
    }

    public abstract class MappedIdContent<T> : MappedIdContent
    {
        public override Type ContextType => typeof(T);

        public override string[] GetContent(GetContentArgs args, object context) =>
            GetContentWithContext(args, (T)context);

        public virtual string[] GetContentWithContext(GetContentArgs args, T context) =>
            new string[] { "" };
    }
}