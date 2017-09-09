// ==========================================================
// TargaImage
//
// Design and implementation by
// - David Polomis (paloma_sw@cox.net)
//
//
// This source code, along with any associated files, is licensed under
// The Code Project Open License (CPOL) 1.02
// A copy of this license can be found in the CPOL.html file 
// which was downloaded with this source code
// or at http://www.codeproject.com/info/cpol10.aspx
//
// 
// COVERED CODE IS PROVIDED UNDER THIS LICENSE ON AN "AS IS" BASIS,
// WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED,
// INCLUDING, WITHOUT LIMITATION, WARRANTIES THAT THE COVERED CODE IS
// FREE OF DEFECTS, MERCHANTABLE, FIT FOR A PARTICULAR PURPOSE OR
// NON-INFRINGING. THE ENTIRE RISK AS TO THE QUALITY AND PERFORMANCE
// OF THE COVERED CODE IS WITH YOU. SHOULD ANY COVERED CODE PROVE
// DEFECTIVE IN ANY RESPECT, YOU (NOT THE INITIAL DEVELOPER OR ANY
// OTHER CONTRIBUTOR) ASSUME THE COST OF ANY NECESSARY SERVICING,
// REPAIR OR CORRECTION. THIS DISCLAIMER OF WARRANTY CONSTITUTES AN
// ESSENTIAL PART OF THIS LICENSE. NO USE OF ANY COVERED CODE IS
// AUTHORIZED HEREUNDER EXCEPT UNDER THIS DISCLAIMER.
//
// Use at your own risk!
//
// ==========================================================


