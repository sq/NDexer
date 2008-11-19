CREATE TABLE Preferences (
Preferences_Name TEXT PRIMARY KEY,
Preferences_Value TEXT
);

CREATE UNIQUE INDEX Index_Preferences_Name ON Preferences(Preferences_Name);

CREATE TABLE Filters (
Filters_ID INTEGER PRIMARY KEY, 
Filters_Language TEXT, 
Filters_Pattern TEXT
);

CREATE TABLE Folders (
Folders_ID INTEGER PRIMARY KEY,
Folders_Path TEXT
);

CREATE TABLE Tags (
Tags_ID INTEGER PRIMARY KEY,
Tags_Name TEXT,
SourceFiles_ID INTEGER,
Tags_LineNumber INTEGER,
TagLanguages_ID INTEGER,
TagKinds_ID INTEGER,
TagContexts_ID INTEGER
);
CREATE INDEX Index_Tags_Name ON Tags(Tags_Name);
CREATE INDEX Index_Tags_SourceFiles_ID ON Tags(SourceFiles_ID);
CREATE INDEX Index_Tags_TagContexts_ID ON Tags(TagContexts_ID);

CREATE TABLE SourceFiles (
SourceFiles_ID INTEGER PRIMARY KEY, 
SourceFiles_Timestamp INTEGER, 
SourceFiles_Path TEXT
);
CREATE UNIQUE INDEX Index_SourceFiles_Path ON SourceFiles(SourceFiles_Path ASC);

CREATE TABLE TagContexts (
TagContexts_ID INTEGER PRIMARY KEY, 
TagContexts_Text TEXT
);
CREATE UNIQUE INDEX Index_TagContexts_Text ON TagContexts(TagContexts_Text ASC);

CREATE TABLE TagKinds (
TagKinds_ID INTEGER PRIMARY KEY, 
TagKinds_Name TEXT
);
CREATE UNIQUE INDEX Index_TagKinds_Name ON TagKinds(TagKinds_Name ASC);

CREATE TABLE TagLanguages (
TagLanguages_ID INTEGER PRIMARY KEY, 
TagLanguages_Name TEXT
);
CREATE UNIQUE INDEX Index_TagLanguages_Name ON TagLanguages(TagLanguages_Name ASC);

CREATE VIEW Tags_And_SourceFiles AS SELECT Tags.*, SourceFiles_Path FROM
    Tags, SourceFiles WHERE
    Tags.SourceFiles_ID = SourceFiles.SourceFiles_ID;

CREATE VIEW Tags_Denormalized AS SELECT Tags.*, SourceFiles_Path, TagContexts_Text, TagKinds_Name, TagLanguages_Name FROM
    Tags, SourceFiles, TagContexts, TagKinds, TagLanguages WHERE
    Tags.SourceFiles_ID = SourceFiles.SourceFiles_ID AND
    Tags.TagContexts_ID = TagContexts.TagContexts_ID AND
    Tags.TagKinds_ID = TagKinds.TagKinds_ID AND
    Tags.TagLanguages_ID = TagLanguages.TagLanguages_ID;
    
CREATE VIRTUAL TABLE FullText USING fts3 (
    SourceFiles_ID INTEGER PRIMARY KEY,
    FileText TEXT
);
