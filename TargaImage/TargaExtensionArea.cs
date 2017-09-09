using System;
using System.Collections.Generic;
using System.Drawing;

namespace Paloma
{
#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
    public class TargaExtensionArea
#pragma warning restore CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
    {
        private int intExtensionSize;

        private string strAuthorName = string.Empty;

        private string strAuthorComments = string.Empty;

        private DateTime dtDateTimeStamp = DateTime.Now;

        private string strJobName = string.Empty;

        private TimeSpan dtJobTime = TimeSpan.Zero;

        private string strSoftwareID = string.Empty;

        private string strSoftwareVersion = string.Empty;

        private Color cKeyColor = Color.Empty;

        private int intPixelAspectRatioNumerator;

        private int intPixelAspectRatioDenominator;

        private int intGammaNumerator;

        private int intGammaDenominator;

        private int intColorCorrectionOffset;

        private int intPostageStampOffset;

        private int intScanLineOffset;

        private int intAttributesType;

        private List<int> intScanLineTable = new List<int>();

        private List<Color> cColorCorrectionTable = new List<Color>();

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        public int ExtensionSize
#pragma warning restore CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        {
            get
            {
                return intExtensionSize;
            }
        }

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        public string AuthorName
#pragma warning restore CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        {
            get
            {
                return strAuthorName;
            }
        }

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        public string AuthorComments
#pragma warning restore CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        {
            get
            {
                return strAuthorComments;
            }
        }

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        public DateTime DateTimeStamp
#pragma warning restore CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        {
            get
            {
                return dtDateTimeStamp;
            }
        }

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        public string JobName
#pragma warning restore CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        {
            get
            {
                return strJobName;
            }
        }

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        public TimeSpan JobTime
#pragma warning restore CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        {
            get
            {
                return dtJobTime;
            }
        }

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        public string SoftwareID
#pragma warning restore CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        {
            get
            {
                return strSoftwareID;
            }
        }

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        public string SoftwareVersion
#pragma warning restore CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        {
            get
            {
                return strSoftwareVersion;
            }
        }

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        public Color KeyColor
#pragma warning restore CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        {
            get
            {
                return cKeyColor;
            }
        }

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        public int PixelAspectRatioNumerator
#pragma warning restore CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        {
            get
            {
                return intPixelAspectRatioNumerator;
            }
        }

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        public int PixelAspectRatioDenominator
#pragma warning restore CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        {
            get
            {
                return intPixelAspectRatioDenominator;
            }
        }

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        public float PixelAspectRatio
#pragma warning restore CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        {
            get
            {
                if (intPixelAspectRatioDenominator > 0)
                {
                    return (float)intPixelAspectRatioNumerator / (float)intPixelAspectRatioDenominator;
                }
                return 0f;
            }
        }

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        public int GammaNumerator
#pragma warning restore CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        {
            get
            {
                return intGammaNumerator;
            }
        }

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        public int GammaDenominator
#pragma warning restore CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        {
            get
            {
                return intGammaDenominator;
            }
        }

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        public float GammaRatio
#pragma warning restore CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        {
            get
            {
                if (intGammaDenominator > 0)
                {
                    float num = (float)intGammaNumerator / (float)intGammaDenominator;
                    return (float)Math.Round((double)num, 1);
                }
                return 1f;
            }
        }

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        public int ColorCorrectionOffset
#pragma warning restore CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        {
            get
            {
                return intColorCorrectionOffset;
            }
        }

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        public int PostageStampOffset
#pragma warning restore CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        {
            get
            {
                return intPostageStampOffset;
            }
        }

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        public int ScanLineOffset
#pragma warning restore CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        {
            get
            {
                return intScanLineOffset;
            }
        }

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        public int AttributesType
#pragma warning restore CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        {
            get
            {
                return intAttributesType;
            }
        }

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        public List<int> ScanLineTable
#pragma warning restore CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        {
            get
            {
                return intScanLineTable;
            }
        }

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        public List<Color> ColorCorrectionTable
#pragma warning restore CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        {
            get
            {
                return cColorCorrectionTable;
            }
        }

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        protected internal void SetExtensionSize(int intExtensionSize)
#pragma warning restore CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        {
            this.intExtensionSize = intExtensionSize;
        }

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        protected internal void SetAuthorName(string strAuthorName)
#pragma warning restore CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        {
            this.strAuthorName = strAuthorName;
        }

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        protected internal void SetAuthorComments(string strAuthorComments)
#pragma warning restore CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        {
            this.strAuthorComments = strAuthorComments;
        }

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        protected internal void SetDateTimeStamp(DateTime dtDateTimeStamp)
#pragma warning restore CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        {
            this.dtDateTimeStamp = dtDateTimeStamp;
        }

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        protected internal void SetJobName(string strJobName)
#pragma warning restore CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        {
            this.strJobName = strJobName;
        }

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        protected internal void SetJobTime(TimeSpan dtJobTime)
#pragma warning restore CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        {
            this.dtJobTime = dtJobTime;
        }

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        protected internal void SetSoftwareID(string strSoftwareID)
#pragma warning restore CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        {
            this.strSoftwareID = strSoftwareID;
        }

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        protected internal void SetSoftwareVersion(string strSoftwareVersion)
#pragma warning restore CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        {
            this.strSoftwareVersion = strSoftwareVersion;
        }

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        protected internal void SetKeyColor(Color cKeyColor)
#pragma warning restore CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        {
            this.cKeyColor = cKeyColor;
        }

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        protected internal void SetPixelAspectRatioNumerator(int intPixelAspectRatioNumerator)
#pragma warning restore CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        {
            this.intPixelAspectRatioNumerator = intPixelAspectRatioNumerator;
        }

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        protected internal void SetPixelAspectRatioDenominator(int intPixelAspectRatioDenominator)
#pragma warning restore CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        {
            this.intPixelAspectRatioDenominator = intPixelAspectRatioDenominator;
        }

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        protected internal void SetGammaNumerator(int intGammaNumerator)
#pragma warning restore CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        {
            this.intGammaNumerator = intGammaNumerator;
        }

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        protected internal void SetGammaDenominator(int intGammaDenominator)
#pragma warning restore CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        {
            this.intGammaDenominator = intGammaDenominator;
        }

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        protected internal void SetColorCorrectionOffset(int intColorCorrectionOffset)
#pragma warning restore CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        {
            this.intColorCorrectionOffset = intColorCorrectionOffset;
        }

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        protected internal void SetPostageStampOffset(int intPostageStampOffset)
#pragma warning restore CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        {
            this.intPostageStampOffset = intPostageStampOffset;
        }

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        protected internal void SetScanLineOffset(int intScanLineOffset)
#pragma warning restore CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        {
            this.intScanLineOffset = intScanLineOffset;
        }

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        protected internal void SetAttributesType(int intAttributesType)
#pragma warning restore CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        {
            this.intAttributesType = intAttributesType;
        }
    }
}
