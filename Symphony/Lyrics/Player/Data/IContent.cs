using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Symphony.Lyrics
{
    public class IContent
    {
        public FrameworkElement Content { get; set; }

        //Layout Field
        private bool _useLayoutRounding = true;
        public bool UseLayoutRounding
        {
            get
            {
                return _useLayoutRounding;
            }
            set
            {
                if (_useLayoutRounding != value)
                {
                    _useLayoutRounding = value;
                    RequireUpdate = true;
                }
            }
        }


        public bool RequireUpdate { get; set; }
        public virtual void Update()
        {

        }
    }
}
