using Project.Translation.Mapping;
using qASIC;

namespace Project
{
    public static partial class ProjectDebug
    {
        public static void LogValueImport(string fieldName, object value)
        {
            UnityEngine.Debug.Log($"Imported '{fieldName}' value: {value?.ToString() ?? string.Empty}");
        }

        public static void LogValueImport(MappedField defineField, object value) =>
            LogValueImport(defineField.id, value);
    }
}