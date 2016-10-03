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
using System.Management;
using System.Text.RegularExpressions;
using Microsoft.Win32;
using System.Diagnostics;

//mmf Struct 
public struct TAIMPRemoteFileInfo
{
    public uint Deprecated1;
    public bool Active;
    public uint BitRate;
    public uint Channels;
    public uint Duration;
    public uint FileSize;
    public ulong FileMark;
    public uint SampleRate;
    public uint TrackNumber;
    public uint AlbumLength;
    public uint ArtistLength;
    public uint DateLength;
    public uint FileNameLength;
    public uint GenreLength;
    public uint TitleLength;
    public uint Deprecated2;
    public uint Deprecated3;
    public uint Deprecated4;
    public uint Deprecated5;
    public uint Deprecated6;
    public uint Deprecated7;
}


namespace AClock
{
    
    public partial class Form1 : Form
    {
        private static string RxString = "";
        private Icon ico;
        bool AIMPconnected = false; //AIMP window found
        //For Aimp mapped memory file
        byte[] Buffer = new byte[1100];
        int arduinoProgressBarVal = 0;
        TAIMPRemoteFileInfo[] mmfInfo = new TAIMPRemoteFileInfo[1];

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string className, string windowTitle);

        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        private static extern IntPtr FindWindow(string lp1, string lp2);

        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam);

        [DllImport("user32.dll")]
        public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, uint dwExtraInfo);


//-----------------------------------------------------------------------------------------------
//                                    General
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            this.Hide();
        }
        public Form1()
        {
            InitializeComponent();
            serialPort1.BaudRate = 19200;
            this.Hide();
            progressBar1.Value = Constants.PROGRESS_BAR_MAX;

        }
        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
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
        private void Form1_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (serialPort1.IsOpen) serialPort1.Close();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            ico = notifyIcon1.Icon;
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!serialPort1.IsOpen) SearchForACLock();
            if (!AIMPconnected) findAIMP();
        }


