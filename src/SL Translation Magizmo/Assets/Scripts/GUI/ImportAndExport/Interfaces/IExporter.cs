namespace Project.Translation.ImportAndExport
{
    public interface IExporter
    {
        string Name { get; }

        void BeginExport();
        void Export();
    }
}