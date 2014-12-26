using System;
using System.Data.Entity;
using FileNotes.Domain;

namespace FileNotes.Web.DataAccess
{
    internal class FnContext : DbContext
    {
        public FnContext(string connectionString)
            : base(connectionString)
        {
            Database.Log = Console.WriteLine;
        }

        public DbSet<EntryFile> EntryFiles { get; set; }

        public DbSet<Note> Notes { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EntryFile>()
                        .ToTable("EntryFile")
                        .HasKey(x => x.Id)
                        .Property(t => t.EntryFileState).HasColumnType("INT").HasColumnName("EntryFileState");

            modelBuilder.Entity<Note>()
                        .ToTable("Note")
                        .Ignore(t => t.DateString)
                        .HasKey(x => x.Id);
        }
    }
}