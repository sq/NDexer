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
        TagDatabase Tags;
        Future ActiveSearch = null;
        string ActiveSearchText = null;
        string PendingSearchText = null;

        public SearchDialog (TagDatabase tags) {
            Tags = tags;

            InitializeComponent();

            lvResults_SizeChanged(lvResults, EventArgs.Empty);
        }

        IEnumerator<object> PerformSearch (string searchText) {
            var queryString =
                @"SELECT Tags_ID, Tags_Name, SourceFiles_Path, Tags_LineNumber " +
                @"FROM Tags, SourceFiles WHERE " +
                @"SourceFiles.SourceFiles_ID == Tags.SourceFiles_ID AND " +
                @"Tags_Name LIKE ? " +
                @"LIMIT 50";

            string[] columnValues = new string[2];

            using (var query = Tags.QueryManager.BuildQuery(queryString))
            using (var iterator = new DbTaskIterator(query, searchText)) {

                yield return iterator.Start();

                lvResults.Items.Clear();

                while (!iterator.Disposed) {
                    columnValues[0] = iterator.Current.GetString(1);
                    string filename = iterator.Current.GetString(2);
                    long lineNumber = iterator.Current.GetInt64(3);
                    string location = String.Format("{0}@{1}", filename, lineNumber);
                    TextRenderer.MeasureText(
                        location, lvResults.Font, new Size(lvResults.Columns[1].Width - 20, lvResults.Height), 
                        TextFormatFlags.PathEllipsis | TextFormatFlags.ModifyString | TextFormatFlags.SingleLine
                    );
                    columnValues[1] = location;

                    lvResults.Items.Add(new ListViewItem(columnValues));

                    yield return iterator.MoveNext();
                }
            }

            if (PendingSearchText != null)
                yield return BeginSearch();
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
            PendingSearchText = searchText;

            if ((ActiveSearch == null) || (ActiveSearch.Completed))
                yield return BeginSearch();
        }

        private void txtFilter_TextChanged (object sender, EventArgs e) {
            string searchText = txtFilter.Text;

            Program.Scheduler.Start(QueueNewSearch(searchText));
        }

        private void lvResults_SizeChanged (object sender, EventArgs e) {
            int totalWidth = lvResults.ClientRectangle.Width - 2;
            lvResults.Columns[0].Width = (totalWidth * 2 / 5);
            lvResults.Columns[1].Width = (totalWidth * 3 / 5);
        }

        private void lvResults_ColumnWidthChanged (object sender, ColumnWidthChangedEventArgs e) {
            int totalWidth = lvResults.ClientRectangle.Width - 2;
            int otherColumn = e.ColumnIndex == 0 ? 1 : 0;
            lvResults.Columns[otherColumn].Width = totalWidth - lvResults.Columns[e.ColumnIndex].Width;
        }
    }
}
