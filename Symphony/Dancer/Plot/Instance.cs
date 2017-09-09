using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Symphony.Dancer
{
    /// <summary>
    /// 인스턴스의 정보를 저장하고, 키프레임을 편집, 저장 합니다
    /// </summary>
    public class Instance : IDisposable
    {
        public string Name { get; set; }
        public double StartPosition { get; set; }
        public double Duration { get; set; }
        public bool View { get; set; }

        public Instance(string Name, double startPosition, double duration)
        {
            this.Name = Name;
            StartPosition = startPosition;
            Duration = duration;
        }

        /// <summary>
        /// 자신을 월드에 생성합니다
        /// </summary>
        /// <param name="RenderControl"></param>
        public virtual void OnLoad(MMF.Controls.WPF.WPFRenderControl RenderControl, string workingDirectory)
        {

        }

        public virtual void OnPlayStarted(double Position)
        {

        }

        public virtual void OnPauseChanged(bool IsPaused)
        {

        }

        public virtual void OnSeeked(double NewPosition)
        {

        }

        public virtual void Dispose()
        {

        }
    }
}
