using System;
using UnityEngine;
using System.Collections.Generic;

namespace qASIC
{
    [Flags]
    public enum RuntimePlatformFlags : ulong
    {
        None = 0,
        Everything = 17179869183,
        OSXEditor = 1,
        OSXPlayer = 2,
        WindowsPlayer = 4,
        WindowsEditor = 8,
        IPhonePlayer = 16,
        Android = 32,
        LinuxPlayer = 64,
        LinuxEditor = 128,
        WebGLPlayer = 256,
        WSAPlayerX86 = 512,
        WSAPlayerX64 = 1024,
        WSAPlayerARM = 2048,
        PS4 = 4096,
        XboxOne = 8192,
        tvOS = 16384,
        Switch = 32768,
        Stadia = 131072,
        GameCoreXboxSeries = 524288,
        GameCoreXboxOne = 1048576,
        PS5 = 2097152,
        EmbeddedLinuxArm64 = 4194304,
        EmbeddedLinuxArm32 = 8388608,
        EmbeddedLinuxX64 = 16777216,
        EmbeddedLinuxX86 = 33554432,
        LinuxServer = 67108864,
        WindowsServer = 134217728,
        OSXServer = 268435456,
        QNXArm32 = 536870912,
        QNXArm64 = 1073741824,
        QNXX64 = 2147483648,
        QNXX86 = 4294967296,
        VisionOS = 8589934592,
    }

    public static class RuntimePlatformExtensions
    {
        public static RuntimePlatformFlags ToRuntimePlatformFlags(this RuntimePlatform platform)
        {
            switch (platform)
            {
                case RuntimePlatform.OSXEditor:
                    return RuntimePlatformFlags.OSXEditor;
                case RuntimePlatform.OSXPlayer:
                    return RuntimePlatformFlags.OSXPlayer;
                case RuntimePlatform.WindowsPlayer:
                    return RuntimePlatformFlags.WindowsPlayer;
                case RuntimePlatform.WindowsEditor:
                    return RuntimePlatformFlags.WindowsEditor;
                case RuntimePlatform.IPhonePlayer:
                    return RuntimePlatformFlags.IPhonePlayer;
                case RuntimePlatform.Android:
                    return RuntimePlatformFlags.Android;
                case RuntimePlatform.LinuxPlayer:
                    return RuntimePlatformFlags.LinuxPlayer;
                case RuntimePlatform.LinuxEditor:
                    return RuntimePlatformFlags.LinuxEditor;
                case RuntimePlatform.WebGLPlayer:
                    return RuntimePlatformFlags.WebGLPlayer;
                case RuntimePlatform.WSAPlayerX86:
                    return RuntimePlatformFlags.WSAPlayerX86;
                case RuntimePlatform.WSAPlayerX64:
                    return RuntimePlatformFlags.WSAPlayerX64;
                case RuntimePlatform.WSAPlayerARM:
                    return RuntimePlatformFlags.WSAPlayerARM;
                case RuntimePlatform.PS4:
                    return RuntimePlatformFlags.PS4;
                case RuntimePlatform.XboxOne:
                    return RuntimePlatformFlags.XboxOne;
                case RuntimePlatform.tvOS:
                    return RuntimePlatformFlags.tvOS;
                case RuntimePlatform.Switch:
                    return RuntimePlatformFlags.Switch;
                case RuntimePlatform.Stadia:
                    return RuntimePlatformFlags.Stadia;
                case RuntimePlatform.GameCoreXboxSeries:
                    return RuntimePlatformFlags.GameCoreXboxSeries;
                case RuntimePlatform.GameCoreXboxOne:
                    return RuntimePlatformFlags.GameCoreXboxOne;
                case RuntimePlatform.PS5:
                    return RuntimePlatformFlags.PS5;
                case RuntimePlatform.EmbeddedLinuxArm64:
                    return RuntimePlatformFlags.EmbeddedLinuxArm64;
                case RuntimePlatform.EmbeddedLinuxArm32:
                    return RuntimePlatformFlags.EmbeddedLinuxArm32;
                case RuntimePlatform.EmbeddedLinuxX64:
                    return RuntimePlatformFlags.EmbeddedLinuxX64;
                case RuntimePlatform.EmbeddedLinuxX86:
                    return RuntimePlatformFlags.EmbeddedLinuxX86;
                case RuntimePlatform.LinuxServer:
                    return RuntimePlatformFlags.LinuxServer;
                case RuntimePlatform.WindowsServer:
                    return RuntimePlatformFlags.WindowsServer;
                case RuntimePlatform.OSXServer:
                    return RuntimePlatformFlags.OSXServer;
                case RuntimePlatform.QNXArm32:
                    return RuntimePlatformFlags.QNXArm32;
                case RuntimePlatform.QNXArm64:
                    return RuntimePlatformFlags.QNXArm64;
                case RuntimePlatform.QNXX64:
                    return RuntimePlatformFlags.QNXX64;
                case RuntimePlatform.QNXX86:
                    return RuntimePlatformFlags.QNXX86;
                case RuntimePlatform.VisionOS:
                    return RuntimePlatformFlags.VisionOS;
                default:
                    return RuntimePlatformFlags.None;
            }
        }

        public static List<RuntimePlatform> ToRuntimePlatform(this RuntimePlatformFlags flags)
        {
            List<RuntimePlatform> list = new List<RuntimePlatform>();
            var values = (RuntimePlatform[])Enum.GetValues(typeof(RuntimePlatform));

            foreach (var item in values)
            {
                var platform = item.ToRuntimePlatformFlags();
                if (platform == RuntimePlatformFlags.None) continue;

                if (flags.HasFlag(platform))
                    list.Add(item);
            }

            return list;
        }
    }
}
