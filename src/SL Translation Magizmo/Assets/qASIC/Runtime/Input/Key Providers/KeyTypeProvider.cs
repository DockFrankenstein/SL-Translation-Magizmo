using System;
using System.Linq;

namespace qASIC.Input.KeyProviders
{
    public abstract class KeyTypeProvider
    {
        public abstract string RootPath { get; }
        public abstract string DisplayName { get; }

        /// <summary>Enum that represents key paths</summary>
        public virtual Type ButtonEnum { get; } = null;

        public abstract string[] KeyPaths { get; }
    }

    public abstract class KeyTypeProvider<T> : KeyTypeProvider where T : Enum
    {
        public override Type ButtonEnum => typeof(T);

        private string[] _keyPaths = null;
        public override string[] KeyPaths
        {
            get
            {
                //Cache
                if (_keyPaths == null)
                    _keyPaths = ((T[])Enum.GetValues(typeof(T)))
                        .Select(x => x.ToString())
                        .ToArray();

                return _keyPaths;
            }
        }
    }
}