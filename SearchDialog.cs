using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SQLite;
using Squared.Task;
using Squared.Task.Data;

namespace Ndexer {
    public partial class SearchDialog : Form {
        enum SearchMode {
            None = -1,
            FindTags = 0,
            FindFiles = 1,
            TagsInFile = 2,
            TagsInContext = 3
        }

        class ColumnInfo {
            public Func<int, int> CalculateWidth;
            public Func<SearchResult, string> GetColumnValue;
        }

        struct SearchResult {
            public string Name;
            public string Filename;
            public long LineNumber;
        }

        ConnectionWrapper Connection = null;
        IFuture ActiveSearch = null;
        IFuture ActiveQueue = null;
        string ActiveSearchText = null;
        SearchMode ActiveSearchMode = SearchMode.None;
        string PendingSearchText = null;
        SearchMode PendingSearchMode = SearchMode.None;
        ListViewItem DefaultListItem = new ListViewItem(new string[3]);
        IList<SearchResult> SearchResults = new List<SearchResult>();
        SearchMode DisplayedSearchMode = SearchMode.None;

        public SearchDialog () {
            InitializeComponent();

            lvResults_SizeChanged(lvResults, EventArgs.Empty);

            this.Enabled = false;
            this.UseWaitCursor = true;
        }

        public void SetConnection(ConnectionWrapper connection) {
            Connection = connection;
            this.Enabled = true;
            this.UseWaitCursor = false;
        }

        private int CalculateLineNumberSize() {
            return TextRenderer.MeasureText("00000", lvResults.Font).Width;
        }

        private ColumnHeader[] GetColumnsForMode (SearchMode searchMode) {
            switch (searchMode) {
                case SearchMode.FindTags:
                case SearchMode.TagsInFile:
                case SearchMode.TagsInContext:
                    return new ColumnHeader[] {
                        new ColumnHeader() { 
                            Text = "Tag Name", 
                            Tag = new ColumnInfo() { 
                                CalculateWidth = (totalWidth) => ((totalWidth - CalculateLineNumberSize()) * 3 / 7),
                                GetColumnValue = (sr) => (sr.Name)
                            }
                        },
                        new ColumnHeader() { 
                            Text = "File Name", 
                            Tag = new ColumnInfo() { 
                                CalculateWidth = (totalWidth) => ((totalWidth - CalculateLineNumberSize()) * 4 / 7),
                                GetColumnValue = (sr) => (sr.Filename)
                            }
                        },
                        new ColumnHeader() { 
                            Text = "Line #", 
                            Tag = new ColumnInfo() { 
                                CalculateWidth = (totalWidth) => (CalculateLineNumberSize()),
                                GetColumnValue = (sr) => (sr.LineNumber.ToString())
                            }
                        },
                    };
                case SearchMode.FindFiles:
                    return new ColumnHeader[] {
                        new ColumnHeader() { 
                            Text = "File Name", 
                            Tag = new ColumnInfo() { 
                                CalculateWidth = (totalWidth) => (totalWidth),
                                GetColumnValue = (sr) => (sr.Filename)
                            }
                        }
                    };
            }

            return new ColumnHeader[0];
        }

        private void SetSearchResults (SearchMode searchMode, IList<SearchResult> items) {
            SearchResults = items;

            if (searchMode != DisplayedSearchMode) {
                var columns = GetColumnsForMode(searchMode);
                DisplayedSearchMode = searchMode;
                lvResults.VirtualListSize = 0;
                lvResults.Columns.Clear();
                lvResults.Columns.AddRange(columns);
            }
            
            lvResults.VirtualListSize = items.Count;

            if ((lvResults.SelectedIndices.Count == 0) && (items.Count > 0))
                lvResults.SelectedIndices.Add(0);

            lvResults_SizeChanged(null, EventArgs.Empty);
        }

