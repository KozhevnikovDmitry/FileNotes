namespace FileNotes.Domain
{
    public class EntryFile
    {
        public int Id { get; set; }
        
        public string SourceFileName { get; set; }

        public long Size { get; set; }

        public EntryFileState EntryFileState { get; set; }

        public int NoteId { get; set; }
    }
}