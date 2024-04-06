using Project.GUI.Hierarchy;
using Project.Translation;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Project.GUI.Preview
{
    public class PreviewElement : MonoBehaviour
    {
        [Label("Linking")]
        public PreviewElement[] linkedElements;

        [HideInInspector] public TranslationManager manager;
        [HideInInspector] public HierarchyController hierarchy;

        protected virtual void Awake()
        {
            Reload();
        }

        public virtual void Reload() { }
        public virtual void Select() { }

        /// <summary>Changes the selected index in the element.</summary>
        /// <param name="newIndex">The new index value.</param>
        /// <param name="silent">If true, element will not change other linked elements.</param>
        public virtual void ChangeIndex(int newIndex, bool silent = false)
        {
            if (!silent)
            {
                ChangeIndexInLinked(newIndex);
            }
        }

        protected void ChangeIndexInLinked(int newIndex)
        {
            var elements = new List<PreviewElement>();
            var newElements = new List<PreviewElement>()
                {
                    this,
                };

            while (newElements.Count > 0)
            {
                elements.AddRange(newElements);

                newElements = newElements
                    .Where(x => x != null)
                    .SelectMany(x => x.linkedElements)
                    .Except(elements)
                    .ToList();
            }

            elements.Remove(this);

            foreach (var item in elements)
            {
                item.ChangeIndex(newIndex, true);
            }
        }
    }
}