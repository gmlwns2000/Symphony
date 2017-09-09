using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Symphony.Util;
using System.Diagnostics;
using System.Collections.Specialized;

namespace Symphony.Server
{
    public enum AccountProperties
    {
        UserID = 0,
        Password = 1,
        Email = 2
    }

    public static class Session
    {
        public const string PHP_SessionIdName = "PHPSESSID";
        public static string SessionID { get; private set; }
        public static string UserID { get; private set; }
        public static string UserEmail { get; private set; }
        public static int UserIndex { get; private set; }

        public static DateTime LoginTime { get; private set; }
        private static bool _logined = false;
        private static bool isLogined
        {
            get
            {
                return _logined;
            }
            set
            {
                if(_logined != value)
                {
                    LoginChanged?.Invoke(null, null);
                }
                _logined = value;
            }
        }

        public static bool IsLogined
        {
            get
            {
                return isLogined;
            }
        }
        public static event EventHandler LoginChanged;

        public readonly static string UnableUsernames = "test,test1,test2,admin,adminester,mysql,관리자,운영자,서버관리자";
        public static string[] UnablueUsernamesArray
        {
            get
            {
                return UnableUsernames.Split(',');
            }
        }

        #region Register

        public async static Task<QueryResult> UnregisterAsync()
        {
            Task<QueryResult> task = new Task<QueryResult>(new Func<QueryResult>(() => { return Unregister(); }));

            task.Start();

            return await task;
        }

        public static QueryResult Unregister()
        {
            if (!IsLogined)
            {
                return new QueryResult(null, LanguageHelper.FindText("Lang_Server_Session_Login_Require"), false);
            }

            try
            {
                NameValueCollection nvc = new NameValueCollection();
                nvc.Add(PHP_SessionIdName, SessionID);

                QueryResult r = Data.PostAndGet.Post(Connection.PHP_UnRegister, nvc);
                if (r.Success)
                {
                    if (r.Message.EndsWith("false"))
                    {
                        if(r.Message.EndsWith("login error false"))
                        {
                            Logout();

                            return new QueryResult(r, LanguageHelper.FindText("Lang_Server_Session_Session_Expired"), false);
                        }
                        else
                        {
                            return new QueryResult(r, LanguageHelper.FindText("Lang_Server_Session_Error_Unregister"), false);
                        }
                    }
                    else
                    {
                        Logout();

                        return new QueryResult(r, LanguageHelper.FindText("Lang_Server_Session_Success_Unregister"), true);
                    }
                }
                else
                {
                    return r;
                }
            }
            catch (Exception e)
            {
                Logger.Error("Session.Unregister", e);

                return new QueryResult(null, e.ToString(), false);
            }
        }

        public static async Task<QueryResult> UserExistAsync(string UID, string PASS)
        {
            Task<QueryResult> task = new Task<QueryResult>(new Func<QueryResult>(() => { return UserExist(UID, PASS); }));

            task.Start();

            return await task;
        }

        public static QueryResult UserExist(string UID, string PASS)
        {
            NameValueCollection nvc = new NameValueCollection();
            nvc.Add("user_id", UID);
            nvc.Add("user_pwd", PASS);

            QueryResult result = Data.PostAndGet.Post(Connection.PHP_UserExist, nvc);

            if (result.Success)
            {
                if (result.Message.EndsWith("false"))
                {
                    result.Success = false;
                    result.Message = LanguageHelper.FindText("Lang_Server_Session_Check_Your_ID_And_Password");
                }
                else
                {
                    result.Success = true;
                }

                return result;
            }
            else
            {
                return result;
            }
        }

        public static async Task<QueryResult> RegisterAsync(string UID, string PASS, string EMAIL)
        {
            Task<QueryResult> task = new Task<QueryResult>(new Func<QueryResult>(() => { return Register(UID, PASS, EMAIL, 0, 5, ""); }), TaskCreationOptions.None);

            task.Start();

            return await task;
        }

