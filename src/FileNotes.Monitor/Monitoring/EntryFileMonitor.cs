using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using FileNotes.Domain;
using FileNotes.Monitor.Tool;

namespace FileNotes.Monitor.Monitoring
{
    /// <summary>
    /// Service, that watchs the directory and moves files data to storage.
    /// </summary>
    public class EntryFileMonitor
    {
        /// <summary>
        /// Connection string to notes storage
        /// </summary>
        private readonly string _connectionString;

        /// <summary>
        /// Directory to watch
        /// </summary>
        private readonly string _sourceDirectory;

        /// <summary>
        /// Directory for processed files
        /// </summary>
        private readonly string _destinationDirectory;

        /// <summary>
        /// Logger of steps
        /// </summary>
        private readonly Log _logger;

        /// <summary>
        /// 
        /// </summary>
        private readonly CancellationTokenSource _tokenSource;
        private bool _isWatching;

        /// <summary>
        /// Service, that watchs the directory and moves files data to storage.
        /// </summary>
        /// <param name="connectionString">Connection string to notes storage</param>
        /// <param name="sourceDirectory">Directory to watch</param>
        /// <param name="destinationDirectory">Directory for processed files</param>
        /// <param name="logger">Logger of steps</param>
        public EntryFileMonitor(string connectionString, string sourceDirectory, string destinationDirectory, Log logger)
        {
            if (string.IsNullOrWhiteSpace(connectionString)) throw new ArgumentNullException("connectionString");
            if (string.IsNullOrWhiteSpace(sourceDirectory)) throw new ArgumentNullException("sourceDirectory");
            if (string.IsNullOrWhiteSpace(destinationDirectory)) throw new ArgumentNullException("destinationDirectory");
            if (logger == null) throw new ArgumentNullException("logger");
            _connectionString = connectionString;
            _sourceDirectory = sourceDirectory;
            _destinationDirectory = destinationDirectory;
            _logger = logger;
            _isWatching = false;
            _tokenSource = new CancellationTokenSource();
        }

        /// <summary>
        /// Begins watch the source directory
        /// </summary>
        public void StartWatch()
        {
            // if already started
            if (_isWatching)
            {
                return;
            }

            _isWatching = true;

            // run is separate task
            Task.Run(() =>
            {
                while (true)
                {
                    // session of watching
                    Watch();

                    // while cancel was not requested
                    if (_tokenSource.Token.IsCancellationRequested)
                    {
                        break;
                    }
                }
            }, _tokenSource.Token);
        }

        /// <summary>
        /// Performs moving and parsing files from source directory.
        /// </summary>
        private void Watch()
        {
            // take 1000 files as enumerable
            var files = Directory.EnumerateFiles(_sourceDirectory);

            // process files as parallel
            Parallel.ForEach(files, async file =>
            {
                // separated connection for file
                using (var db = new SqlConnection(_connectionString))
                {
                    db.Open();

                    // only if cancellation was not requested
                    if (_tokenSource.Token.IsCancellationRequested)
                    {
                        return;
                    }

                    // move file
                    var movedFile = await Move(db, file);

                    // no such file or it is locked. Try it later
                    if (string.IsNullOrEmpty(movedFile))
                    {
                        return;
                    }

                    // read data from moved file
                    var data = await Read(db, movedFile);

                    // parse data to Note instance
                    var note = await Parse(db, data, file);

                    // save note to storage
                    await Save(db, note, file, data != null ? data.Length : 0);
                }
            });
        }

        /// <summary>
        /// Moves file to directory for processed files. Returns fullpath to moved file.
        /// </summary>
        /// <param name="db">Connection</param>
        /// <param name="file">Path to file in source directory</param>
        /// <returns>Fullpath to moved file. Returns <c>null</c> when fails to move</returns>
        private async Task<string> Move(IDbConnection db, string file)
        {
            try
            {
                // combine path to move
                var destFile = Path.Combine(_destinationDirectory, string.Format("{0}_{1}", Guid.NewGuid(), Path.GetFileName(file)));

                await _logger.LogMessageAsync(db, string.Format("Move file [{0}] to [{1}]", file, destFile));
                // move!
                File.Move(file, destFile);

                return destFile;
            }
            catch (Exception ex)
            {
                // no such file or it is locked
                _logger.LogMessage(db, string.Format("Fail to move file [{0}]: \r\n {1}", file, ex));
                return null;
            }
        }

