namespace Agora.DAOs.Messages.RemotePrint.Constants
{
    
    public static class RemotePrint
    {
        /// <summary>
        /// All actors are copied from the following documents:
        /// - 'SF1601 Postkomponent - Beskedfordeler - PKO_PostStatus'
        /// - 'SF1601 Programmer's Guide'
        /// - 'SF1601 Postkomponent - KombiPostAfsend - Feltbeskrivelse'
        /// 
        /// All documents come from the provided documentation package for SF1601 from digitaliersingkataloget.
        /// The documentation package can be downloaded here: https://digitaliseringskatalog.dk/integration/sf1601
        /// </summary>
        public static class ActorNames
        {
            public const string SentLetter = "FjernprintStrålfors";
            public const string GetStatusStraalfors = "Strålfors";
            public const string GetStatusEdora = "Edora";
            public const string GetStatusKMD = "KMD Charlie Tango";
            public const string KombiCodePhysical = "Fysisk Post";
        }

        /// <summary>
        /// All actions are copied from the document 'SF1601 Postkomponent - Beskedfordeler - PKO_PostStatus'
        /// from the provided documentation package for SF1601 from digitaliersingkataloget.
        /// The documentation package can be downloaded here: https://digitaliseringskatalog.dk/integration/sf1601
        /// </summary>
        public static class Actions
        {
            /// <summary>
            /// When a letter has been created by the sending system at the remote print provider.
            /// </summary>
            public const string Afsendt = "db5a6025-caa3-45c3-8e02-4e6fa8142ade";

            /// <summary>
            /// When a letter has been received by the remote print provider and is being processed centrally.
            /// </summary>
            public const string ModtagetFjernprint = "dd98e71c-41ac-4305-a47b-8f86b00e639b";

            /// <summary>
            /// Used for any error during the letter’s lifecycle (processing, technical/business receipt, or final status).
            /// </summary>
            public const string Fejlet = "e225a75c-4b63-46c4-9423-77f5c8762445";

            /// <summary>
            /// When the letter is waiting to be delivered to print.
            /// </summary>
            public const string Klar = "e2b30d57-504a-4e2e-ae2d-a1394a9cb0b8";

            /// <summary>
            /// When the letter has been delivered to print and enveloping.
            /// </summary>
            public const string AfleveretTilPrintOgKuvertering = "e3c0a021-2070-40d4-b7b7-754aeba762e9";

            /// <summary>
            /// When Strålfors has handed the letter over to Post Danmark (status used only by remote print).
            /// </summary>
            public const string ModtagetPostDanmark = "e94a0a8b-60a0-42ad-8b83-52c9e15d0fb3";

            /// <summary>
            /// When the letter has been successfully recalled.
            /// </summary>
            public const string Tilbagekaldt = "f03904aa-df6e-417f-bd74-6a01a61adbcf";

            /// <summary>
            /// Update from Post Danmark due to return mail, address change information, proof of receipt, or TT number (status used only by remote print).
            /// </summary>
            public const string OpdateringFraPostDanmark = "f4e6eb11-0261-4198-8b5d-be92a9b1a35d";
        }
    }
}
