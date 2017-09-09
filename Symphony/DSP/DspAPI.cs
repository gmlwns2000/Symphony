using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Symphony.Player;

namespace Symphony.DSP
{
    public class DspAPI
    {
        private bool debug = true;
        private PlayerLogger log;
        private string Name;

        public DspAPI(string Name)
        {
            this.Name = Name;
            log = new PlayerLogger(Name);
        }
        
        public void SetDebug(bool debug)
        {
            this.debug = debug;
            log.debug = debug;
        }

        public void Test()
        {
            log.dlog("API Test");
        }
    }
}
