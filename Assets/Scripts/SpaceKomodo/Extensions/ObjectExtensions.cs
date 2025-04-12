using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace SpaceKomodo.Extensions
{
    public static class ObjectExtensions
    {
        public static T DeepClone<T>(this T sourceObject)
        {
            using var memoryStream = new MemoryStream();
            var formatter = new BinaryFormatter();
            formatter.Serialize(memoryStream, sourceObject);
            memoryStream.Position = 0;

            return (T) formatter.Deserialize(memoryStream);
        }
    }
}