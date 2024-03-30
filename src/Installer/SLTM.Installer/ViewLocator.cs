using Avalonia.Controls;
using Avalonia.Controls.Templates;
using SLTM.Installer.ViewModels;
using System;

namespace SLTM.Installer
{
    public class ViewLocator : IDataTemplate
    {
        public Control Build(object? data)
        {
            if (data is null)
                return new TextBlock() { Text = "data was null" };

            var name = data.GetType().FullName!.Replace("ViewModel", "View");
            var type = Type.GetType(name);

            if (type == null)
                return new TextBlock { Text = $"Not Found: {name}" };

            return (Control)Activator.CreateInstance(type)!;
        }

        public bool Match(object? data) =>
            data is ViewModelBase;
    }
}
