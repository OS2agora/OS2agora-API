using System.Collections.Generic;

namespace Agora.Models.Common.CustomResponse
{
    public sealed class ResponseList<T> : List<T>
    {
        public Dictionary<string, object> Meta { get; set; } = new Dictionary<string, object>();

        public ResponseList() { }

        public ResponseList(IEnumerable<T> data, IEnumerable<T> existingList, string metaDataKey, object metaData) : base(data)
        {
            CopyExistingMetaData(existingList);
            AddMetaData(metaDataKey, metaData);
        }

        public ResponseList(IEnumerable<T> data, string metaDataKey, object metaData) : this(data, data, metaDataKey, metaData) { }

        public ResponseList(IEnumerable<T> data, IEnumerable<T> existingList) : this(data, existingList, null, null) { }

        private void CopyExistingMetaData(IEnumerable<T> list)
        {
            if (list is ResponseList<T> responseList && responseList?.Meta != null)
            {
                CopyMetaData(responseList);
            }
        }

        private void AddMetaData<TMetaData>(string key, TMetaData metaData)
        {
            if (!string.IsNullOrEmpty(key))
            {
                Meta[key] = metaData;
            }
        }

        private void CopyMetaData(ResponseList<T> existingList)
        {
            foreach (var metaData in existingList?.Meta)
            {
                Meta[metaData.Key] = metaData.Value;
            }
        }

    }
}
