using qASIC.Files;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Project
{
    [Serializable]
    public class RecentsManager : IEnumerable<string>
    {
        public const int MAX_RECENTS = 64;

        public GenericFilePath path;

        List<string> Recents { get; set; } = new List<string>();

        public event Action OnRecentsChanged;

        public void Load()
        {
            var fullPath = path.GetFullPath();

            if (!File.Exists(fullPath))
                return;

            var txt = File.ReadAllText(fullPath);

            Recents = txt.SplitByLines()
                .ToList();

            EnsureCorrectSize();
            OnRecentsChanged?.Invoke();
        }

        public void Add(string item)
        {
            Recents.Insert(0, item);

            EnsureCorrectSize();

            var fullPath = path.GetFullPath();
            var containingFolder = Path.GetDirectoryName(fullPath);

            if (!Directory.Exists(containingFolder))
                Directory.CreateDirectory(containingFolder);

            File.WriteAllText(fullPath, string.Join("\n", Recents));

            OnRecentsChanged?.Invoke();
        }

        void EnsureCorrectSize()
        {
            while (Recents.Count > MAX_RECENTS)
                Recents.RemoveAt(Recents.Count - 1);
        }

        public IEnumerator<string> GetEnumerator() =>
            Recents.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();
    }
}