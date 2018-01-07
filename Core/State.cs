using Common;
using Common.Data;
using Common.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public static class State
    {
        public static Common.IO.Logger Logger
        {
            get { return _logger ?? (_logger = new Common.IO.Logger(Paths.CoreLogFileName)); }
        }

        private static Common.IO.Logger _logger;

        public static DraggableUIState UIState
            => _uiState ?? (_uiState = new DraggableUIState());

        private static DraggableUIState _uiState;
    }
}
