using UnityEngine;

namespace qASIC
{
    public static partial class qDebug
    {
        public static void Log(object message, Color color) =>
            Log(message, new qColor((byte)(color.r * 255), (byte)(color.g * 255), (byte)(color.b * 255), (byte)(color.a * 255)));
    }
}