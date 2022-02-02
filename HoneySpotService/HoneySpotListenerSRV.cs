using System;
using System.Threading;
using NLog;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;

namespace HoneySpotService
{
    /// <summary>
    /// This is the HoneySpotService. It's main purpose is to listen through a TCP socket and collect every line of commands he receives for further uses.
    /// </summary>
    public class HoneySpotListenerSRV
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public static String CheckState_path;
        public static String WL_path = @".\Logger\WhiteList.db";
        public static String sniff_path = @".\Logger\sniff";
        public static String logger_folder = @".\Logger";
        List<string> ReceivedTraffic = new List<string>();
        string current_dateTime = "";
        string src_ip_addr = "";
        public List<string> All_ReceivedTraffic = new List<string>();

        private static int err_count = 0;

        private readonly Thread _thread;
        public HoneySpotListenerSRV()
        {
            _thread = new Thread(DoWork);
        }

        public void Start()
        {
            _thread.Start();
        }
        public void Stop()
        {
            _thread.Abort();
        }

        public void OnNewClient(TcpListener tcp_server, List<string> All_ReceivedTraffic, string CheckState_path, TcpClient client)
        {
            try
            {
                src_ip_addr = client.Client.RemoteEndPoint.ToString();
                // We must remove port from src_ip_addr for later uses 
                int index = src_ip_addr.IndexOf(":");
                if (index >= 0)
                    src_ip_addr = src_ip_addr.Substring(0, index);
                Logger.Info("Connection from " + src_ip_addr + " received.");

                // Read data received from client through a StreamReader in order to take only a single line (this could easily be our attacker payload, or part of it)
                // Get a stream object for reading and writing
                NetworkStream stream = client.GetStream();
                // Set a 10 millisecond timeout for reading.
                stream.ReadTimeout = 10000;

                // Read all lines received from connected client
                StreamReader streamRd = new StreamReader(stream);

                while (true)
                {
                    try
                    {
                        string streamLine = streamRd.ReadLine();
                        Logger.Info("Received line. Writing to sniff file...");
                        Logger.Debug(streamLine);

                        current_dateTime = DateTime.Now.ToString();

                        ///IP WHITELIST SECTION                        
                        //init Array Whitelist
                        string[] logFile = File.ReadAllLines(WL_path);
                        //
                        List<string> logList = new List<string>(logFile);

                        if (!logList.Contains(src_ip_addr))
                        {
                            ReceivedTraffic.Add(current_dateTime + "," + src_ip_addr + "," + streamLine);
                            string Content = "Sniff Path: " + sniff_path + " -- Received Traffic: " + ReceivedTraffic[(ReceivedTraffic.Count - 1)] + Environment.NewLine;
#if DEBUG
                            Console.WriteLine(Content);
#endif
                            // Write Received Traffic Line to sniff_path
                            int i = 0;
                            // Trying 5 times in case of failure due to concurrent threads handling
                            while (i < 4)
                            {
                                try
                                {
                                    File.AppendAllText(sniff_path, ReceivedTraffic[(ReceivedTraffic.Count - 1)] + Environment.NewLine);
                                    break;
                                }
                                catch
                                {
                                    i++;
                                    Thread.Sleep(500);
                                    Logger.Info("Unable to write to sniff file " + sniff_path);
                                    continue;
                                }
                            }


                            // Set CurrState file check output for CheckMK plugin in - CRITICAL STATE           
                            i = 0;
                            // Trying 5 times in case of failure due to concurrent threads handling
                            while (i < 4)
                            {
                                try
                                {
                                    if(!File.ReadAllText(CheckState_path).Contains("CRITICAL"))
                                    {
                                        File.WriteAllText(CheckState_path, "CRITICAL - CRIT STATE -" + Content);
                                    }
                                    break;
                                }
                                catch
                                {
                                    i++;
                                    Thread.Sleep(500);
                                    Logger.Info("Unable to write to CheckState file " + CheckState_path);
                                    continue;
                                }
                            }
                        }
                        else
                        {
                            Logger.Info("Source ip " + src_ip_addr + " is present in whitelist.db" 
                                        + "Traffic won't be logged...");
                        }

                        client.Close();
                        break;
                    }
                    catch (Exception ex)
                    {
                        Logger.Debug("[DoWork -- TCP Listener -- ERROR --> ] " + ex.Message);
                        // If any exception occurs try to close the client again
                        try
                        {
                            client.Close();
                            break;
                        }
                        catch
                        {
                            break;
                        }
                    }
                }
            }
            catch
            {
                try
                {
                    client.Close();
                }
                catch
                {
                    // Do Nothing
                }
            }
        }
        private void DoWork()
        {
            while(true)
            {
                // Creating working dir (Logger)
                if (!Directory.Exists(logger_folder))
                {
                    Directory.CreateDirectory(logger_folder);
                }
                // Creating sniff file
                if (!File.Exists(sniff_path))
                {
                    File.AppendAllText(sniff_path, "");
                }
                // Creating WhiteList file
                if (!File.Exists(WL_path))
                {
                    File.AppendAllText(WL_path, "");
                }

                TcpListener server = null;
                try
                {
                    // Set the TcpListener on port 6859.
                    Int32 port = 6859;
                    // Set/reset CheckState_path
                    CheckState_path = "";
                    CheckState_path = CheckState_path + "HoneySpotter_" + port.ToString() + ".CurrState";

                    /// Set/Reset CurrState file check output for CheckMK plugin in - OK STATE                            
                    File.WriteAllText(CheckState_path, "OK - OK STATE");

                    IPAddress addr = IPAddress.Parse("0.0.0.0");

                    // TcpListener server = new TcpListener(port);
                    server = new TcpListener(addr, port);

                    // Start listening for client requests.
                    server.Start();

                    //Console.Write("Waiting for a connection... ");
                    Logger.Info("Starting listener on port " + port.ToString() + "...");

                    // Buffer for reading data
                    Byte[] bytes = new Byte[256];

                    // Let's set a counter and a timeout for our listening sessions
                    int counter = 1;
                    TimeSpan timeout = DateTime.Now.TimeOfDay;
                    timeout += TimeSpan.FromSeconds(60);

                    // Listen for a total of 30 minutes, then restart
                    while (counter <= 30)
                    {
                        // Enter the listening loop.
                        while (DateTime.Now.TimeOfDay < timeout)
                        {
                            /*
                            Let's take care of Multiple Concurrent Clients
                            After having called the blocking state for listening let's spawn a new thread each time a client has connected to avoid the whole program to wait indefinitely
                            */
                            // Perform a blocking call to accept requests.
                            TcpClient client = server.AcceptTcpClient();
                            Thread _OnNewClientThread = new Thread(() => OnNewClient(server, All_ReceivedTraffic, CheckState_path, client));
                            _OnNewClientThread.Start();
                            counter++;
                        }
                    }
                    // Stop listening for clients
                    Logger.Info("Counter value reached. Stopping server...");
                    server.Stop();
                }
                catch (SocketException e)
                {
                    //Console.WriteLine("SocketException: {0}", e);
                    Logger.Error("Socket Exception: {0}", e);
                    err_count++;
                }
                finally
                {
                    // Stop listening for clients
                    server.Stop();
                    err_count++;
                }
            }
        }
    }
}
