using FileNotes.Domain;

namespace FileNotes.Web.DataAccess
{
    public interface INoteRepository
    {
        Note Get(int id);
    }
}