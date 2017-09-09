using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symphony.Dancer
{
    public class PlotSearchResult
    {
        public readonly PlotExistState Exist;
        public readonly string WebPath;
        public readonly string LocalPath;

        public PlotSearchResult(PlotExistState exist, string webPath = null, string localPath = null)
        {
            Exist = exist;
            WebPath = webPath;
            LocalPath = localPath;
        }
    }

    public enum PlotExistState
    {
        /// <summary>
        /// 로컬에 플롯이 존재하지만 인터넷 연결이 없습니다.
        /// </summary>
        UnsureLocal = 0,

        /// <summary>
        /// 로컬에만 플롯이 존재합니다.
        /// </summary>
        LocalExist = 1,

        /// <summary>
        /// 로컬과 웹에 플롯이 존재합니다.
        /// </summary>
        GlobalExist = 2,

        /// <summary>
        /// 웹에 플롯이 존재합니다.
        /// </summary>
        WebExitst = 3,

        /// <summary>
        /// 존재하지 않습니다.
        /// </summary>
        None = -1
    }
}
