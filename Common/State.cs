using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public static class State
    {
        public static List<ExitCode> ExitCodes = new List<ExitCode>
        {
            new ExitCode
            {
                Code = 0,
                Message = "Success"
            },
            new ExitCode
            {
                Code = 1,
                Message = "Invalid arguments"
            },
            new ExitCode
            {
                Code = 2,
                Message = "View init error"
            },
        };

        internal static IO.Logger Logger { get; set; } = new IO.Logger(Paths.CommonLogFileName);
    }
}
