using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace TCPSocketServer
{

    public class SynchronousSocketListener
    {
        // Incoming data from the client.
        public static string data = null;
        public static bool RunUDPServer = true;
        public static bool TCPStarted = true;
        public static string LogPath = @"C:\Logs\";
        public static string ServerLog = @"ServerLog.txt";
        public static string ServerLogPath = LogPath + ServerLog;
        public static string Msg;
        public static bool WriteToLogFile = false;


        public static void SetUpLogFile()
        {
            System.IO.Directory.CreateDirectory(LogPath);
            if (!File.Exists(ServerLogPath))
            {
                // Create a file to write to. 
                using (StreamWriter sw = File.CreateText(ServerLogPath))
                {
                    sw.Close();
                }
            }
        }



        public static void WriteToFile(string TextToWrite)
        {
            using (StreamWriter configwriter = File.AppendText(ServerLogPath))
            {
                configwriter.WriteLine(TextToWrite);
                configwriter.Close();
            }
        }

        public static void LogWeb(string MessageToLog)
        {
            string Msg = DateTime.Now.ToString("HH:mm:ss") + " Web Automation: " + MessageToLog;
            Console.WriteLine(Msg);
            if (MessageToLog.ToUpper() != "<LOGON>")
            {
                if (WriteToLogFile)
                {
                    WriteToFile(Msg);
                }
            }

        }

        public static void LogUDP(string MessageToLog)
        {
            Msg = DateTime.Now.ToString("HH:mm:ss") + " UDP Server: " + MessageToLog;
            Console.WriteLine(Msg);
            if (MessageToLog.ToUpper() != "<LOGON>")
            {
                if (WriteToLogFile)
                {
                    WriteToFile(Msg);
                }
            }
           
        }

        public static void LogTCP(string MessageToLog)
        {
            Msg = DateTime.Now.ToString("HH:mm:ss") + " TCP Server: " + MessageToLog;
            Console.WriteLine(Msg);
            if (MessageToLog.ToUpper() != "<LOGON>")
            {
                if (WriteToLogFile)
                {
                    WriteToFile(Msg);
                }
            }
        }

        public static void StartListeningUDP()
        {
            try
            {


                LogUDP("Attempting to start UDP server on port 3334");
                byte[] data = new byte[1024];
                IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 3334);
                UdpClient newsock = new UdpClient(ipep);
                LogUDP("Waiting for a client...");
                IPEndPoint sender = new IPEndPoint(IPAddress.Any,0);
                data = newsock.Receive(ref sender);
                LogUDP("Message received from: " + sender.ToString());
                LogUDP(Encoding.ASCII.GetString(data, 0, data.Length));
                string welcome = "Welcome to my test server";
                data = Encoding.ASCII.GetBytes(welcome);
                newsock.Send(data, data.Length, sender);

                while (RunUDPServer)
                {
                    data = newsock.Receive(ref sender);
                    LogUDP(Encoding.ASCII.GetString(data, 0, data.Length));
                    newsock.Send(data, data.Length, sender);
                }

                ipep = null;
                newsock.Close();
                newsock= null;
                sender = null;
                data = null;


            }
            catch (Exception)
            {

                LogUDP("UDP server stopped!");
            }
        }

        public static void StartListeningTCP()
        {
            // Data buffer for incoming data.
            byte[] bytes = new Byte[1024];

            // Establish the local endpoint for the socket.
            // Dns.GetHostName returns the name of the 
            // host running the application.
            IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 3333);

            if (TCPStarted)
            {
                LogTCP("TCPServer v1.1 Listening on IP: " + ipAddress.ToString() + " Port: 3333"); 
            }
            

            TCPStarted = false;

            // Create a TCP/IP socket.
            Socket listener = new Socket(AddressFamily.InterNetwork,
            SocketType.Stream, ProtocolType.Tcp);
           

            // Bind the socket to the local endpoint and 
            // listen for incoming connections.
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(10);

                

                // Start listening for connections.
                while (true)
                {
                    LogTCP("Waiting for a connection...");
                    // Program is suspended while waiting for an incoming connection.
                    Socket handler = listener.Accept();
                    LogTCP("Client found!");
                    TCPStarted = true;
                    data = null;

                    // An incoming connection needs to be processed.
                    while (true)
                    {
                        data = null;
                        bytes = new byte[1024];
                        int bytesRec = handler.Receive(bytes);
                        data = Encoding.UTF8.GetString(bytes, 0, bytesRec); 


                        if(data.ToUpper().IndexOf("<LOGON>") > -1)
                        {
                            byte[] message = Encoding.UTF8.GetBytes("Server logging has been turned on...");
                            handler.Send(message);
                            WriteToLogFile = true;
                        }



                        if(data.ToUpper().IndexOf("<LOGOFF>") > -1)
                        {
                            byte[] message = Encoding.UTF8.GetBytes("Server logging has been turned off...");
                            handler.Send(message);
                            WriteToLogFile = false;
                        }


                        if (data.ToUpper().IndexOf("<SERVERKILLCONNECTION>") > -1)
                        {
                            LogTCP("Client has disconnected.");

                            // Echo the data back to the client.
                            byte[] msg = Encoding.UTF8.GetBytes("Goodbye!");

                            handler.Send(msg);
                            handler.Shutdown(SocketShutdown.Both);
                            handler.Close();
                            break;
                        }
                        else if(data.ToUpper().IndexOf("GET") > -1)
                        {
                            byte[] message = Encoding.UTF8.GetBytes(@"HTTP/1.1 200 OK\r\nContent-Type: text/html");
                            handler.Send(message);
                            WriteToLogFile = true;
                        }
                        else if (data == "<GAMEPADINFO>")
                        {
                            RunUDPServer = true;
                            Thread UDPServerThread = new Thread(StartListeningUDP);
                            UDPServerThread.Start();
                            LogTCP(data.ToString());
                            LogTCP("Sending client GP server info...");
                            byte[] message = Encoding.UTF8.GetBytes("192.168.1.140:8888");
                            handler.Send(message);
                        }
                        else if (data == "<VIDEOINFO>")
                        {
                            byte[] message = Encoding.UTF8.GetBytes("www.thomasworkshop.com:88");
                            handler.Send(message);
                        }
                        else if (data == "<GAMEPADKILL>")
                        {
                            RunUDPServer = false;
                            LogTCP(data.ToString());
                            LogTCP("Killing UDP gamepad server thread...");
                            byte[] message = Encoding.UTF8.GetBytes("<GAMEPADKILL-OK>");
                            handler.Send(message);
                        }
                        else if (data == "<KILLUDPSERVER>")
                        {
                            RunUDPServer = false;
                        }
                        else
                        {
                            LogTCP(data.ToString());
                            byte[] message = Encoding.UTF8.GetBytes("...");
                            handler.Send(message);
                        }
                    }

                    // Show the data on the console.
                  
                }

            }
            catch (Exception e)
            {
                string ex = e.ToString();
                //Console.WriteLine(e.ToString());
            }

           // Console.WriteLine("\nPress ENTER to continue...");
           // Console.Read();

        }

        public static int Main(String[] args)
        {
            SetUpLogFile();
            while (true)
            {
               StartListeningTCP();
            }
        }
    }
}
