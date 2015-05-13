using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace ObjectDiff
{
    internal static class BinarySerializeService
    {
        internal static byte[] ObjectToByteArray(Object obj)
        {
            if (obj == null)
                return null;
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        public static Object ByteArrayToObject(byte[] arrBytes)
        {
            using (var stream = new MemoryStream())
            {
                var bf = new BinaryFormatter();
                stream.Write(arrBytes, 0, arrBytes.Length);
                stream.Seek(0, SeekOrigin.Begin);
                var obj = bf.Deserialize(stream);
                return obj;
            }
        }
    }
}
