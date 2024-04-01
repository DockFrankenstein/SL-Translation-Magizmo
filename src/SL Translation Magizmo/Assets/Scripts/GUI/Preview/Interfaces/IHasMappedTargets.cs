using System.Collections.Generic;

namespace Project.GUI.Preview.Interfaces
{
    public interface IHasMappedTargets
    {
        IEnumerable<MappedIdTarget> GetListOfTargets();
        void ChangeTarget(string id);
    }
}