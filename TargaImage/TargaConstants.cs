namespace Paloma
{
    internal static class TargaConstants
    {
        internal const int HeaderByteLength = 18;

        internal const int FooterByteLength = 26;

        internal const int FooterSignatureOffsetFromEnd = 18;

        internal const int FooterSignatureByteLength = 16;

        internal const int FooterReservedCharByteLength = 1;

        internal const int ExtensionAreaAuthorNameByteLength = 41;

        internal const int ExtensionAreaAuthorCommentsByteLength = 324;

        internal const int ExtensionAreaJobNameByteLength = 41;

        internal const int ExtensionAreaSoftwareIDByteLength = 41;

        internal const int ExtensionAreaSoftwareVersionLetterByteLength = 1;

        internal const int ExtensionAreaColorCorrectionTableValueLength = 256;

        internal const string TargaFooterASCIISignature = "TRUEVISION-XFILE";
    }
}
