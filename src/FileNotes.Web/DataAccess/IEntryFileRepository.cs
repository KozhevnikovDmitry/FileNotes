using System;
using System.Collections.Generic;
using FileNotes.Domain;

namespace FileNotes.Web.DataAccess
{
    public interface IEntryFileRepository
    {
        Tuple<int, List<EntryFile>> Query(int? id,
                                          string fileName,
                                          int? state,
                                          int? size,
                                          int amount,
                                          int page,
                                          string sortColumn,
                                          string sortOrder);
    }
}