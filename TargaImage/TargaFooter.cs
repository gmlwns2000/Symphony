namespace Paloma
{
#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
    public class TargaFooter
#pragma warning restore CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
    {
        private int intExtensionAreaOffset;

        private int intDeveloperDirectoryOffset;

        private string strSignature = string.Empty;

        private string strReservedCharacter = string.Empty;

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        public int ExtensionAreaOffset
#pragma warning restore CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        {
            get
            {
                return intExtensionAreaOffset;
            }
        }

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        public int DeveloperDirectoryOffset
#pragma warning restore CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        {
            get
            {
                return intDeveloperDirectoryOffset;
            }
        }

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        public string Signature
#pragma warning restore CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        {
            get
            {
                return strSignature;
            }
        }

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        public string ReservedCharacter
#pragma warning restore CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        {
            get
            {
                return strReservedCharacter;
            }
        }

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        protected internal void SetExtensionAreaOffset(int intExtensionAreaOffset)
#pragma warning restore CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        {
            this.intExtensionAreaOffset = intExtensionAreaOffset;
        }

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        protected internal void SetDeveloperDirectoryOffset(int intDeveloperDirectoryOffset)
#pragma warning restore CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        {
            this.intDeveloperDirectoryOffset = intDeveloperDirectoryOffset;
        }

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        protected internal void SetSignature(string strSignature)
#pragma warning restore CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        {
            this.strSignature = strSignature;
        }

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        protected internal void SetReservedCharacter(string strReservedCharacter)
#pragma warning restore CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        {
            this.strReservedCharacter = strReservedCharacter;
        }
    }
}
