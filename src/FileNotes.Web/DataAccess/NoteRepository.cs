using System.Linq;
using FileNotes.Domain;

namespace FileNotes.Web.DataAccess
{
    internal class NoteRepository : Repository, INoteRepository
    {
        public NoteRepository(FnContext db)
            : base(db)
        {

        }

        public Note Get(int id)
        {
            return Execute(db => db.Notes.SingleOrDefault(t => t.Id == id));
        }
    }
}