using System.Collections.Generic;
using UnityEngine;

namespace Project.Text
{
    /// <summary>Scriptable Object used for applying a text post processing effect in <see cref="TextPostProcessing"/>.</summary>
    public abstract class TextPostProcessorBase : ScriptableObject
    {
        [SerializeField] string[] tagNames;

        /// <summary>Tag names used for determining if this effect should be used.</summary>
        public string[] TagNames =>
            tagNames;

        /// <summary>Processes sections.</summary>
        /// <param name="data">Data from the start tag (e.g. for 'color=red' the data is 'red').</param>
        /// <param name="sections">List of sections containing text to apply the effect to.</param>
        /// <returns>If processing was successfull.</returns>
        public abstract bool Process(string data, List<TextPostProcessing.SectionData> sections);
    }
}