using System;
using FileNotes.Domain;
using FileNotes.Web.DataAccess;

namespace FileNotes.Web.Models.NoteModel
{
    public class NoteFacade
    {
        private readonly INoteRepository _noteRepository;

        public NoteFacade(INoteRepository noteRepository)
        {
            if (noteRepository == null) throw new ArgumentNullException("noteRepository");
            _noteRepository = noteRepository;
        }
        
        public NoteVm GetForShow(int id)
        {
            try
            {
                return MapToVm(_noteRepository.Get(id));
            }
            catch (Exception ex)
            {
                throw new ApplicationException(string.Format("Fail to get note id=[{0}]", id), ex);
            }
        }

        private NoteVm MapToVm(Note note)
        {
            return new NoteVm
            {
                Id = note.Id,
                NoteId = note.NoteId,
                Date = note.DateString,
                Content = note.Content
            };
        }
    }
}