using Symphony.Util;
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

namespace Symphony.UI.Settings
{
    /// <summary>
    /// SkinGradientStopEditor.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SkinGradientStopEditor : UserControl
    {
        bool inited = false;
        GradientStopCollection coll;

        private int _selectedIndex = -1;
        public int SelectedIndex
        {
            get
            {
                return _selectedIndex;
            }
            set
            {
                if (value != _selectedIndex)
                {
                    _selectedIndex = value;
                }
            }
        }

        public event EventHandler<ObjectChangedArgs<GradientStopCollection>> GradientStopCollectionUpdated;
        public event EventHandler<int> SelectionChanged;

        public SkinGradientStopEditor()
        {
            InitializeComponent();
        }

        public void Init(GradientStopCollection coll)
        {
            this.coll = coll.Clone();

            Update();
        }

        public void Update()
        {
            inited = false;

            pinGrid.Children.Clear();

            int index = 0;
            foreach(GradientStop stop in coll)
            {
                SkinGradientStopPinEditor pinEditor = new SkinGradientStopPinEditor(stop, pinGrid);

                if(SelectedIndex == index)
                {
                    pinEditor.Selected = true;
                }

                pinGrid.Children.Add(pinEditor);

                pinEditor.GradientStopUpdated += delegate (object sender, ObjectChangedArgs<GradientStop> arg)
                {
                    GradientStopCollection newColl = new GradientStopCollection();

                    foreach(SkinGradientStopPinEditor pe in pinGrid.Children)
                    {
                        newColl.Add(pe.stop);
                    }

                    coll = newColl;

                    UpdateBrush();

                    GradientStopCollectionUpdated?.Invoke(this, new ObjectChangedArgs<GradientStopCollection>(newColl));
                };

                pinEditor.Clicked += delegate (object sender, ObjectChangedArgs<GradientStop> arg)
                {
                    SelectedIndex = coll.IndexOf(arg.Object);

                    foreach(SkinGradientStopPinEditor p in pinGrid.Children)
                    {
                        p.Selected = false;
                    }

                    pinEditor.Selected = true;

                    SelectionChanged?.Invoke(this, SelectedIndex);
                };

                index++;
            }

            UpdateBrush();

            inited = true;
        }

        private void UpdateBrush()
        {
            LinearGradientBrush brush = new LinearGradientBrush(coll.Clone(), new Point(0, 0.5), new Point(1, 0.5));
            brush.Freeze();

            gradient.Fill = brush;
        }

        private void gradient_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(inited && e.ClickCount == 2)
            {
                Point pt = e.GetPosition(pinGrid);

                double offset = Math.Min(1, Math.Max(0, pt.X / pinGrid.ActualWidth));

                coll.Add(new GradientStop(RussianRullet.RandomColor(), offset));
                
                SelectedIndex = coll.Count - 1;

                GradientStopCollectionUpdated?.Invoke(this, new ObjectChangedArgs<GradientStopCollection>(coll));

                SelectionChanged?.Invoke(this, SelectedIndex);

                Update();
            }
        }

        private void Bt_Remove_Click(object sender, RoutedEventArgs e)
        {
            if (inited && coll.Count > 1 && SelectedIndex > -1 && SelectedIndex < coll.Count)
            {
                coll.RemoveAt(SelectedIndex);

                foreach(SkinGradientStopPinEditor p in pinGrid.Children)
                {
                    p.Selected = false;
                }

                SelectedIndex = -1;

                GradientStopCollectionUpdated?.Invoke(this, new ObjectChangedArgs<GradientStopCollection>(coll));

                SelectionChanged?.Invoke(this, SelectedIndex);

                Update();
            }
        }

        private void UserControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (inited)
            {
                if (e.Key == Key.Delete)
                {
                    Bt_Remove_Click(sender, null);
                }
            }
        }
    }
}
