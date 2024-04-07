using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Project
{
    public static partial class PUtility
    { 
        [DllImport("user32.dll", EntryPoint = "SetWindowText")]
        static extern bool SetWindowText(System.IntPtr hwnd, System.String lpString);
        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        static extern System.IntPtr FindWindow(System.String className, System.String windowName);

        public static string GenerateDisplayName(string id)
        {
            return string.Join(" ", id
                .Split('_')
                .Where(x => x.Length > 0)
                .Select(x => $"{x[0].ToString().ToUpper()}{x.Substring(1, x.Length - 1)}"));
        }

        static System.IntPtr? _windowPtr = null;
        public static System.IntPtr WindowPtr
        {
            get
            {
                if (_windowPtr == null)
                    _windowPtr = FindWindow(null, Application.productName);

                return _windowPtr ?? System.IntPtr.Zero;
            }
        }

        public static void ChangeWindowTitle(string title) =>
            SetWindowText(WindowPtr, title);
    }
}