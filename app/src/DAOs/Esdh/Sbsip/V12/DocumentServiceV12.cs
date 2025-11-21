using System;
using Agora.DAOs.Esdh.Sbsip.DTOs;
using Agora.DAOs.Esdh.Sbsip.V12.Interface;
using Agora.Operations.ApplicationOptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Agora.DAOs.Esdh.Sbsip.V12
{
    public class DocumentServiceV12 : BaseService, IDocumentServiceV12
    {
        private readonly ILogger<DocumentServiceV12> _logger;

        public DocumentServiceV12(HttpClient httpClient, IOptions<SbsipOptions> sbsipOptions,
            TokenService sbsipTokenService, ILogger<DocumentServiceV12> logger,
            ILogger<BaseService> baseServiceLogger) : base(httpClient, sbsipOptions, sbsipTokenService,
            baseServiceLogger)
        {
            _logger = logger;
        }

        public async Task<List<DelforloebOutputDtoV10>> GetDocumentTypes(int caseId)
        {
            HttpResponseMessage response = null;
            try
            {
                response = await HttpClient.GetAsync($"delforloeb/sag/{caseId}");
                response.EnsureSuccessStatusCode();
            }
            catch (Exception exception)
            {
                _logger.LogWarning("Failed to get document type. CaseId=<{CaseId}> ErrorMessage=<{ErrorMessage}>.",
                    caseId, exception.Message);
                throw;
            }

            var result = await response.Content.ReadAsStringAsync();

            var documentTypes = JsonConvert.DeserializeObject<List<DelforloebOutputDtoV10>>(result);
            return documentTypes;
        }

        public async Task<DokumentMetadataDtoV12> JournaliseDocument(int caseId, byte[] file, string fileName, string fileDescription, string fileContentType)
        {
            var documentDto = new JournaliserDokumentInputDtoV9
            {
                SagID = caseId,
                DokumentNavn = fileName,
                Beskrivelse = fileDescription
            };

            HttpResponseMessage response = null;
            using (var content = new MultipartFormDataContent())
            {
                //Content-Disposition: form-data; name="json"
                var stringContent = new StringContent(JsonConvert.SerializeObject(documentDto));
                stringContent.Headers.Add("Content-Disposition", "form-data; name=\"json\"");
                content.Add(stringContent, "json");

                //Content-Disposition: form-data; name="file"; filename="C:\....\x.xyz";
                var streamContent = new StreamContent(new MemoryStream(file));
                streamContent.Headers.Add("Content-Type", fileContentType);
                streamContent.Headers.Add("Content-Disposition", "form-data; name=\"file\"; filename=\"" + fileName + "\"");
                content.Add(streamContent, "file", fileName);

                try
                {
                    response = await HttpClient.PostAsync("dokument/journaliser", content);
                    response.EnsureSuccessStatusCode();
                }
                catch (Exception exception)
                {
                    _logger.LogWarning("Failed to journalize document with case ID {CaseId}. FileName=<{fileName}>, FileDescription=<{FileDescription}>, FileContentType=<{FileContentType}> ErrorMessage=<{ErrorMessage}>.",
                        caseId, fileName, fileDescription, fileContentType, exception.Message);
                    throw;
                }
            }

            var result = await response.Content.ReadAsStringAsync();

            var documentMetaData = JsonConvert.DeserializeObject<DokumentMetadataDtoV12>(result);

            return documentMetaData;
        }

        public async Task<DokumentMetadataDtoV10> ConnectDocumentAndCase(int documentTypeId, int documentId)
        {
            var documentDto = new DokumentRegistreringInputDtoV10
            {
                DokumentRegistreringId = documentId
            };

            var postContent = new StringContent(JsonConvert.SerializeObject(documentDto), Encoding.UTF8,
                "application/json");

            HttpResponseMessage response = null;
            try
            {
                response = await HttpClient.PostAsync($"delforloeb/{documentTypeId}/dokument", postContent);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception exception)
            {
                _logger.LogWarning("Failed to connect document and case. DocumentTypeId=<{DocumentTypeId}>, DocumentId=<{DocumentId}>, ErrorMessage=<{ErrorMessage}>.",
                    documentTypeId, documentId, exception.Message);
                throw;
            }

            var result = await response.Content.ReadAsStringAsync();

            var documentMetaData = JsonConvert.DeserializeObject<DokumentMetadataDtoV10>(result);
            return documentMetaData;
        }
    }
}