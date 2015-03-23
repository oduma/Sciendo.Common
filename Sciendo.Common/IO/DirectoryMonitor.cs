using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace Sciendo.Common.IO
{
    public delegate void FileSystemEvent(String path);
    
    public delegate void FileRenamedEvent(string fromPath, string toPath);

  public class DirectoryMonitor
  {
        class PendingEvent
        {
          public DateTime TimeStamp {get;set;}
          public WatcherChangeTypes ChangeType {get;set;}
          public RenamedEventArgs RenamedEventArgs { get; set; }
        }

        private readonly FileSystemWatcher _fileSystemWatcher = 
        new FileSystemWatcher();
    
        private readonly Dictionary<string, PendingEvent> _pendingEvents = 
        new Dictionary<string, PendingEvent>();

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

        private string _parentDirectory=string.Empty;

        private void OnChange(object sender, FileSystemEventArgs e)
        {
            if(Directory.Exists(e.FullPath))
            {
                if ((string.IsNullOrEmpty(_parentDirectory) || _parentDirectory != e.FullPath))
                {
                    _parentDirectory = GetParentDirectory(e.FullPath);
                    foreach (var filePath in Directory.GetFiles(e.FullPath, "*.*", SearchOption.AllDirectories))
                    {
                        QueueEventForFile(filePath, e.ChangeType);
                    }
                }
                else
                {
                    _parentDirectory=string.Empty;
                }
            }
            else
            {
                QueueEventForFile(e.FullPath, e.ChangeType);
            }
        }

        private void QueueEventForFile(string filePath, WatcherChangeTypes watcherChangeType)
        {
            // Don't want other threads messing with the pending events right now
            lock (_pendingEvents)
            {

                // Save a timestamp for the most recent event for this path
                _pendingEvents[filePath] = new PendingEvent { TimeStamp = DateTime.Now, ChangeType = watcherChangeType };

                // Start a timer if not already started
                if (!_timerStarted)
                {
                    _timer.Change(100, 100);
                    _timerStarted = true;
                }
            }
        }

        private void OnRename(object sender, RenamedEventArgs e)
        {
            // Don't want other threads messing with the pending events right now
            lock (_pendingEvents)
            {
                _parentDirectory = GetParentDirectory(e.OldFullPath);

                // Save a timestamp for the most recent event for this path
                _pendingEvents[e.FullPath] = new PendingEvent { TimeStamp = DateTime.Now, ChangeType = e.ChangeType, RenamedEventArgs = e };

                // Start a timer if not already started
                if (!_timerStarted)
                {
                    _timer.Change(100, 100);
                    _timerStarted = true;
                }
            }
        }

        private string GetParentDirectory(string currentDirectory)
        {
            var parentDirectory = string.Empty;
            var pathParts = currentDirectory.Split(new[] { Path.DirectorySeparatorChar });
            for (int i = 0; i < pathParts.Length - 1; i++)
            {
                parentDirectory += (@"\" + pathParts[i]);
            }
            return parentDirectory.Substring(1, parentDirectory.Length - 1);
        }

        private void OnTimeout(object state)
        {
            Dictionary<string, PendingEvent> uniquePendingEvents;

            // Don't want other threads messing with the pending events right now
            lock (_pendingEvents)
            {
                // Get a list of all paths that should have events thrown
                uniquePendingEvents = FindReadyPaths(_pendingEvents);

                // Remove paths that are going to be used now
                uniquePendingEvents.Keys.ToList().ForEach(delegate(string path)
                {
                    _pendingEvents.Remove(path);
                });

                // Stop the timer if there are no more events pending
                if (_pendingEvents.Count == 0)
                {
                    _timer.Change(Timeout.Infinite, Timeout.Infinite);
                    _timerStarted = false;
                }
            }

            // Fire an event for each path that has changed
            uniquePendingEvents.Keys.ToList().ForEach(delegate(string key)
            {

                FireEvent(key,uniquePendingEvents[key]);
            });
        }

        private Dictionary<string, PendingEvent> FindReadyPaths(Dictionary<string, PendingEvent> events)
        {
            Dictionary<string, PendingEvent> results = new Dictionary<string, PendingEvent>();
            DateTime now = DateTime.Now;

            foreach (KeyValuePair<string, PendingEvent> entry in events)
            {
                // If the path has not received a new event in the last 75ms
                // an event for the path should be fired
                double diff = now.Subtract(entry.Value.TimeStamp).TotalMilliseconds;
                if (diff >= 75)
                {
                    results.Add(entry.Key,entry.Value);
                }
            }

            return results;
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
                        foreach(var newFilePath in Directory.GetFiles(evt.RenamedEventArgs.FullPath,"*.*",SearchOption.AllDirectories))
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

