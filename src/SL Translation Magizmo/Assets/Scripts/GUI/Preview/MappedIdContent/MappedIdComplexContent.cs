using UnityEngine;
using Project.Translation;
using System;
using System.Collections.Generic;

namespace Project.GUI.Preview
{
    public class MappedIdComplexContent : MappedIdContent
    {
        public Element element;

        public override string[] GetContent(GetContentArgs args, object context)
        {
            return new string[] { GetContent(args.manager, element) };
        }

        string GetContent(TranslationManager manager, Element element)
        {
            var txt = element.defaultContent;

            if (!string.IsNullOrEmpty(element.id) &&
                manager.File.Entries.TryGetValue(element.id, out var val) &&
                !string.IsNullOrWhiteSpace(val.content))
                txt = val.content;

            foreach (var item in element.dynamicValues)
            {
                if (item == null) continue;
                var dynamicVal = item.GetContent(manager);
                txt = txt.Replace(item.tag, dynamicVal);
            }

            return txt;
        }

        [Serializable]
        public class Element
        {
            public string id;
            public string defaultContent;

            [SerializeReference, Subclass(IsList = true)]
            public List<DynamicValue> dynamicValues = new List<DynamicValue>();

            [Serializable]
            public abstract class DynamicValue
            {
                public string tag;

                public abstract string GetContent(TranslationManager manager);
            }

            public class StaticValue : DynamicValue
            {
                public string staticContent;

                public override string GetContent(TranslationManager manager) =>
                    staticContent;
            }

            public class EntryValue : DynamicValue
            {
                public Element entryContent;

                public override string GetContent(TranslationManager manager) =>
                    manager.File.Entries.TryGetValue(entryContent.id, out var val) && !string.IsNullOrWhiteSpace(val.content) ?
                            val.content :
                            entryContent.defaultContent;
            }
        }
    }
}