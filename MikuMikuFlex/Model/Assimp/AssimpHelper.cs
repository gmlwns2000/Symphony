using Assimp;
using SlimDX;

namespace MMF.Model.Assimp
{
    public static class AssimpHelper
    {
        public static Vector4 ToSlimDX(this Color4D color)
        {
            return new Vector4(color.R, color.G, color.B, color.A);
        }

        public static Vector4 ToSlimDXVec4(this Vector3D vec, float w = 1f)
        {
            return new Vector4(vec.X, vec.Y, vec.Z, w);
        }

        public static Vector3 ToSlimDX(this Vector3D vec)
        {
            return new Vector3(vec.X, vec.Y, vec.Z);
        }

        public static Vector2 ToSlimDXVec2(this Vector3D vec)
        {
            return new Vector2(vec.X, vec.Y);
        }

        public static Vector3 InvZ(this Vector3 source)
        {
            source.Z = -source.Z;
            return source;
        }

        public static Vector4 InvZ(this Vector4 source)
        {
            source.Z = -source.Z;
            return source;
        }

        public static Vector3 InvX(this Vector3 source)
        {
            source.X = -source.X;
            return source;
        }

        public static Vector4 InvX(this Vector4 source)
        {
            source.X = -source.X;
            return source;
        }

        public static string getFilterString(AssimpFileFilter fileFilter)
        {
            System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
            stringBuilder.Append("全てのファイル|*.*|");
            if (fileFilter.HasFlag(AssimpFileFilter.CommonModelFile))
            {
                stringBuilder.Append("COLLADAファイル|*.dae|");
                stringBuilder.Append("Blenderファイル|*.blend|");
                stringBuilder.Append("3DS Max 3DSファイル|*.3ds|");
                stringBuilder.Append("3DS Max ASEファイル|*.ase|");
                stringBuilder.Append("Wavefront Objectファイル|*.obj|");
                stringBuilder.Append("IFCファイル|*.ifc|");
                stringBuilder.Append("XGLファイル|*.xgl|");
                stringBuilder.Append("Auto CAD DXF|*.dxf|");
                stringBuilder.Append("LiveWaveファイル|*.lwo|");
                stringBuilder.Append("LiveWave Sceneファイル|*.lws|");
                stringBuilder.Append("Modoファイル|*.lxo|");
                stringBuilder.Append("StereoLithoGraphyファイル|*.stl|");
                stringBuilder.Append("DirectX Xファイル|*.x|");
                stringBuilder.Append("AC3Dファイル|*.ac|");
                stringBuilder.Append("MilkShape3D ファイル|*.ms3d|");
                stringBuilder.Append("TrueSpaceファイル|*.cob,*.scn|");
            }
            if (fileFilter.HasFlag(AssimpFileFilter.CommonGameEngineFile))
            {
                stringBuilder.Append("Ogre XMLファイル|*.xml|");
                stringBuilder.Append("Irrlicht Meshファイル|.irrmesh|");
                stringBuilder.Append("Irrlicht Sceneファイル|*.irr|");
            }
            if (fileFilter.HasFlag(AssimpFileFilter.CommonGameFile))
            {
                stringBuilder.Append("Quakeファイル|*.mdl,*.md2,*.md3,*.pk3|");
                stringBuilder.Append("Return to Castle Wolfenstein|*.mdc|");
                stringBuilder.Append("Doom3ファイル|*.md5|");
                stringBuilder.Append("ValveModel|*.smd,*.vta|");
                stringBuilder.Append("Starcraft II M3|*.m3|");
                stringBuilder.Append("Unreal|*.3d|");
            }
            if (fileFilter.HasFlag(AssimpFileFilter.OtherFile))
            {
                stringBuilder.Append("BlitzBasic3Dファイル|*.b3d|");
                stringBuilder.Append("Quick3Dファイル|*.q3d,*.q3s|");
                stringBuilder.Append("Neutral File Format/Sence8ファイル|*.nff|");
                stringBuilder.Append("ObjectFileFormatファイル|*.off|");
                stringBuilder.Append("PovRAY Rawファイル|*.raw|");
                stringBuilder.Append("Terragen Terrainファイル|*.ter|");
                stringBuilder.Append("3DGSファイル|*.mdl,*.hmp|");
                stringBuilder.Append("Izware Nendoファイル|*.ndo|");
            }
            if (stringBuilder.Length > 0)
            {
                stringBuilder.Remove(stringBuilder.Length - 1, 1);
            }
            return stringBuilder.ToString();
        }
    }
}
