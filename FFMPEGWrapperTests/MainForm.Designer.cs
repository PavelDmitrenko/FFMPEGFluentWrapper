
namespace DeloAudioRecorder
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.m_startRecButton = new System.Windows.Forms.Button();
            this.m_stopRecButton = new System.Windows.Forms.Button();
            this.m_recordingDevicesList = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.m_AudioLevelPrg = new System.Windows.Forms.ProgressBar();
            this.m_recordProgressPanel = new System.Windows.Forms.Panel();
            this.m_bitrateLab = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.m_openFolderButton = new System.Windows.Forms.Button();
            this.panel3 = new System.Windows.Forms.Panel();
            this.m_recordProgressPanel.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_startRecButton
            // 
            this.m_startRecButton.Location = new System.Drawing.Point(14, 12);
            this.m_startRecButton.Name = "m_startRecButton";
            this.m_startRecButton.Size = new System.Drawing.Size(90, 23);
            this.m_startRecButton.TabIndex = 0;
            this.m_startRecButton.Text = "Start recording";
            this.m_startRecButton.UseVisualStyleBackColor = true;
            this.m_startRecButton.Click += new System.EventHandler(this.m_startRecButton_Click);
            // 
            // m_stopRecButton
            // 
            this.m_stopRecButton.Location = new System.Drawing.Point(113, 12);
            this.m_stopRecButton.Name = "m_stopRecButton";
            this.m_stopRecButton.Size = new System.Drawing.Size(93, 23);
            this.m_stopRecButton.TabIndex = 1;
            this.m_stopRecButton.Text = "Stop recording";
            this.m_stopRecButton.UseVisualStyleBackColor = true;
            this.m_stopRecButton.Click += new System.EventHandler(this.m_stopRecButton_Click);
            // 
            // m_recordingDevicesList
            // 
            this.m_recordingDevicesList.FormattingEnabled = true;
            this.m_recordingDevicesList.Location = new System.Drawing.Point(9, 27);
            this.m_recordingDevicesList.Name = "m_recordingDevicesList";
            this.m_recordingDevicesList.Size = new System.Drawing.Size(309, 21);
            this.m_recordingDevicesList.TabIndex = 2;
            this.m_recordingDevicesList.SelectedIndexChanged += new System.EventHandler(this.m_recordingDevicesList_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(84, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Capture devices";
            // 
            // m_AudioLevelPrg
            // 
            this.m_AudioLevelPrg.Location = new System.Drawing.Point(9, 5);
            this.m_AudioLevelPrg.Name = "m_AudioLevelPrg";
            this.m_AudioLevelPrg.Size = new System.Drawing.Size(309, 11);
            this.m_AudioLevelPrg.TabIndex = 4;
            // 
            // m_recordProgressPanel
            // 
            this.m_recordProgressPanel.Controls.Add(this.m_bitrateLab);
            this.m_recordProgressPanel.Controls.Add(this.m_AudioLevelPrg);
            this.m_recordProgressPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.m_recordProgressPanel.Location = new System.Drawing.Point(0, 63);
            this.m_recordProgressPanel.Name = "m_recordProgressPanel";
            this.m_recordProgressPanel.Size = new System.Drawing.Size(336, 45);
            this.m_recordProgressPanel.TabIndex = 5;
            // 
            // m_bitrateLab
            // 
            this.m_bitrateLab.AutoSize = true;
            this.m_bitrateLab.Location = new System.Drawing.Point(8, 19);
            this.m_bitrateLab.Name = "m_bitrateLab";
            this.m_bitrateLab.Size = new System.Drawing.Size(68, 13);
            this.m_bitrateLab.TabIndex = 5;
            this.m_bitrateLab.Text = "m_bitrateLab";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.m_openFolderButton);
            this.panel2.Controls.Add(this.m_startRecButton);
            this.panel2.Controls.Add(this.m_stopRecButton);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 114);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(336, 48);
            this.panel2.TabIndex = 6;
            // 
            // m_openFolderButton
            // 
            this.m_openFolderButton.Location = new System.Drawing.Point(256, 13);
            this.m_openFolderButton.Name = "m_openFolderButton";
            this.m_openFolderButton.Size = new System.Drawing.Size(70, 23);
            this.m_openFolderButton.TabIndex = 2;
            this.m_openFolderButton.Text = "Open folder";
            this.m_openFolderButton.UseVisualStyleBackColor = true;
            this.m_openFolderButton.Click += new System.EventHandler(this.m_openFolderButton_Click);
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.label1);
            this.panel3.Controls.Add(this.m_recordingDevicesList);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(336, 63);
            this.panel3.TabIndex = 7;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(336, 162);
            this.Controls.Add(this.m_recordProgressPanel);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FFMPEG Wrapper Test";
            this.Load += new System.EventHandler(this.FormLoad);
            this.m_recordProgressPanel.ResumeLayout(false);
            this.m_recordProgressPanel.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button m_startRecButton;
        private System.Windows.Forms.Button m_stopRecButton;
        private System.Windows.Forms.ComboBox m_recordingDevicesList;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ProgressBar m_AudioLevelPrg;
        private System.Windows.Forms.Panel m_recordProgressPanel;
        private System.Windows.Forms.Label m_bitrateLab;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button m_openFolderButton;
    }
}

