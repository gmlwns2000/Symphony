using SlimDX;

namespace OpenMMDFormat
{
    public static class OpenMMDFormatVecExtension
    {
        public static Vector2 ToSlimDX(this bvec2 vec)
        {
            return new Vector2(vec.x, vec.y);
        }

        public static Quaternion ToSlimDX(this vec4 vec)
        {
            return new Quaternion(vec.x, vec.y, vec.z, vec.w);
        }
    }
}
