using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symphony.Server
{
    public class Account
    {
        public string UserID;
        public string UserEmail;
        public int UserIndex;

        public Account(string UID, string UserEmail, int UserIndex)
        {
            UserID = UID;
            this.UserEmail = UserEmail;
            this.UserIndex = UserIndex;
        }
    }
}
