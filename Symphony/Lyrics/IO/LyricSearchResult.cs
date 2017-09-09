using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Symphony.Server;

namespace Symphony.Lyrics
{
    public class LyricSearchResult
    {
        public readonly LyricExistState Exist;
        public RegisteredLyric RegisteredLyric;
        public readonly string LocalPath;

        public LyricSearchResult(LyricExistState exist, string localPath = null, RegisteredLyric registeredLyric = null)
        {
            Exist = exist;
            LocalPath = localPath;
            RegisteredLyric = registeredLyric;
        }
    }

    public enum LyricExistState
    {
        /// <summary>
        /// 로컬에 가사가 존재하지만 인터넷 연결이 없습니다.
        /// </summary>
        UnsureLocal = 0,

        /// <summary>
        /// 로컬에만 가사가 존재합니다.
        /// </summary>
        LocalExist = 1,

        /// <summary>
        /// 로컬과 웹에 가사가 존재합니다.
        /// </summary>
        GlobalExist = 2,

        /// <summary>
        /// 웹에 가사가 존재합니다.
        /// </summary>
        WebExitst = 3,

        /// <summary>
        /// 존재하지 않습니다.
        /// </summary>
        None = -1
    }
}
