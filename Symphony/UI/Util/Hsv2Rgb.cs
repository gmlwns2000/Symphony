using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Symphony.UI
{
    public static class Hsv2Rgb
    {
        //Convertion Source http://svn.int64.org/viewvc/int64/snips/HslColorExtension.cs?view=markup ;

        private static double GetRGB(double v, double tmp1, double tmp2)
	    {
	        if(v< 1.0 / 6.0)
	            return tmp1 + (tmp2 - tmp1) * 6.0f * v;
	            
	        if(v< 0.5)
	            return tmp2;
	            
	        if(v< 2.0 / 3.0)
	            return tmp1 + (tmp2 - tmp1) * ((2.0 / 3.0) - v) * 6.0;
	            
	        return tmp1;
	    }
	
	    private static float GetScRGB(double rgb)
	    {
	        if(rgb<double.Epsilon)
	            return 0.0f;
	
	        if(rgb <= 0.04045)
	            return (float)(rgb* (1.0 / 12.92));
	
	        if(rgb< 1.0)
	            return (float)Math.Pow((rgb + 0.055) * (1.0 / 1.055), 2.4);
	
	        return 1.0f;
	    }

        public static Color GetColor(double hue, double saturation, double brightness, double alpha)
        {
            saturation = saturation / 100;
            brightness = brightness / 100;
            hue = hue / 360;
            double r = 0, g = 0, b = 0;
            if (saturation == 0)
            {
                r = g = b = (double)(brightness * 255.0f + 0.5f);
            }
            else
            {
                double h = (hue - (double)Math.Floor(hue)) * 6.0f;
                double f = h - (double)Math.Floor(h);
                double p = brightness * (1.0f - saturation);
                double q = brightness * (1.0f - saturation * f);
                double t = brightness * (1.0f - (saturation * (1.0f - f)));
                switch ((int)h)
                {
                    case 0:
                        r = (int)(brightness * 255.0f + 0.5f);
                        g = (int)(t * 255.0f + 0.5f);
                        b = (int)(p * 255.0f + 0.5f);
                        break;
                    case 1:
                        r = (int)(q * 255.0f + 0.5f);
                        g = (int)(brightness * 255.0f + 0.5f);
                        b = (int)(p * 255.0f + 0.5f);
                        break;
                    case 2:
                        r = (int)(p * 255.0f + 0.5f);
                        g = (int)(brightness * 255.0f + 0.5f);
                        b = (int)(t * 255.0f + 0.5f);
                        break;
                    case 3:
                        r = (int)(p * 255.0f + 0.5f);
                        g = (int)(q * 255.0f + 0.5f);
                        b = (int)(brightness * 255.0f + 0.5f);
                        break;
                    case 4:
                        r = (int)(t * 255.0f + 0.5f);
                        g = (int)(p * 255.0f + 0.5f);
                        b = (int)(brightness * 255.0f + 0.5f);
                        break;
                    case 5:
                        r = (int)(brightness * 255.0f + 0.5f);
                        g = (int)(p * 255.0f + 0.5f);
                        b = (int)(q * 255.0f + 0.5f);
                        break;
                }
            }
            return Color.FromArgb(Convert.ToByte(alpha*255), 
                Convert.ToByte(Math.Max(0, Math.Min(255, r))), 
                Convert.ToByte(Math.Min(255, Math.Max(0, g))), 
                Convert.ToByte(Math.Min(255, Math.Max(0, b))));
        }

        public static void ColorToHSV(Color c, ref double hue, ref double saturation, ref double value)
        {
            System.Drawing.Color color = System.Drawing.Color.FromArgb(c.A, c.R, c.G, c.B);

            int max = Math.Max(color.R, Math.Max(color.G, color.B));
            int min = Math.Min(color.R, Math.Min(color.G, color.B));

            hue = color.GetHue();
            saturation = (max == 0) ? 0 : 1d - (1d * min / max);
            saturation = saturation * 100;
            value = max / 255d;
            value = value * 100;
        }
    }
}
