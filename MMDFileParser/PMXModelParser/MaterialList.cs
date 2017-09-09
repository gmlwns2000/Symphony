using System.Collections.Generic;
using System.IO;

namespace MMDFileParser.PMXModelParser
{
    public class MaterialList
    {
        public int MaterialCount
        {
            get;
            private set;
        }

        public List<MaterialData> Materials
        {
            get;
            private set;
        }

        internal static MaterialList getMaterialList(FileStream fs, Header header)
        {
            MaterialList materialList = new MaterialList();
            materialList.Materials = new List<MaterialData>();
            materialList.MaterialCount = ParserHelper.getInt(fs);
            for (int i = 0; i < materialList.MaterialCount; i++)
            {
                materialList.Materials.Add(MaterialData.getMaterial(fs, header));
            }
            return materialList;
        }

        public MaterialData getMaterialByIndex(int index)
        {
            int num = 0;
            for (int i = 0; i < MaterialCount; i++)
            {
                num += Materials[i].VertexNumber / 3;
                if (index < num)
                {
                    return Materials[i];
                }
            }
            throw new InvalidDataException();
        }
    }
}
