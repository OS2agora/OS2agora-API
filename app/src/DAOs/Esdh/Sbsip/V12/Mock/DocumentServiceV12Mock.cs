using BallerupKommune.DAOs.Esdh.Sbsip.DTOs;
using BallerupKommune.DAOs.Esdh.Sbsip.V12.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BallerupKommune.DAOs.Esdh.Sbsip.V12.Mock
{
    public class DocumentServiceV12Mock : IDocumentServiceV12
    {
        public async Task<List<DelforloebOutputDtoV10>> GetDocumentTypes(int caseId)
        {
            var documentTypes = new List<DelforloebOutputDtoV10>();
            return documentTypes;
        }

        public async Task<DokumentMetadataDtoV12> JournaliseDocument(int caseId, byte[] file, string fileName, string fileDescription, string fileContentType)
        {
            var documentMetaData = new DokumentMetadataDtoV12();
            return documentMetaData;
        }

        public async Task<DokumentMetadataDtoV10> ConnectDocumentAndCase(int documentTypeId, int documentId)
        {
            var documentMetaData = new DokumentMetadataDtoV10();
            return documentMetaData;
        }
    }
}