using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Squared.Task;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

namespace Ndexer {
#if !MONO
    public class NotepadPlusPlusDirector : Director, IBasicDirector {
        public NotepadPlusPlusDirector (string applicationPath)
            : base(applicationPath) {
        }

        protected override IntPtr FindDirectorWindow () {
            return IntPtr.Zero;
        }

        protected override IntPtr FindEditorWindow () {
            return FindWindow("Notepad++", null);
        }

        public void Launch (string arguments) {
            var info = new ProcessStartInfo(_ApplicationPath, arguments);
            var process = Process.Start(info);
            process.WaitForInputIdle();
            _EditorWindow = FindEditorWindow();
            process.Dispose();
        }

        public void OpenFile (string filename) {
            Launch(filename);
        }

        public void OpenFile (string filename, long initialLineNumber) {
            Launch(String.Format("-n{1:0} \"{0}\"", filename, initialLineNumber));
        }

        public static bool LocateExecutable (ref string filename) {
            return Director.TryLocateExecutable(Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), @"Notepad++\Notepad++.exe"
                ), ref filename
            );
        }
    }
#endif
}
