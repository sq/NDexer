CREATE TABLE Preferences (
	Preferences_Name TEXT PRIMARY KEY NOT NULL,
	Preferences_Value VARIANT
);

CREATE UNIQUE INDEX Index_Preferences_Name ON Preferences(Preferences_Name);

CREATE TABLE Filters (
	Filters_ID INTEGER PRIMARY KEY NOT NULL, 
	Filters_Pattern TEXT NOT NULL
);

CREATE TABLE Folders (
	Folders_ID INTEGER PRIMARY KEY NOT NULL,
	Folders_Path TEXT NOT NULL,
	Folders_Excluded BOOLEAN NOT NULL
);

CREATE TABLE SourceFiles (
	SourceFiles_ID INTEGER PRIMARY KEY NOT NULL, 
	SourceFiles_Timestamp INTEGER NOT NULL, 
	SourceFiles_Path TEXT NOT NULL
);
CREATE UNIQUE INDEX Index_SourceFiles_Path ON SourceFiles(SourceFiles_Path ASC);
    
CREATE VIRTUAL TABLE FullText USING fts3 (
    SourceFiles_ID INTEGER PRIMARY KEY NOT NULL,
    FileText TEXT NOT NULL
);

PRAGMA user_version=4;