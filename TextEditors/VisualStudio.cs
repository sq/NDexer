using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Squared.Task;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Diagnostics;
using EnvDTE;

namespace Ndexer {
    public class VisualStudioDirector : Director, IBasicDirector {
        const string DTEProgID = "VisualStudio.DTE";

        public VisualStudioDirector (string applicationPath)
            : base(applicationPath) {
            var dteType = typeof(DTE);
            DTE dte = GetRunningInstance();
            if (dte == null)
                Launch("");
        }

        internal DTE GetRunningInstance () {
            try {
                return (DTE)System.Runtime.InteropServices.Marshal.GetActiveObject(DTEProgID);
            } catch (COMException ce) {
                if (ce.ErrorCode == -2147221021) {
                    // Unavailable
                } else {
                    throw;
                }
            }
            return null;
        }

        protected override IntPtr FindDirectorWindow () {
            return IntPtr.Zero;
        }

        protected override IntPtr FindEditorWindow () {
            return IntPtr.Zero;
        }

        public void Launch (string arguments) {
            var info = new ProcessStartInfo(_ApplicationPath, arguments);
            var process = System.Diagnostics.Process.Start(info);
            process.WaitForInputIdle();
            _EditorWindow = FindEditorWindow();
            process.Dispose();
        }

        public void OpenFile (string filename) {
            GetRunningInstance().ExecuteCommand("File.OpenFile", filename);
        }

        public void OpenFile (string filename, long initialLineNumber) {
            OpenFile(filename);
            GetRunningInstance().ExecuteCommand("Edit.GoTo", initialLineNumber.ToString());
        }
    }
}
