namespace MMDFileParser
{
    public static class CGHelper
    {
        public static float Clamp(float value, float min, float max)
        {
            float result;
            if (min > value)
            {
                result = min;
            }
            else if (max < value)
            {
                result = max;
            }
            else
            {
                result = value;
            }
            return result;
        }
    }
}
