using System;

namespace FileNotes.Monitor
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var fileNoteMonitor = Bootstrapper.CreateMonitor();
                fileNoteMonitor.StartWatch();

                Console.WriteLine("Press \'q\' to quit.");
                while (Console.Read() != 'q') ;

                fileNoteMonitor.StopWatch();

            }
            catch (Exception ex)
            {
                // critical crashing error
                Console.WriteLine(ex);
                Console.ReadKey();
            }
        }
    }
}
