using qASIC;

namespace Project
{
    public static partial class ProjectDebug
    {
        public static void LogValueImport(string fieldName, object value)
        {
            qDebug.Log($"Imported '{fieldName}' value: {value?.ToString() ?? string.Empty}");
        }
    }
}