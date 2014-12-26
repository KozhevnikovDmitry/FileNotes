USE master
GO
CREATE DATABASE [filenotes];
GO
USE [filenotes]
GO
CREATE LOGIN [filenotes] WITH PASSWORD = 'till25for@';
GO
CREATE USER [filenotes] FOR LOGIN [filenotes];
GO
CREATE TABLE [Note]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1, 1),	
	[NoteId] INT NOT NULL,	
	[Date] DateTime NOT NULL,
	[Content] VARCHAR(1024) NULL
)
GO
CREATE TABLE [EntryFile]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1, 1),
	[SourceFileName] VARCHAR(260) NOT NULL,
	[Size] BIGINT NULL,
	[EntryFileState] INT NOT NUll,
	[NoteId] INT NOT NULL,
    CONSTRAINT [FK_EntryFile_Note] FOREIGN KEY ([NoteId]) REFERENCES [Note](Id)
)
GO
CREATE TABLE [Log]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1, 1),
	[Msg] VARCHAR(1000) NOT NULL,
	[Date] DateTime NOT NULL
)
GO
GRANT SELECT, INSERT on [EntryFile] to [filenotes]
GRANT SELECT, INSERT on [Note] to [filenotes]
GRANT SELECT, INSERT on [Log] to [filenotes]
GO