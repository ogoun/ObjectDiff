namespace ObjectDiff.Internal.Serialize
{
    internal static class ObjSerializer
    {
        public static string Serialize(object obj)
        {
            return obj.ToString();
        }

        public static object Deserialize(string serializedString)
        {
            return serializedString;
        }
    }
}
