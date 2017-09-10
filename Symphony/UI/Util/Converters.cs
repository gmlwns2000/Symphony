using Symphony.Lyrics;
using Symphony.Player;
using Symphony.Player.DSP.CSCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Symphony.UI
{
    public static class ConvertHelper
    {
        public static double TryToDouble(object obj)
        {
            if (obj is double)
            {
                return (double)obj;
            }
            else if (obj is int)
            {
                return (int)obj;
            }
            else if(obj is long)
            {
                return (long)obj;
            }
            else
            {
                throw new NotFiniteNumberException();
            }
        }
    }

    public class DoubleToText : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = ConvertHelper.TryToDouble(value);
            if (val % 1 == 0)
                return val.ToString();
            else
                return val.ToString("0.0");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return System.Convert.ToDouble((string)value);
        }
    }

    public class MsToFPS : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return 1000 / ConvertHelper.TryToDouble(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Math.Max(1, 1000 / ConvertHelper.TryToDouble(value));
        }
    }

    public class PercentToText : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return $"{Math.Round(ConvertHelper.TryToDouble(value) * 100)}%";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class FadeInModeToIndex : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int)((FadeInMode)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (FadeInMode)((int)value);
        }
    }

    public class FadeOutModeToIndex : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int)((FadeOutMode)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (FadeOutMode)((int)value);
        }
    }

    public class HorizontalAlignmentToIndex : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int)((HorizontalAlignment)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (HorizontalAlignment)((int)value);
        }
    }

    public class VerticalAlignmentToIndex : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int)((VerticalAlignment)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (VerticalAlignment)((int)value);
        }
    }

    public class BarRenderTypesToIndex : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int)((BarRenderTypes)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (BarRenderTypes)((int)value);
        }
    }

    public class ScalingStrategyToIndex : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int)((ScalingStrategy)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (ScalingStrategy)((int)value);
        }
    }

    public class ResamplingModeToIndex : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int)((ResamplingMode)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (ResamplingMode)((int)value);
        }
    }
}
