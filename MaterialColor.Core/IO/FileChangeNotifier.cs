using System;
using System.IO;

namespace MaterialColor.Core.IO
{
    public class FileChangeNotifier : IDisposable
    {
        public FileChangeNotifier()
        {
            var directory = Common.Paths.Directory;

            _elementColorInfosWatcher = new FileSystemWatcher(directory);
            _typeColorOffsetsWatcher = new FileSystemWatcher(directory);
            _injectorStateWatcher = new FileSystemWatcher(directory);
            _configuratorStateWatcher = new FileSystemWatcher(directory);

            _elementColorInfosWatcher.Filter = Common.Paths.ElementColorInfosFilePath;
            _typeColorOffsetsWatcher.Filter = Common.Paths.TypeColorsFilePath;
            _injectorStateWatcher.Filter = Common.Paths.InjectorStateFilePath;
            _configuratorStateWatcher.Filter = Common.Paths.ConfiguratorStateFilePath;

            _elementColorInfosWatcher.NotifyFilter =
                _typeColorOffsetsWatcher.NotifyFilter =
                _injectorStateWatcher.NotifyFilter =
                _configuratorStateWatcher.NotifyFilter =
                NotifyFilters.LastWrite | NotifyFilters.Size | NotifyFilters.Attributes;

            _elementColorInfosWatcher.EnableRaisingEvents =
                _typeColorOffsetsWatcher.EnableRaisingEvents =
                _injectorStateWatcher.EnableRaisingEvents =
                _configuratorStateWatcher.EnableRaisingEvents =
                true;
        }

        public void Dispose()
        {
            _elementColorInfosWatcher?.Dispose();
            _typeColorOffsetsWatcher?.Dispose();
            _injectorStateWatcher?.Dispose();
            _configuratorStateWatcher?.Dispose();
        }

        private FileSystemWatcher _elementColorInfosWatcher;
        private FileSystemWatcher _typeColorOffsetsWatcher;
        private FileSystemWatcher _injectorStateWatcher;
        private FileSystemWatcher _configuratorStateWatcher;

        public event FileSystemEventHandler ElementColorInfosChanged
        {
            add
            {
                _elementColorInfosWatcher.Changed += value;
            }
            remove
            {
                _elementColorInfosWatcher.Changed -= value;
            }
        }

        public event FileSystemEventHandler TypeColorOffsetsChanged
        {
            add
            {
                _typeColorOffsetsWatcher.Changed += value;
            }
            remove
            {
                _typeColorOffsetsWatcher.Changed -= value;
            }
        }

        public event FileSystemEventHandler InjectorStateChanged
        {
            add
            {
                _injectorStateWatcher.Changed += value;
            }
            remove
            {
                _injectorStateWatcher.Changed -= value;
            }
        }

        public event FileSystemEventHandler ConfiguratorStateChanged
        {
            add
            {
                _configuratorStateWatcher.Changed += value;
            }
            remove
            {
                _configuratorStateWatcher.Changed -= value;
            }
        }
    }
}
