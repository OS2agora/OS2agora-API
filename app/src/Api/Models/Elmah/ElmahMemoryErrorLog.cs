using System;
using System.Collections.Generic;
using System.Linq;
using ElmahCore;

namespace BallerupKommune.Api.Models.Elmah
{
    public class ElmahMemoryErrorLog : ErrorLog
    {
        private readonly MemoryErrorLog _innerMemoryErrorLog = new MemoryErrorLog(size: 100);
        private const string RedactedReplacement = "<Redacted>";
        private const string CookieHeaderKey = "Header_Cookie";
        
        public override string Log(Error error) {
            var id = Guid.NewGuid();
            Log(id, error);
            return id.ToString();
        }
        
        public override void Log(Guid id, Error error)
        {
            RedactCookies(error);
            _innerMemoryErrorLog.Log(id, error);
        }
        
        public override ErrorLogEntry GetError(string id) => _innerMemoryErrorLog.GetError(id);
        public override int GetErrors(int errorIndex, int pageSize, ICollection<ErrorLogEntry> errorEntryList) =>
            _innerMemoryErrorLog.GetErrors(errorIndex, pageSize, errorEntryList);

        private static void RedactCookies(Error error)
        {
            foreach (string cookie in error.Cookies.AllKeys)
            { 
                error.Cookies.Set(cookie, RedactedReplacement);   
            }

            if (error.ServerVariables.AllKeys.Contains(CookieHeaderKey))
            {
                error.ServerVariables.Set(CookieHeaderKey, RedactedReplacement);
            }
        }
    }
}