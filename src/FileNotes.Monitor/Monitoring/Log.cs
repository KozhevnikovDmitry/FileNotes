using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using FileNotes.Monitor.Tool;

namespace FileNotes.Monitor.Monitoring
{
    /// <summary>
    /// Logging service
    /// </summary>
    public class Log
    {
        /// <summary>
        /// Logs the <paramref name="message"/> to storage with  <paramref name="db"/> connection.
        /// </summary>
        /// <param name="db">Connection</param>
        /// <param name="message">Log message</param>
        public void LogMessage(IDbConnection db, string message)
        {
            try
            {
                var cmd = PrepareLogCommand(db, message);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                LogToDisc(message, ex);
            }
        }

        /// <summary>
        /// Asyncronously logs the <paramref name="message"/> to storage with  <paramref name="db"/> connection.
        /// </summary>
        /// <param name="db">Connection</param>
        /// <param name="message">Log message</param>
        public async Task LogMessageAsync(IDbConnection db, string message)
        {
            try
            {
                var cmd = PrepareLogCommand(db, message);
                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                LogToDisc(message, ex);
            }
        }

        private SqlCommand PrepareLogCommand(IDbConnection db, string message)
        {
            Debug.WriteLine(message);
            var cmd = db.CreateCommand() as SqlCommand;
            cmd.CommandText = @"INSERT INTO [dbo].[Log]
                                               ([Date], [Msg])
                                        VALUES (@Date, @Msg)";
            cmd.Parameters.Add(new SqlParameter("Date", DateTime.UtcNow));
            cmd.Parameters.Add(new SqlParameter("Msg", message.SafetyTake(1000)));

            return cmd;
        }

        private async void LogToDisc(string originalMsg, Exception loggingEx)
        {
            try
            {
                Debug.WriteLine(originalMsg);
                Debug.WriteLine(loggingEx);

                var fileMode = FileMode.Append;
                if (!File.Exists("failure_log.txt"))
                {
                    fileMode = FileMode.OpenOrCreate;
                }

                using (var writer = new StreamWriter(new FileStream("failure_log.txt", fileMode, FileAccess.Write, FileShare.Write), Encoding.UTF8))
                {
                    var now = DateTime.UtcNow;
                    await writer.WriteLineAsync(string.Format("{0} {1}: {2}", now.ToLongDateString(), now.ToLongTimeString(), originalMsg));
                    await writer.WriteLineAsync(string.Format("{0} {1}: {2}", now.ToLongDateString(), now.ToLongTimeString(), loggingEx));
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