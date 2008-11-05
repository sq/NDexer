using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Ndexer {
    public class SearchResultListView : ListView {
        public SearchResultListView () 
            : base() {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        }

        public new int VirtualListSize {
            get {
                return base.VirtualListSize;
            }
            set {
                if (base.VirtualListSize > value) {
                    int index = 0;
                    if (SelectedIndices.Count > 0) {
                        index = SelectedIndices[0];
                    }
                    if (index >= value)
                        index = value - 1;
                    EnsureVisible(index);
                }

                base.VirtualListSize = value;
            }
        }
    }
}
