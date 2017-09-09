using System.Drawing;

namespace Paloma
{
    internal static class Utilities
    {
        internal static int GetBits(byte b, int offset, int count)
        {
            return b >> offset & (1 << count) - 1;
        }

        internal static Color GetColorFrom2Bytes(byte one, byte two)
        {
            int bits = Utilities.GetBits(one, 2, 5);
            int red = bits << 3;
            int bits2 = Utilities.GetBits(one, 0, 2);
            int num = bits2 << 6;
            bits2 = Utilities.GetBits(two, 5, 3);
            int num2 = bits2 << 3;
            int green = num + num2;
            int bits3 = Utilities.GetBits(two, 0, 5);
            int blue = bits3 << 3;
            int bits4 = Utilities.GetBits(one, 7, 1);
            int alpha = bits4 * 255;
            return Color.FromArgb(alpha, red, green, blue);
        }

        internal static string GetIntBinaryString(int n)
        {
            char[] array = new char[32];
            int num = 31;
            for (int i = 0; i < 32; i++)
            {
                if ((n & 1 << i) != 0)
                {
                    array[num] = '1';
                }
                else
                {
                    array[num] = '0';
                }
                num--;
            }
            return new string(array);
        }

        internal static string GetInt16BinaryString(short n)
        {
            char[] array = new char[16];
            int num = 15;
            for (int i = 0; i < 16; i++)
            {
                if (((int)n & 1 << i) != 0)
                {
                    array[num] = '1';
                }
                else
                {
                    array[num] = '0';
                }
                num--;
            }
            return new string(array);
        }
    }
}