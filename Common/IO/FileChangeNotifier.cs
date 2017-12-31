using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Common.IO
{
    public static class FileChangeNotifier
    {
        private static readonly List<FileWatcherInfo> _fileWatcherInfos = new List<FileWatcherInfo>();

        public static void StartFileWatch(string filter, string parentDirectory, FileSystemEventHandler callback)
        {
            FileWatcherInfo watcherInfo = null;

            foreach (var info in _fileWatcherInfos)
            {
                var watcher = info.FileWatcher;

                if (watcher.Filter != filter || watcher.Path != parentDirectory) continue;
                watcherInfo = info; // TODO REFACTOR (weakman) This assigned value is never used....
                return;             // ...because of this return,
            }

            if (watcherInfo == null) // REFACTOR (weakman)  , Which means this is currently always true...
            {
                IOHelper.EnsureDirectoryExists(parentDirectory);

                watcherInfo = new FileWatcherInfo(new FileSystemWatcher(parentDirectory, filter), callback);

                InitializeWatcher(watcherInfo.FileWatcher);

                _fileWatcherInfos.Add(watcherInfo);
            }
            else // REFACTOR (weakman) ...so this is never executed
            {
                foreach (var sub in watcherInfo.Subscribers)
                {
                    if (sub == callback)
                    {
                        // already subscribed to this watcher
                        return;
                    }
                }

                watcherInfo.TryAddSubscriber(callback);
            }
        }

        public static void StopFileWatch(string filter, string parentDirectory, FileSystemEventHandler callback)
        {
            var watcherInfo = _fileWatcherInfos.FirstOrDefault(
                info => info.FileWatcher.Filter == filter &&
                        info.FileWatcher.Path == parentDirectory);

            if (watcherInfo == null) return;

            watcherInfo.TryRemoveSubscriber(callback);

            if (!watcherInfo.HasSubscribers())
            {
                _fileWatcherInfos.Remove(watcherInfo);
            }
        }

        private class FileWatcherInfo
        {
            public FileWatcherInfo(FileSystemWatcher watcher, FileSystemEventHandler callback)
            {
                FileWatcher = watcher;

                TryAddSubscriber(callback);
            }

            public readonly FileSystemWatcher FileWatcher;
            public readonly List<FileSystemEventHandler> Subscribers = new List<FileSystemEventHandler>();

            public bool TryAddSubscriber(FileSystemEventHandler callback)
            {
                try
                {
                    FileWatcher.Changed += callback;
                    Subscribers.Add(callback);

                    return true;
                }
                catch
                {
                    return false;
                }
            }

            public bool TryRemoveSubscriber(FileSystemEventHandler callback)
            {
                bool result = true;

                try
                {
                    FileWatcher.Changed -= callback;
                }
                catch
                {
                    result = false;
                }

                try
                {
                    Subscribers.Remove(callback);
                }
                catch
                {
                    result = false;
                }

                return result;
            }

            public bool HasSubscribers()
                => Subscribers.Count > 0;
        }

        private static void InitializeWatcher(FileSystemWatcher watcher)
        {
            watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size | NotifyFilters.Attributes | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            watcher.EnableRaisingEvents = true;
        }
    }
}
