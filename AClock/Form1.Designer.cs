namespace AClock
{
    partial class Form1
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.RxTextBox = new System.Windows.Forms.RichTextBox();
            this.InitButton = new System.Windows.Forms.Button();
            this.SendButton = new System.Windows.Forms.Button();
            this.ClearButton = new System.Windows.Forms.Button();
            this.TxTextBox = new System.Windows.Forms.TextBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.PreviousButton = new System.Windows.Forms.Button();
            this.NextButton = new System.Windows.Forms.Button();
            this.VolDownButton = new System.Windows.Forms.Button();
            this.VolUpButton = new System.Windows.Forms.Button();
            this.PlayPauseButton = new System.Windows.Forms.Button();
            this.MuteButton = new System.Windows.Forms.Button();
            this.ReadButton = new System.Windows.Forms.Button();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.serialPort1 = new System.IO.Ports.SerialPort(this.components);
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // RxTextBox
            // 
            this.RxTextBox.Location = new System.Drawing.Point(21, 38);
            this.RxTextBox.Name = "RxTextBox";
            this.RxTextBox.Size = new System.Drawing.Size(260, 91);
            this.RxTextBox.TabIndex = 1;
            this.RxTextBox.Text = "";
            // 
            // InitButton
            // 
            this.InitButton.Location = new System.Drawing.Point(301, 12);
            this.InitButton.Name = "InitButton";
            this.InitButton.Size = new System.Drawing.Size(75, 23);
            this.InitButton.TabIndex = 2;
            this.InitButton.Text = "Open Port";
            this.InitButton.UseVisualStyleBackColor = true;
            this.InitButton.Click += new System.EventHandler(this.InitButton_Click);
            // 
            // SendButton
            // 
            this.SendButton.Enabled = false;
            this.SendButton.Location = new System.Drawing.Point(301, 41);
            this.SendButton.Name = "SendButton";
            this.SendButton.Size = new System.Drawing.Size(75, 23);
            this.SendButton.TabIndex = 3;
            this.SendButton.Text = "Send";
            this.SendButton.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.SendButton.UseVisualStyleBackColor = true;
            this.SendButton.Click += new System.EventHandler(this.SendButton_Click);
            // 
            // ClearButton
            // 
            this.ClearButton.Location = new System.Drawing.Point(301, 70);
            this.ClearButton.Name = "ClearButton";
            this.ClearButton.Size = new System.Drawing.Size(75, 23);
            this.ClearButton.TabIndex = 4;
            this.ClearButton.Text = "Clear";
            this.ClearButton.UseVisualStyleBackColor = true;
            this.ClearButton.Click += new System.EventHandler(this.ClearButton_Click);
            // 
            // TxTextBox
            // 
            this.TxTextBox.Location = new System.Drawing.Point(21, 12);
            this.TxTextBox.Name = "TxTextBox";
            this.TxTextBox.Size = new System.Drawing.Size(260, 20);
            this.TxTextBox.TabIndex = 5;
            this.TxTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TxTextBox_KeyDown_1);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(21, 135);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(355, 23);
            this.progressBar1.TabIndex = 6;
            // 
            // PreviousButton
            // 
            this.PreviousButton.Location = new System.Drawing.Point(82, 164);
            this.PreviousButton.Name = "PreviousButton";
            this.PreviousButton.Size = new System.Drawing.Size(75, 23);
            this.PreviousButton.TabIndex = 7;
            this.PreviousButton.Text = "<<";
            this.PreviousButton.UseVisualStyleBackColor = true;
            this.PreviousButton.Click += new System.EventHandler(this.PreviousButton_Click);
            // 
            // NextButton
            // 
            this.NextButton.Location = new System.Drawing.Point(244, 164);
            this.NextButton.Name = "NextButton";
            this.NextButton.Size = new System.Drawing.Size(75, 23);
            this.NextButton.TabIndex = 8;
            this.NextButton.Text = ">>";
            this.NextButton.UseVisualStyleBackColor = true;
            this.NextButton.Click += new System.EventHandler(this.NextButton_Click);
            // 
            // VolDownButton
            // 
            this.VolDownButton.Location = new System.Drawing.Point(82, 193);
            this.VolDownButton.Name = "VolDownButton";
            this.VolDownButton.Size = new System.Drawing.Size(75, 23);
            this.VolDownButton.TabIndex = 9;
            this.VolDownButton.Text = "Vol -";
            this.VolDownButton.UseVisualStyleBackColor = true;
            this.VolDownButton.Click += new System.EventHandler(this.VolDownButton_Click);
            // 
            // VolUpButton
            // 
            this.VolUpButton.Location = new System.Drawing.Point(244, 193);
            this.VolUpButton.Name = "VolUpButton";
            this.VolUpButton.Size = new System.Drawing.Size(75, 23);
            this.VolUpButton.TabIndex = 10;
            this.VolUpButton.Text = "Vol +";
            this.VolUpButton.UseVisualStyleBackColor = true;
            this.VolUpButton.Click += new System.EventHandler(this.VolUpButton_Click);
            // 
            // PlayPauseButton
            // 
            this.PlayPauseButton.Location = new System.Drawing.Point(163, 164);
            this.PlayPauseButton.Name = "PlayPauseButton";
            this.PlayPauseButton.Size = new System.Drawing.Size(75, 23);
            this.PlayPauseButton.TabIndex = 11;
            this.PlayPauseButton.Text = "Play/Pause";
            this.PlayPauseButton.UseVisualStyleBackColor = true;
            this.PlayPauseButton.Click += new System.EventHandler(this.PlayPauseButton_Click);
            // 
            // MuteButton
            // 
            this.MuteButton.Location = new System.Drawing.Point(163, 193);
            this.MuteButton.Name = "MuteButton";
            this.MuteButton.Size = new System.Drawing.Size(75, 23);
            this.MuteButton.TabIndex = 12;
            this.MuteButton.Text = "Mute/Unmute";
            this.MuteButton.UseVisualStyleBackColor = true;
            this.MuteButton.Click += new System.EventHandler(this.MuteButton_Click);
            // 
            // ReadButton
            // 
            this.ReadButton.Location = new System.Drawing.Point(301, 99);
            this.ReadButton.Name = "ReadButton";
            this.ReadButton.Size = new System.Drawing.Size(75, 23);
            this.ReadButton.TabIndex = 13;
            this.ReadButton.Text = "Read mmf";
            this.ReadButton.UseVisualStyleBackColor = true;
            this.ReadButton.Click += new System.EventHandler(this.ReadButton_Click);
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "AClock";
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseDoubleClick);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 15000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(388, 228);
            this.Controls.Add(this.ReadButton);
            this.Controls.Add(this.MuteButton);
            this.Controls.Add(this.PlayPauseButton);
            this.Controls.Add(this.VolUpButton);
            this.Controls.Add(this.VolDownButton);
            this.Controls.Add(this.NextButton);
            this.Controls.Add(this.PreviousButton);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.TxTextBox);
            this.Controls.Add(this.ClearButton);
            this.Controls.Add(this.SendButton);
            this.Controls.Add(this.InitButton);
            this.Controls.Add(this.RxTextBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "AClock";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox RxTextBox;
        private System.Windows.Forms.Button InitButton;
        private System.Windows.Forms.Button SendButton;
        private System.Windows.Forms.Button ClearButton;
        private System.Windows.Forms.TextBox TxTextBox;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button PreviousButton;
        private System.Windows.Forms.Button NextButton;
        private System.Windows.Forms.Button VolDownButton;
        private System.Windows.Forms.Button VolUpButton;
        private System.Windows.Forms.Button PlayPauseButton;
        private System.Windows.Forms.Button MuteButton;
        private System.Windows.Forms.Button ReadButton;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.IO.Ports.SerialPort serialPort1;
        private System.Windows.Forms.Timer timer1;
    }
}

