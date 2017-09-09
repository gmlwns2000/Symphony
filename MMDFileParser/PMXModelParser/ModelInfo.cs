using System;
using System.IO;

namespace MMDFileParser.PMXModelParser
{
    public class ModelInfo
    {
        internal static ModelInfo getModelInfo(FileStream fs, Header header)
        {
            ModelInfo info = new ModelInfo();
            info.ModelName = ParserHelper.getTextBuf(fs, header.Encode);
            info.ModelName_En = ParserHelper.getTextBuf(fs, header.Encode);
            info.ModelComment = ParserHelper.getTextBuf(fs, header.Encode);
            info.ModelComment_En = ParserHelper.getTextBuf(fs, header.Encode);
            return info;
        }

        public String ModelName { get; private set; }

        public String ModelName_En { get; private set; }

        public String ModelComment { get; private set; }

        public String ModelComment_En { get; private set; }
    }
}
