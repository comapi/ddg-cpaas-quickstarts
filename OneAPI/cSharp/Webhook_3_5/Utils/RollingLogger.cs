using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Webhook_3_5.Utils
{
    public class RollingLogger
    {
        readonly static string LOG_FILE = Path.GetTempPath() + "comapi_webhook.log";
        readonly static int MaxRolledLogCount = 3;
        readonly static int MaxLogSize = (1024 * 1024 * 5); // Log size in bytes (5MB)

        static RollingLogger()
        {
            // Output log file path
            Debug.WriteLine("Comapi webhook logging path: " + LOG_FILE);
        }

        public static void LogMessage(string msg)
        {
            lock (LOG_FILE)
            {
                RollLogFile(LOG_FILE);
                File.AppendAllText(LOG_FILE, msg + Environment.NewLine, Encoding.UTF8);
                Debug.WriteLine(msg);
            }
        }

        private static void RollLogFile(string logFilePath)
        {
            try
            {
                var length = new FileInfo(logFilePath).Length;

                if (length > MaxLogSize)
                {
                    var path = Path.GetDirectoryName(logFilePath);
                    var wildLogName = Path.GetFileNameWithoutExtension(logFilePath) + "*" + Path.GetExtension(logFilePath);
                    var bareLogFilePath = Path.Combine(path, Path.GetFileNameWithoutExtension(logFilePath));

                    string[] logFileList = Directory.GetFiles(path, wildLogName, SearchOption.TopDirectoryOnly);

                    if (logFileList.Length > 0)
                    {
                        // Only take files like logfilename.log and logfilename.0.log, so there also can be a maximum of 10 additional rolled files (0..9)
                        var rolledLogFileList = logFileList.Where(fileName => fileName.Length == (logFilePath.Length + 2)).ToArray();
                        Array.Sort(rolledLogFileList, 0, rolledLogFileList.Length);

                        if (rolledLogFileList.Length >= MaxRolledLogCount)
                        {
                            File.Delete(rolledLogFileList[MaxRolledLogCount - 1]);
                            var list = rolledLogFileList.ToList();
                            list.RemoveAt(MaxRolledLogCount - 1);
                            rolledLogFileList = list.ToArray();
                        }

                        // Move remaining rolled files
                        for (int i = rolledLogFileList.Length; i > 0; --i)
                        {
                            File.Move(rolledLogFileList[i - 1], bareLogFilePath + "." + i + Path.GetExtension(logFilePath));
                        }

                        var targetPath = bareLogFilePath + ".0" + Path.GetExtension(logFilePath);

                        // Move original file
                        File.Move(logFilePath, targetPath);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }
    }
}
