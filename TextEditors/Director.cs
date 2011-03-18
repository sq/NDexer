using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Squared.Task;
using System.IO;

namespace Ndexer {
#if !MONO
    public delegate bool LocateExecutableHandler (ref string filename);

    public interface IBasicDirector : IDisposable {
        void OpenFile (string filename);
        void OpenFile (string filename, long initialLineNumber);
        void BringToFront ();
    }

    public interface IAdvancedDirector {
        void JumpToLine (long lineNumber);
        void FindText (string text);
    }

    public abstract class Director : NativeWindow, IDisposable {
        private const int WM_COPYDATA = 0x4A;
        private const int WS_EX_NOACTIVATE = 0x08000000;

        [StructLayout(LayoutKind.Sequential)]
        private struct COPYDATASTRUCT {
            public UInt32 ExtraData;
            public int NumCharacters;
            public IntPtr TextPointer;
        }

        [DllImport("User32.dll", EntryPoint = "SendMessage", SetLastError = true, CharSet = CharSet.Ansi)]
        private static extern int SendMessage (IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Ansi)]
        protected static extern IntPtr FindWindow (string lpClassName, string lpWindowName);
        [DllImport("user32.dll", SetLastError = true)]
        protected static extern int SetForegroundWindow (IntPtr hWnd);

        protected IntPtr _DirectorWindow = IntPtr.Zero;
        protected IntPtr _EditorWindow = IntPtr.Zero;
        protected string _ApplicationPath = null;

        private IFuture _CopyDataWaiter = null;

        protected abstract IntPtr FindDirectorWindow ();
        protected abstract IntPtr FindEditorWindow ();

        protected Director (string applicationPath)
            : base() {
            _ApplicationPath = applicationPath;

            _DirectorWindow = FindDirectorWindow();
            _EditorWindow = FindEditorWindow();

            var cp = new CreateParams {
                Caption = "NDexer.Director.ListenerWindow",
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

        protected override void WndProc (ref Message m) {
            if ((m.Msg == WM_COPYDATA) && (_CopyDataWaiter != null)) {
                var f = _CopyDataWaiter;
                _CopyDataWaiter = null;

                var cds = (COPYDATASTRUCT)Marshal.PtrToStructure(m.LParam, typeof(COPYDATASTRUCT));
                var data = Marshal.PtrToStringAnsi(cds.TextPointer, cds.NumCharacters);

                f.SetResult(data, null);
                m.Result = new IntPtr(1);
                return;
            }

            base.WndProc(ref m);
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

        public IFuture WaitForCopyData () {
            _CopyDataWaiter = new Future<object>();
            return _CopyDataWaiter;
        }

        public void BringToFront () {
            SetForegroundWindow(_EditorWindow);
        }

        public void Dispose () {
            base.DestroyHandle();
        }

        protected static bool TryLocateExecutable (string candidatePath, ref string result) {
            if (File.Exists(candidatePath)) {
                result = candidatePath;
                return true;
            }

            return false;
        }
    }
#endif
}
