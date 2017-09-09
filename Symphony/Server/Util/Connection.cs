using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Symphony.Server
{
    public static class Connection
    {
        private static string WebAddress = @"http://symphony.dothome.co.kr/v1/";
        private static string Localhost = @"http://localhost/";
        public static string Address = WebAddress;

        public static string PHP_DataSearch = Address + @"data/search.php";

        public static string PHP_LyricUpload = Address + @"data/upload_lyric.php";
        public static string DIR_LyricFolder = Address + @"upload/lyric/";

        public static string PHP_IsLogined = Address + @"session/islogin.php";
        public static string PHP_Login = Address + @"session/login.php";
        public static string PHP_Logout = Address + @"session/logout.php";
        public static string PHP_UserExist = Address + @"session/user_exists.php";
        public static string PHP_Register = Address + @"session/register.php";
        public static string PHP_UnRegister = Address + @"session/unregister.php";
        public static string PHP_EditAccount = Address + @"session/edit_user.php";

        public static string PHP_SongSearch = Address + @"song/search.php";
        public static string PHP_SongRegister = Address + @"song/register.php";
    }
}
