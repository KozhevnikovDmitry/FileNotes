using System;
using System.Transactions;

namespace FileNotes.Web.DataAccess
{
    internal abstract class Repository : IDisposable
    {
        private readonly FnContext _db;

        protected Repository(FnContext db)
        {
            if (db == null) throw new ArgumentNullException("db");
            _db = db;
        }

        protected T Execute<T>(Func<FnContext, T> func)
        {
            using (var transaction = new TransactionScope())
            {
                var result = func(_db);
                transaction.Complete();
                return result;
            }
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}