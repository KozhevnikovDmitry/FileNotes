using System;
using System.Linq;
using FileNotes.Domain;
using FileNotes.Web.DataAccess;
using FileNotes.Web.Tool;

namespace FileNotes.Web.Models.EntryFileModel
{
    public class EntryFileFacade
    {
        private readonly IEntryFileRepository _entryFileRepository;

        public EntryFileFacade(IEntryFileRepository entryFileRepository)
        {
            if (entryFileRepository == null) throw new ArgumentNullException("entryFileRepository");
            _entryFileRepository = entryFileRepository;
        }

        public OutputEntriesVm GetAll(InputEntriesVm entriesVm)
        {
            try
            {
                var entryFiles = _entryFileRepository.Query(entriesVm.Id,
                                                            entriesVm.FileName,
                                                            entriesVm.EntryFileState,
                                                            entriesVm.Size,
                                                            entriesVm.rows,
                                                            entriesVm.page,
                                                            entriesVm.sidx,
                                                            entriesVm.sord);
                
                double count = entryFiles.Item1;
                double pages = count / entriesVm.rows;
                return new OutputEntriesVm
                {
                    records = entryFiles.Item1.ToString(),
                    total = Convert.ToInt32(Math.Ceiling(pages)),
                    rows = entryFiles.Item2.Select(MapToVm).ToList(),
                    page = entriesVm.page
                };
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Fail to load file entries", ex);
            }
        }

        private EntryFileVm MapToVm(EntryFile entryFile)
        {
            return new EntryFileVm
            {
                Id = entryFile.Id,
                SourceFileName = entryFile.SourceFileName,
                EntryFileState = entryFile.EntryFileState.GetDescription(),
                Size = entryFile.Size,
                NoteId = entryFile.NoteId
            };
        }
    }
}