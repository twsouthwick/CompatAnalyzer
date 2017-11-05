using System.IO;

namespace CompatibilityAnalyzer.Models
{
    public static class SerializerExtensions
    {
        public static byte[] Serialize(this IModelSerializer serializer, AnalyzeRequest request)
        {
            using (var ms = new MemoryStream())
            {
                serializer.Serialize(request, ms);

                return ms.ToArray();
            }
        }

        public static AnalyzeRequest Deserialize(this IModelSerializer serializer, byte[] bytes)
        {
            using (var ms = new MemoryStream(bytes))
            {
                return serializer.Deserialize(ms);
            }
        }
    }
}
