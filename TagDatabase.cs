using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;

namespace Ndexer {
    public class TagDatabase : IDisposable {
        public struct Filter {
            public int ID;
            public string Pattern;
            public string Language;
        }

        public struct Change {
            public string Filename;
            public bool Deleted;
        }

        public struct Folder {
            public int ID;
            public string Path;
        }

        public struct SourceFile {
            public int ID;
            public string Path;
            public long Timestamp;
        }

        private SQLiteCommand
            _GetContextID,
            _GetKindID,
            _GetLanguageID,
            _GetSourceFileID,
            _GetSourceFileTimestamp,
            _GetFilters,
            _GetFolders,
            _GetSourceFiles,
            _MakeContextID,
            _MakeKindID,
            _MakeLanguageID,
            _MakeSourceFileID,
            _UpdateSourceFileTimestamp,
            _DeleteTagsForFile,
            _DeleteSourceFile,
            _DeleteTagsForFolder,
            _DeleteSourceFilesForFolder,
            _LastInsertID,
            _InsertTag;

        public SQLiteConnection Connection;

        public TagDatabase (string filename) {
            string connectionString = String.Format("Data Source={0}", filename);
            Connection = new SQLiteConnection(connectionString);
            Connection.Open();

            CompileQueries();
        }

        private void CompileQueries () {
            _GetContextID = CompileQuery(@"SELECT TagContexts_ID FROM TagContexts WHERE TagContexts_Text = ?");
            _GetKindID = CompileQuery(@"SELECT TagKinds_ID FROM TagKinds WHERE TagKinds_Name = ?");
            _GetLanguageID = CompileQuery(@"SELECT TagLanguages_ID FROM TagLanguages WHERE TagLanguages_Name = ?");
            _GetSourceFileID = CompileQuery(@"SELECT SourceFiles_ID FROM SourceFiles WHERE SourceFiles_Path = ?");
            _GetSourceFileTimestamp = CompileQuery(@"SELECT SourceFiles_Timestamp FROM SourceFiles WHERE SourceFiles_Path = ?");
            _GetFilters = CompileQuery(@"SELECT Filters_ID, Filters_Pattern, Filters_Language FROM Filters");
            _GetFolders = CompileQuery(@"SELECT Folders_ID, Folders_Path FROM Folders");
            _GetSourceFiles = CompileQuery(@"SELECT SourceFiles_ID, SourceFiles_Path, SourceFiles_Timestamp FROM SourceFiles");
            _LastInsertID = CompileQuery(@"SELECT last_insert_rowid()");
            _MakeContextID = CompileQuery(@"INSERT INTO TagContexts (TagContexts_Text) VALUES (?)");
            _MakeKindID = CompileQuery(@"INSERT INTO TagKinds (TagKinds_Name) VALUES (?)");
            _MakeLanguageID = CompileQuery(@"INSERT INTO TagLanguages (TagLanguages_Name) VALUES (?)");
            _MakeSourceFileID = CompileQuery(@"INSERT INTO SourceFiles (SourceFiles_Path, SourceFiles_Timestamp) VALUES (?, ?)");
            _UpdateSourceFileTimestamp = CompileQuery(@"INSERT OR REPLACE INTO SourceFiles (SourceFiles_Path, SourceFiles_Timestamp) VALUES (?, ?)");
            _DeleteTagsForFile = CompileQuery(@"DELETE FROM Tags WHERE SourceFiles_ID = ?");
            _DeleteSourceFile = CompileQuery(@"DELETE FROM SourceFiles WHERE SourceFiles_ID = ?");
            _DeleteTagsForFolder = CompileQuery(
                @"DELETE FROM Tags WHERE " +
                @"Tags.SourceFiles_ID IN ( " +
                @"SELECT SourceFiles_ID FROM SourceFiles WHERE " +
                @"SourceFiles.SourceFiles_Path LIKE ? )"
            );
            _DeleteSourceFilesForFolder = CompileQuery(@"DELETE FROM SourceFiles WHERE SourceFiles_Path LIKE ?");
            _InsertTag = CompileQuery(
                @"INSERT INTO Tags (" +
                @"Tags_Name, SourceFiles_ID, Tags_LineNumber, TagKinds_ID, TagContexts_ID, TagLanguages_ID" +
                @") VALUES (" +
                @"?, ?, ?, ?, ?, ?" +
                @")"
            );
        }

        public IEnumerable<Filter> GetFilters () {
            using (var reader = _GetFilters.ExecuteReader()) {
                while (reader.Read()) {
                    var filter = new Filter();
                    filter.ID = reader.GetInt32(0);
                    filter.Pattern = reader.GetString(1);
                    filter.Language = reader.GetString(2);
                    yield return filter;
                }
            }
        }

        public IEnumerable<Folder> GetFolders () {
            using (var reader = _GetFolders.ExecuteReader()) {
                while (reader.Read()) {
                    var folder = new Folder();
                    folder.ID = reader.GetInt32(0);
                    folder.Path = reader.GetString(1);
                    yield return folder;
                }
            }
        }

        public IEnumerable<SourceFile> GetSourceFiles () {
            using (var reader = _GetSourceFiles.ExecuteReader()) {
                while (reader.Read()) {
                    var sf = new SourceFile();
                    sf.ID = reader.GetInt32(0);
                    sf.Path = reader.GetString(1);
                    sf.Timestamp = reader.GetInt64(2);
                    yield return sf;
                }
            }
        }

