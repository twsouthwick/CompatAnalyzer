using System.IO;

namespace CompatibilityAnalyzer.Models
{
    public interface IModelSerializer
    {
        AnalyzeRequest Deserialize(Stream stream);

        void Serialize(AnalyzeRequest request, Stream stream);
    }
}
