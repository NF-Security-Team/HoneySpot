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
        public static String sniff_path = @".\Logger\sniff";
        public static String logger_folder = @".\Logger";
        List<string> ReceivedTraffic = new List<string>();
        string current_dateTime = "";
        string src_ip_addr = "";

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


        private void DoWork()
        {

            // Creating working dir (Logger)
            if (!Directory.Exists(logger_folder))
            {
                Directory.CreateDirectory(logger_folder);
            }
            // Creating sniff file
            if (!File.Exists(sniff_path))
            {
                File.AppendAllText(sniff_path,"");
            }
            // Creating WhiteList file
            if (!File.Exists(sniff_path))
            {
                File.AppendAllText(WL_path, "");
            }

            TcpListener server = null;
            try
            {
                // Set the TcpListener on port 6859.
                Int32 port = 6859;
                CheckState_path = CheckState_path + "HoneySpotter_" + port.ToString() + ".CurrState";

                IPAddress addr = IPAddress.Parse("0.0.0.0");

                // TcpListener server = new TcpListener(port);
                server = new TcpListener(addr, port);

                // Start listening for client requests.
                server.Start();

                // Buffer for reading data
                Byte[] bytes = new Byte[256];

                // Enter the listening loop.
                while (true)
                {
                    //Console.Write("Waiting for a connection... ");
                    Logger.Info("Starting listener on port " + port.ToString() + "..." + Environment.NewLine);

                    // Perform a blocking call to accept requests.
                    // You could also use server.AcceptSocket() here.
                    TcpClient client = server.AcceptTcpClient();
                    Logger.Info("Connection received. Server connected." + Environment.NewLine);
                    src_ip_addr = client.Client.RemoteEndPoint.ToString();

                    //data = null;

                    // Get a stream object for reading and writing
                    NetworkStream stream = client.GetStream();

                    // Read all lines received from connected client
                    StreamReader streamRd = new StreamReader(stream);
                    
                    while(true)
                    {
                        try
                        {
                            string streamLine = streamRd.ReadLine();
                            Logger.Info("Received line. Writing to sniff file..." + Environment.NewLine);
                            Logger.Debug(streamLine + Environment.NewLine);

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

                                /// Set CurrState file check output for CheckMK plugin in - CRITICAL STATE                            
                                File.WriteAllText(CheckState_path, "CRITICAL - CRIT STATE -" + Content);
                                //wait 30 minutes
                                Thread.Sleep(1000 * 60 * 30);
                                /// ReSet CurrState file check output for CheckMK plugin in - OK STATE                            
                                File.WriteAllText(CheckState_path, "OK - OK STATE");

                            }
                            /*
                            //Only needed for testing purposes...
                            File.AppendAllText(sniff_path, ReceivedTraffic[(ReceivedTraffic.Count-1)] + Environment.NewLine);
                            */

                            client.Close();
                            break;
                        }
                        catch (Exception ex)
                        {
                            Logger.Debug("[DoWork -- TCP Listener -- ERROR --> ] " + ex.Message);
                        }
                    }
                }
            }
            catch (SocketException e)
            {
                //Console.WriteLine("SocketException: {0}", e);
                Logger.Error("Socket Exception: {0}", e);
                err_count++;
            }
            finally
            {
                // Stop listening for new clients.
                server.Stop();
                err_count++;
            }
        }
    }
}
