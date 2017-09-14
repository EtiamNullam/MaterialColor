using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaterialColor.Common.IO
{
    public class Logger
    {
        public Logger(string fileName)
        {
            _fileName = fileName;
        }

        private string _fileName;

        public void Log(string message)
        {
            IOHelper.EnsureDirectoryExists(Paths.LogsDirectory);

            var path = Paths.LogsDirectory + Path.DirectorySeparatorChar + _fileName;

            using (var writer = new StreamWriter(path, true))
            {
                var now = System.DateTime.Now;

                writer.WriteLine($"[{now.ToShortDateString()}, {now.ToShortTimeString()}] {message}");
                writer.Close();
            }
        }

        public void Log(Exception exception)
        {
            Log($"{exception?.Message}\n{exception?.StackTrace}");
        }
    }
}
