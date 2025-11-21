using Agora.DAOs.ContentScanners.DataScanner;
using Agora.Operations.Common.Enums;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Agora.DAOs.UnitTests.ContentScanners.DataScanners
{
    public class SimpleDataScannerTests
    {
        [TestCaseSource(nameof(GetDirtyContents))]
        public async Task ScanTextContent_DirtyContent(string content)
        {
            var dataScanner = new SimpleDataScanner();
            var result = await dataScanner.ScanTextContentAsync(content);
            Assert.AreEqual(DataScannerResult.Dirty, result);
        }

        [TestCaseSource(nameof(GetCleanContents))]
        public async Task ScanTextContent_CleanContent(string content)
        {
            var dataScanner = new SimpleDataScanner();
            var result = await dataScanner.ScanTextContentAsync(content);
            Assert.AreEqual(DataScannerResult.Clean, result);
        }

        private static IEnumerable<string> GetDirtyContents()
        {
            yield return "Her er mit CPR-nummer: 0104909995";
            yield return "Her er mit CPR-nummer: 010490-9995";
            yield return "Dette er ikke et cpr-nummer (jo det er): 010490 9995";
            yield return "Et anderledes format 010490 - 9995 kan også noget";
            yield return "Et kendt falsk positiv +4512341234";
            yield return "Et andet falsk positiv 500990-9995";
            yield return "To cpr-numre er bedre end et 0104909995 010490-9995";
        }

        private static IEnumerable<string> GetCleanContents()
        {
            yield return "Denne tekst indeholder ikke noget snavs";
            yield return "Giv på et kald på tlf. 12341234";
            yield return "Min email findes her: myname@internet.com";
            yield return "Min fødselsdag er 050995";
            yield return "Cvr nummeret på virksomheden er 10099416";
        }
    }
}