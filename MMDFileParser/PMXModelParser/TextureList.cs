using System.Collections.Generic;
using System.IO;

namespace MMDFileParser.PMXModelParser
{
    public class TextureList
    {
        public int TextureCount
        {
            get;
            private set;
        }

        public List<string> TexturePathes
        {
            get;
            private set;
        }

        internal static TextureList getTextureList(FileStream fs, Header header)
        {
            TextureList textureList = new TextureList();
            textureList.TexturePathes = new List<string>();
            textureList.TextureCount = ParserHelper.getInt(fs);
            for (int i = 0; i < textureList.TextureCount; i++)
            {
                textureList.TexturePathes.Add(ParserHelper.getTextBuf(fs, header.Encode));
            }
            return textureList;
        }
    }
}
