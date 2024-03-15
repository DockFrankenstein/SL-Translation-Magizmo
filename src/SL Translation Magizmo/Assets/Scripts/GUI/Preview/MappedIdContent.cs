using Project.Translation;
using UnityEngine;

namespace Project.GUI.Preview
{
    public abstract class MappedIdContent : MonoBehaviour
    {
        public virtual string GetContent(TranslationManager manager, string id) =>
            string.Empty;
    }
}