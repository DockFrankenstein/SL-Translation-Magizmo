namespace Project.Translation.ImportAndExport
{
    public interface IImporter
    {
        string Name { get; }

        void BeginImport();
    }
}