using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FusionClient.Utils.Manager
{
    public class CrashManager
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsInvalid(Vector3 vector)
        {
            return float.IsNaN(vector.x) || float.IsNaN(vector.x) ||
                float.IsNaN(vector.y) || float.IsNaN(vector.y) ||
                float.IsNaN(vector.z) || float.IsNaN(vector.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsInvalid(Quaternion quaternion)
        {
            return float.IsNaN(quaternion.w) || float.IsNaN(quaternion.w) ||
                float.IsNaN(quaternion.x) || float.IsNaN(quaternion.x) ||
                float.IsNaN(quaternion.y) || float.IsNaN(quaternion.y) ||
                float.IsNaN(quaternion.z) || float.IsNaN(quaternion.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Clamp(float value, float min, float max)
        {
            if (value < min)
                return min;
            if (value > max)
                return max;

            return value;
        }
    }
}
