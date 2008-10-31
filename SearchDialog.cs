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

namespace Ndexer {
    public partial class SearchDialog : Form {
        struct SearchResult {
            public string Name;
            public string Filename;
            public long LineNumber;
        }

        TagDatabase Tags;
        Future ActiveSearch = null;
        Future ActiveQueue = null;
        string ActiveSearchText = null;
        string PendingSearchText = null;
        ListViewItem DefaultListItem = new ListViewItem(new string[3]);
        SearchResult[] SearchResults = new SearchResult[0];

        public SearchDialog (TagDatabase tags) {
            Tags = tags;

            InitializeComponent();

            lvResults_SizeChanged(lvResults, EventArgs.Empty);
        }

        private void SetSearchResults (SearchResult[] items) {
            SearchResults = items;
            lvResults.VirtualListSize = items.Length;
        }

        private string BuildQueryString () {
            var queryString =
                @"SELECT Tags_ID, Tags_Name, SourceFiles_Path, Tags_LineNumber " +
                @"FROM Tags_And_SourceFiles WHERE " +
                @"Tags_Name = @searchText " +
                @"UNION ALL " +
                @"SELECT * FROM (SELECT Tags_ID, Tags_Name, SourceFiles_Path, Tags_LineNumber " +
                @"FROM Tags_And_SourceFiles WHERE " +
                @"Tags_Name GLOB @queryText LIMIT 500)";

            return queryString;
        }

        IEnumerator<object> PerformSearch (string searchText) {
            var queryString = BuildQueryString();

            string[] columnValues = new string[3];

            pbProgress.Style = ProgressBarStyle.Marquee;
            lblStatus.Text = String.Format("{0} result(s) found so far...", lvResults.Items.Count);

            var buffer = new List<SearchResult>();
            var item = new SearchResult();

            SetSearchResults(buffer.ToArray());

            if (searchText.Length > 0) {
                string queryText = searchText + "?*";

                using (var query = Tags.Connection.BuildQuery(queryString))
                using (var iterator = new DbTaskIterator(
                    query,
                    new NamedParam { N = "searchText", V = searchText },
                    new NamedParam { N = "queryText", V = queryText }
                )) {
                    yield return iterator.Start();

                    while (!iterator.Disposed) {
                        if (PendingSearchText != null)
                            break;

                        item.Name = iterator.Current.GetString(1);
                        item.Filename = iterator.Current.GetString(2);
                        item.LineNumber = iterator.Current.GetInt64(3);

                        buffer.Add(item);

                        if ((buffer.Count % 50 == 0) || ((buffer.Count < 20) && (buffer.Count % 5 == 1))) {
                            lblStatus.Text = String.Format("{0} result(s) found so far...", buffer.Count);
                            SetSearchResults(buffer.ToArray());
                        }

                        yield return iterator.MoveNext();
                    }
                }
            }

            if (PendingSearchText != null) {
                yield return BeginSearch();
            } else {
                SetSearchResults(buffer.ToArray());
                lblStatus.Text = String.Format("{0} result(s) found.", buffer.Count);
                pbProgress.Style = ProgressBarStyle.Continuous;
            }
        }

        IEnumerator<object> BeginSearch () {
            ActiveSearchText = PendingSearchText;
            PendingSearchText = null;
            ActiveSearch = Program.Scheduler.Start(
                PerformSearch(ActiveSearchText),
                TaskExecutionPolicy.RunWhileFutureLives
            );
            yield break;
        }

        IEnumerator<object> QueueNewSearch (string searchText) {
            if ((ActiveSearch == null) || (ActiveSearch.Completed)) {
                PendingSearchText = searchText;
                ActiveQueue = null;
                yield return BeginSearch();
            } else {
                PendingSearchText = searchText;
                ActiveQueue = null;
            }
        }

        private void txtFilter_TextChanged (object sender, EventArgs e) {
            string searchText = txtFilter.Text.Trim();

            ActiveQueue = Program.Scheduler.Start(
                QueueNewSearch(searchText),
                TaskExecutionPolicy.RunAsBackgroundTask
            );
        }

        private void lvResults_SizeChanged (object sender, EventArgs e) {
            int totalSize = lvResults.ClientSize.Width - 2;
            int lineNumberSize = TextRenderer.MeasureText("00000", lvResults.Font).Width;
            totalSize -= lineNumberSize;
            lvResults.Columns[0].Width = totalSize * 3 / 7;
            lvResults.Columns[1].Width = totalSize * 4 / 7;
            lvResults.Columns[2].Width = lineNumberSize;
        }

        private void lvResults_DoubleClick (object sender, EventArgs e) {
            SearchResult item;
            try {
                item = SearchResults[lvResults.SelectedIndices[0]];
            } catch {
                return;
            }
            try {
                using (var director = new SciTEDirector()) {
                    director.OpenFile(item.Filename, item.LineNumber);
                    director.FindText(item.Name);
                    director.BringToFront();
                }
            } catch (SciTENotRunningException) {
                MessageBox.Show(this, "SciTE not running", "Error");
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
            if ((e.ItemIndex < 0) || (e.ItemIndex >= SearchResults.Length))
                e.Item = DefaultListItem;
            else {
                SearchResult item = SearchResults[e.ItemIndex];
                string[] subitems = new string[3];
                subitems[0] = item.Name;
                subitems[1] = item.Filename;
                subitems[2] = item.LineNumber.ToString();
                e.Item = new ListViewItem(subitems);
            }
        }

        private void SearchDialog_FormClosing (object sender, FormClosingEventArgs e) {
            if (ActiveSearch != null)
                ActiveSearch.Dispose();
        }
    }
}
