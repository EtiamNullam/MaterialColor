using System;
using System.Collections.Generic;

namespace Core
{
    public static class UpdateQueueManager
    {
        public static void EnqueueAction(EventHandler action)
        {
            _actionQueue.Enqueue(action);
        }

        private static readonly Queue<EventHandler> _actionQueue = new Queue<EventHandler>();

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
