using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace TemplateAction.Cache
{
    public class FileDependencyWatcher
    {
        private static HashSet<string> _changePaths = new HashSet<string>();
        private Timer _timer = null;
        private FileSystemWatcher _watcher;
        private ConcurrentDictionary<string, FileDependency> _dependencys = new ConcurrentDictionary<string, FileDependency>();
        public FileDependencyWatcher(string path, string filter = "*.*")
        {
            _watcher = new FileSystemWatcher();
            _watcher.Filter = filter;
            _watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName;
            _watcher.Path = path;
            _watcher.EnableRaisingEvents = true;
            _watcher.IncludeSubdirectories = true;
            _watcher.Changed += OnProcess;
            _watcher.Deleted += OnProcess;
            _timer = new Timer(new TimerCallback(OnWatchedFileChange),
                 null, Timeout.Infinite, Timeout.Infinite);
        }
        private void OnProcess(object sender, FileSystemEventArgs e)
        {
            string changefile = e.FullPath.ToLower();
            lock (_changePaths)
            {
                if (!_changePaths.Contains(changefile))
                {
                    _changePaths.Add(changefile);
                }
                _timer.Change(500, Timeout.Infinite);
            }
        }
        private void OnWatchedFileChange(object state)
        {
            List<string> backup = new List<string>();
            lock (_changePaths)
            {
                backup.AddRange(_changePaths);
                _changePaths.Clear();
            }
            foreach (string changefile in backup)
            {
                FileDependency filedep;
                if (_dependencys.TryRemove(changefile, out filedep))
                {
                    filedep.NoticeChange();
                }
            }
        }
        public FileDependency CreateFileDependency(string path)
        {
            FileDependency filedep = new FileDependency();
            if (_dependencys.TryAdd(path, filedep))
            {
                return filedep;
            }
            return null;
        }

    }
}
