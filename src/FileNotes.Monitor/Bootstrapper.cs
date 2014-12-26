using System;
using System.Configuration;
using System.IO;
using FileNotes.Monitor.Monitoring;

namespace FileNotes.Monitor
{
    public static class Bootstrapper
    {
        public static EntryFileMonitor CreateMonitor()
        {
            try
            {
                var sourceFolder = ConfigurationManager.AppSettings["sourceFolder"];
                if (!Directory.Exists(sourceFolder))
                {
                    throw new ApplicationException(string.Format("Error: source folder [{0}] is not found", sourceFolder));
                }

                var destionationFolder = ConfigurationManager.AppSettings["destinationFolder"];
                if (!Directory.Exists(destionationFolder))
                {
                    throw new ApplicationException(string.Format("Error: destination folder [{0}] is not found",
                        destionationFolder));
                }

                var connectionString = ConfigurationManager.ConnectionStrings["filenotes"].ConnectionString;

                return new EntryFileMonitor(connectionString, sourceFolder, destionationFolder, new Log());
            }
            catch (ApplicationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Fail to bootstrap file note monitor", ex);
            }
        }
    }
}
