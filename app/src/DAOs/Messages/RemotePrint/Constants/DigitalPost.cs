namespace Agora.DAOs.Messages.RemotePrint.Constants
{
    public static class DigitalPost
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
            public const string SentLetter = "DigitalPost";
            public const string GetStatus = "Digital Post";
            public const string KombiCodeDigital = "Digital Post";
        }

        /// <summary>
        /// All actions are copied from the document 'SF1601 Postkomponent - Beskedfordeler - PKO_PostStatus'
        /// from the provided documentation package for SF1601 from digitaliersingkataloget.
        /// The documentation package can be downloaded here: https://digitaliseringskatalog.dk/integration/sf1601
        /// </summary>
        public static class Actions
        {
            /// <summary>
            /// Used for any errors that can occur during the letter's lifecycle, i.e., processing, technical/business receipt, and final status.
            /// </summary>
            public const string Fejlet = "e225a75c-4b63-46c4-9423-77f5c8762445";

            /// <summary>
            /// When the Digital Post solution has confirmed that the letter is delivered to the end user.
            /// </summary>
            public const string AfleveretDigitalPost = "f7161a89-5068-4023-bc80-d7f4daad2a2e";

            /// <summary>
            /// When the Digital Post solution has confirmed that the letter has been received.
            /// </summary>
            public const string ModtagetDigitalPost = "eb866ca2-b871-4387-b501-12cde577bd58";
        }
    }

}
