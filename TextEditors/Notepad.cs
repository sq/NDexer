using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ndexer {
    public class NotepadDirector : Director {
        protected override IntPtr FindDirectorWindow () {
            return FindWindow("Notepad", null);
        }

        protected override IntPtr FindEditorWindow () {
            return FindWindow("Notepad", null);
        }
    }
}
