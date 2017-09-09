using System.Collections.Generic;
using System.Drawing;

namespace Paloma
{
#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
    public class TargaHeader
#pragma warning restore CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
    {
        private byte bImageIDLength;

        private ColorMapType eColorMapType;

        private ImageType eImageType;

        private short sColorMapFirstEntryIndex;

        private short sColorMapLength;

        private byte bColorMapEntrySize;

        private short sXOrigin;

        private short sYOrigin;

        private short sWidth;

        private short sHeight;

        private byte bPixelDepth;

        private byte bImageDescriptor;

        private VerticalTransferOrder eVerticalTransferOrder = VerticalTransferOrder.UNKNOWN;

        private HorizontalTransferOrder eHorizontalTransferOrder = HorizontalTransferOrder.UNKNOWN;

        private byte bAttributeBits;

        private string strImageIDValue = string.Empty;

        private List<Color> cColorMap = new List<Color>();

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        public byte ImageIDLength
#pragma warning restore CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        {
            get
            {
                return bImageIDLength;
            }
        }

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        public ColorMapType ColorMapType
#pragma warning restore CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        {
            get
            {
                return eColorMapType;
            }
        }

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        public ImageType ImageType
#pragma warning restore CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        {
            get
            {
                return eImageType;
            }
        }

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        public short ColorMapFirstEntryIndex
#pragma warning restore CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        {
            get
            {
                return sColorMapFirstEntryIndex;
            }
        }

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        public short ColorMapLength
#pragma warning restore CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        {
            get
            {
                return sColorMapLength;
            }
        }

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        public byte ColorMapEntrySize
#pragma warning restore CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        {
            get
            {
                return bColorMapEntrySize;
            }
        }

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        public short XOrigin
#pragma warning restore CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        {
            get
            {
                return sXOrigin;
            }
        }

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        public short YOrigin
#pragma warning restore CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        {
            get
            {
                return sYOrigin;
            }
        }

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        public short Width
#pragma warning restore CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        {
            get
            {
                return sWidth;
            }
        }

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        public short Height
#pragma warning restore CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        {
            get
            {
                return sHeight;
            }
        }

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        public byte PixelDepth
#pragma warning restore CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        {
            get
            {
                return bPixelDepth;
            }
        }

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        protected internal byte ImageDescriptor
#pragma warning restore CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        {
            get
            {
                return bImageDescriptor;
            }
            set
            {
                bImageDescriptor = value;
            }
        }

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        public FirstPixelDestination FirstPixelDestination
#pragma warning restore CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        {
            get
            {
                if (eVerticalTransferOrder == VerticalTransferOrder.UNKNOWN || eHorizontalTransferOrder == HorizontalTransferOrder.UNKNOWN)
                {
                    return FirstPixelDestination.UNKNOWN;
                }
                if (eVerticalTransferOrder == VerticalTransferOrder.BOTTOM && eHorizontalTransferOrder == HorizontalTransferOrder.LEFT)
                {
                    return FirstPixelDestination.BOTTOM_LEFT;
                }
                if (eVerticalTransferOrder == VerticalTransferOrder.BOTTOM && eHorizontalTransferOrder == HorizontalTransferOrder.RIGHT)
                {
                    return FirstPixelDestination.BOTTOM_RIGHT;
                }
                if (eVerticalTransferOrder == VerticalTransferOrder.TOP && eHorizontalTransferOrder == HorizontalTransferOrder.LEFT)
                {
                    return FirstPixelDestination.TOP_LEFT;
                }
                return FirstPixelDestination.TOP_RIGHT;
            }
        }

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        public VerticalTransferOrder VerticalTransferOrder
#pragma warning restore CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        {
            get
            {
                return eVerticalTransferOrder;
            }
        }

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        public HorizontalTransferOrder HorizontalTransferOrder
#pragma warning restore CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        {
            get
            {
                return eHorizontalTransferOrder;
            }
        }

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        public byte AttributeBits
#pragma warning restore CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        {
            get
            {
                return bAttributeBits;
            }
        }

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        public string ImageIDValue
#pragma warning restore CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        {
            get
            {
                return strImageIDValue;
            }
        }

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        public List<Color> ColorMap
#pragma warning restore CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        {
            get
            {
                return cColorMap;
            }
        }

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        public int ImageDataOffset
#pragma warning restore CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        {
            get
            {
                int num = 18;
                num += (int)bImageIDLength;
                int num2 = 0;
                byte b = bColorMapEntrySize;
                switch (b)
                {
                    case 15:
                        num2 = 2;
                        break;
                    case 16:
                        num2 = 2;
                        break;
                    default:
                        if (b != 24)
                        {
                            if (b == 32)
                            {
                                num2 = 4;
                            }
                        }
                        else
                        {
                            num2 = 3;
                        }
                        break;
                }
                return num + (int)sColorMapLength * num2;
            }
        }

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        public int BytesPerPixel
#pragma warning restore CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        {
            get
            {
                return (int)(bPixelDepth / 8);
            }
        }

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        protected internal void SetImageIDLength(byte bImageIDLength)
#pragma warning restore CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        {
            this.bImageIDLength = bImageIDLength;
        }

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        protected internal void SetColorMapType(ColorMapType eColorMapType)
#pragma warning restore CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        {
            this.eColorMapType = eColorMapType;
        }

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        protected internal void SetImageType(ImageType eImageType)
#pragma warning restore CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        {
            this.eImageType = eImageType;
        }

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        protected internal void SetColorMapFirstEntryIndex(short sColorMapFirstEntryIndex)
#pragma warning restore CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        {
            this.sColorMapFirstEntryIndex = sColorMapFirstEntryIndex;
        }

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        protected internal void SetColorMapLength(short sColorMapLength)
#pragma warning restore CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        {
            this.sColorMapLength = sColorMapLength;
        }

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        protected internal void SetColorMapEntrySize(byte bColorMapEntrySize)
#pragma warning restore CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        {
            this.bColorMapEntrySize = bColorMapEntrySize;
        }

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        protected internal void SetXOrigin(short sXOrigin)
#pragma warning restore CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        {
            this.sXOrigin = sXOrigin;
        }

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        protected internal void SetYOrigin(short sYOrigin)
#pragma warning restore CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        {
            this.sYOrigin = sYOrigin;
        }

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        protected internal void SetWidth(short sWidth)
#pragma warning restore CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        {
            this.sWidth = sWidth;
        }

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        protected internal void SetHeight(short sHeight)
#pragma warning restore CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        {
            this.sHeight = sHeight;
        }

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        protected internal void SetPixelDepth(byte bPixelDepth)
#pragma warning restore CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        {
            this.bPixelDepth = bPixelDepth;
        }

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        protected internal void SetVerticalTransferOrder(VerticalTransferOrder eVerticalTransferOrder)
#pragma warning restore CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        {
            this.eVerticalTransferOrder = eVerticalTransferOrder;
        }

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        protected internal void SetHorizontalTransferOrder(HorizontalTransferOrder eHorizontalTransferOrder)
#pragma warning restore CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        {
            this.eHorizontalTransferOrder = eHorizontalTransferOrder;
        }

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        protected internal void SetAttributeBits(byte bAttributeBits)
#pragma warning restore CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        {
            this.bAttributeBits = bAttributeBits;
        }

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        protected internal void SetImageIDValue(string strImageIDValue)
#pragma warning restore CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        {
            this.strImageIDValue = strImageIDValue;
        }
    }
}
