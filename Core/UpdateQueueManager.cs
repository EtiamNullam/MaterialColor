using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public static class UpdateQueueManager
    {
        public static void EnqueueAction(EventHandler action)
        {
            _actionQueue.Enqueue(action);
        }

        private static Queue<EventHandler> _actionQueue = new Queue<EventHandler>();

        public static void OnGameUpdate()
        {
            while (_actionQueue.Count > 0)
            {
                var action = _actionQueue.Dequeue();

                try
                {
                    action.Invoke(null, EventArgs.Empty);
                }
                catch
                {
                    // log
                }
            }
        }
    }
}
