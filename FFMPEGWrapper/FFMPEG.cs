using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace FFMPEGWrapper
{
    public class FFMPEG
    {
        private int _bitrate = 128;
        private int _sampling = 44100;
        private Channels _channels = FFMPEGWrapper.Channels.Stereo;
        private EncodingFormat _encodingFormat = FFMPEGWrapper.EncodingFormat.MP3;
        private string _destination;
        private string _device;
        private TimeSpan _eventReportInterval = TimeSpan.FromMilliseconds(500);
        private string _path;
        private static readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1);
        private event Action<FFMPEGEvent> _onEvent;
        private readonly Regex _ffmpegRegexLevels = new Regex(@"\[Parsed_ametadata[\S\ ]*\] lavfi\.astats\.Overall\.RMS_level=(?<db1>-?\d*\.?\d*)", RegexOptions.Compiled);
        private readonly Regex _ffmpegRegexTime = new Regex(@"size=\s*(?<size>\d*)(?<size_unit>kB) time=(?<time>\d\d:\d\d:\d\d\.?\d?\d?) bitrate=\s*(?<bitrate>\d*\.?\d*?)kbits\/s", RegexOptions.Compiled);
        private Process _process;

        public FFMPEG()
        {
            AppDomain root = AppDomain.CurrentDomain;
            root.ProcessExit += _DomainUnload;
        }

        #region _StartFFMpeg
        private void _StartFFMpeg(string arguments, bool waitForExit, Action<string> onData)
        {
            _process = new Process
            {
                EnableRaisingEvents = true,
                StartInfo = new ProcessStartInfo
                {
                    UseShellExecute = false,
                    FileName = _path,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    Arguments = $"-hide_banner {arguments}",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    RedirectStandardInput = true,
                    StandardErrorEncoding = new UTF8Encoding(),
                    StandardOutputEncoding = new UTF8Encoding()
                }
            };

            _process.OutputDataReceived += (sender, e) =>
            {
                onData(e.Data);
                Debug.WriteLine(e.Data);
            };

            _process.ErrorDataReceived += (sender, e) =>
            {
                onData(e.Data);
                Debug.WriteLine(e.Data);
            };

            _process.Start();
            _process.BeginErrorReadLine();
            _process.BeginOutputReadLine();

            if (waitForExit)
                _process.WaitForExit();
        }
        #endregion

        public FFMPEG FFMPEGPath(string path)
        {
            _path = path;
            return this;
        }

        public FFMPEG Bitrate(int bitrate)
        {
            _bitrate = bitrate;
            return this;
        }

        public FFMPEG Sampling(int sampling)
        {
            _sampling = sampling;
            return this;
        }

        public FFMPEG Channels(Channels channels)
        {
            _channels = channels;
            return this;
        }
        public FFMPEG EncodingFormat(EncodingFormat encodingFormat)
        {
            _encodingFormat = encodingFormat;
            return this;
        }

        public FFMPEG Destination(string destination)
        {
            _destination = destination;
            return this;
        }


        public FFMPEG EventReportInterval(TimeSpan eventReportInterval)
        {
            _eventReportInterval = eventReportInterval;
            return this;
        }

        public FFMPEG CaptureDevice(string device)
        {
            _device = device;
            return this;
        }
        public FFMPEG OnEvent(Action<FFMPEGEvent> onEvent)
        {
            _onEvent = onEvent;
            return this;
        }

        /// <summary>
        /// Enumerate DirectShow capture devices
        /// </summary>
        #region EnumerateCaptureDevices
        public List<AudioDevice> EnumerateCaptureDevices()
        {
            string defaultCaptureDevice = null;

            try
            {
                WINMMUtils winmmUtils = new WINMMUtils();
                defaultCaptureDevice = winmmUtils.GetDefaultDevice();
            }
            catch (Exception e)
            {
                // ignored
            }

            List<AudioDevice> availableDevices = new List<AudioDevice>();
            StringBuilder sb = new StringBuilder();

            _StartFFMpeg("-list_devices true -f dshow -i dummy",
                      true,
                      output =>
                      {
                          if (string.IsNullOrEmpty(output))
                              return;
                          sb.AppendLine(output);
                      });

            string data = sb.ToString();

            bool isAudioDevice = false;
            foreach (string line in data.Split(new[] { Environment.NewLine }, StringSplitOptions.None))
            {
                string trimmed = line.Trim();
                if (trimmed.Contains("DirectShow audio devices"))
                {
                    isAudioDevice = true;
                }

                if (isAudioDevice)
                {
                    if (trimmed.Contains("Alternative name"))
                        continue;

                    Regex deviceNameRegex = new Regex(@"\[dshow @ \S*\]  ""(?<device_name>[\S*\s]*)""", RegexOptions.Compiled);
                    MatchCollection matchDevices = deviceNameRegex.Matches(line);

                    if (matchDevices.Count == 1)
                    {
                        string deviceName = matchDevices[0].Groups["device_name"].Value;

                        // defaultCaptureDevice contains only first 31 characters, so not using full equility
                        bool isDefault = !string.IsNullOrEmpty(defaultCaptureDevice)
                                         && deviceName.ToUpper().StartsWith(defaultCaptureDevice.ToUpper());

                        AudioDevice item = new AudioDevice(DeviceType.Capture, deviceName, isDefault);
                        availableDevices.Add(item);
                    }
                }
            }

            return availableDevices;
        }
        #endregion

        #region CaptureStart
        public void CaptureStart()
        {
            if (string.IsNullOrEmpty(_destination))
                throw new Exception("No destination specified");

            _TerminateAllFFMPEGInstances();

            CancellationTokenSource cts = new CancellationTokenSource();
            ThreadPool.QueueUserWorkItem(task =>
            {
                CancellationToken token = (CancellationToken)task;
                if (token.IsCancellationRequested)
                    return;

                DateTime eventPublished = DateTime.Now;

                FFMPEGEvent ffmpegEvent = new FFMPEGEvent();
                decimal dbfsMax = decimal.MaxValue;
                decimal silenceLevel = 0;

                string encoder;
                bool audioOnly = false;
                switch (_encodingFormat)
                {
                    case FFMPEGWrapper.EncodingFormat.MP3:
                        encoder = "libmp3lame";
                        audioOnly = true;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                string cmd = $"-y -f dshow -i audio=\"{_device}\" " +
                             $"-af astats=metadata=1:reset=1,ametadata=print:key=lavfi.astats.Overall.RMS_level " +
                             $"{(audioOnly ? "-vn" : null)} -ar {_sampling} -ac {(int)_channels} -b:a {_bitrate}k -c:a {encoder} {_destination}";

                _StartFFMpeg(cmd,
                    false,
                    output =>
                    {
                        if (string.IsNullOrEmpty(output))
                            return;

                        _semaphoreSlim.Wait(token);

                        try
                        {
                            if (output.StartsWith("[Parsed_"))
                            {
                                MatchCollection matchLevels = _ffmpegRegexLevels.Matches(output);

                                if (matchLevels.Count == 1)
                                {
                                    decimal db1 = _ParseDecimal(matchLevels[0].Groups["db1"].Value);
                                    decimal currentDBFS = Math.Abs(db1);

                                    if (currentDBFS < dbfsMax)
                                    {
                                        dbfsMax = currentDBFS;
                                    }

                                    if (currentDBFS > silenceLevel)
                                        silenceLevel = currentDBFS;

                                    decimal level = silenceLevel - currentDBFS;
                                    decimal levelProc = Math.Round(level * 100 / dbfsMax, 4);

                                    ffmpegEvent.CurrentAudioLevel = levelProc > 100 ? 100 : levelProc;

                                }
                            }

                            if (output.StartsWith("size="))
                            {
                                MatchCollection matchTime = _ffmpegRegexTime.Matches(output);

                                if (matchTime.Count == 1)
                                {
                                    int size = int.Parse(matchTime[0].Groups["size"].Value);
                                    ffmpegEvent.Size = size * 1024;
                                    string timeSpan = matchTime[0].Groups["time"].Value;
                                    ffmpegEvent.Bitrate = _ParseDecimal(matchTime[0].Groups["bitrate"].Value);
                                    ffmpegEvent.Duration = TimeSpan.ParseExact(timeSpan, "c", CultureInfo.InvariantCulture);
                                }
                            }

                            if (DateTime.Now.Subtract(eventPublished) > _eventReportInterval
                                && ffmpegEvent.Duration != default)
                            {
                                _onEvent?.Invoke(ffmpegEvent);
                                eventPublished = DateTime.Now;
                            }
                        }
                        finally
                        {
                            _semaphoreSlim.Release();
                        }

                    });
            }, cts.Token);

        }
        #endregion

        #region CaptureStop
        public void CaptureStop()
        {

            // Try to close gracefully by sending SIGTERM
            try
            {
                ConsoleUtils.SendSigterm(_process);
            }
            catch
            {
                // ignored
            }

            _TerminateAllFFMPEGInstances();
        }
        #endregion

        #region _ParseDecimal
        private static decimal _ParseDecimal(string str)
        {
            string tobeReplaced = ",";
            if (Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator == ",")
                tobeReplaced = ".";

            str = str.Replace(tobeReplaced, Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator);
            return decimal.Parse(str);

        }
        #endregion

        #region _CloseAllInstances
        private void _TerminateAllFFMPEGInstances()
        {
            foreach (Process p in Process.GetProcesses())
            {
                try
                {
                    if (p.ProcessName.Equals(Path.GetFileNameWithoutExtension(_path), StringComparison.CurrentCultureIgnoreCase)
                        && p.MainModule != null
                        && p.MainModule.FileName.Equals(_path, StringComparison.InvariantCultureIgnoreCase))
                    {
                        p.Kill();
                    }
                }
                catch
                {
                    // ignored
                }

            }
        }
        #endregion

        #region _DomainUnload
        private void _DomainUnload(object sender, EventArgs e)
        {
            _TerminateAllFFMPEGInstances();
        }
        #endregion

        #region ConcatFiles
        public void ConcatFiles(string outputFile, params string[] files)
        {
            string filesJoined = string.Join("|", files);
            string cmd = $"-i \"concat:{filesJoined}\" -acodec copy -vcodec copy \"{outputFile}\"";

            _StartFFMpeg(cmd,
                true,
                output =>
                {
                    if (string.IsNullOrEmpty(output))
                        return;
                });
        }
        #endregion

        #region GetDuration
        public int GetDuration(string fileName)
        {
            string cmd = $"-hide_banner -i {fileName} -f null";
            int sec = 0;
            _StartFFMpeg(cmd,
                true,
                output =>
                {
                    if (string.IsNullOrEmpty(output))
                        return;

                    if (output.TrimStart().StartsWith("Duration"))
                    {
                        int iStart = output.IndexOf(":");
                        int iEnd = output.IndexOf(",");
                        string outputStr = output.TrimStart().Substring(iStart, iEnd - iStart - 2);
                        TimeSpan ts = TimeSpan.Parse(outputStr);
                        sec = ts.Seconds;
                    }
                });
            return sec;
        }
        #endregion

    }
}
