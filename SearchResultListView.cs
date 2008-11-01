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
                if (Items.Count > 0)
                    EnsureVisible(0);

                base.VirtualListSize = value;
            }
        }
    }
}
