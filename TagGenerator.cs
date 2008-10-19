using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Squared.Task;
using System.Threading;

namespace Ndexer {
    public class TagGenerator {
        public string ApplicationPath;
        public string Arguments;

        public TagGenerator (string app, string arguments) {
            ApplicationPath = app;
            Arguments = arguments;
        }

        public IEnumerator<string> GenerateTags (IEnumerable<string> sourceFilenames, int numFiles, IndexingStatusDialog statusDialog) {
            var info = new ProcessStartInfo(ApplicationPath, Arguments);
            info.UseShellExecute = false;
            info.RedirectStandardOutput = true;
            info.RedirectStandardInput = true;
            info.CreateNoWindow = true;
            info.WindowStyle = ProcessWindowStyle.Hidden;
            var process = Process.Start(info);
            try {
                var outputAdapter = new StreamDataAdapter(process.StandardOutput.BaseStream, false);
                var inputAdapter = new StreamDataAdapter(process.StandardInput.BaseStream, false);
                var stdout = new AsyncTextReader(outputAdapter, Encoding.ASCII);
                var stdin = new AsyncTextWriter(inputAdapter, Encoding.ASCII);

                int currentFile = 0;

                var enumerator = sourceFilenames.GetEnumerator();

                OnComplete[] oc = new OnComplete[1];
                OnComplete filenameWriter = (f, r, e) => {
                    if (enumerator.MoveNext()) {
                        process.StandardInput.BaseStream.Flush();
                        string filename = enumerator.Current;
                        ThreadPool.QueueUserWorkItem((_) => {
                            var nextFuture = stdin.WriteLine(filename);
                            nextFuture.RegisterOnComplete(oc[0]);
                        });
                    } else {
                        process.StandardInput.BaseStream.Close();
                    }
                };
                oc[0] = filenameWriter;
                filenameWriter(null, null, null);

                long lastUpdate = DateTime.Now.Ticks;
                long updateStep = TimeSpan.FromSeconds(0.1).Ticks;

                string currentLine = null;
                while (true) {
                    long now = DateTime.Now.Ticks;
                    if ((now - lastUpdate) > updateStep) {
                        lastUpdate = now;
                        float progressValue = currentFile / (float)numFiles;
                        statusDialog.SetStatus(
                            String.Format("Updating index... {0}/{1}", currentFile, numFiles),
                            progressValue
                        );
                    }
                    var f = stdout.ReadLine();
                    if (currentLine != null) {
                        if (currentLine == "[[<>]]") {
                            currentFile += 1;
                        }
                        yield return currentLine;
                    }
                    while (!f.Completed)
                        Thread.Sleep(0);
                    currentLine = f.Result as string;
                    if (currentLine == null)
                        break;
                }
            } finally {
                if (!process.HasExited) {
                    process.WaitForExit(1000);
                    process.Kill();
                }
            }
        }

        void process_ErrorDataReceived (object sender, DataReceivedEventArgs e) {
            Console.WriteLine("Error: {0}", e.Data);
        }

        void process_OutputDataReceived (object sender, DataReceivedEventArgs e) {
            Console.WriteLine("Output: {0}", e.Data);
        }
    }
}
