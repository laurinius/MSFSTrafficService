using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficService
{
    class Logger
    {
        static Logger()
        {
            File.Delete("app.log");
        }

        public static void Log(string msg)
        {
            try
            {
                using StreamWriter w = File.AppendText("app.log");
                w.WriteLine(DateTime.Now.ToString("HH:mm:ss.fff: ") + msg);
                Debug.WriteLine(msg);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }
    }
}
