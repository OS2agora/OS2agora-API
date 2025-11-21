namespace Agora.Operations.Common.TextResolverKeys
{
    public class MunicipalityKey : TextResolverBaseKey
    {
        private MunicipalityKey(string value) : base(value) { }
        public static MunicipalityKey Kobenhavn => new MunicipalityKey("Kobenhavn");
        public static MunicipalityKey Ballerup => new MunicipalityKey("Ballerup");
        public static MunicipalityKey Novataris => new MunicipalityKey("Novataris");
        public static MunicipalityKey OS2 => new MunicipalityKey("OS2");
    }
}