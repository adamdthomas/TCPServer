using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using System.Runtime.InteropServices;


namespace TCPSocketServer
{
    class WebAutomation
    {

        public static void FoscamLogin(string Address, string UserName, string Password)
        {
            SynchronousSocketListener.LogWeb("Attempting to log into Foscam viewer...");
            
            WebAutomationToolkit.Web.WebDriver = new InternetExplorerDriver();
WebAutomationToolkit.Web.NavigateToURL(Address);
      
     // FFAllowPlugin();
WebAutomationToolkit.Utilities.Wait(5);
            
            WebAutomationToolkit.Web.Sync.SyncByID("username", 10);
            //WebAutomationToolkit.Web.Edit.SetTextByID("username", UserName);
            WebAutomationToolkit.Web.Edit.SetTextByCSSPath("#passwd", Password);
            WebAutomationToolkit.Web.Button.ClickByCSSPath("#login_ok");
            WebAutomationToolkit.Web.Sync.SyncByCSSPath("#LiveMenu", 10);
            WebAutomationToolkit.Web.Button.ClickByCSSPath("#LiveMenu");
            SynchronousSocketListener.LogWeb("Webcam viewer has been launched successfully...");

        }

            [DllImport("USER32.DLL", CharSet = CharSet.Unicode)]
            public static extern IntPtr FindWindow(string lpClassName,
            string lpWindowName);

            // Activate an application window.
            [DllImport("USER32.DLL")]
            public static extern bool SetForegroundWindow(IntPtr hWnd);

            // Send a series of key presses to the Calculator application. 
            private static void FFAllowPlugin()
            {
                // Get a handle to the Calculator application. The window class 
                // and window name were obtained using the Spy++ tool.
                IntPtr calculatorHandle = FindWindow("MozillaWindowClass","IPCam Client - Mozilla Firefox");

                // Verify that Calculator is a running process. 
                if (calculatorHandle == IntPtr.Zero)
                {
                    MessageBox.Show("Calculator is not running.");
                    return;
                }

                // Make Calculator the foreground application and send it  
                // a set of calculations.
                SetForegroundWindow(calculatorHandle);
                SendKeys.SendWait("{TAB}");
                SendKeys.SendWait("{TAB}");
                SendKeys.SendWait("{TAB}");
                SendKeys.SendWait("{TAB}");

                SendKeys.SendWait(" ");
                SendKeys.SendWait("{ENTER}");

                WebAutomationToolkit.Utilities.Wait(5);


                    }


    }
}
