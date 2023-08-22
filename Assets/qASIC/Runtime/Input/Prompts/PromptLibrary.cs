using UnityEngine;
using System;
using System.Collections.Generic;
using qASIC.Input.Map;
using qASIC.Input.Devices;
using System.Linq;

namespace qASIC.Input.Prompts
{
    [Serializable]
    public class PromptLibrary : ScriptableObject
    {
        public const string EXTENSION = "cbpl";

        [SerializeField] int defaultVariant = 0; 
        [SerializeField] List<PromptsVariant> variants = new List<PromptsVariant>();
        
        public List<PromptsVariant> Variants
        {
            get => variants;
            set => variants = value;
        }

        /// <returns>Returns the prompt variant for specified device.</returns>
        public PromptsVariant ForDevice(IInputDevice device)
        {
            if (variants.Count == 0 || device == null)
                return null;

            var targetPrompts = variants
                .Where(x => x.deviceTypes.Contains(device.DeviceType));

            return targetPrompts.Count() == 0 && variants.IndexInRange(defaultVariant) ?
                variants[defaultVariant] :
                targetPrompts.FirstOrDefault();
        }
    }
}