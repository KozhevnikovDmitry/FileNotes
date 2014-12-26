using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using Dapper;
using FileNotes.Monitor.Tool;

namespace FileNotes.Monitor.Monitoring
{
    /// <summary>
    /// Logging service
    /// </summary>
    public class Log
    {
        /// <summary>
        /// Logs the <paramref name="message"/> to storage with  <paramref name="db"/> connection
        /// </summary>
        /// <param name="db">Connection</param>
        /// <param name="message">Log message</param>
        public void LogMessage(IDbConnection db, string message)
        {
            try
            {
                Debug.WriteLine(message);

                db.Execute(@"INSERT INTO [dbo].[Log]
                                               ([Date], [Msg])
                                        VALUES (@Date, @Msg)",
                    new
                    {
                        @Date = DateTime.UtcNow,
                        @Msg = message.SafetyTake(1000)
                    });
            }
            catch (Exception ex)
            {
                LogToDisc(message, ex);
            }
        }

        private void LogToDisc(string originalMsg, Exception loggingEx)
        {
            try
            {
                Debug.WriteLine(originalMsg);
                Debug.WriteLine(loggingEx);

                using (var writer = new StreamWriter(new FileStream("failure_log.txt", FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write)))
                {
                    var now = DateTime.UtcNow;
                    writer.WriteLine("{0} {1}: {2}", now.ToLongDateString(), now.ToLongTimeString(), originalMsg);
                    writer.WriteLine("{0} {1}: {2}", now.ToLongDateString(), now.ToLongTimeString(), loggingEx);
                }
            }
            catch (Exception ex)
            {
                // sorry, folks
                Debug.Fail(ex.ToString());
            }
        }
    }
}