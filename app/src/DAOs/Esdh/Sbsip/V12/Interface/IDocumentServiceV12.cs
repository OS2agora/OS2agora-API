using System;
using System.Collections.Generic;
using System.Text;
using BallerupKommune.DAOs.Esdh.Sbsip.DTOs;
using System.Threading.Tasks;

namespace BallerupKommune.DAOs.Esdh.Sbsip.V12.Interface
{
    public interface IDocumentServiceV12
    {
        Task<List<DelforloebOutputDtoV10>> GetDocumentTypes(int caseId);

        Task<DokumentMetadataDtoV12> JournaliseDocument(int caseId, byte[] file, string fileName, string fileDescription, string fileContentType);

        Task<DokumentMetadataDtoV10> ConnectDocumentAndCase(int documentTypeId, int documentId);
    }
}
