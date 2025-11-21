using MigraDoc.DocumentObjectModel;

namespace Agora.Operations.Common.Interfaces.Files.Pdf
{
    public interface IPdfForm
    {
        Document GenerateContent();
        void CleanUp();
    }
}
