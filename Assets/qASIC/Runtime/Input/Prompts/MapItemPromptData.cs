using System.Collections.Generic;
using qASIC.Input.Map;
using System.Linq;

namespace qASIC.Input.Prompts
{
    public class MapItemPromptData
    {
        public MapItemPromptData()
        {

        }

        public MapItemPromptData(params PromptGroup[] groups) : this()
        {
            promptGroups = groups.ToList();
            requiredKeyPathCount = groups.Length;
        }

        public MapItemPromptData(params InputBinding[] bindings) : this()
        {
            foreach (var binding in bindings)
            {
                for (int i = 0; i < binding.keys.Count; i++)
                {
                    if (promptGroups.Count <= i)
                        promptGroups.Add(new PromptGroup());

                    promptGroups[i].keyPaths.Add(binding.keys[i]);
                }
            }

            requiredKeyPathCount = bindings.Length;
        }

        public MapItemPromptData(InputMap map, params string[] guids) : this(guids
            .Select(x => map.GetItem<InputBinding>(x))
            .Where(x => x != null)
            .ToArray())
        { }

        public int requiredKeyPathCount = 0;
        public List<PromptGroup> promptGroups = new List<PromptGroup>();

        public class PromptGroup
        {
            public PromptGroup() { }

            public PromptGroup(List<string> keyPaths) : this()
            {
                this.keyPaths = keyPaths;
            }

            public List<string> keyPaths = new List<string>();
        }
    }
}