using System.ComponentModel.DataAnnotations;

namespace FileNotes.Web.Models.EntryFileModel
{
    public class EntryFileVm
    {
        [Display(Name = "Id")]
        public int Id { get; set; }

        [Display(Name = "File size")]
        public long Size { get; set; }

        [Display(Name = "Precess status")]
        public string EntryFileState { get; set; }

        [Display(Name = "File name")]
        public string SourceFileName { get; set; }

        public int NoteId { get; set; }
    }
}