using MMDFileParser.PMXModelParser;
using SlimDX;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MMDFileParser
{
    internal static class ParserHelper
    {
        internal static string getTextBuf(Stream fs, EncodeType encode)
        {
            byte[] array = new byte[4];
            fs.Read(array, 0, 4);
            int num = BitConverter.ToInt32(array, 0);
            byte[] array2 = new byte[num];
            fs.Read(array2, 0, num);
            string @string;
            if (encode == EncodeType.UTF8)
            {
                @string = Encoding.UTF8.GetString(array2);
            }
            else
            {
                @string = Encoding.Unicode.GetString(array2);
            }
            return @string;
        }

        internal static float getFloat(Stream fs)
        {
            byte[] array = new byte[4];
            fs.Read(array, 0, 4);
            return BitConverter.ToSingle(array, 0);
        }

        internal static Vector4 getFloat4(Stream fs)
        {
            byte[] array = new byte[16];
            fs.Read(array, 0, 16);
            return new Vector4(BitConverter.ToSingle(array, 0), BitConverter.ToSingle(array, 4), BitConverter.ToSingle(array, 8), BitConverter.ToSingle(array, 12));
        }

        internal static Vector3 getFloat3(Stream fs)
        {
            byte[] array = new byte[12];
            fs.Read(array, 0, 12);
            return new Vector3(BitConverter.ToSingle(array, 0), BitConverter.ToSingle(array, 4), BitConverter.ToSingle(array, 8));
        }

        internal static Vector2 getFloat2(Stream fs)
        {
            byte[] array = new byte[8];
            fs.Read(array, 0, 8);
            return new Vector2(BitConverter.ToSingle(array, 0), BitConverter.ToSingle(array, 4));
        }

        internal static int getInt(Stream fs)
        {
            byte[] array = new byte[4];
            fs.Read(array, 0, 4);
            return BitConverter.ToInt32(array, 0);
        }

        internal static ushort getUShort(Stream fs)
        {
            byte[] array = new byte[2];
            fs.Read(array, 0, 2);
            return BitConverter.ToUInt16(array, 0);
        }

        internal static byte getByte(Stream fs)
        {
            byte[] array = new byte[1];
            fs.Read(array, 0, 1);
            return array[0];
        }

        internal static int getIndex(Stream fs, int size)
        {
            byte[] array = new byte[size];
            fs.Read(array, 0, size);
            switch (size)
            {
                case 1:
                    {
                        int result = (sbyte)array[0];
                        return result;
                    }
                case 2:
                    {
                        int result = BitConverter.ToInt16(array, 0);
                        return result;
                    }
                case 4:
                    {
                        int result = BitConverter.ToInt32(array, 0);
                        return result;
                    }
            }
            throw new InvalidDataException();
        }

        internal static uint getVertexIndex(Stream fs, int size)
        {
            byte[] array = new byte[size];
            fs.Read(array, 0, size);
            switch (size)
            {
                case 1:
                    {
                        uint result = array[0];
                        return result;
                    }
                case 2:
                    {
                        uint result = BitConverter.ToUInt16(array, 0);
                        return result;
                    }
                case 4:
                    {
                        uint result = BitConverter.ToUInt32(array, 0);
                        return result;
                    }
            }
            throw new InvalidDataException();
        }

        internal static bool isFlagEnabled(short chk, short flag)
        {
            return (chk & flag) == flag;
        }

        internal static string getShift_JISString(Stream fs, int length)
        {
            Encoding encoding = Encoding.GetEncoding("Shift_JIS");
            List<byte> list = new List<byte>();
            for (int i = 0; i < length; i++)
            {
                byte[] array = new byte[]
                {
                    ParserHelper.getByte(fs)
                };
                if (encoding.GetString(array)[0] == '\0')
                {
                    fs.Read(new byte[length - (i + 1)], 0, length - (i + 1));
                    break;
                }
                list.Add(array[0]);
            }
            return encoding.GetString(list.ToArray());
        }

        internal static uint getDWORD(Stream fs)
        {
            byte[] array = new byte[4];
            if (fs.Read(array, 0, 4) == 0)
            {
                throw new EndOfStreamException();
            }
            return BitConverter.ToUInt32(array, 0);
        }

        internal static Quaternion getQuaternion(Stream fs)
        {
            byte[] array = new byte[16];
            fs.Read(array, 0, 16);
            return new Quaternion(BitConverter.ToSingle(array, 0), BitConverter.ToSingle(array, 4), BitConverter.ToSingle(array, 8), BitConverter.ToSingle(array, 12));
        }
    }
}
