using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;

namespace Kason.Net.Common.Utilities
{
    internal class WatcherTimer
    {
        private int TimeoutMillis = 1000;

        System.Threading.Timer m_timer = null;
        List<String> files = new List<string>();
        FileSystemEventHandler fswHandler = null;

        public WatcherTimer(FileSystemEventHandler watchHandler, int timerInterval)
        {
            m_timer = new System.Threading.Timer(new TimerCallback(OnTimer),
                        null, Timeout.Infinite, Timeout.Infinite);
            fswHandler = watchHandler;


            TimeoutMillis = timerInterval;
        }

        public WatcherTimer(FileSystemEventHandler watchHandler)
            : this(watchHandler, 500)
        {
        }


        public void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            Mutex mutex = new Mutex(false, "FSW");
            mutex.WaitOne();
            if (!files.Contains(e.Name))
            {
                files.Add(e.Name);
            }
            mutex.ReleaseMutex();

            m_timer.Change(TimeoutMillis, Timeout.Infinite);
        }

        private void OnTimer(object state)
        {
            List<String> backup = new List<string>();

            Mutex mutex = new Mutex(false, "FSW");
            mutex.WaitOne();
            backup.AddRange(files);
            files.Clear();
            mutex.ReleaseMutex();


            foreach (string file in backup)
            {
                fswHandler(this, new FileSystemEventArgs(
                       WatcherChangeTypes.Changed, string.Empty, file));
            }

        }
    }

    public class FCFileWatcher
    {
        private FileSystemWatcher fsw;

        public event FileSystemEventHandler Changed;
        //public event FileSystemEventHandler Created;
        //public event FileSystemEventHandler Deleted;
        //public event ErrorEventHandler Error;
        //public event RenamedEventHandler Renamed;

        public FCFileWatcher(string path)
        {
            fsw = new FileSystemWatcher(path);
        }
        public FCFileWatcher(string path, string filter)
        {
            fsw = new FileSystemWatcher(path, filter);
        }

        public void Start()
        {
            var watcher = new WatcherTimer(Changed);

            fsw.Changed += watcher.OnFileChanged;
            //fsw.Created += this.Created;
            //fsw.Deleted += this.Deleted;
            //fsw.Error += this.Error;
            //fsw.Renamed += this.Renamed;
            fsw.EnableRaisingEvents = true;
        }
    }
}
