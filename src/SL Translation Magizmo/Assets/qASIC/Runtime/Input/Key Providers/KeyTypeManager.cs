using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace qASIC.Input.KeyProviders
{
    public static class KeyTypeManager
    {
        public const string KEY_PATH_ALLOWED_CHARACTERS = "AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpRrSsTtUuVvWwXxYyZz1234567890_- ";

        private static KeyTypeProvider[] _keyTypeProviders = null;
        public static KeyTypeProvider[] KeyTypeProviders
        {
            get
            {
                if (_keyTypeProviders == null)
                    _keyTypeProviders = GetEveryKeyType();

                return _keyTypeProviders;
            }
        }

        private static Dictionary<string, KeyTypeProvider> _keyTypeProvidersDictionary = null;
        public static Dictionary<string, KeyTypeProvider> KeyTypeProvidersDictionary
        {
            get
            {
                if (_keyTypeProvidersDictionary == null)
                    _keyTypeProvidersDictionary = KeyTypeProviders
                        .ToDictionary(x => x.RootPath);

                return _keyTypeProvidersDictionary;
            }
        }

        public static KeyTypeProvider[] GetEveryKeyType()
        {
            IEnumerable<Type> deviceTypes = TypeFinder.FindAllTypes<KeyTypeProvider>()
                .Where(x => x != null && x.IsClass && !x.IsAbstract);

            List<KeyTypeProvider> types = new List<KeyTypeProvider>();
            foreach (var deviceType in deviceTypes)
            {
                ConstructorInfo constructor = deviceType.GetConstructor(Type.EmptyTypes);
                KeyTypeProvider device = (KeyTypeProvider)constructor.Invoke(null);
                types.Add(device);
            }

            return types
                .GroupBy(x => x.RootPath)
                .Select(x => x.First())
                .ToArray();
        }

        static string[] _keyPaths = null;
        public static string[] KeyPaths
        {
            get
            {
                if (_keyPaths == null)
                {
                    _keyPaths = KeyTypeProviders
                        .SelectMany(x => x.KeyPaths
                            .Select(y => $"{x.RootPath}/{y}"))
                        .Where(x => IsKeyPathCorrectlyFormatted(x, true))
                        .ToArray();

                    //Add key paths from 
                    _keyPaths = TypeFinder.FindMethodsWithAttribute<KeyPathsAttribute>()
                        .Where(x => x.ReturnType == typeof(string[]) && x.IsStatic)
                        .Select(x => (string[])x.Invoke(null, new object[0]))
                        .SelectMany(x => x)
                        .Where(x => IsKeyPathCorrectlyFormatted(x, true))
                        .Concat(_keyPaths)
                        .ToArray();
                }

                return _keyPaths;
            }
        }

        public static KeyTypeProvider GetProviderByRootPath(string rootPath)
        {
            var targets = KeyTypeProviders.Where(x => x.RootPath == rootPath);

            if (targets.Count() == 1)
                return targets.First();

            return null;
        }

        public static KeyTypeProvider GetProviderFromPath(string path)
        {
            string rootPath = path.Split('/').FirstOrDefault();
            return GetProviderByRootPath(rootPath);
        }

        public static bool IsKeyPathCorrectlyFormatted(string keyPath, bool log = false)
        {
            //Split path into directories
            var pathDir = keyPath.Split('/');
            bool correctFormatting = true;


            // Key is a root path
            correctFormatting &= pathDir.Length > 1;

            // There are empty directories in path
            correctFormatting &= pathDir.All(x => !string.IsNullOrEmpty(x));

            // Path is not using correct formatting
            correctFormatting &= string.Join(string.Empty, pathDir)
                .All(x => KEY_PATH_ALLOWED_CHARACTERS.Contains(x));
            

            if (!correctFormatting && log)
                qDebug.Log($"[Key Path] Key Path '{keyPath}' is using incorrect formatting");

            return correctFormatting;
        }
    }
}
