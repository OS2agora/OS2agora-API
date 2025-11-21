using Agora.Models.Models.Records;
using Agora.Operations.Common.Interfaces.Files.Excel;
using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Data;

namespace Agora.DAOs.Files.Excel.Forms
{
    public abstract class BaseExcelForm : IExcelForm
    {
        protected XLWorkbook Workbook = new();

        public IEnumerable<CommentRecord> CommentRecords { get; set; }
        public IEnumerable<UserRecord> UserRecords { get; set; }

        public abstract XLWorkbook GenerateContent();

        protected IXLWorksheet AddWorkSheet(string name)
        {
            return Workbook.Worksheets.Add(name);
        }

        protected DataTable GenerateTable(Dictionary<string, Type> headers)
        {
            var table = new DataTable();

            foreach (var header in headers)
            {
                table.Columns.Add(header.Key, header.Value);
            }

            return table;
        }
    }
}
