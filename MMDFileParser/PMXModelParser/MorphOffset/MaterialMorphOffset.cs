using SlimDX;
using System.IO;

namespace MMDFileParser.PMXModelParser.MorphOffset
{
    public class MaterialMorphOffset : MorphOffsetBase
    {
        public int MaterialIndex
        {
            get;
            private set;
        }

        public byte OffsetCalclationType
        {
            get;
            private set;
        }

        public Vector4 Diffuse
        {
            get;
            private set;
        }

        public Vector3 Specular
        {
            get;
            private set;
        }

        public float SpecularCoefficient
        {
            get;
            private set;
        }

        public Vector3 Ambient
        {
            get;
            private set;
        }

        public Vector4 EdgeColor
        {
            get;
            private set;
        }

        public float EdgeSize
        {
            get;
            private set;
        }

        public Vector4 TextureCoefficient
        {
            get;
            private set;
        }

        public Vector4 SphereTextureCoefficient
        {
            get;
            private set;
        }

        public Vector4 ToonTextureCoefficient
        {
            get;
            private set;
        }

        internal static MaterialMorphOffset getMaterialMorph(FileStream fs, Header header)
        {
            return new MaterialMorphOffset
            {
                type = MorphType.Matrial,
                MaterialIndex = ParserHelper.getIndex(fs, header.MaterialIndexSize),
                OffsetCalclationType = ParserHelper.getByte(fs),
                Diffuse = ParserHelper.getFloat4(fs),
                Specular = ParserHelper.getFloat3(fs),
                SpecularCoefficient = ParserHelper.getFloat(fs),
                Ambient = ParserHelper.getFloat3(fs),
                EdgeColor = ParserHelper.getFloat4(fs),
                EdgeSize = ParserHelper.getFloat(fs),
                TextureCoefficient = ParserHelper.getFloat4(fs),
                SphereTextureCoefficient = ParserHelper.getFloat4(fs),
                ToonTextureCoefficient = ParserHelper.getFloat4(fs)
            };
        }
    }
}