        public static QueryResult Register(string UID, string PASS, string EMAIL, int retry, int maxRetry, string exception)
        {
            if(retry >= maxRetry)
            {
                return new QueryResult(null, exception, false);
            }

            QueryResult resultID = CheckID(UID);
            if (!resultID.Success)
            {
                return resultID;
            }

            QueryResult resultPass = CheckPassword(PASS);
            if (!resultPass.Success)
            {
                return resultPass;
            }

            QueryResult resultEmail = CheckEmail(EMAIL);
            if (!resultEmail.Success)
            {
                return resultEmail;
            }

            NameValueCollection nvc = new NameValueCollection();
            nvc.Add("user_id", UID);
            nvc.Add("user_pwd", PASS);
            nvc.Add("user_email", EMAIL);

            QueryResult r = Data.PostAndGet.Post(Connection.PHP_Register, nvc);

            if (r.Success)
            {
                if (r.Message.EndsWith("true"))
                {
                    return new QueryResult(r, string.Format(LanguageHelper.FindText("Lang_Server_Session_Welcome"),UID), true);
                }
                else
                {
                    return new QueryResult(r, LanguageHelper.FindText("Lang_Server_Session_Used_ID"), false);
                }
            }
            else
            {
                return r;
            }
        }

        #endregion Register

        #region Login

        public static bool CheckLogin()
        {
            try
            {
                NameValueCollection nvc = new NameValueCollection();
                nvc.Add(PHP_SessionIdName, SessionID);

                QueryResult r = Data.PostAndGet.Post(Connection.PHP_IsLogined, nvc);

                if (r.Success)
                {
                    if (r.Message.EndsWith("true"))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);

                return false;
            }
        }

