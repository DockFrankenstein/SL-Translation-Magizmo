using Project.Translation.Defines;
using qASIC;

namespace Project
{
    public static partial class ProjectDebug
    {
        public static void LogValueImport(string fieldName, object value)
        {
            qDebug.Log($"Imported '{fieldName}' value: {value?.ToString() ?? string.Empty}");
        }

        public static void LogValueImport(DefineField defineField, object value) =>
            LogValueImport(defineField.id, value);
    }
}