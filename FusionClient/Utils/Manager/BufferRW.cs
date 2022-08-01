using System;
using UnityEngine;

namespace FC.Utils
{
    internal class BufferRW
    {
        public static byte[] Vector3ToBytes(Vector3 vector3)
        {
            byte[] buffer = new byte[12];
            Buffer.BlockCopy(BitConverter.GetBytes(vector3.x), 0, buffer, 0, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(vector3.y), 0, buffer, 4, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(vector3.z), 0, buffer, 8, 4);
            return buffer;
        }
        public static byte[] Vector4ToBytes(Vector4 vector4)
        {
            byte[] buffer = new byte[16];
            Buffer.BlockCopy(BitConverter.GetBytes(vector4.x), 0, buffer, 0, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(vector4.y), 0, buffer, 4, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(vector4.z), 0, buffer, 8, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(vector4.w), 0, buffer, 12, 4);
            return buffer;
        }
        public static byte[] QuaternionToBytes(Quaternion quaternion)
        {
            byte[] buffer = new byte[16];
            Buffer.BlockCopy(BitConverter.GetBytes(quaternion.x), 0, buffer, 0, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(quaternion.y), 0, buffer, 4, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(quaternion.z), 0, buffer, 8, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(quaternion.w), 0, buffer, 12, 4);
            return buffer;
        }
        public static byte[] ShortQuaternionToBytes(Quaternion quaternion)
        {
            byte[] buffer = new byte[16 / 2];
            Buffer.BlockCopy(BitConverter.GetBytes(Mathf.FloatToHalf(quaternion.x)), 0, buffer, 0, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(Mathf.FloatToHalf(quaternion.y)), 0, buffer, 2, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(Mathf.FloatToHalf(quaternion.z)), 0, buffer, 4, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(Mathf.FloatToHalf(quaternion.w)), 0, buffer, 6, 2);
            return buffer;
        }
        public static Vector2 ReadVector2(byte[] buffer, int index)
        {
            var x = BitConverter.ToSingle(buffer, index);
            var y = BitConverter.ToSingle(buffer, index + 4);
            return new Vector2(x, y);
        }
        public static Vector3 ReadVector3(byte[] buffer, int index)
        {
            var x = BitConverter.ToSingle(buffer, index);
            var y = BitConverter.ToSingle(buffer, index + 4);
            var z = BitConverter.ToSingle(buffer, index + 8);
            return new Vector3(x, y, z);
        }
        public static byte[] WriteVector3(Vector3 Vector3, int index, byte[] buffer)
        {
            Buffer.BlockCopy(BitConverter.GetBytes(Vector3.x), 0, buffer, index, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(Vector3.y), 0, buffer, index + 4, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(Vector3.z), 0, buffer, index + 8, 4);
            return buffer;
        }
        public static Vector4 ReadVector4(byte[] buffer, int index)
        {
            var x = BitConverter.ToSingle(buffer, index);
            var y = BitConverter.ToSingle(buffer, index + 4);
            var z = BitConverter.ToSingle(buffer, index + 8);
            var w = BitConverter.ToSingle(buffer, index + 12);
            return new Vector4(x, y, z, w);
        }
        public static byte[] WriteVector4(Vector4 Vector4, int index, byte[] buffer)
        {
            Buffer.BlockCopy(BitConverter.GetBytes(Vector4.x), 0, buffer, index, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(Vector4.y), 0, buffer, index + 4, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(Vector4.z), 0, buffer, index + 8, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(Vector4.w), 0, buffer, index + 12, 4);
            return buffer;
        }
        public static Quaternion ReadQuaternion(byte[] buffer, int index)
        {
            var x = BitConverter.ToSingle(buffer, index);
            var y = BitConverter.ToSingle(buffer, index + 4);
            var z = BitConverter.ToSingle(buffer, index + 8);
            var w = BitConverter.ToSingle(buffer, index + 12);
            return new Quaternion(x, y, z, w);
        }
        public static Quaternion ReadShortQuaternion(byte[] buffer, int index)
        {
            var x = BitConverter.ToUInt16(buffer, index);
            var y = BitConverter.ToUInt16(buffer, index + 2);
            var z = BitConverter.ToUInt16(buffer, index + 4);
            var w = BitConverter.ToUInt16(buffer, index + 6);
            return new Quaternion(Mathf.HalfToFloat(x), Mathf.HalfToFloat(y), Mathf.HalfToFloat(z), Mathf.HalfToFloat(w));
        }
        public static byte[] WriteQuaternion(Quaternion quaternion, int index, byte[] buffer)
        {
            Buffer.BlockCopy(QuaternionToBytes(quaternion), 0, buffer, index, 16);
            return buffer;
        }
        public static byte[] WriteShortQuaternion(Quaternion quaternion, int index, byte[] buffer)
        {
            Buffer.BlockCopy(ShortQuaternionToBytes(quaternion), 0, buffer, index, 16);
            return buffer;
        }
        public static float[] ReadSingleArray(byte[] buffer, int index, int length)
        {
            float[] ResultBuffer = new float[length];
            for (int i = 0; i < length; i++)
                ResultBuffer[i] = BitConverter.ToSingle(buffer, index + (i * 4));
            return ResultBuffer;
        }
        public static int[] ReadintArray(byte[] buffer, int index, int length)
        {
            int[] ResultBuffer = new int[length];
            for (int i = 0; i < length; i++)
                ResultBuffer[i] = BitConverter.ToInt32(buffer, index + (i * 4));
            return ResultBuffer;
        }
        public static short[] ReadShortArray(byte[] buffer, int index, int length)
        {
            short[] ResultBuffer = new short[length];
            for (int i = 0; i < length; i++)
                ResultBuffer[i] = BitConverter.ToInt16(buffer, index + (i * 2));
            return ResultBuffer;
        }
        public static void WriteShortArray(byte[] buffer, int index, short[] sex)
        {
            for (int i = 0; i < sex.Length; i++)
            {
                byte[] ShortBytes = BitConverter.GetBytes(sex[i]);
                Buffer.BlockCopy(ShortBytes, 0, buffer, index + (i * 2), 2);
            }
        }
    }
}