        public static void Logout()
        {
            if (IsLogined)
            {
                try
                {
                    NameValueCollection nvc = new NameValueCollection();
                    nvc.Add(PHP_SessionIdName, SessionID);

                    QueryResult r = Data.PostAndGet.Post(Connection.PHP_Logout, nvc);

                    Logger.Log("LogOutMessage: ", r.Message);
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }

            isLogined = false;

            UserID = "";
            UserIndex = -1;
            SessionID = "";
            UserEmail = "";
        }

        public static async Task<QueryResult> LoginAsync(string UID, string PASS)
        {
            Task<QueryResult> task = new Task<QueryResult>(new Func<QueryResult>(() => { return Login(UID, PASS, 0, 5, ""); }), TaskCreationOptions.None);

            task.Start();

            return await task;
        }

        public static QueryResult Login(string UID, string PASS, int retry, int maxRetry, string exception)
        {
            if (retry >= maxRetry)
            {
                isLogined = false;

                return new QueryResult(null, exception, false);
            }

            try
            {
                NameValueCollection nvc = new NameValueCollection();
                nvc.Add("user_id", UID);
                nvc.Add("user_pw", PASS);

                QueryResult r = Data.PostAndGet.Post(Connection.PHP_Login, nvc);

                if (r.Success)
                {
                    if (r.Message.EndsWith("true"))
                    {
                        UserID = UID;

                        string[] spl = r.Message.Split('\n');

                        UserEmail = spl[spl.Length - 2];

                        UserIndex = Convert.ToInt32(spl[spl.Length - 3]);

                        SessionID = spl[spl.Length - 4];

                        isLogined = true;

                        return new QueryResult(r, r.Message, true);
                    }
                    else
                    {
                        isLogined = false;

                        return new QueryResult(r, LanguageHelper.FindText("Lang_Server_Session_Check_Your_ID_And_Password"), isLogined);
                    }
                }
                else
                {
                    isLogined = false;

                    return new QueryResult(r, r.Message, isLogined);
                }
            }
            catch (Exception e)
            {
                Logger.Error("Session.Login", e);
                
                Task.Delay(50);

                return Login(UID, PASS, retry + 1, maxRetry, e.ToString());
            }
        }

        #endregion Login

        #region EditAccount

        public static async Task<QueryResult> EditAccountAsync(string email, string pass)
        {
            Task < QueryResult > task = new Task<QueryResult>(new Func<QueryResult>(() => { return EditAccount(email, pass); }), TaskCreationOptions.None);

            task.Start();

            return await task;
        }

        public static QueryResult EditAccount(string email, string password)
        {
            if (!IsLogined)
            {
                return new QueryResult(null, LanguageHelper.FindText("Lang_Server_Session_Login_Require"), false);
            }

            QueryResult rEmail = CheckEmail(email);
            if (!rEmail.Success)
            {
                return rEmail;
            }

            if (password != "")
            {
                QueryResult rPass = CheckPassword(password);
                if (!rPass.Success)
                {
                    return rPass;
                }
            }

            try
            {
                NameValueCollection nvc = new NameValueCollection();
                nvc.Add(PHP_SessionIdName, SessionID);
                nvc.Add("user_index", UserIndex.ToString());
                nvc.Add("new_user_email", email);
                if (password != "")
                {
                    nvc.Add("new_user_pwd", password);
                }

                QueryResult r = Data.PostAndGet.Post(Connection.PHP_EditAccount, nvc);

                if (r.Success)
                {
                    if (r.Message.EndsWith("false"))
                    {
                        if(r.Message.EndsWith("login error false"))
                        {
                            return new QueryResult(r, LanguageHelper.FindText("Lang_Server_Session_Session_Expired"), false);
                        }
                        else
                        {
                            if (r.Message.Contains("email updated"))
                            {
                                UserEmail = email;
                            }

                            return new QueryResult(r, LanguageHelper.FindText("Lang_Server_Session_Error_Server_Program"), false);
                        }
                    }
                    else if(r.Message.EndsWith("true"))
                    {
                        return new QueryResult(r, LanguageHelper.FindText("Lang_Server_Session_Success_Edit_Account"), true);
                    }
                    else
                    {
                        return new QueryResult(r, LanguageHelper.FindText("Lang_Server_Session_Unknown_Error"), false, r);
                    }
                }
                else
                {
                    return r;
                }
            }
            catch (Exception e)
            {
                Logger.Error("Session.EditAccount", e);

                return new QueryResult(null, LanguageHelper.FindText("Lang_Server_Session_Error_EditAccount"), false);
            }
        }

        #endregion EditAccount

        #region CheckField

        public static QueryResult CheckEmail(string Email)
        {
            if (!TextTool.StringEmpty(Email) && Email.Split('@') != null && Email.Split('@').Length >= 2)
            {
                return new QueryResult(null, "", true);
            }
            else
            {
                return new QueryResult(null, LanguageHelper.FindText("Lang_Server_Session_Unknown_Email_Format"), false);
            }
        }

        public static QueryResult CheckID(string ID)
        {
            if(!TextTool.StringEmpty(ID) && ID.Length >= 4 && ID.Length <= 24)
            {
                bool cant = false;
                for(int i=0; i<UnablueUsernamesArray.Length; i++)
                {
                    if(ID == UnablueUsernamesArray[i])
                    {
                        cant = true;
                        break;
                    }
                }

                if (cant)
                {
                    return new QueryResult(null, LanguageHelper.FindText("Lang_Server_Session_ID_Cannot_Use"), false);
                }
                else
                {
                    return new QueryResult(null, "", true);
                }
            }
            else
            {
                return new QueryResult(null, LanguageHelper.FindText("Lang_Server_Session_ID_Format"), false);
            }
        }

        public static QueryResult CheckPassword(string pass)
        {
            if(!TextTool.StringEmpty(pass) && pass.Length >=6 && pass.Length <= 64)
            {
                return new QueryResult(null, "", true);
            }
            else
            {
                return new QueryResult(null, LanguageHelper.FindText("Lang_Server_Session_Password_Format"), false);
            }
        }

        #endregion CheckField
    }
}
