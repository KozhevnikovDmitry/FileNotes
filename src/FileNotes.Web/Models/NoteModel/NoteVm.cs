using System.ComponentModel.DataAnnotations;

namespace FileNotes.Web.Models.NoteModel
{
    public class NoteVm
    {
        [Display(Name = "Id")]
        public int Id { get; set; }

        [Display(Name = "Note Id")]
        public int NoteId { get; set; }

        [Display(Name = "Note date")]
        public string Date { get; set; }

        [Display(Name = "Note content")]
        public string Content { get; set; }
    }
}