using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Symphony.Dancer
{
    public enum Units
    {
        Auto = 0,
        Percent = 1,
        Point = 2,
        Pixel = 3,
    }
    public class Value
    {
        public double value;
        public Units Unit;
        public string UnitString
        {
            get
            {
                switch (Unit)
                {
                    case Units.Auto:
                        return "Auto";
                    case Units.Percent:
                        return "Percent";
                    case Units.Point:
                        return "Point";
                    case Units.Pixel:
                        return "Pixel";
                    default:
                        return "Auto";
                }
            }
            set
            {
                switch (value.ToLower())
                {
                    case "auto":
                        Unit = Units.Auto;
                        break;
                    case "percent":
                        Unit = Units.Percent;
                        break;
                    case "point":
                        Unit = Units.Point;
                        break;
                    case "pixel":
                        Unit = Units.Pixel;
                        break;
                    default:
                        Unit = Units.Auto;
                        break;
                }
            }
        }

        public Value(double value, string UnitString)
        {
            this.value = value;
            this.UnitString = UnitString;
        }

        public Value(double value, Units unit = Units.Auto)
        {
            Unit = unit;
            this.value = value;
        }

        public double ConvertPositionX(ComposerRender Renderer)
        {
            if(this.Unit == Units.Auto || this.Unit == Units.Percent)
            {
                //Auto is Percent Unit
                return this.value * Renderer.ActualWidth;
            }
            else if(this.Unit == Units.Pixel)
            {
                return this.value;
            }
            else if(this.Unit == Units.Point)
            {
                return -1;
            }
            return -1;
        }

        public double ConvertPositionY(ComposerRender Renderer)
        {
            if (this.Unit == Units.Auto || this.Unit == Units.Percent)
            {
                //Auto is Percent Unit
                return this.value * Renderer.ActualHeight;
            }
            else if (this.Unit == Units.Pixel)
            {
                return this.value;
            }
            else if (this.Unit == Units.Point)
            {
                return -1;
            }
            return -1;
        }
    }
}
