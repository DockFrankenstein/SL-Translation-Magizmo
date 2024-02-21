using UnityEngine;

namespace Project.GUI.Inspector
{
    public abstract class InspectorNameProvider : MonoBehaviour
    {
        public abstract bool TryGetName(IApplicationObject obj, out string name);
    }
}