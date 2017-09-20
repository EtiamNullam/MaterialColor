using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Json
{
    public abstract class BaseManager
    {
        public BaseManager(JsonManager manager, IO.Logger logger = null)
        {
            _logger = logger;
            _manager = manager;
        }

        protected IO.Logger _logger;
        protected JsonManager _manager;
    }
}
