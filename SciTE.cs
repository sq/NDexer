using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Squared.Task;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Ndexer {
    public class SciTENotRunningException : Exception {
        public SciTENotRunningException ()
            : base("Unable to find a running instance of SciTE") {
        }
    }

    public class SciTEDirector : NativeWindow, IDisposable {
        private const int WM_COPYDATA = 0x4A;
        private const int WS_EX_NOACTIVATE = 0x08000000;

        [StructLayout(LayoutKind.Sequential)]
        private struct COPYDATASTRUCT {
            public UInt32 ExtraData;
            public int NumCharacters;
            public IntPtr TextPointer;
        }

        [DllImport("User32.dll", EntryPoint="SendMessage", SetLastError=true, CharSet=CharSet.Ansi)]
        private static extern int SendMessage (IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll", SetLastError=true, CharSet=CharSet.Ansi)]
        private static extern IntPtr FindWindow (string lpClassName, string lpWindowName);
        [DllImport("user32.dll", SetLastError=true)]
        private static extern int SetForegroundWindow (IntPtr hWnd);

        private IntPtr _DirectorWindow = IntPtr.Zero;
        private IntPtr _EditorWindow = IntPtr.Zero;
        private Future _ResponseWaiter = null;

        public SciTEDirector () {
            _DirectorWindow = FindWindow("DirectorExtension", "DirectorExtension");
            _EditorWindow = FindWindow("SciTEWindow", null);

            if (_DirectorWindow == IntPtr.Zero || _EditorWindow == IntPtr.Zero)
                throw new SciTENotRunningException();

            var cp = new CreateParams {
                Caption = "NDexer.SciTEDirector.ListenerWindow",
                X = 0,
                Y = 0,
                Width = 0,
                Height = 0,
                Style = 0,
                ExStyle = WS_EX_NOACTIVATE,
                Parent = new IntPtr(-3)
            };
            CreateHandle(cp);
        }

        internal static int SendCopyData (IntPtr recipientWindow, IntPtr sendingWindow, string data, UInt32 extraData) {
            var cds = new COPYDATASTRUCT();
            cds.ExtraData = extraData;
            cds.NumCharacters = data.Length;
            cds.TextPointer = Marshal.StringToHGlobalAnsi(data);
            var gch = GCHandle.Alloc(cds, GCHandleType.Pinned);
            try {
                return SendMessage(recipientWindow, WM_COPYDATA, sendingWindow, gch.AddrOfPinnedObject());
            } finally {
                gch.Free();
                Marshal.FreeHGlobal(cds.TextPointer);
            }
        }

        public Future SendCommand (string command, params object[] parameters) {
            var f = new Future();
            _ResponseWaiter = f;

            string commandText = String.Format(command, parameters);

            if (SendCopyData(_DirectorWindow, this.Handle, commandText, 0) != 0)
                throw new Exception("Failed to send command to SciTE.");

            return f;
        }

        protected override void WndProc (ref Message m) {
            if ((m.Msg == WM_COPYDATA) && (_ResponseWaiter != null)) {
                var f = _ResponseWaiter;
                _ResponseWaiter = null;

                var cds = (COPYDATASTRUCT)Marshal.PtrToStructure(m.LParam, typeof(COPYDATASTRUCT));
                var data = Marshal.PtrToStringAnsi(cds.TextPointer, cds.NumCharacters);

                f.SetResult(data, null);
                m.Result = new IntPtr(1);
                return;
            }

            base.WndProc(ref m);
        }

        public void BringToFront () {
            SetForegroundWindow(_EditorWindow);   
        }

        public void Dispose () {
            base.DestroyHandle();
        }

        public string Escape (string text) {
            return String.Format(
                "{0}",
                text.Replace("\\", "\\\\").Replace("\n", "\\n").Replace("\t", "\\t").Replace("\r", "\\r").Replace("\"", "\\\"")
            );
        }
    }
}
