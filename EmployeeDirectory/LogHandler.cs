using System;
using System.Collections.Generic;
using System.Text;

namespace DobsonUtilities
{
    sealed class LogHandler
    {
        // Constructors
        public LogHandler()
        {

        }

        public LogHandler(string LogFile)
        {
            if (!string.IsNullOrEmpty(LogFile))
                logFile = LogFile;
        }

        public LogHandler(string LogFile, UInt64 MaxFileSize)
        {
            if (!string.IsNullOrEmpty(LogFile))
                logFile = LogFile;
            this.MaxFileSize = MaxFileSize;
        }

        // Getters and Setters
        public string logFile { get; private set; } = Environment.ExpandEnvironmentVariables("%Temp%").ToString() + "\\" + System.IO.Path.GetFileNameWithoutExtension(System.AppDomain.CurrentDomain.FriendlyName).ToString() + ".log";
        public UInt64 MaxFileSize { get; private set; } = 10000000000;

        // functions
        public int AppendMessage(string msg)
        {
            // Create a file object to check file size
            System.IO.FileInfo fileInfo = new System.IO.FileInfo(logFile);

            // delete file if it has gotten too large
            if (System.IO.File.Exists(logFile) && (UInt64)fileInfo.Length > MaxFileSize)
            {
                try
                {
                    System.IO.File.Delete(logFile);
                }
                catch
                {
                    return 1;
                }
            }

            // create file if it doesn't exist, or append to existing file
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(logFile, true))
            {
                try
                {
                    sw.WriteLine($"{ DateTime.Now.ToString("yyyy/MM/dd h:mm:ss tt") }: { msg }");
                }
                catch
                {
                    return 1;
                }
            }
            return 0;
        }
    }
}
