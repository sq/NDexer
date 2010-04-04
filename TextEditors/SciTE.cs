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
    public class SciTENotRunningException : Exception {
        public SciTENotRunningException ()
            : base("Unable to find a running instance of SciTE") {
        }
    }

    public class SciTEDirector : Director, IBasicDirector, IAdvancedDirector {
        public SciTEDirector (string applicationPath)
            : base (applicationPath) {
            if (_DirectorWindow == IntPtr.Zero || _EditorWindow == IntPtr.Zero)
                Launch();

            if (_DirectorWindow == IntPtr.Zero || _EditorWindow == IntPtr.Zero)
                throw new SciTENotRunningException();
        }

        protected override IntPtr FindDirectorWindow () {
            return FindWindow("DirectorExtension", "DirectorExtension");
        }

        protected override IntPtr FindEditorWindow () {
            return FindWindow("SciTEWindow", null);
        }

        public void Launch () {
            var info = new ProcessStartInfo(_ApplicationPath, "");
            var process = Process.Start(info);
            process.WaitForInputIdle();
            _DirectorWindow = FindDirectorWindow();
            _EditorWindow = FindEditorWindow();
            process.Dispose();
        }

        public Future SendCommand (string command, params object[] parameters) {
            var response = WaitForCopyData();

            string commandText = String.Format(command, parameters);

            if (SendCopyData(_DirectorWindow, this.Handle, commandText, 0) != 0)
                throw new Exception("Failed to send command to SciTE.");

            return response;
        }

        public string Escape (string text) {
            return String.Format(
                "{0}",
                text.Replace("\\", "\\\\").Replace("\n", "\\n").Replace("\t", "\\t").Replace("\r", "\\r").Replace("\"", "\\\"")
            );
        }

        public void OpenFile (string filename) {
            SendCommand("open:{0}", Escape(filename));
        }

        public void OpenFile (string filename, long initialLineNumber) {
            OpenFile(filename);
            JumpToLine(initialLineNumber);
        }

        public void JumpToLine (long lineNumber) {
            SendCommand("goto:{0},{1}", lineNumber, 0);
        }

        public void FindText (string text) {
            SendCommand("find:{0}", Escape(text));
        }

        public static bool LocateExecutable (ref string filename) {
            return Director.TryLocateExecutable(Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), @"SciTE\SciTE.exe"
                ), ref filename
            );
        }
    }
}
