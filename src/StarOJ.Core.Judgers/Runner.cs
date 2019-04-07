using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StarOJ.Core.Judgers
{
    internal enum RunnerState
    {
        Pending,
        Running,
        Ended,
        OutOfMemory,
        OutOfTime,
    }

    internal class Runner : IDisposable
    {
        long MaximumPagedMemorySize64 { get; set; }

        long MaximumPeakPagedMemorySize64 { get; set; }

        ProcessStartInfo StartInfo { get; set; }

        public string Output { get; private set; }

        public string Error { get; private set; }

        public TextReader Input { get; set; }

        public int DataCollectSpeed { get; set; } = 5;


        /// <summary>
        /// True if read all input before write to process
        /// </summary>
        public bool InputAll { get; set; } = true;

        public long? MemoryLimit { get; set; }

        public TimeSpan TimeLimit { get; set; }

        public long MaximumMemory => Math.Max(MaximumPagedMemorySize64, MaximumPeakPagedMemorySize64);

        public TimeSpan RunningTime => EndTime - StartTime;

        public DateTimeOffset StartTime { get; private set; }

        public DateTimeOffset EndTime { get; private set; }

        public int ExitCode { get; private set; }

        /// <summary>
        /// Gets if the app is running
        /// </summary>
        public RunnerState State { get; private set; }

        /// <summary>
        /// Gets the internal process
        /// </summary>
        public Process Process { get; private set; }

        BackgroundWorker bwMemory;

        public Runner(ProcessStartInfo startInfo)
        {
            StartInfo = startInfo;
            State = RunnerState.Pending;
        }

        public async Task Run()
        {
            StartInfo.UseShellExecute = false;
            StartInfo.RedirectStandardError = true;
            StartInfo.RedirectStandardInput = true;
            StartInfo.RedirectStandardOutput = true;
            // StartInfo.StandardOutputEncoding = TextIO.UTF8WithoutBOM;
            // StartInfo.StandardErrorEncoding = TextIO.UTF8WithoutBOM;

            Process = new Process
            {
                StartInfo = StartInfo
            };

            Process.EnableRaisingEvents = true;
            if (bwMemory != null) bwMemory.Dispose();
            bwMemory = new BackgroundWorker { WorkerSupportsCancellation = true };
            bwMemory.DoWork += (sender, e) =>
            {
                MaximumPagedMemorySize64 = MaximumPeakPagedMemorySize64 = 0;
                while (State == RunnerState.Running && !e.Cancel)
                {
                    try
                    {
                        MaximumPagedMemorySize64 = Math.Max(MaximumPagedMemorySize64, Process.PagedMemorySize64);
                        MaximumPeakPagedMemorySize64 = Math.Max(MaximumPeakPagedMemorySize64, Process.PeakPagedMemorySize64);
                        if (MemoryLimit.HasValue && MaximumMemory > MemoryLimit)
                        {
                            State = RunnerState.OutOfMemory;
                            Process.Kill();
                        }
                        Thread.Sleep(DataCollectSpeed);
                    }
                    catch
                    {

                    }
                }
            };
            Process.Exited += Process_Exited;

            State = RunnerState.Running;

            bwMemory.RunWorkerAsync();

            Process.Start();

            StartTime = DateTimeOffset.Now;

            if (Input != null)
            {
                if (InputAll)
                {
                    string input = await Input.ReadToEndAsync();
                    await Process.StandardInput.WriteAsync(input);
                    StartTime = DateTimeOffset.Now;
                }
                else
                {
                    while (Input.Peek() != -1)
                    {
                        await Process.StandardInput.WriteLineAsync(await Input.ReadLineAsync());
                    }
                    StartTime = DateTimeOffset.Now;
                }
            }
            Process.StandardInput.Close();

            if (Process.WaitForExit((int)Math.Ceiling(TimeLimit.TotalMilliseconds)))
            {
                State = RunnerState.Ended;
            }
            else
            {
                State = RunnerState.OutOfTime;
                Process.Kill();
                Process.WaitForExit();
            }
        }

        private void Process_Exited(object sender, EventArgs e)
        {
            try
            {
                ExitCode = Process.ExitCode;
                if (bwMemory?.IsBusy == true) bwMemory.CancelAsync();
                Process.WaitForExit();
            }
            catch
            {

            }

            EndTime = DateTimeOffset.Now;

            StringBuilder output = new StringBuilder();

            try
            {
                while (!Process.StandardOutput.EndOfStream)
                    output.AppendLine(Process.StandardOutput.ReadLine());
            }
            catch { }
            Output = output.ToString();

            StringBuilder error = new StringBuilder();
            try
            {
                while (!Process.StandardError.EndOfStream)
                    error.AppendLine(Process.StandardError.ReadLine());
            }
            catch { }
            Error = error.ToString();
        }


        /// <summary>
        /// Dispose resources
        /// </summary>
        public void Dispose()
        {
            ((IDisposable)bwMemory)?.Dispose();
            ((IDisposable)Process)?.Dispose();
        }
    }
}