        private TaskEnumerator<IDataRecord> BuildQuery (SearchMode searchMode, string searchText) {
            switch (searchMode) {
                case SearchMode.FindTags: {
                        var query = Connection.BuildQuery(
                            @"SELECT Tags_Name, SourceFiles_Path, Tags_LineNumber " +
                            @"FROM Tags_And_SourceFiles WHERE " +
                            @"Tags_Name = ? " +
                            @"UNION ALL " +
                            @"SELECT Tags_Name, SourceFiles_Path, Tags_LineNumber " +
                            @"FROM Tags_And_SourceFiles WHERE " +
                            @"Tags_Name GLOB ?"
                        );
                        return query.Execute(searchText, searchText + "?*");
                    }
                case SearchMode.FindFiles: {
                        var query = Connection.BuildQuery(
                            @"SELECT SourceFiles_Path " +
                            @"FROM SourceFiles WHERE " +
                            @"SourceFiles_Path GLOB ? " +
                            @"UNION ALL " +
                            @"SELECT SourceFiles_Path " +
                            @"FROM SourceFiles WHERE " +
                            @"SourceFiles_Path GLOB ?"
                        );
                        return query.Execute(@"*\" + searchText, @"*\" + searchText + "?*");
                    }
                case SearchMode.TagsInFile: {
                        var query = Connection.BuildQuery(
                            @"SELECT Tags_Name, SourceFiles_Path, Tags_LineNumber " +
                            @"FROM Tags_And_SourceFiles WHERE " +
                            @"SourceFiles_Path GLOB ? "
                        );
                        return query.Execute(@"*\" + searchText);
                    }
                case SearchMode.TagsInContext: {
                        var query = Connection.BuildQuery(
                            @"SELECT Tags_Name, SourceFiles_Path, Tags_LineNumber " +
                            @"FROM Tags_And_SourceFiles JOIN TagContexts USING (TagContexts_ID) WHERE " +
                            @"TagContexts_Text = ?"
                        );
                        return query.Execute(searchText);
                    }
            }

            throw new InvalidOperationException();
        }

        IEnumerator<object> PerformSearch (SearchMode searchMode, string searchText) {
            string[] columnValues = new string[3];

            pbProgress.Style = ProgressBarStyle.Marquee;
            lblStatus.Text = String.Format("Starting search...");

            var buffer = new List<SearchResult>();
            var item = new SearchResult();

            SetSearchResults(searchMode, buffer);

            if (searchText.Length > 0) {
                using (var iterator = BuildQuery(searchMode, searchText))
                while (!iterator.Disposed) {
                    if (PendingSearchText != null)
                        break;

                    yield return iterator.Fetch();

                    foreach (var current in iterator) {
                        switch (searchMode) {
                            case SearchMode.FindFiles:
                                item.Filename = current.GetString(0);
                            break;
                            case SearchMode.FindTags:
                            case SearchMode.TagsInFile:
                            case SearchMode.TagsInContext:
                                item.Name = current.GetString(0);
                                item.Filename = current.GetString(1);
                                item.LineNumber = current.GetInt64(2);
                            break;
                        }

                        buffer.Add(item);

                        if ((buffer.Count % 50 == 0) || ((buffer.Count < 20) && (buffer.Count % 5 == 1))) {
                            lblStatus.Text = String.Format("{0} result(s) found so far...", buffer.Count);
                            SetSearchResults(searchMode, buffer);
                        }
                    }
                }
            }

            if (PendingSearchText != null) {
                yield return BeginSearch();
            } else {
                SetSearchResults(searchMode, buffer);
                lblStatus.Text = String.Format("{0} result(s) found.", buffer.Count);
                pbProgress.Style = ProgressBarStyle.Continuous;
            }
        }

        IEnumerator<object> BeginSearch () {
            ActiveSearchText = PendingSearchText;
            ActiveSearchMode = PendingSearchMode;
            PendingSearchText = null;
            PendingSearchMode = SearchMode.None;
            ActiveSearch = Program.Scheduler.Start(
                PerformSearch(ActiveSearchMode, ActiveSearchText),
                TaskExecutionPolicy.RunAsBackgroundTask
            );
            yield break;
        }

        IEnumerator<object> QueueNewSearch (SearchMode searchMode, string searchText) {
            ActiveQueue = null;
            PendingSearchMode = searchMode;
            PendingSearchText = searchText;

            if ((ActiveSearch == null) || (ActiveSearch.Completed)) {
                yield return BeginSearch();
            }
        }

        private void txtFilter_TextChanged (object sender, EventArgs e) {
            SearchParametersChanged();
        }

        private void lvResults_SizeChanged (object sender, EventArgs e) {
            int totalSize = lvResults.ClientSize.Width - 3;
            for (int i = 0; i < lvResults.Columns.Count; i++) {
                int newWidth = (lvResults.Columns[i].Tag as ColumnInfo).CalculateWidth(totalSize);
                if (lvResults.Columns[i].Width != newWidth)
                    lvResults.Columns[i].Width = newWidth;
            }
        }

        private void lvResults_DoubleClick (object sender, EventArgs e) {
            SearchResult item;
            try {
                item = SearchResults[lvResults.SelectedIndices[0]];
            } catch {
                return;
            }
            using (var director = Program.GetDirector()) {
                switch (DisplayedSearchMode) {
                    case SearchMode.FindFiles:
                        director.OpenFile(item.Filename);
                        director.BringToFront();
                        break;
                    case SearchMode.FindTags:
                    case SearchMode.TagsInFile:
                    case SearchMode.TagsInContext:
                        director.OpenFile(item.Filename, item.LineNumber);
                        if (director is IAdvancedDirector)
                            ((IAdvancedDirector)director).FindText(item.Name);
                        director.BringToFront();
                        break;
                } 
            }
        }

        private void lvResults_DrawSubItem (object sender, DrawListViewSubItemEventArgs e) {
            if (e.ColumnIndex == 0) {
                e.DrawDefault = true;
                return;
            }

            if (e.Item.Selected)
                using (var backgroundBrush = new SolidBrush(lvResults.Focused ? SystemColors.Highlight : SystemColors.ButtonFace))
                    e.Graphics.FillRectangle(backgroundBrush, e.Bounds.X, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height);

            var textColor = lvResults.ForeColor;
            if (e.Item.Selected && lvResults.Focused)
                textColor = SystemColors.HighlightText;

            using (var textBrush = new SolidBrush(textColor)) {
                var textRect = new RectangleF(e.Bounds.Left, e.Bounds.Top, e.Bounds.Width, e.Bounds.Height);
                var stringFormat = StringFormat.GenericDefault;
                stringFormat.FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.FitBlackBox;
                stringFormat.Trimming = StringTrimming.EllipsisPath;
                e.Graphics.DrawString(e.SubItem.Text, lvResults.Font, textBrush, textRect, stringFormat);
            }
        }

        private void lvResults_RetrieveVirtualItem (object sender, RetrieveVirtualItemEventArgs e) {
            if ((e.ItemIndex < 0) || (e.ItemIndex >= SearchResults.Count))
                e.Item = DefaultListItem;
            else {
                SearchResult item = SearchResults[e.ItemIndex];
                string[] subitems = new string[lvResults.Columns.Count];
                for (int i = 0; i < lvResults.Columns.Count; i++)
                    subitems[i] = (lvResults.Columns[i].Tag as ColumnInfo).GetColumnValue(item);
                e.Item = new ListViewItem(subitems);
            }
        }

        private void SearchDialog_FormClosing (object sender, FormClosingEventArgs e) {
            if (Connection != null) {
                Connection.Dispose();
                Connection = null;
            }
            if (ActiveSearch != null) {
                ActiveSearch.Dispose();
                ActiveSearch = null;
            }
            if (ActiveQueue != null) {
                ActiveQueue.Dispose();
                ActiveQueue = null;
            }
        }

        private void tcFilter_SelectedIndexChanged(object sender, EventArgs e) {
            SearchParametersChanged();
        }

        private void SearchParametersChanged() {
            var searchMode = (SearchMode)tcFilter.SelectedIndex;
            string searchText = txtFilter.Text.Trim();

            ActiveQueue = Program.Scheduler.Start(
                QueueNewSearch(searchMode, searchText),
                TaskExecutionPolicy.RunAsBackgroundTask
            );
        }

        private void txtFilter_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter) {
                e.Handled = true;
                e.SuppressKeyPress = true;
                lvResults_DoubleClick(null, EventArgs.Empty);
            } else if (e.KeyCode == Keys.Tab && e.Control == true) {
                int newIndex = tcFilter.SelectedIndex + (e.Shift ? -1 : 1);
                if (newIndex >= tcFilter.TabCount)
                    newIndex = 0;
                else if (newIndex < 0)
                    newIndex = tcFilter.TabCount - 1;

                tcFilter.SelectedIndex = newIndex;
                txtFilter.Focus();
            }
        }

        private void SearchDialog_FormClosed (object sender, FormClosedEventArgs e) {
            this.Dispose();
        }
    }
}
