using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CEP.Common
{
    public class Task
    {
        private static long _ID = 0;
        public object ID { get; set; }
        public object ProcessID { get; set; }
        public object TaskType { get; set; }
        public object TaskActor { get; set; }

        public int Index { get; set; }

        public Task ()
        {
            this.ID = newIdentifier();
            this.Index = 0;
        }

        private static string newIdentifier()
        {
            _ID++;
            var tID = ("0000000000" + _ID.ToString(CultureInfo.InvariantCulture));
            return String.Format("T{0}", tID.Substring(tID.Length - 10, 10));
        }

      
    }
}
