using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Symphony.UI
{
    /// <summary>
    /// DoubleTextBox.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class DoubleTextBox : UserControl
    {
        public static DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(double), typeof(DoubleTextBox), new FrameworkPropertyMetadata(0.0, new PropertyChangedCallback((DependencyObject sender, DependencyPropertyChangedEventArgs arg)=> 
        {
            DoubleTextBox textbox = (DoubleTextBox)sender;

            textbox.ValueChanged?.Invoke(textbox, new RoutedPropertyChangedEventArgs<double>((double)arg.OldValue, (double)arg.NewValue));

            if (textbox.blockUpdate)
                return;

            textbox.inited = false;
            textbox.tb.Text = ((double)arg.NewValue).ToString("0.0");
            textbox.inited = true;
        })));

        private static SolidColorBrush _warnBrush;
        public static SolidColorBrush WarnBrush
        {
            get
            {
                if(_warnBrush == null)
                {
                    _warnBrush = new SolidColorBrush(Color.FromArgb(180, 255, 30, 50));
                    _warnBrush.Freeze();
                }
                return _warnBrush;
            }
        }

        public double Value
        {
            get
            {
                return (double)GetValue(ValueProperty);
            }
            set
            {
                SetValue(ValueProperty, value);
            }
        }


        public event EventHandler<RoutedPropertyChangedEventArgs<double>> ValueChanged;

        private Brush borderBrush;
        private bool inited = false;
        private bool blockUpdate = false;

        public DoubleTextBox()
        {
            InitializeComponent();
            Value = 0;

            borderBrush = tb.BorderBrush;

            tb.Text = Value.ToString("0.0");

            inited = true;
        }

        DispatcherTimer timer;
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!inited)
                return;

            if(timer == null)
            {
                timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromMilliseconds(200);
                timer.Tick += delegate (object obj, EventArgs arg)
                {
                    convert();

                    timer.Stop();
                };
            }

            if (timer.IsEnabled)
                timer.Stop();

            timer.Start();
        }

        private void convert()
        {
            try
            {
                string text = tb.Text;

                double value = Convert.ToDouble(text);

                blockUpdate = true;
                Value = value;
                blockUpdate = false;

                tb.BorderBrush = borderBrush;
            }
            catch
            {
                tb.BorderBrush = WarnBrush;
            }
        }

        private void tb_LostFocus(object sender, RoutedEventArgs e)
        {
            convert();

            if(inited)
                tb.Text = Value.ToString("0.0");
        }
    }
}
