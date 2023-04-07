﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchonLightingSystem.Common
{
    public enum Level
    {
        Error,
        Warning,
        Information,
        Debug,
        Trace
    }

    internal class Log
    {
        public Level Level { get; set; }
        public string Message { get; set; }
    }

    internal class LogEventArgs : EventArgs
    {
        public Log Log { get; set; }
    }

    internal static class Logger
    {
        public static event EventHandler<LogEventArgs> LatestLogEvent;

        private static List<Log> logs = new List<Log>();
        private static object listLock = new object();
        private static Level level = Level.Information;

        public static void Write(Level lev, string message)
        {
            Trace.WriteLine(message);
            var log = new Log { Level = lev, Message = message };
            lock(listLock)
                logs.Add(log);

            if(lev <= level)
            {
                OnLogWritten(new LogEventArgs { Log = log });
            }

            
        }

        public static void Clear()
        {
            lock (listLock)
                logs.Clear();
        }

        public static List<Log> GetLogs()
        {
            lock (listLock)
                return logs.Where(l => l.Level <= Logger.level).ToList();
        }

        public static void SetLevel(Level lev)
        {
            level = lev;
        }

        public static Level GetLevel()
        {
            return level;
        }

        private static void OnLogWritten(LogEventArgs e)
        {
            EventHandler<LogEventArgs> handler = LatestLogEvent;
            if (handler != null)
            {
                handler(LatestLogEvent, e);
            }
        }
    }
}
