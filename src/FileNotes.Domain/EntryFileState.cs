using System.ComponentModel;

namespace FileNotes.Domain
{
    public enum EntryFileState
    {
        [Description("Parsed succesfully")]
        Parsed = 0,

        [Description("Parsed with errors")]
        NotParsed = 1,

        [Description("Lost. Can not access")]
        Lost = 2,
    }
}