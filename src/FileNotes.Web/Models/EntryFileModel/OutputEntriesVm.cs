using System.Collections.Generic;

namespace FileNotes.Web.Models.EntryFileModel
{
    public class OutputEntriesVm
    {
        public string records { get; set; }

        public int total { get; set; }

        public int page { get; set; }

        public List<EntryFileVm> rows { get; set; }
    }
}