using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Symphony.Util
{
    public class TextTool
    {
        public static string FileNameFix(string path)
        {
            char[] pathChars = Path.GetInvalidFileNameChars();

            foreach (char c in pathChars)
            {
                path = path.Replace(c, '-');
            }

            path = Regex.Replace(path, "[\x00-\x08\x0B\x0C\x0E-\x1F]", string.Empty, RegexOptions.Compiled);

            return path.Trim();
        }

        public static bool StringEmpty(string text)
        {
            return string.IsNullOrEmpty(text) || string.IsNullOrWhiteSpace(text);
        }

        public static string SafeSpecialChars(string text)
        {
            if (text == null)
                return null;

            text = text.Replace(@"\", @"\\");
            text = text.Replace(@"'", @"\'");
            text = text.Replace(@"`", @"\`");
            return text;
        }

        public static string UnsafeSpecialChars(string text)
        {
            if (text == null)
                return null;

            text = text.Replace(@"\`", @"`");
            text = text.Replace(@"\'", @"'");
            text = text.Replace(@"\\", @"\");
            return text;
        }
    }
}
