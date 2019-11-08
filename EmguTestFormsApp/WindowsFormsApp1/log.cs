using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Windows.Forms;
namespace WindowsFormsApp1
{
    public  class LogManager
    {
        
        private static string logPath = string.Empty;
        /// <summary>
        /// 保存日志的文件夹
        /// </summary>
       
        public static string LogPath
        {
            get
            {
                if (logPath == string.Empty)
                {
                    
                        logPath = AppDomain.CurrentDomain.BaseDirectory;
                    
                }
                return logPath;
            }
            set { logPath = value; }
        }

        private static string logFielPrefix = string.Empty;
        /// <summary>
        /// 日志文件前缀
        /// </summary>
        public static string LogFielPrefix
        {
            get { return logFielPrefix; }
            set { logFielPrefix = value; }
        }

        /// <summary>
        /// 写日志
        /// </summary>
        public static void WriteLog(string logFile, string msg)
        {
            try
            {
                string logfilename = LogPath + LogFielPrefix + logFile + " " + DateTime.Now.ToString("yyyyMMdd") + ".Log";
                using (System.IO.StreamWriter sw = System.IO.File.AppendText(logfilename))
                {
                    sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss: ") + msg);                    
                }                                   
            }
            catch
            {
                MessageBox.Show("creat the logfile error!!!");
            }
        }

        /// <summary>
        /// 写日志
        /// </summary>
        public static void WriteLog(LogFile logFile, string msg)
        {
            WriteLog(logFile.ToString(), msg);
        }
    }

    /// <summary>
    /// 日志类型
    /// </summary>
    public enum LogFile
    {
        Trace,
        Warning,
        Error,
        SQL
    }
}
