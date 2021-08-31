using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using FFMPEGWrapper;

namespace DeloAudioRecorder
{
    public partial class MainForm : Form
    {
        private List<AudioDevice> _availableDevices;
        private AudioDevice _selectedRecordingDevice;
        private FFMPEG _ffmpeg;
        private string _ffmpegPath;

        #region ctor
        public MainForm()
        {
            InitializeComponent();
        }
        #endregion

        #region FormLoad
        private void FormLoad(object sender, EventArgs e)
        {
            _ffmpegPath = _GetFFMpegPath();
            _ffmpeg = new FFMPEG();
            _availableDevices = _ffmpeg
                                    .FFMPEGPath(_ffmpegPath)
                                    .EnumerateCaptureDevices();

            m_recordingDevicesList.DisplayMember = nameof(AudioDevice.Name);

            foreach (AudioDevice device in _availableDevices)
                m_recordingDevicesList.Items.Add(device);

            AudioDevice defaultDevice = _availableDevices.FirstOrDefault(x => x.IsDefault);
            if (defaultDevice != null)
            {
                foreach (AudioDevice cbi in m_recordingDevicesList.Items.Cast<AudioDevice>())
                {
                    if (cbi.Name.Equals(defaultDevice.Name))
                    {
                        m_recordingDevicesList.SelectedItem = cbi;
                        break;
                    }
                }
            }

            _SetInitialState();
        }
        #endregion

        private void _SetInitialState()
        {
            m_startRecButton.Enabled = m_stopRecButton.Enabled = m_recordProgressPanel.Enabled = false;
            m_bitrateLab.Text = string.Empty;
            m_AudioLevelPrg.Value = 0;

            if (m_recordingDevicesList.SelectedItem != null)
                m_startRecButton.Enabled = true;

            m_openFolderButton.Enabled = !string.IsNullOrEmpty(_ffmpeg._destination);
        }

        #region m_startRecButton_Click
        private void m_startRecButton_Click(object sender, EventArgs e)
        {
            if (_selectedRecordingDevice == null)
            {
                MessageBox.Show("Capture device not selected or not exists", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            m_startRecButton.Enabled = false;
            m_stopRecButton.Enabled = true;

            m_AudioLevelPrg.Minimum = 0;
            m_AudioLevelPrg.Maximum = 100000;

            _ffmpeg
                .FFMPEGPath(_ffmpegPath)
                .Bitrate(128)
                .Sampling(44100)
                .Channels(Channels.Mono)
                .CaptureDevice(_selectedRecordingDevice.Name)
                .Destination(Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.mp3"))
                .OnEvent(data =>
                {
                    if (!IsDisposed)
                        Invoke(new Action<FFMPEGEvent>(eventData =>
                        {
                            m_AudioLevelPrg.Value = (int)(data.CurrentLevel * 1000);
                            m_bitrateLab.Text = $"Duration: {eventData.Duration:g}; Size: {eventData.Size / 1024} KB; Bitrate: {eventData.Bitrate}kb/s";
                        }), data);

                })
                .CaptureStart();

            _ffmpeg._destination = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.mp3");
        }
        #endregion

        #region _GetFFMpegPath
        private static string _GetFFMpegPath()
        {
            string ffmpegPath = Path.Combine(Application.StartupPath, "ffmpeg.exe");
            if (!File.Exists(ffmpegPath))
            {
                MessageBox.Show("Cant find ffmpeg file", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw new FileNotFoundException("FFMPEG");
            }

            return ffmpegPath;
        }
        #endregion

        #region m_recordingDevicesList_SelectedIndexChanged
        private void m_recordingDevicesList_SelectedIndexChanged(object sender, EventArgs e)
        {
            _selectedRecordingDevice = ((ComboBox)sender).SelectedItem as AudioDevice;
        }
        #endregion

        #region m_stopRecButton_Click
        private void m_stopRecButton_Click(object sender, EventArgs e)
        {
            _ffmpeg.CaptureStop();
            _SetInitialState();
        }
        #endregion

        #region m_openFolderButton_Click
        private void m_openFolderButton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_ffmpeg._destination))
            {
                string dir = Path.GetDirectoryName(_ffmpeg._destination);
                if (Directory.Exists(dir))
                {
                    Process.Start("explorer.exe", Path.GetDirectoryName(_ffmpeg._destination));
                    return;
                }
            }

            MessageBox.Show("FileName not defined", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        #endregion

    }
}
