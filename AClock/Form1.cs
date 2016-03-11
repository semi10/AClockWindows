using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.MemoryMappedFiles;
using System.IO.Ports;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;



namespace AClock
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll")]
        public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, uint dwExtraInfo);

        public Form1()
        {
            InitializeComponent();
            serialPort1.PortName = "COM3";
            serialPort1.BaudRate = 19200;
        }

        private static string RxString = "";

        private void InitButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (!serialPort1.IsOpen)
                {
                    serialPort1.Open();
                    InitButton.Text = "Close Port";
                    SendButton.Enabled = true;
                }
                else
                {
                    serialPort1.Close();
                    InitButton.Text = "Open Port";
                    SendButton.Enabled = false;
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            serialPort1.Close();
        }

        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try {
                RxString = serialPort1.ReadExisting();
                if (RxString.Equals("Q")) keybd_event((byte)Constants.VK_VOLUME_DOWN, 0, Constants.KEYEVENTF_EXTENDEDKEY | 0, 0);
                else if (RxString.Equals("L")) keybd_event((byte)Constants.VK_VOLUME_UP, 0, Constants.KEYEVENTF_EXTENDEDKEY | 0, 0);
                else if (RxString.Equals("M")) keybd_event((byte)Constants.VK_VOLUME_MUTE, 0, Constants.KEYEVENTF_EXTENDEDKEY | 0, 0);
                else if (RxString.Equals("+")) keybd_event((byte)Constants.VK_MEDIA_NEXT_TRACK, 0, Constants.KEYEVENTF_EXTENDEDKEY | 0, 0);
                else if (RxString.Equals("-")) keybd_event((byte)Constants.VK_MEDIA_PREV_TRACK, 0, Constants.KEYEVENTF_EXTENDEDKEY | 0, 0);
                else if (RxString.Equals("P")) keybd_event((byte)Constants.VK_MEDIA_PLAY_PAUSE, 0, Constants.KEYEVENTF_EXTENDEDKEY | 0, 0);
                else this.Invoke(new EventHandler(DisplayText));
            }
            catch (System.TimeoutException){
            }
        }

        private void DisplayText(object sender, EventArgs e)
        {
            RxTextBox.AppendText(RxString);
            RxTextBox.ScrollToCaret();
        }

        private void SendButton_Click(object sender, EventArgs e)
        {
            serialPort1.Write(TxTextBox.Text);
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            RxTextBox.Clear();
            TxTextBox.Clear();
        }

        private void TxTextBox_KeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && serialPort1.IsOpen) serialPort1.Write(TxTextBox.Text);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                if (!serialPort1.IsOpen)
                {
                    serialPort1.Open();
                    InitButton.Text = "Close Port";
                    SendButton.Enabled = true;
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void NextButton_Click(object sender, EventArgs e)
        {
            keybd_event((byte)Constants.VK_MEDIA_NEXT_TRACK, 0, Constants.KEYEVENTF_EXTENDEDKEY | 0, 0);
        }

        private void PreviousButton_Click(object sender, EventArgs e)
        {
            keybd_event((byte)Constants.VK_MEDIA_PREV_TRACK, 0, Constants.KEYEVENTF_EXTENDEDKEY | 0, 0);
        }

        private void PlayPauseButton_Click(object sender, EventArgs e)
        {
            keybd_event((byte)Constants.VK_MEDIA_PLAY_PAUSE, 0, Constants.KEYEVENTF_EXTENDEDKEY | 0, 0);
        }

        private void MuteButton_Click(object sender, EventArgs e)
        {
            keybd_event((byte)Constants.VK_VOLUME_MUTE, 0, Constants.KEYEVENTF_EXTENDEDKEY | 0, 0);
        }

        private void VolDownButton_Click(object sender, EventArgs e)
        {
            keybd_event((byte)Constants.VK_VOLUME_DOWN, 0, Constants.KEYEVENTF_EXTENDEDKEY | 0, 0);
        }

        private void VolUpButton_Click(object sender, EventArgs e)
        {
            keybd_event((byte)Constants.VK_VOLUME_UP, 0, Constants.KEYEVENTF_EXTENDEDKEY | 0, 0);
        }

        private void ReadButton_Click(object sender, EventArgs e)
        {
            using (MemoryMappedFile mmf = MemoryMappedFile.OpenExisting("AIMP2_RemoteInfo"))
            using (MemoryMappedViewAccessor accessor = mmf.CreateViewAccessor())
                for(int i = 0; i < 2505; i++)
                {
                    RxTextBox.Text += (accessor.ReadChar(i).ToString());
                }
                
        }
    }

    static class Constants
    {
        public const uint KEYEVENTF_KEYUP = 0x0002;
        public const uint KEYEVENTF_EXTENDEDKEY = 0x0001;
        public const int VK_MEDIA_PLAY_PAUSE = 0xB3;
        public const int VK_MEDIA_NEXT_TRACK = 0xB0;
        public const int VK_MEDIA_PREV_TRACK = 0xB1;
        public const int VK_VOLUME_MUTE = 0xAD;
        public const int VK_VOLUME_UP = 0xAF;
        public const int VK_VOLUME_DOWN = 0xAE;
    }
}