        /// <summary>
        /// Reads and returns file data. 
        /// </summary>
        /// <param name="db">Connection</param>
        /// <param name="file">File to read</param>
        /// <returns>All bytes of file. Returns <c>null</c> when fails to read.</returns>
        private async Task<byte[]> Read(IDbConnection db, string file)
        {
            try
            {
                await _logger.LogMessageAsync(db, string.Format("Read file [{0}]", file));
                using (var fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true))
                {
                    var buff = new byte[file.Length];
                    await fs.ReadAsync(buff, 0, file.Length);
                    return buff;
                }
            }
            catch (Exception ex)
            {
                _logger.LogMessage(db, string.Format("Fail to read file [{0}]: \r\n {1}", file, ex));
                return null;
            }
        }

        /// <summary>
        /// Parses file data to <see cref="Note"/> instance
        /// </summary>
        /// <param name="db">Connection</param>
        /// <param name="data">File bytes</param>
        /// <param name="srcFile">Name of file</param>
        /// <returns>Returns <c>Note</c> instance. Returns <c>NotParsedNote</c> instance when fails to parse. Returns <c>LostNote</c> when <paramref name="data"/> is null.</returns>
        private async Task<Note> Parse(IDbConnection db, byte[] data, string srcFile)
        {
            await _logger.LogMessageAsync(db, string.Format("Parse data of file [{0}]", srcFile));

            // if data is lost
            if (data == null)
            {
                await _logger.LogMessageAsync(db, string.Format("File [{0}] was lost", srcFile));
                return new LostNote
                {
                    Id = 0,
                    Date = DateTime.UtcNow,
                    Content = "File was lost"
                };
            }

            try
            {
                // parse
                var xml = new XmlDocument();
                xml.Load(new StreamReader(new MemoryStream(data), Encoding.UTF8));
                return new Note
                {
                    NoteId = Convert.ToInt32(xml.ChildNodes[1]["id"].InnerText),
                    DateString = xml.ChildNodes[1]["date"].InnerText,
                    Content = xml.ChildNodes[1]["content"].InnerText
                };
            }
            catch (Exception ex)
            {
                _logger.LogMessage(db, string.Format("File [{0}] was parsed with errors: \r\n {1}", srcFile, ex));

                // fail to parse
                return new NotParsedNote
                {
                    Id = 0,
                    Date = DateTime.UtcNow,
                    Content = "Xml format error"
                };
            }
        }

        /// <summary>
        /// Saves <see cref="Note"/> instance to storage. Generate bounded <see cref="EntryFile"/> instance.
        /// </summary>
        /// <param name="db">Connection</param>
        /// <param name="note">Note to save</param>
        /// <param name="srcFile">Path to file with note entry</param>
        /// <param name="size">Size of file</param>
        private async Task Save(IDbConnection db, Note note, string srcFile, int size)
        {
            try
            {
                await _logger.LogMessageAsync(db, string.Format("Save note id=[{0}] from file [{1}]", note, srcFile));

                var cmd = db.CreateCommand() as SqlCommand;
                cmd.CommandText = @"INSERT INTO [dbo].[Note]
                                               ([NoteId], 
                                                [Date], 
                                                [Content])
                                        VALUES (@NoteId, 
                                                @Date, 
                                                @Content);
                             INSERT INTO [dbo].[EntryFile]
                                              ([SourceFileName], 
                                               [Size],
                                               [EntryFileState],
                                               [NoteId])
                                     VALUES (@SourceFileName,
                                             @Size,
                                             @EntryFileState,
                                             CAST(SCOPE_IDENTITY() as int));";

                cmd.Parameters.Add(new SqlParameter("NoteId", note.NoteId));
                cmd.Parameters.Add(new SqlParameter("Date", note.Date));
                cmd.Parameters.Add(new SqlParameter("Content", note.Content.SafetyTake(1024)));
                cmd.Parameters.Add(new SqlParameter("SourceFileName", Path.GetFileName(srcFile)));
                cmd.Parameters.Add(new SqlParameter("Size", size));
                cmd.Parameters.Add(new SqlParameter("EntryFileState", (int)(note is NotParsedNote ?
                                                                          EntryFileState.NotParsed :
                                                                          note is LostNote ?
                                                                                            EntryFileState.Lost :
                                                                                            EntryFileState.Parsed)));
                await cmd.ExecuteNonQueryAsync();

            }
            catch (Exception ex)
            {
                _logger.LogMessage(db, string.Format("Fail to save note id=[{0}] from file [{1}] \r\n {2}", note, srcFile, ex));
            }
        }

        /// <summary>
        /// Stops watching directory
        /// </summary>
        public void StopWatch()
        {
            // if already stopped
            if (!_isWatching)
            {
                return;
            }

            _isWatching = false;

            // cancel token
            _tokenSource.Cancel();

            // wait for finish processing
            _tokenSource.Token.WaitHandle.WaitOne();
        }

        private class LostNote : Note
        {
        }

        private class NotParsedNote : Note
        {

        }
    }
}