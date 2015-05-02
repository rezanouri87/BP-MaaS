using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CEP.Common
{
    public class Process
    {
        private static long _ID = 0;
        public object ID { get; set; }

        public int Index { get; set; }

        public Process ()
        {
            this.ID = newIdentifier();
            this.Index = 1;
        }

        private static string newIdentifier()
        {
            _ID++;
            var pID = ("0000000000" + _ID.ToString(CultureInfo.InvariantCulture));
            return String.Format("P{0}", pID.Substring(pID.Length - 10, 10));
        }

      
    }
}
