using System;
using System.Collections.Generic;
using System.Linq;
using FileNotes.Domain;
using FileNotes.Web.Tool;

namespace FileNotes.Web.DataAccess
{
    internal class EntryFileRepository : Repository, IEntryFileRepository
    {
        public EntryFileRepository(FnContext db)
            : base(db)
        {

        }

        public Tuple<int, List<EntryFile>> Query(int? id,
                                                 string fileName,
                                                 int? state,
                                                 int? size,
                                                 int amount,
                                                 int page,
                                                 string sortColumn,
                                                 string sortOrder)
        {
            return Execute(db =>
            {
                var query = db.EntryFiles.AsQueryable();

                if (id.HasValue)
                {
                    query = query.Where(t => t.Id == id.Value);
                }

                if (!string.IsNullOrEmpty(fileName))
                {
                    query = query.Where(t => t.SourceFileName.Contains(fileName));
                }

                if (state.HasValue)
                {
                    query = query.Where(t => t.EntryFileState == (EntryFileState)state.Value);
                }

                if (size.HasValue)
                {
                    query = query.Where(t => t.Size == size.Value);
                }

                query = query.OrderBy(t => t.Id);

                var desc = sortOrder == "desc";
                var exmp = new EntryFile();
                if (sortColumn == Prop.GetName(() => exmp.Id))
                {
                    query = desc ? query.OrderByDescending(t => t.Id) : query.OrderBy(t => t.Id);
                }
                if (sortColumn == Prop.GetName(() => exmp.SourceFileName))
                {
                    query = desc ? query.OrderByDescending(t => t.SourceFileName) : query.OrderBy(t => t.SourceFileName);
                }
                if (sortColumn == Prop.GetName(() => exmp.Size))
                {
                    query = desc ? query.OrderByDescending(t => t.Size) : query.OrderBy(t => t.Size);
                }
                if (sortColumn == Prop.GetName(() => exmp.EntryFileState))
                {
                    query = desc ? query.OrderByDescending(t => t.EntryFileState) : query.OrderBy(t => t.EntryFileState);
                }

                var total = query.Count();
                var result = query.Skip((page-1) * amount).Take(amount).ToList();

                return new Tuple<int, List<EntryFile>>(total, result);
            });
        }
    }
}