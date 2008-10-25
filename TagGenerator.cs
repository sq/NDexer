using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Squared.Task;
using System.Threading;

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
        public string ApplicationPath;
        public string Arguments;

        public TagGenerator (string app, string arguments) {
            ApplicationPath = app;
            Arguments = arguments;
        }

        public IEnumerator<object> GenerateTags (BlockingQueue<string> sourceFilenames, BlockingQueue<string> outputLines, Func<string, Future> onNextFile) {
            var info = new ProcessStartInfo(ApplicationPath, Arguments);
            info.UseShellExecute = false;
            info.RedirectStandardOutput = true;
            info.RedirectStandardInput = true;
            info.CreateNoWindow = true;
            info.WindowStyle = ProcessWindowStyle.Hidden;

            using (var process = new ChildProcess(Process.Start(info)))
            using (var outputAdapter = new StreamDataAdapter(process.StandardOutput, false))
            using (var inputAdapter = new StreamDataAdapter(process.StandardInput, false))
            using (var stdout = new AsyncTextReader(outputAdapter, Encoding.ASCII))
            using (var stdin = new AsyncTextWriter(inputAdapter, Encoding.ASCII)) {
                while (true) {
                    string filename = null;
                    {
                        var f = sourceFilenames.Dequeue();
                        yield return f;
                        filename = (string)f.Result;
                    }

                    Console.WriteLine(filename);

                    yield return onNextFile(filename);

                    yield return stdin.WriteLine(filename);
                    process.StandardInput.Flush();

                    while (true) {
                        var f = stdout.ReadLine();
                        yield return f;
                        string currentLine = f.Result as string;

                        if ((currentLine == null) || (currentLine == "[[<>]]")) {
                            break;
                        } else {
                            outputLines.Enqueue(currentLine);
                        }
                    }
                }
            }
        }
    }
}