        public void DeleteSourceFile (string filename) {
            int id = Convert.ToInt32(ExecuteQuery(_GetSourceFileID, filename));
            ExecuteQuery(_DeleteTagsForFile, id);
            ExecuteQuery(_DeleteSourceFile, id);
            Console.WriteLine("Deleted file {0} from database.", filename);
        }

        public void DeleteTagsForFile (string filename) {
            int id = Convert.ToInt32(ExecuteQuery(_GetSourceFileID, filename));
            ExecuteQuery(_DeleteTagsForFile, id);
        }

        public void DeleteSourceFileOrFolder (string filename) {
            int id = Convert.ToInt32(ExecuteQuery(_GetSourceFileID, filename));
            if (id != 0) {
                ExecuteQuery(_DeleteTagsForFile, id);
                ExecuteQuery(_DeleteSourceFile, id);
                Console.WriteLine("Deleted file {0} from database.", filename);
            } else {
                if (!filename.EndsWith("\\"))
                    filename += "\\";
                filename += "%";
                ExecuteQuery(_DeleteTagsForFolder, filename);
                ExecuteQuery(_DeleteSourceFilesForFolder, filename);
                Console.WriteLine("Deleted folder {0} from database.", filename);
            }
        }

        public IEnumerable<Change> UpdateFileListAndGetChangeSet () {
            string filters = String.Join(
                ";",
                (from f in GetFilters() select f.Pattern).ToArray()
            );

            foreach (var file in GetSourceFiles()) {
                if (!System.IO.File.Exists(file.Path))
                    yield return new Change { Filename = file.Path, Deleted = true };
            }

            foreach (var folder in GetFolders()) {
                foreach (var entry in Squared.Util.IO.EnumDirectoryEntries(
                    folder.Path, filters, true, Squared.Util.IO.IsFile
                )) {
                    long newTimestamp = entry.LastWritten;
                    long oldTimestamp = (long)(GetSourceFileTimestamp(entry.Name) ?? (long)0);
                    if (newTimestamp > oldTimestamp)
                        yield return new Change { Filename = entry.Name, Deleted = false };
                }
            }

            yield break;
        }

        public int GetLastInsertID () {
            object result = _LastInsertID.ExecuteScalar();
            return Convert.ToInt32(result);
        }

        public object GetSourceFileTimestamp (string path) {
            return ExecuteQuery(_GetSourceFileTimestamp, path);
        }

        public void UpdateSourceFileTimestamp (string path, long timestamp) {
            ExecuteQuery(_UpdateSourceFileTimestamp, path, timestamp);
        }

        public int GetSourceFileID (string path) {
            object id = ExecuteQuery(_GetSourceFileID, path);
            if (id is long) {
                return Convert.ToInt32(id);
            } else {
                return MakeSourceFileID(path, 0);
            }
        }

        internal int MakeSourceFileID (string path, int timestamp) {
            ExecuteQuery(_MakeSourceFileID, path, timestamp);
            return GetLastInsertID();
        }

        public int GetKindID (string kind) {
            if (kind == null)
                return 0;

            object id = ExecuteQuery(_GetKindID, kind);
            if (id is long) {
                return Convert.ToInt32(id);
            } else {
                ExecuteQuery(_MakeKindID, kind);
                return GetLastInsertID();
            }
        }

        public int GetContextID (string context) {
            if (context == null)
                return 0;

            object id = ExecuteQuery(_GetContextID, context);
            if (id is long) {
                return Convert.ToInt32(id);
            } else {
                ExecuteQuery(_MakeContextID, context);
                return GetLastInsertID();
            }
        }

        public int GetLanguageID (string language) {
            if (language == null)
                return 0;

            object id = ExecuteQuery(_GetLanguageID, language);
            if (id is long) {
                return Convert.ToInt32(id);
            } else {
                ExecuteQuery(_MakeLanguageID, language);
                return GetLastInsertID();
            }
        }

        public object ExecuteQuery (SQLiteCommand command, params object[] parameters) {
            for (int i = 0; i < parameters.Length; i++) {
                command.Parameters[i].Value = parameters[i];
            }
            return command.ExecuteScalar();
        }

        public SQLiteCommand CompileQuery (string sql) {
            var cmd = new SQLiteCommand(
                sql, Connection
            );
            string[] parts = sql.Split('?');
            int numParams = parts.Length - 1;
            for (int i = 0; i < numParams; i++)
                cmd.Parameters.Add(cmd.CreateParameter());
            return cmd;
        }

        public int AddTag (Tag tag) {
            int sourceFileID = GetSourceFileID(tag.SourceFile);
            int kindID = GetKindID(tag.Kind);
            int contextID = GetContextID(tag.Context);
            int languageID = GetLanguageID(tag.Language);
            ExecuteQuery(_InsertTag,
                tag.Name, sourceFileID, tag.LineNumber,
                kindID, contextID, languageID
            );
            return GetLastInsertID();
        }

        public int ExecuteSQL (string sql) {
            var cmd = CompileQuery(sql);
            return cmd.ExecuteNonQuery();
        }

        public void Clear () {
            ExecuteSQL("DELETE FROM Tags");
        }

        void IDisposable.Dispose () {
            Connection.Dispose();
        }
    }
}
