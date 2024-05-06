using System;
using System.Linq;
using System.Runtime.Serialization;
using UnityEngine;

namespace Project.Translation.Data
{
    public class SaveFileSerializer
    {
        public SaveFileSerializer()
        {

        }

        public SaveFileSerializer(TranslationManager manager)
        {
            Updater = new TranslationFileUpdater(manager);
        }

        public TranslationFileUpdater Updater { get; private set; } = null;

        public SaveFile Load(string path)
        {
            var txt = System.IO.File.ReadAllText(path);

            var lines = txt.SplitByLines();
            var fileVersionString = lines.First();
            var fileVersion = TranslationFileUpdater.CURRENT_FILE_VERSION;

            if (int.TryParse(fileVersionString, out int newFileVersion))
            {
                if (newFileVersion < TranslationFileUpdater.LOWEST_SUPPORTED_FILE_VERSION)
                    throw new SerializerException($"This file has been saved in an older version ({newFileVersion}) that is no longer supported (lowest supported version: {TranslationFileUpdater.LOWEST_SUPPORTED_FILE_VERSION}).");

                if (newFileVersion > fileVersion)
                    throw new SerializerException($"This file has been saved in a newer version ({newFileVersion}, current version: {TranslationFileUpdater.CURRENT_FILE_VERSION}). You have to update the application in order to load it.");

                fileVersion = newFileVersion;
                txt = string.Join("\n", lines.Skip(1));
            }

            var file = JsonUtility.FromJson<SaveFile>(txt);

            if (Updater != null)
                Updater.EnsureFileIsUpToDate(file, fileVersion);

            return file;
        }

        public void Save(string path, SaveFile file)
        {
            var json = JsonUtility.ToJson(file, true);
            var txt = $"{TranslationFileUpdater.CURRENT_FILE_VERSION}\n{json}";
            System.IO.File.WriteAllText(path, txt);
        }

        public class SerializerException : Exception
        {
            public SerializerException() : base() { }
            public SerializerException(string message) : base(message) { }
            public SerializerException(string message, Exception innerException) : base(message, innerException) { }
            protected SerializerException(SerializationInfo info, StreamingContext context) : base(info, context) { }
        }
    }
}