//-----------------------------------------------------------------------------------------------
//                                    Control Buttons
        private void InitButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (!serialPort1.IsOpen) SearchForACLock();
                else serialPort1.Close();
                CheckConnection();
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (System.IO.IOException ex)
            {
                MessageBox.Show("Can't find ACLock. Check Bluetooth connection");
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
            IntPtr hwnd = IntPtr.Zero;
            IntPtr hwndChild = IntPtr.Zero;

            //Get a handle for the Calculator Application main window
            hwnd = FindWindow(null, "AIMP2_RemoteInfo");
            if (hwnd == IntPtr.Zero)
            {
                if (MessageBox.Show("Couldn't find the AIMP2" + " application. Do you want to start it?", "TestWinAPI", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    System.Diagnostics.Process.Start("AIMP");
                }
            }
            else
            {
                AIMPconnected = true;
                SendMessage(hwnd, Constants.WM_AIMP_COMMAND, (IntPtr)Constants.AIMP_RA_CMD_REGISTER_NOTIFY, this.Handle);
                sendTrackInfo();
            }
        }


//-----------------------------------------------------------------------------------------------
//                                    Data From AIMP
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        protected override void WndProc(ref Message m)
        {  
            // Listen for operating system messages.
            switch (m.Msg)
            {
                case Constants.WM_SYSCOMMAND:   //Minimize to system tray
                    if (m.WParam.ToInt32() == Constants.SC_MINIMIZE){
                        this.Hide();
                        return;
                    }
                    break;
                case Constants.WM_AIMP_NOTIFY:  //On AIMP track start
                    switch ((int)m.WParam)
                    {
                        case Constants.AIMP_RA_NOTIFY_TRACK_START:   
                            sendProgressInfo();
                            sendTrackInfo();
                            break;
                        case Constants.AIMP_RA_NOTIFY_PROPERTY: //On AIMP property change

                            switch ((int)m.LParam)
                            {
                                case Constants.AIMP_RA_PROPERTY_PLAYER_POSITION:
                                    sendProgressInfo();
                                    break;
                                case Constants.AIMP_RA_PROPERTY_PLAYER_STATE:
                                    sendStateInfo();
                                    break;
                            }
                            break;     
                    }
                    break;
            }
            base.WndProc(ref m);
        }
        private void findAIMP()
        {
            IntPtr hwnd = IntPtr.Zero;

            //Get a handle for the AIMP Application main window
            hwnd = FindWindow(null, "AIMP2_RemoteInfo");
            if (hwnd == IntPtr.Zero)
            {

            }
            else
            {
                AIMPconnected = true;
                SendMessage(hwnd, Constants.WM_AIMP_COMMAND, (IntPtr)Constants.AIMP_RA_CMD_REGISTER_NOTIFY, this.Handle);
                sendTrackInfo();
            }
        }
        private void sendTrackInfo()
        {
            MemoryMappedFile mmf = MemoryMappedFile.OpenExisting("AIMP2_RemoteInfo");
            MemoryMappedViewAccessor accessor = mmf.CreateViewAccessor();

            mmfInfo[0] = new TAIMPRemoteFileInfo();

            accessor.ReadArray(0, mmfInfo, 0, 1);

            uint mmfIndex = 0;
            mmfIndex += mmfInfo[0].AlbumLength;
            mmfIndex += mmfInfo[0].ArtistLength;
            mmfIndex += mmfInfo[0].DateLength;
            mmfIndex += mmfInfo[0].FileNameLength;
            mmfIndex += mmfInfo[0].GenreLength;

            accessor.ReadArray(88 + mmfIndex * 2, Buffer, 0, 220);
            string Ascii = Encoding.Unicode.GetString(Buffer);
            //RxTextBox.Text = Ascii;
            if (serialPort1.IsOpen) serialPort1.WriteLine("p: " + Ascii);

            IntPtr hwnd = IntPtr.Zero;

            //Get a handle for the AIMP Application main window
            hwnd = FindWindow(null, "AIMP2_RemoteInfo");

            progressBar1.Maximum = SendMessage(hwnd, Constants.WM_AIMP_PROPERTY, (IntPtr)Constants.AIMP_RA_PROPERTY_PLAYER_DURATION, IntPtr.Zero);

        }
        private void sendProgressInfo()
        {
            int currentPlayerPosition;
            IntPtr hwnd = IntPtr.Zero;

            //Get a handle for the AIMP Application main window
            hwnd = FindWindow(null, "AIMP2_RemoteInfo");
            currentPlayerPosition = SendMessage(hwnd, Constants.WM_AIMP_PROPERTY, (IntPtr)Constants.AIMP_RA_PROPERTY_PLAYER_POSITION, IntPtr.Zero);
             
            if (currentPlayerPosition >= 0 && currentPlayerPosition < progressBar1.Maximum)
            {
                progressBar1.Value = currentPlayerPosition;
                if (serialPort1.IsOpen)
                {    
                    int newArduinoProgressBarVal = currentPlayerPosition * 33 / progressBar1.Maximum;
                    RxTextBox.AppendText("Current: " + arduinoProgressBarVal + " New: " + newArduinoProgressBarVal + '\n');
                    if(arduinoProgressBarVal != newArduinoProgressBarVal){
                        RxTextBox.AppendText("Refresh \n");
                        serialPort1.WriteLine("t: " + newArduinoProgressBarVal);
                        arduinoProgressBarVal = newArduinoProgressBarVal;
                    }
                    
                }
            }
        }
        private void sendStateInfo()
        {
            IntPtr hwnd = IntPtr.Zero;

            //Get a handle for the AIMP Application main window
            hwnd = FindWindow(null, "AIMP2_RemoteInfo");

           int playingState = SendMessage(hwnd, Constants.WM_AIMP_PROPERTY, (IntPtr)Constants.AIMP_RA_PROPERTY_PLAYER_STATE, IntPtr.Zero);

           serialPort1.WriteLine("s: " + playingState);

           switch (playingState){
               case 0:  //Stopped
                   AIMPconnected = false;
                   if (progressBar1.Value != 0) progressBar1.Value = 0;
                   break;
               case 1:  //Paused
                   
                   break;
               case 2:  //Playing

                   break;
           }
        }


//-----------------------------------------------------------------------------------------------
//                                     Serial Port

        private static string[] GetBluetoothPort()
        {
            Regex regexPortName = new Regex(@"(COM\d+)");

            List<string> portList = new List<string>();

            ManagementObjectSearcher searchSerial = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity");

            foreach (ManagementObject obj in searchSerial.Get())
            {
                string name = obj["Name"] as string;
                string classGuid = obj["ClassGuid"] as string;
                string deviceID = obj["DeviceID"] as string;

                if (classGuid != null && deviceID != null)
                {
                    if (String.Equals(classGuid, "{4d36e978-e325-11ce-bfc1-08002be10318}", StringComparison.InvariantCulture))
                    {
                        string[] tokens = deviceID.Split('&');

                        if (tokens.Length >= 4)
                        {
                            string[] addressToken = tokens[4].Split('_');
                            string bluetoothAddress = addressToken[0];

                            Match m = regexPortName.Match(name);
                            string comPortNumber = "";
                            if (m.Success)
                            {
                                comPortNumber = m.Groups[1].ToString();
                            }

                            if (Convert.ToUInt64(bluetoothAddress, 16) > 0)
                            {
                                string bluetoothName = GetBluetoothRegistryName(bluetoothAddress);
                                portList.Add(String.Format("{0} {1} {2}", bluetoothName, bluetoothAddress, comPortNumber));
                            }
                        }
                    }
                }
            }

            return portList.ToArray();
        }
        private static string GetBluetoothRegistryName(string address)
        {
            string deviceName = "";

            string registryPath = @"SYSTEM\CurrentControlSet\Services\BTHPORT\Parameters\Devices";
            string devicePath = String.Format(@"{0}\{1}", registryPath, address);

            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(devicePath))
            {
                if (key != null)
                {
                    Object o = key.GetValue("Name");

                    byte[] raw = o as byte[];

                    if (raw != null)
                    {
                        deviceName = Encoding.ASCII.GetString(raw);
                    }
                }
            }

            return deviceName;
        }
        public void SearchForACLock(){
            string[] BluetoothPort = GetBluetoothPort();
            foreach (string port in BluetoothPort)
            {
                string[] info = port.Split();
                if (info[0].StartsWith("AClock")) 
                {
                    serialPort1.PortName = info[2];
                    ConnectToAClock();
                }
            }
        }
        private void ConnectToAClock()
        {
            try
            {
                if (!serialPort1.IsOpen)
                {
                    serialPort1.Open();
                    serialPort1.DataReceived += new SerialDataReceivedEventHandler(serialPort1_DataReceived);
                    CheckConnection();
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (System.IO.IOException ex)
            {
                MessageBox.Show(ex.Message);
            }


        }
        private void CheckConnection(){
            if (serialPort1.IsOpen)
            {
                InitButton.Text = "Close Port";
                SendButton.Enabled = true;
            }
            else
            {
                InitButton.Text = "Open Port";
                SendButton.Enabled = false;
            }
        }
        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                RxString = serialPort1.ReadExisting();
                if (RxString.Equals("Q")) keybd_event((byte)Constants.VK_VOLUME_DOWN, 0, Constants.KEYEVENTF_EXTENDEDKEY | 0, 0);
                else if (RxString.Equals("L")) keybd_event((byte)Constants.VK_VOLUME_UP, 0, Constants.KEYEVENTF_EXTENDEDKEY | 0, 0);
                else if (RxString.Equals("M")) keybd_event((byte)Constants.VK_VOLUME_MUTE, 0, Constants.KEYEVENTF_EXTENDEDKEY | 0, 0);
                else if (RxString.Equals("+")) keybd_event((byte)Constants.VK_MEDIA_NEXT_TRACK, 0, Constants.KEYEVENTF_EXTENDEDKEY | 0, 0);
                else if (RxString.Equals("-")) keybd_event((byte)Constants.VK_MEDIA_PREV_TRACK, 0, Constants.KEYEVENTF_EXTENDEDKEY | 0, 0);
                else if (RxString.Equals("P")) keybd_event((byte)Constants.VK_MEDIA_PLAY_PAUSE, 0, Constants.KEYEVENTF_EXTENDEDKEY | 0, 0);
                else this.Invoke(new EventHandler(DisplayText));
            }
            catch (System.TimeoutException)
            {
            }
        }
    }


     public static class Constants
    {
        //ProgressBar
         public const int PROGRESS_BAR_MAX = 32;
        //Windows
        public const int SC_MINIMIZE = 0xf020;

        //Global HotKeys
        public const uint KEYEVENTF_KEYUP = 0x0002;
        public const uint KEYEVENTF_EXTENDEDKEY = 0x0001;
        public const int VK_MEDIA_PLAY_PAUSE = 0xB3;
        public const int VK_MEDIA_NEXT_TRACK = 0xB0;
        public const int VK_MEDIA_PREV_TRACK = 0xB1;
        public const int VK_VOLUME_MUTE = 0xAD;
        public const int VK_VOLUME_UP = 0xAF;
        public const int VK_VOLUME_DOWN = 0xAE;

        //AIMPRemoteAccessClass

        //GET:  SendMessage(Handle, WM_AIMP_PROPERTY, PropertyID | AIMP_RA_PROPVALUE_GET, 0);
        //SET:  SendMessage(Handle, WM_AIMP_PROPERTY, PropertyID | AIMP_RA_PROPVALUE_SET, NewValue);
        public const int WM_USER = 0x400;
        public const int WM_AIMP_COMMAND = WM_USER + 0x75;
        public const int WM_AIMP_NOTIFY = WM_USER + 0x76;
        public const int WM_AIMP_PROPERTY = WM_USER + 0x77;
        public const int WM_SYSCOMMAND = 0x0112;
        public const int AIMP_RA_PROPVALUE_GET = 0;
        public const int AIMP_RA_PROPVALUE_SET = 1;
        public const int AIMP_RA_CMD_BASE = 10;
        public const int AIMP_RA_CMD_NEXT = AIMP_RA_CMD_BASE + 7;
        public const int AIMP_RA_NOTIFY_BASE = 0;
        public const int AIMP_RA_CMD_REGISTER_NOTIFY = AIMP_RA_CMD_BASE + 1;
        public const int AIMP_RA_CMD_UNREGISTER_NOTIFY = AIMP_RA_CMD_BASE + 2;
        public const int AIMP_RA_NOTIFY_TRACK_INFO = AIMP_RA_NOTIFY_BASE + 1;
        public const int AIMP_RA_NOTIFY_TRACK_START = AIMP_RA_NOTIFY_BASE + 2;
        public const int AIMP_RA_NOTIFY_PROPERTY = AIMP_RA_NOTIFY_BASE + 3;
        public const int AIMP_RA_PROPERTY_PLAYER_POSITION = 0x20;
        public const int AIMP_RA_PROPERTY_PLAYER_DURATION = 0x30;
        public const int AIMP_RA_PROPERTY_PLAYER_STATE = 0x40;
    }
}
