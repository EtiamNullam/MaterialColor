﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaterialColor
{
    public class FileChangeNotifier : IDisposable
    {
        public FileChangeNotifier()
        {
            var directory = Common.DefaultPaths.Directory;

            _elementColorInfosWatcher = new FileSystemWatcher(directory);
            _typeColorOffsetsWatcher = new FileSystemWatcher(directory);
            _injectorStateWatcher = new FileSystemWatcher(directory);

            _elementColorInfosWatcher.Filter = Common.DefaultPaths.ElementColorInfosFilePath;
            _elementColorInfosWatcher.Filter = Common.DefaultPaths.TypeColorsFilePath;
            _injectorStateWatcher.Filter = Common.DefaultPaths.InjectorStateFilePath;

            _elementColorInfosWatcher.NotifyFilter =
                _typeColorOffsetsWatcher.NotifyFilter =
                _injectorStateWatcher.NotifyFilter =
                NotifyFilters.LastWrite | NotifyFilters.Size | NotifyFilters.Attributes;

            _elementColorInfosWatcher.EnableRaisingEvents =
                _typeColorOffsetsWatcher.EnableRaisingEvents =
                _injectorStateWatcher.EnableRaisingEvents =
                true;
        }

        public void Dispose()
        {
            _elementColorInfosWatcher?.Dispose();
            _typeColorOffsetsWatcher?.Dispose();
        }

        private FileSystemWatcher _elementColorInfosWatcher;
        private FileSystemWatcher _typeColorOffsetsWatcher;
        private FileSystemWatcher _injectorStateWatcher;

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

        public event FileSystemEventHandler TypeColorsChanged
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
    }
}
