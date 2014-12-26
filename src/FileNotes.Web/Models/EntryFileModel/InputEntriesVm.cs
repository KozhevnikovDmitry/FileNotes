namespace FileNotes.Web.Models.EntryFileModel
{
    public class InputEntriesVm
    {
        public string sidx { get; set; }

        public string sord { get; set; }
        
        public int page { get; set; }

        public int rows { get; set; }
        
        public int? Id { get; set; }

        public string FileName { get; set; }

        public int? EntryFileState { get; set; }

        public int? Size { get; set; }
    }
}