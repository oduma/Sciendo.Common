using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;

namespace Sciendo.Common.IO
{
    public delegate void FileSystemEvent(String path);

    public delegate void FileRenamedEvent(string fromPath, string toPath);

    public class DirectoryMonitor
    {
        class PendingEvent
        {
            public WatcherChangeTypes ChangeType { get; set; }
            public RenamedEventArgs RenamedEventArgs { get; set; }
        }

        private readonly FileSystemWatcher _fileSystemWatcher =
        new FileSystemWatcher();

        private readonly ConcurrentDictionary<string, PendingEvent> _pendingEvents =
        new ConcurrentDictionary<string, PendingEvent>();

        private readonly Timer _timer;
        private bool _timerStarted;

        public DirectoryMonitor(string dirPath)
        {
            _fileSystemWatcher.Path = dirPath;
            _fileSystemWatcher.IncludeSubdirectories = true;
            _fileSystemWatcher.Created += OnChange;
            _fileSystemWatcher.Changed += OnChange;
            _fileSystemWatcher.Deleted += OnChange;
            _fileSystemWatcher.Renamed += OnRename;

            _timer = new Timer(OnTimeout, null, Timeout.Infinite, Timeout.Infinite);
        }

        public event FileSystemEvent Change;

        public event FileSystemEvent Delete;

        public event FileRenamedEvent Rename;

        public void Start()
        {
            _fileSystemWatcher.EnableRaisingEvents = true;
        }

        public void Stop()
        {
            _fileSystemWatcher.EnableRaisingEvents = false;
        }

        private void OnChange(object sender, FileSystemEventArgs e)
        {
            QueueChangeEvent(e.FullPath, e.ChangeType);
        }

        private void QueueChangeEvent(string filePath, WatcherChangeTypes watcherChangeType)
        {
            if (watcherChangeType != WatcherChangeTypes.Created
                && watcherChangeType != WatcherChangeTypes.All && !Directory.Exists(filePath))
            {
                _pendingEvents.AddOrUpdate(filePath,
                    new PendingEvent { ChangeType = watcherChangeType }, (key, existingEvent) => existingEvent);
                if (!_timerStarted)
                {
                    _timer.Change(100, 100);
                    _timerStarted = true;
                }
            }
        }

        private void OnRename(object sender, RenamedEventArgs e)
        {
            var pendingEvent = new PendingEvent { ChangeType = e.ChangeType, RenamedEventArgs = e };
            _pendingEvents.AddOrUpdate(e.FullPath, pendingEvent, (key, existingEvent) =>
            {
                existingEvent.ChangeType = pendingEvent.ChangeType;
                existingEvent.RenamedEventArgs = pendingEvent.RenamedEventArgs;
                return existingEvent;
            });
            if (!_timerStarted)
            {
                _timer.Change(100, 100);
                _timerStarted = true;
            }
        }

        private void OnTimeout(object state)
        {
            if (_pendingEvents.Count == 0)
            {
                _timer.Change(Timeout.Infinite, Timeout.Infinite);
                _timerStarted = false;
            }

            foreach (string key in _pendingEvents.Keys)
            {
                PendingEvent pendingEvent;
                if (_pendingEvents.TryRemove(key, out pendingEvent))
                {
                    FireEvent(key, pendingEvent);
                }
            }
        }

        private void FireEvent(string path, PendingEvent evt)
        {
            switch (evt.ChangeType)
            {
                case WatcherChangeTypes.Changed:
                case WatcherChangeTypes.Created:
                case WatcherChangeTypes.All:
                    if (Change != null)
                        Change(path);
                    break;
                case WatcherChangeTypes.Deleted:
                    if (Delete != null)
                        Delete(path);
                    break;
                case WatcherChangeTypes.Renamed:
                    if (!Directory.Exists(evt.RenamedEventArgs.FullPath))
                    {
                        if (Rename != null)
                            Rename(evt.RenamedEventArgs.OldFullPath, evt.RenamedEventArgs.FullPath);
                    }
                    else
                    {
                        foreach (var newFilePath in Directory.GetFiles(evt.RenamedEventArgs.FullPath, "*.*", SearchOption.AllDirectories))
                        {
                            if (Rename != null)
                                Rename(newFilePath.Replace(evt.RenamedEventArgs.FullPath, evt.RenamedEventArgs.OldFullPath), newFilePath);
                        }
                    }
                    break;
            }
        }
    }
}

