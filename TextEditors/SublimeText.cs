using System;
using System.Diagnostics;
using System.IO;

namespace Ndexer {
#if !MONO
    public class SublimeTextDirector : Director, IBasicDirector {
        public SublimeTextDirector (string applicationPath)
            : base(applicationPath) {
        }

        protected override IntPtr FindDirectorWindow () {
            return IntPtr.Zero;
        }

        protected override IntPtr FindEditorWindow () {
            return FindWindow("PX_WINDOW_CLASS", null);
        }

        public void Launch (string arguments) {
            var info = new ProcessStartInfo(_ApplicationPath, arguments);
            var process = Process.Start(info);
            try {
                process.WaitForInputIdle();
            } catch (Exception exc) {
            }
            _EditorWindow = FindEditorWindow();
            process.Dispose();
        }

        public void OpenFile (string filename) {
            Launch(filename);
        }

        public void OpenFile (string filename, long initialLineNumber) {
			Launch(String.Format("\"{0}\":{1:0}", filename, initialLineNumber));
        }

        public static bool LocateExecutable (ref string filename) {
			return false;
        }
    }
#endif
}