using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Paloma
{
#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
    public class TargaImage : IDisposable
#pragma warning restore CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
    {
        private TargaHeader objTargaHeader;

        private TargaExtensionArea objTargaExtensionArea;

        private TargaFooter objTargaFooter;

        private Bitmap bmpTargaImage;

        private Bitmap bmpImageThumbnail;

        private TGAFormat eTGAFormat;

        private string strFileName = string.Empty;

        private int intStride;

        private int intPadding;

        private GCHandle ImageByteHandle;

        private GCHandle ThumbnailByteHandle;

        private List<List<byte>> rows = new List<List<byte>>();

        private List<byte> row = new List<byte>();

        private bool disposed;

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        public TargaHeader Header
#pragma warning restore CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        {
            get
            {
                return objTargaHeader;
            }
        }

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        public TargaExtensionArea ExtensionArea
#pragma warning restore CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        {
            get
            {
                return objTargaExtensionArea;
            }
        }

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        public TargaFooter Footer
#pragma warning restore CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        {
            get
            {
                return objTargaFooter;
            }
        }

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        public TGAFormat Format
#pragma warning restore CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        {
            get
            {
                return eTGAFormat;
            }
        }

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        public Bitmap Image
#pragma warning restore CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        {
            get
            {
                return bmpTargaImage;
            }
        }

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        public Bitmap Thumbnail
#pragma warning restore CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        {
            get
            {
                return bmpImageThumbnail;
            }
        }

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        public string FileName
#pragma warning restore CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        {
            get
            {
                return strFileName;
            }
        }

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        public int Stride
#pragma warning restore CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        {
            get
            {
                return intStride;
            }
        }

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        public int Padding
#pragma warning restore CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        {
            get
            {
                return intPadding;
            }
        }

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        public TargaImage()
#pragma warning restore CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        {
            objTargaFooter = new TargaFooter();
            objTargaHeader = new TargaHeader();
            objTargaExtensionArea = new TargaExtensionArea();
            bmpTargaImage = null;
            bmpImageThumbnail = null;
        }

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        ~TargaImage()
#pragma warning restore CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        {
            Dispose(false);
        }

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        public TargaImage(string strFileName) : this()
#pragma warning restore CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        {
            if (!(Path.GetExtension(strFileName).ToLower() == ".tga"))
            {
                throw new Exception("Error loading file, file '" + strFileName + "' must have an extension of '.tga'.");
            }
            if (File.Exists(strFileName))
            {
                this.strFileName = strFileName;
                byte[] array = File.ReadAllBytes(this.strFileName);
                if (array != null && array.Length > 0)
                {
                    MemoryStream memoryStream2;
                    MemoryStream memoryStream = memoryStream2 = new MemoryStream(array);
                    try
                    {
                        if (memoryStream != null && memoryStream.Length > 0L && memoryStream.CanSeek)
                        {
                            BinaryReader binaryReader;
                            BinaryReader binReader = binaryReader = new BinaryReader(memoryStream);
                            try
                            {
                                LoadTGAFooterInfo(binReader);
                                LoadTGAHeaderInfo(binReader);
                                LoadTGAExtensionArea(binReader);
                                LoadTGAImage(binReader);
                                goto IMJINKWAN;
                            }
                            finally
                            {
                                if (binaryReader != null)
                                {
                                    ((IDisposable)binaryReader).Dispose();
                                }
                            }
#pragma warning disable CS0162 // 접근할 수 없는 코드가 있습니다.
                            goto GOTOHELLEXCEPTIONS;
#pragma warning restore CS0162 // 접근할 수 없는 코드가 있습니다.
                            IMJINKWAN:
                            return;
                        }
                        GOTOHELLEXCEPTIONS:
                        throw new Exception("Error loading file, could not read file from disk.");
                    }
                    finally
                    {
                        if (memoryStream2 != null)
                        {
                            ((IDisposable)memoryStream2).Dispose();
                        }
                    }
                }
                throw new Exception("Error loading file, could not read file from disk.");
            }
            throw new Exception("Error loading file, could not find file '" + strFileName + "' on disk.");
        }

        private void LoadTGAFooterInfo(BinaryReader binReader)
        {
            if (binReader != null && binReader.BaseStream != null && binReader.BaseStream.Length > 0L && binReader.BaseStream.CanSeek)
            {
                try
                {
                    binReader.BaseStream.Seek(-18L, SeekOrigin.End);
                    string arg_5F_0 = Encoding.ASCII.GetString(binReader.ReadBytes(16));
                    char[] trimChars = new char[1];
                    string text = arg_5F_0.TrimEnd(trimChars);
                    if (string.Compare(text, "TRUEVISION-XFILE") == 0)
                    {
                        eTGAFormat = TGAFormat.NEW_TGA;
                        binReader.BaseStream.Seek(-26L, SeekOrigin.End);
                        int extensionAreaOffset = binReader.ReadInt32();
                        int developerDirectoryOffset = binReader.ReadInt32();
                        binReader.ReadBytes(16);
                        string arg_C2_0 = Encoding.ASCII.GetString(binReader.ReadBytes(1));
                        char[] trimChars2 = new char[1];
                        string reservedCharacter = arg_C2_0.TrimEnd(trimChars2);
                        objTargaFooter.SetExtensionAreaOffset(extensionAreaOffset);
                        objTargaFooter.SetDeveloperDirectoryOffset(developerDirectoryOffset);
                        objTargaFooter.SetSignature(text);
                        objTargaFooter.SetReservedCharacter(reservedCharacter);
                    }
                    else
                    {
                        eTGAFormat = TGAFormat.ORIGINAL_TGA;
                    }
                    return;
                }
                catch (Exception ex)
                {
                    ClearAll();
                    throw ex;
                }
            }
            ClearAll();
            throw new Exception("Error loading file, could not read file from disk.");
        }

        private void LoadTGAHeaderInfo(BinaryReader binReader)
        {
            if (binReader != null && binReader.BaseStream != null && binReader.BaseStream.Length > 0L && binReader.BaseStream.CanSeek)
            {
                try
                {
                    binReader.BaseStream.Seek(0L, SeekOrigin.Begin);
                    objTargaHeader.SetImageIDLength(binReader.ReadByte());
                    objTargaHeader.SetColorMapType((ColorMapType)binReader.ReadByte());
                    objTargaHeader.SetImageType((ImageType)binReader.ReadByte());
                    objTargaHeader.SetColorMapFirstEntryIndex(binReader.ReadInt16());
                    objTargaHeader.SetColorMapLength(binReader.ReadInt16());
                    objTargaHeader.SetColorMapEntrySize(binReader.ReadByte());
                    objTargaHeader.SetXOrigin(binReader.ReadInt16());
                    objTargaHeader.SetYOrigin(binReader.ReadInt16());
                    objTargaHeader.SetWidth(binReader.ReadInt16());
                    objTargaHeader.SetHeight(binReader.ReadInt16());
                    byte b = binReader.ReadByte();
                    byte b2 = b;
                    if (b2 <= 16)
                    {
                        if (b2 != 8 && b2 != 16)
                        {
                            goto BAEGOPA;
                        }
                    }
                    else if (b2 != 24 && b2 != 32)
                    {
                        goto BAEGOPA;
                    }
                    objTargaHeader.SetPixelDepth(b);
                    byte b3 = binReader.ReadByte();
                    objTargaHeader.SetAttributeBits((byte)Utilities.GetBits(b3, 0, 4));
                    objTargaHeader.SetVerticalTransferOrder((VerticalTransferOrder)Utilities.GetBits(b3, 5, 1));
                    objTargaHeader.SetHorizontalTransferOrder((HorizontalTransferOrder)Utilities.GetBits(b3, 4, 1));
                    if (objTargaHeader.ImageIDLength > 0)
                    {
                        byte[] bytes = binReader.ReadBytes((int)objTargaHeader.ImageIDLength);
                        TargaHeader arg_1B5_0 = objTargaHeader;
                        string arg_1B0_0 = Encoding.ASCII.GetString(bytes);
                        char[] trimChars = new char[1];
                        arg_1B5_0.SetImageIDValue(arg_1B0_0.TrimEnd(trimChars));
                    }
                    goto HAMBURGER;
                    BAEGOPA:
                    ClearAll();
                    throw new Exception("Targa Image only supports 8, 16, 24, or 32 bit pixel depths.");
                }
                catch (Exception ex)
                {
                    ClearAll();
                    throw ex;
                }
                HAMBURGER:
                if (objTargaHeader.ColorMapType == ColorMapType.COLOR_MAP_INCLUDED)
                {
                    if (objTargaHeader.ImageType == ImageType.UNCOMPRESSED_COLOR_MAPPED || objTargaHeader.ImageType == ImageType.RUN_LENGTH_ENCODED_COLOR_MAPPED)
                    {
                        if (objTargaHeader.ColorMapLength > 0)
                        {
                            try
                            {
                                for (int i = 0; i < (int)objTargaHeader.ColorMapLength; i++)
                                {
                                    byte colorMapEntrySize = objTargaHeader.ColorMapEntrySize;
                                    switch (colorMapEntrySize)
                                    {
                                        case 15:
                                            {
                                                byte[] array = binReader.ReadBytes(2);
                                                objTargaHeader.ColorMap.Add(Utilities.GetColorFrom2Bytes(array[1], array[0]));
                                                break;
                                            }
                                        case 16:
                                            {
                                                byte[] array2 = binReader.ReadBytes(2);
                                                objTargaHeader.ColorMap.Add(Utilities.GetColorFrom2Bytes(array2[1], array2[0]));
                                                break;
                                            }
                                        default:
                                            if (colorMapEntrySize != 24)
                                            {
                                                if (colorMapEntrySize != 32)
                                                {
                                                    ClearAll();
                                                    throw new Exception("TargaImage only supports ColorMap Entry Sizes of 15, 16, 24 or 32 bits.");
                                                }
                                                int alpha = Convert.ToInt32(binReader.ReadByte());
                                                int blue = Convert.ToInt32(binReader.ReadByte());
                                                int green = Convert.ToInt32(binReader.ReadByte());
                                                int red = Convert.ToInt32(binReader.ReadByte());
                                                objTargaHeader.ColorMap.Add(Color.FromArgb(alpha, red, green, blue));
                                            }
                                            else
                                            {
                                                int blue = Convert.ToInt32(binReader.ReadByte());
                                                int green = Convert.ToInt32(binReader.ReadByte());
                                                int red = Convert.ToInt32(binReader.ReadByte());
                                                objTargaHeader.ColorMap.Add(Color.FromArgb(red, green, blue));
                                            }
                                            break;
                                    }
                                }
                                return;
                            }
                            catch (Exception ex2)
                            {
                                ClearAll();
                                throw ex2;
                            }
                        }
                        ClearAll();
                        throw new Exception("Image Type requires a Color Map and Color Map Length is zero.");
                    }
                }
                else if (objTargaHeader.ImageType == ImageType.UNCOMPRESSED_COLOR_MAPPED || objTargaHeader.ImageType == ImageType.RUN_LENGTH_ENCODED_COLOR_MAPPED)
                {
                    ClearAll();
                    throw new Exception("Image Type requires a Color Map and there was not a Color Map included in the file.");
                }
                return;
            }
            ClearAll();
            throw new Exception("Error loading file, could not read file from disk.");
        }

        private void LoadTGAExtensionArea(BinaryReader binReader)
        {
            if (binReader != null && binReader.BaseStream != null && binReader.BaseStream.Length > 0L && binReader.BaseStream.CanSeek)
            {
                if (objTargaFooter.ExtensionAreaOffset > 0)
                {
                    try
                    {
                        binReader.BaseStream.Seek((long)objTargaFooter.ExtensionAreaOffset, SeekOrigin.Begin);
                        objTargaExtensionArea.SetExtensionSize((int)binReader.ReadInt16());
                        TargaExtensionArea arg_95_0 = objTargaExtensionArea;
                        string arg_90_0 = Encoding.ASCII.GetString(binReader.ReadBytes(41));
                        char[] trimChars = new char[1];
                        arg_95_0.SetAuthorName(arg_90_0.TrimEnd(trimChars));
                        TargaExtensionArea arg_C4_0 = objTargaExtensionArea;
                        string arg_BF_0 = Encoding.ASCII.GetString(binReader.ReadBytes(324));
                        char[] trimChars2 = new char[1];
                        arg_C4_0.SetAuthorComments(arg_BF_0.TrimEnd(trimChars2));
                        short num = binReader.ReadInt16();
                        short num2 = binReader.ReadInt16();
                        short num3 = binReader.ReadInt16();
                        short hours = binReader.ReadInt16();
                        short minutes = binReader.ReadInt16();
                        short seconds = binReader.ReadInt16();
                        string text = string.Concat(new string[]
                        {
                            num.ToString(),
                            "/",
                            num2.ToString(),
                            "/",
                            num3.ToString(),
                            " "
                        });
                        string text2 = text;
                        text = string.Concat(new string[]
                        {
                            text2,
                            hours.ToString(),
                            ":",
                            minutes.ToString(),
                            ":",
                            seconds.ToString()
                        });
                        DateTime dateTimeStamp;
                        if (DateTime.TryParse(text, out dateTimeStamp))
                        {
                            objTargaExtensionArea.SetDateTimeStamp(dateTimeStamp);
                        }
                        TargaExtensionArea arg_1CF_0 = objTargaExtensionArea;
                        string arg_1CA_0 = Encoding.ASCII.GetString(binReader.ReadBytes(41));
                        char[] trimChars3 = new char[1];
                        arg_1CF_0.SetJobName(arg_1CA_0.TrimEnd(trimChars3));
                        hours = binReader.ReadInt16();
                        minutes = binReader.ReadInt16();
                        seconds = binReader.ReadInt16();
                        TimeSpan jobTime = new TimeSpan((int)hours, (int)minutes, (int)seconds);
                        objTargaExtensionArea.SetJobTime(jobTime);
                        TargaExtensionArea arg_22B_0 = objTargaExtensionArea;
                        string arg_226_0 = Encoding.ASCII.GetString(binReader.ReadBytes(41));
                        char[] trimChars4 = new char[1];
                        arg_22B_0.SetSoftwareID(arg_226_0.TrimEnd(trimChars4));
                        float num4 = (float)binReader.ReadInt16() / 100f;
                        string arg_25A_0 = Encoding.ASCII.GetString(binReader.ReadBytes(1));
                        char[] trimChars5 = new char[1];
                        string str = arg_25A_0.TrimEnd(trimChars5);
                        objTargaExtensionArea.SetSoftwareID(num4.ToString("F2") + str);
                        int alpha = (int)binReader.ReadByte();
                        int red = (int)binReader.ReadByte();
                        int blue = (int)binReader.ReadByte();
                        int green = (int)binReader.ReadByte();
                        objTargaExtensionArea.SetKeyColor(Color.FromArgb(alpha, red, green, blue));
                        objTargaExtensionArea.SetPixelAspectRatioNumerator((int)binReader.ReadInt16());
                        objTargaExtensionArea.SetPixelAspectRatioDenominator((int)binReader.ReadInt16());
                        objTargaExtensionArea.SetGammaNumerator((int)binReader.ReadInt16());
                        objTargaExtensionArea.SetGammaDenominator((int)binReader.ReadInt16());
                        objTargaExtensionArea.SetColorCorrectionOffset(binReader.ReadInt32());
                        objTargaExtensionArea.SetPostageStampOffset(binReader.ReadInt32());
                        objTargaExtensionArea.SetScanLineOffset(binReader.ReadInt32());
                        objTargaExtensionArea.SetAttributesType((int)binReader.ReadByte());
                        if (objTargaExtensionArea.ScanLineOffset > 0)
                        {
                            binReader.BaseStream.Seek((long)objTargaExtensionArea.ScanLineOffset, SeekOrigin.Begin);
                            for (int i = 0; i < (int)objTargaHeader.Height; i++)
                            {
                                objTargaExtensionArea.ScanLineTable.Add(binReader.ReadInt32());
                            }
                        }
                        if (objTargaExtensionArea.ColorCorrectionOffset > 0)
                        {
                            binReader.BaseStream.Seek((long)objTargaExtensionArea.ColorCorrectionOffset, SeekOrigin.Begin);
                            for (int j = 0; j < 256; j++)
                            {
                                alpha = (int)binReader.ReadInt16();
                                red = (int)binReader.ReadInt16();
                                blue = (int)binReader.ReadInt16();
                                green = (int)binReader.ReadInt16();
                                objTargaExtensionArea.ColorCorrectionTable.Add(Color.FromArgb(alpha, red, green, blue));
                            }
                        }
                        return;
                    }
                    catch (Exception ex)
                    {
                        ClearAll();
                        throw ex;
                    }
#pragma warning disable CS0162 // 접근할 수 없는 코드가 있습니다.
                    goto CLEARALL;
#pragma warning restore CS0162 // 접근할 수 없는 코드가 있습니다.
                }
                return;
            }
            CLEARALL:
            ClearAll();
            throw new Exception("Error loading file, could not read file from disk.");
        }

        private byte[] LoadImageBytes(BinaryReader binReader)
        {
            byte[] result = null;
            if (binReader != null && binReader.BaseStream != null && binReader.BaseStream.Length > 0L && binReader.BaseStream.CanSeek)
            {
                if (objTargaHeader.ImageDataOffset > 0)
                {
                    byte[] array = new byte[intPadding];
                    binReader.BaseStream.Seek((long)objTargaHeader.ImageDataOffset, SeekOrigin.Begin);
                    int num = (int)objTargaHeader.Width * objTargaHeader.BytesPerPixel;
                    int num2 = num * (int)objTargaHeader.Height;
                    if (objTargaHeader.ImageType == ImageType.RUN_LENGTH_ENCODED_BLACK_AND_WHITE || objTargaHeader.ImageType == ImageType.RUN_LENGTH_ENCODED_COLOR_MAPPED || objTargaHeader.ImageType == ImageType.RUN_LENGTH_ENCODED_TRUE_COLOR)
                    {
                        int i = 0;
                        int num3 = 0;
                        while (i < num2)
                        {
                            byte b = binReader.ReadByte();
                            int bits = Utilities.GetBits(b, 7, 1);
                            int num4 = Utilities.GetBits(b, 0, 7) + 1;
                            if (bits == 1)
                            {
                                byte[] array2 = binReader.ReadBytes(objTargaHeader.BytesPerPixel);
                                for (int j = 0; j < num4; j++)
                                {
                                    byte[] array3 = array2;
                                    for (int k = 0; k < array3.Length; k++)
                                    {
                                        byte item = array3[k];
                                        row.Add(item);
                                    }
                                    num3 += array2.Length;
                                    i += array2.Length;
                                    if (num3 == num)
                                    {
                                        rows.Add(row);
                                        row = new List<byte>();
                                        num3 = 0;
                                    }
                                }
                            }
                            else if (bits == 0)
                            {
                                int num5 = num4 * objTargaHeader.BytesPerPixel;
                                for (int l = 0; l < num5; l++)
                                {
                                    row.Add(binReader.ReadByte());
                                    i++;
                                    num3++;
                                    if (num3 == num)
                                    {
                                        rows.Add(row);
                                        row = new List<byte>();
                                        num3 = 0;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        for (int m = 0; m < (int)objTargaHeader.Height; m++)
                        {
                            for (int n = 0; n < num; n++)
                            {
                                row.Add(binReader.ReadByte());
                            }
                            rows.Add(row);
                            row = new List<byte>();
                        }
                    }
                    bool flag = false;
                    bool flag2 = false;
                    switch (objTargaHeader.FirstPixelDestination)
                    {
                        case FirstPixelDestination.UNKNOWN:
                        case FirstPixelDestination.BOTTOM_RIGHT:
                            flag = true;
                            flag2 = false;
                            break;
                        case FirstPixelDestination.TOP_LEFT:
                            flag = false;
                            flag2 = true;
                            break;
                        case FirstPixelDestination.TOP_RIGHT:
                            flag = false;
                            flag2 = false;
                            break;
                        case FirstPixelDestination.BOTTOM_LEFT:
                            flag = true;
                            flag2 = true;
                            break;
                    }
                    MemoryStream memoryStream2;
                    MemoryStream memoryStream = memoryStream2 = new MemoryStream();
                    try
                    {
                        if (flag)
                        {
                            rows.Reverse();
                        }
                        for (int num6 = 0; num6 < rows.Count; num6++)
                        {
                            if (flag2)
                            {
                                rows[num6].Reverse();
                            }
                            byte[] array4 = rows[num6].ToArray();
                            memoryStream.Write(array4, 0, array4.Length);
                            memoryStream.Write(array, 0, array.Length);
                        }
                        result = memoryStream.ToArray();
                        return result;
                    }
                    finally
                    {
                        if (memoryStream2 != null)
                        {
                            ((IDisposable)memoryStream2).Dispose();
                        }
                    }
                }
                ClearAll();
                throw new Exception("Error loading file, No image data in file.");
            }
            ClearAll();
            throw new Exception("Error loading file, could not read file from disk.");
        }

        private void LoadTGAImage(BinaryReader binReader)
        {
            intStride = (objTargaHeader.Width * (short)objTargaHeader.PixelDepth + 31 & -32) >> 3;
            intPadding = intStride - (int)((objTargaHeader.Width * (short)objTargaHeader.PixelDepth + 7) / 8);
            byte[] value = LoadImageBytes(binReader);
            ImageByteHandle = GCHandle.Alloc(value, GCHandleType.Pinned);
            if (bmpTargaImage != null)
            {
                bmpTargaImage.Dispose();
            }
            if (bmpImageThumbnail != null)
            {
                bmpImageThumbnail.Dispose();
            }
            PixelFormat pixelFormat = GetPixelFormat();
            bmpTargaImage = new Bitmap((int)objTargaHeader.Width, (int)objTargaHeader.Height, intStride, pixelFormat, ImageByteHandle.AddrOfPinnedObject());
            LoadThumbnail(binReader, pixelFormat);
            if (objTargaHeader.ColorMap.Count > 0)
            {
                ColorPalette palette = bmpTargaImage.Palette;
                for (int i = 0; i < objTargaHeader.ColorMap.Count; i++)
                {
                    if (objTargaExtensionArea.AttributesType == 0 || objTargaExtensionArea.AttributesType == 1)
                    {
                        palette.Entries[i] = Color.FromArgb(255, (int)objTargaHeader.ColorMap[i].R, (int)objTargaHeader.ColorMap[i].G, (int)objTargaHeader.ColorMap[i].B);
                    }
                    else
                    {
                        palette.Entries[i] = objTargaHeader.ColorMap[i];
                    }
                }
                bmpTargaImage.Palette = palette;
                if (bmpImageThumbnail != null)
                {
                    bmpImageThumbnail.Palette = palette;
                    return;
                }
            }
            else if (objTargaHeader.PixelDepth == 8 && (objTargaHeader.ImageType == ImageType.UNCOMPRESSED_BLACK_AND_WHITE || objTargaHeader.ImageType == ImageType.RUN_LENGTH_ENCODED_BLACK_AND_WHITE))
            {
                ColorPalette palette2 = bmpTargaImage.Palette;
                for (int j = 0; j < 256; j++)
                {
                    palette2.Entries[j] = Color.FromArgb(j, j, j);
                }
                bmpTargaImage.Palette = palette2;
                if (bmpImageThumbnail != null)
                {
                    bmpImageThumbnail.Palette = palette2;
                }
            }
        }

        private PixelFormat GetPixelFormat()
        {
            PixelFormat result = PixelFormat.Undefined;
            byte pixelDepth = objTargaHeader.PixelDepth;
            if (pixelDepth <= 16)
            {
                if (pixelDepth != 8)
                {
                    if (pixelDepth == 16)
                    {
                        if (Format == TGAFormat.NEW_TGA)
                        {
                            switch (objTargaExtensionArea.AttributesType)
                            {
                                case 0:
                                case 1:
                                case 2:
                                    result = PixelFormat.Format16bppRgb555;
                                    break;
                                case 3:
                                    result = PixelFormat.Format16bppArgb1555;
                                    break;
                            }
                        }
                        else
                        {
                            result = PixelFormat.Format16bppRgb555;
                        }
                    }
                }
                else
                {
                    result = PixelFormat.Format8bppIndexed;
                }
            }
            else if (pixelDepth != 24)
            {
                if (pixelDepth == 32)
                {
                    if (Format == TGAFormat.NEW_TGA)
                    {
                        switch (objTargaExtensionArea.AttributesType)
                        {
                            case 0:
                            case 3:
                                result = PixelFormat.Format32bppArgb;
                                break;
                            case 1:
                            case 2:
                                result = PixelFormat.Format32bppRgb;
                                break;
                            case 4:
                                result = PixelFormat.Format32bppPArgb;
                                break;
                        }
                    }
                    else
                    {
                        result = PixelFormat.Format32bppRgb;
                    }
                }
            }
            else
            {
                result = PixelFormat.Format24bppRgb;
            }
            return result;
        }

        private void LoadThumbnail(BinaryReader binReader, PixelFormat pfPixelFormat)
        {
            byte[] array = null;
            if (binReader != null && binReader.BaseStream != null && binReader.BaseStream.Length > 0L && binReader.BaseStream.CanSeek)
            {
                if (ExtensionArea.PostageStampOffset > 0)
                {
                    binReader.BaseStream.Seek((long)ExtensionArea.PostageStampOffset, SeekOrigin.Begin);
                    int num = (int)binReader.ReadByte();
                    int num2 = (int)binReader.ReadByte();
                    int num3 = (num * (int)objTargaHeader.PixelDepth + 31 & -32) >> 3;
                    int num4 = num3 - (num * (int)objTargaHeader.PixelDepth + 7) / 8;
                    List<List<byte>> list = new List<List<byte>>();
                    List<byte> list2 = new List<byte>();
                    byte[] array2 = new byte[num4];
                    bool flag = false;
                    bool flag2 = false;
                    MemoryStream memoryStream2;
                    MemoryStream memoryStream = memoryStream2 = new MemoryStream();
                    try
                    {
                        int num5 = num * (int)(objTargaHeader.PixelDepth / 8);
                        for (int i = 0; i < num2; i++)
                        {
                            for (int j = 0; j < num5; j++)
                            {
                                list2.Add(binReader.ReadByte());
                            }
                            list.Add(list2);
                            list2 = new List<byte>();
                        }
                        switch (objTargaHeader.FirstPixelDestination)
                        {
                            case FirstPixelDestination.UNKNOWN:
                            case FirstPixelDestination.BOTTOM_RIGHT:
                                flag2 = true;
                                flag = false;
                                break;
                            case FirstPixelDestination.TOP_RIGHT:
                                flag2 = false;
                                flag = false;
                                break;
                        }
                        if (flag2)
                        {
                            list.Reverse();
                        }
                        for (int k = 0; k < list.Count; k++)
                        {
                            if (flag)
                            {
                                list[k].Reverse();
                            }
                            byte[] array3 = list[k].ToArray();
                            memoryStream.Write(array3, 0, array3.Length);
                            memoryStream.Write(array2, 0, array2.Length);
                        }
                        array = memoryStream.ToArray();
                    }
                    finally
                    {
                        if (memoryStream2 != null)
                        {
                            ((IDisposable)memoryStream2).Dispose();
                        }
                    }
                    if (array != null && array.Length > 0)
                    {
                        ThumbnailByteHandle = GCHandle.Alloc(array, GCHandleType.Pinned);
                        bmpImageThumbnail = new Bitmap(num, num2, num3, pfPixelFormat, ThumbnailByteHandle.AddrOfPinnedObject());
                        return;
                    }
                }
                else if (bmpImageThumbnail != null)
                {
                    bmpImageThumbnail.Dispose();
                    bmpImageThumbnail = null;
                    return;
                }
            }
            else if (bmpImageThumbnail != null)
            {
                bmpImageThumbnail.Dispose();
                bmpImageThumbnail = null;
            }
        }

        private void ClearAll()
        {
            if (bmpTargaImage != null)
            {
                bmpTargaImage.Dispose();
                bmpTargaImage = null;
            }
            if (ImageByteHandle.IsAllocated)
            {
                ImageByteHandle.Free();
            }
            if (ThumbnailByteHandle.IsAllocated)
            {
                ThumbnailByteHandle.Free();
            }
            objTargaHeader = new TargaHeader();
            objTargaExtensionArea = new TargaExtensionArea();
            objTargaFooter = new TargaFooter();
            eTGAFormat = TGAFormat.UNKNOWN;
            intStride = 0;
            intPadding = 0;
            rows.Clear();
            row.Clear();
            strFileName = string.Empty;
        }

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        public static Bitmap LoadTargaImage(string sFileName)
#pragma warning restore CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        {
            Bitmap result = null;
            using (TargaImage targaImage = new TargaImage(sFileName))
            {
                result = new Bitmap(targaImage.Image);
            }
            return result;
        }

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        public void Dispose()
#pragma warning restore CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        protected virtual void Dispose(bool disposing)
#pragma warning restore CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        {
            if (!disposed && disposing)
            {
                if (bmpTargaImage != null)
                {
                    bmpTargaImage.Dispose();
                }
                if (bmpImageThumbnail != null)
                {
                    bmpImageThumbnail.Dispose();
                }
                if (ImageByteHandle.IsAllocated)
                {
                    ImageByteHandle.Free();
                }
                if (ThumbnailByteHandle.IsAllocated)
                {
                    ThumbnailByteHandle.Free();
                }
            }
            disposed = true;
        }
    }
}
