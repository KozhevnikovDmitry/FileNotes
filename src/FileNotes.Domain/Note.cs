using System;
using System.Globalization;

namespace FileNotes.Domain
{
    public class Note
    {
        public int Id { get; set; }
        
        public int NoteId { get; set; }

        public DateTime Date { get; set; }

        public string DateString
        {
            get { return this.Date.ToString("dd:MM:yyyy HH:mm:ss"); }
            set { this.Date = DateTime.ParseExact(value, "dd:MM:yyyy HH:mm:ss", new CultureInfo("ru-ru")); }
        }

        public string Content { get; set; }
    }
}