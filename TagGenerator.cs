using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Squared.Task;
using System.Threading;
using Squared.Task.IO;

namespace Ndexer {
    internal struct ChildProcess : IDisposable {
        public Process Process;

        public ChildProcess (Process process) {
            Process = process;
            Process.PriorityClass = Process.GetCurrentProcess().PriorityClass;
        }

        public System.IO.Stream StandardInput {
            get {
                return Process.StandardInput.BaseStream;
            }
        }

        public System.IO.Stream StandardOutput {
            get {
                return Process.StandardOutput.BaseStream;
            }
        }   

        public void Dispose () {
            if (!Process.HasExited) {
                Process.StandardInput.BaseStream.Close();
                Process.WaitForExit(500);
            }
            if (!Process.HasExited)
                Process.Kill();
        }
    }
    
    public class TagGenerator {
        public const string FilterTerminator = "--[[<<-->>]]--";

        public string ApplicationPath;
        public string Arguments;

        public TagGenerator (string ctags, string languageMap) {
            ApplicationPath = ctags;
            Arguments = String.Format(
                "--filter=yes --filter-terminator={0}\n --fields=+afmikKlnsStz --sort=no --langmap={1}",
                FilterTerminator,
                languageMap
            );
        }

        internal IEnumerator<object> WriteFilenames (IEnumerable<string> filenames, Func<string, Future> writeLine) {
            Future pendingLine = null;

            foreach (string filename in filenames) {
                if (pendingLine != null)
                    yield return pendingLine;

                pendingLine = writeLine(filename);
            }
        }

        public IEnumerator<object> GenerateTags (IEnumerable<string> filenames) {
            var info = new ProcessStartInfo(ApplicationPath, Arguments);
            info.UseShellExecute = false;
            info.RedirectStandardOutput = true;
            info.RedirectStandardInput = true;
            info.CreateNoWindow = true;
            info.WindowStyle = ProcessWindowStyle.Hidden;
            info.ErrorDialog = false;

            var _process = Process.Start(info);
            _process.PriorityClass = ProcessPriorityClass.Idle;

            using (var process = new ChildProcess(_process))
            using (var outputAdapter = new StreamDataAdapter(process.StandardOutput, false))
            using (var inputAdapter = new StreamDataAdapter(process.StandardInput, false))
            using (var stdout = new AsyncTextReader(outputAdapter, Encoding.ASCII))
            using (var stdin = new AsyncTextWriter(inputAdapter, Encoding.ASCII)) {

                var writeLine = (Func<string, Future>)(
                    (fn) => {
                        return Future.RunInThread(
                            (Action<string>)(
                                (fn_) => {
                                    _process.StandardInput.WriteLine(fn_);
                                    _process.StandardInput.Flush();
                                }
                            ),
                            fn
                        );
                    }
                );

                var filenameWriter = Program.Scheduler.Start(
                    WriteFilenames(filenames, writeLine),
                    TaskExecutionPolicy.RunAsBackgroundTask
                );

                var currentFilename = filenames.GetEnumerator();
                if (!currentFilename.MoveNext())
                    throw new InvalidOperationException("Empty list of filenames");

                Func<TagGroup> getNewGroup = () => {
                    long lastWriteTime = System.IO.File.GetLastWriteTimeUtc(currentFilename.Current).ToFileTimeUtc();
                    return new TagGroup(currentFilename.Current, lastWriteTime);
                };

                TagGroup group = null;

                while (!_process.HasExited) {
                    var f = stdout.ReadLine();
                    yield return f;
                    string currentLine = f.Result as string;

                    if (currentLine == null) {
                        break;
                    } else if (currentLine == FilterTerminator) {
                        if (group == null)
                            group = getNewGroup();

                        yield return new NextValue(group);

                        group = null;

                        if (!currentFilename.MoveNext())
                            _process.StandardInput.Close();
                    } else {
                        if (group == null)
                            group = getNewGroup();

                        Tag tag;
                        if (TagReader.ReadTag(currentLine, out tag))
                            group.Add(tag);
                    }
                }

                if (_process.ExitCode != 0) {
                    throw new Exception(
                        String.Format("ctags terminated with exit code {0}", _process.ExitCode)
                    );
                }
            }
        }
    }
}
