using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.IO
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
            IOHelper.EnsureDirectoryExists(Paths.LogsPath);

            var path = Paths.LogsPath + Path.DirectorySeparatorChar + _fileName;

            using (var writer = new StreamWriter(path, true))
            {
                var now = System.DateTime.Now;

                writer.WriteLine($"[{now.ToShortDateString()}, {now.TimeOfDay}] {message}\r\n");
                writer.Close();
            }
        }

        public void Log(Exception exception)
        {
            Log($"{exception?.Message}\n{exception?.StackTrace}");
        }

        public void LogProperties(object target)
        {
            var builder = new StringBuilder();

            foreach (var property in target.GetType().GetProperties())
            {
                builder.Append(property.Name);
                builder.Append(":");
                builder.Append(property.GetValue(target));
                builder.Append("\n");
            }

            Log(builder.ToString());
        }
    }
}
