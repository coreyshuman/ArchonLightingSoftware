using ArchonLightingSystem.Models;
using Microsoft.VisualBasic.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

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

    public class LevelTag
    {
        public Level Level { get; private set; }
        public string Name { get; private set; }
        public Color Color { get; private set; }

        public LevelTag(Level level)
        {
            Level = level;

            switch (level)
            {
                case Level.Error:
                    Color = Color.PaleVioletRed;
                    Name += "[ERROR]";
                    break;
                case Level.Warning:
                    Color = Color.Goldenrod;
                    Name += "[WARN ]";
                    break;
                case Level.Debug:
                    Color = Color.MediumOrchid;
                    Name += "[DEBUG]";
                    break;
                case Level.Trace:
                    Color = Color.LightSkyBlue;
                    Name += "[TRACE]";
                    break;
                default:
                case Level.Information:
                    Color = Color.White;
                    Name += "[INFO ]";
                    break;
            }
        }
    }

    internal class Log
    {
        public Level Level { get; private set; }
        public LevelTag Tag { get; private set; }
        public string Message { get; private set; }
        public DateTime Time { get; private set; }

        public Log(Level level, string message)
        {
            Level = level;
            Tag = new LevelTag(level);
            Message = message;
            Time = DateTime.Now;
        }
    }

    internal class LogEventArgs : EventArgs
    {
        public Log Log { get; set; }
    }

    internal static class Logger
    {
        public static event EventHandler<LogEventArgs> LatestLogEvent;

        private static List<Log> logs = new List<Log>();
        private static List<Log> logFileBuffer = new List<Log>();
        private static object listLock = new object();
        private static object logFileLock = new object();
        private static DateTime lastLogUpdate = DateTime.MinValue;
        private static Level minDisplayLevel = Level.Information;
        private static string LogFilePath { get; } = AppContext.BaseDirectory + "ArchonLightingSoftware.log";

        private static void WriteLogFile(IList<Log> logList)
        {
            try
            {
                using (TextWriter tw = File.AppendText(LogFilePath))
                {
                    foreach(Log log in logList)
                    {
                        tw.WriteLine($"{log.Time.ToString("s")} {log.Tag.Name} {log.Message}");
                    }
                    tw.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to write log file: {ex.Message}");
            }
        }

        public static void Write(Level level, string message)
        {
            Trace.WriteLine(message);
            var log = new Log(level, message);
            lock(listLock)
                logs.Add(log);

            lock(logFileLock)
                logFileBuffer.Add(log);

            if(level <= minDisplayLevel)
            {
                OnLogWritten(new LogEventArgs { Log = log });
            }

            if(DateTime.Now - lastLogUpdate > TimeSpan.FromMilliseconds(500))
            {
                lock(logFileBuffer)
                {
                    WriteLogFile(logFileBuffer);
                    logFileBuffer.Clear();
                }
                lastLogUpdate = DateTime.Now;
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
                return logs.Where(l => l.Level <= minDisplayLevel).ToList();
        }

        public static void SetMinDisplayLevel(Level level)
        {
            minDisplayLevel = level;
        }

        public static Level GetMinDisplayLevel()
        {
            return minDisplayLevel;
        }

        public static LevelTag GetLevelTag(Level level)
        {
            return new LevelTag(level);
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
