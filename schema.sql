CREATE TABLE Preferences (
	Preferences_Name TEXT PRIMARY KEY,
	Preferences_Value VARIANT
);

CREATE UNIQUE INDEX Index_Preferences_Name ON Preferences(Preferences_Name);

CREATE TABLE Filters (
	Filters_ID INTEGER PRIMARY KEY, 
	Filters_Pattern TEXT
);

CREATE TABLE Folders (
	Folders_ID INTEGER PRIMARY KEY,
	Folders_Path TEXT
);

CREATE TABLE SourceFiles (
	SourceFiles_ID INTEGER PRIMARY KEY, 
	SourceFiles_Timestamp INTEGER, 
	SourceFiles_Path TEXT
);
CREATE UNIQUE INDEX Index_SourceFiles_Path ON SourceFiles(SourceFiles_Path ASC);
    
CREATE VIRTUAL TABLE FullText USING fts3 (
    SourceFiles_ID INTEGER PRIMARY KEY,
    FileText TEXT
);

PRAGMA user_version=2;