using System;
using System.IO;
using System.Text;

namespace MMDFileParser.PMXModelParser
{
    public class Header
    {
        public float Version
        {
            get;
            private set;
        }

        public EncodeType Encode
        {
            get;
            private set;
        }

        public int AdditionalUVCount
        {
            get;
            private set;
        }

        public int VertexIndexSize
        {
            get;
            private set;
        }

        public int TextureIndexSize
        {
            get;
            private set;
        }

        public int MaterialIndexSize
        {
            get;
            private set;
        }

        public int BoneIndexSize
        {
            get;
            private set;
        }

        public int MorphIndexSize
        {
            get;
            private set;
        }

        public int RigidBodyIndexSize
        {
            get;
            private set;
        }

        internal static Header getHeader(FileStream fs)
        {
            Header header = new Header();
            byte[] array = new byte[4];
            fs.Read(array, 0, 4);
            if (Encoding.Unicode.GetString(array, 0, 4) != "PMX " && Encoding.UTF8.GetString(array, 0, 4) != "PMX ")
            {
                throw new InvalidDataException("PMXファイルのマジックナンバーが間違っています。ファイルの破損か対応バージョンではありません。");
            }
            header.Version = ParserHelper.getFloat(fs);
            if (ParserHelper.getByte(fs) != 8)
            {
                throw new NotImplementedException();
            }
            byte[] array2 = new byte[8];
            fs.Read(array2, 0, 8);
            header.Encode = ((array2[0] == 1) ? EncodeType.UTF8 : EncodeType.UTF16LE);
            header.AdditionalUVCount = array2[1];
            header.VertexIndexSize = array2[2];
            header.TextureIndexSize = array2[3];
            header.MaterialIndexSize = array2[4];
            header.BoneIndexSize = array2[5];
            header.MorphIndexSize = array2[6];
            header.RigidBodyIndexSize = array2[7];
            return header;
        }
    }
}
