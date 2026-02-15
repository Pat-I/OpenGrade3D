using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;


namespace OpenGrade
{
    public partial class FormGPS
    {
        // Send and Recv socket
        private Socket loopBackSocket;
        public static UdpClient udpServer;
        private static int port = 15555; // Port to listen on

        // UDP Socket from AgIO v6.3.3
        private EndPoint endPointLoopBack = new IPEndPoint(IPAddress.Loopback, 0);

        public bool isUDPNetworkConnected;

        private EndPoint epAgIO = new IPEndPoint(IPAddress.Parse("127.255.255.255"), 17777);
        //private IPEndPoint epNtrip = new IPEndPoint(IPAddress.Parse(Properties.Settings.Default.setIP_autoSteerIP), 2233);
        //end from AgIO v6.3.3

        private bool isSendConnected = true;

        // Data stream
        private byte[] buffer = new byte[1024];

        // Status delegate
        private delegate void UpdateRecvMessageDelegate(string recvMessage);
        private UpdateRecvMessageDelegate updateRecvMessageDelegate = null;

        public void SendPgnToLoop(byte[] byteData)
        {
            if (loopBackSocket != null && byteData.Length > 2)
            {
                try
                {
                    int crc = 0;
                    for (int i = 2; i + 1 < byteData.Length; i++)
                    {
                        crc += byteData[i];
                    }
                    byteData[byteData.Length - 1] = (byte)crc;

                    loopBackSocket.BeginSendTo(byteData, 0, byteData.Length, SocketFlags.None,
                        epAgIO, new AsyncCallback(SendAsyncLoopData), null);
                }
                catch (Exception)
                {
                    //Log.EventWriter("Sending UDP Message" + e.ToString());
                    //MessageBox.Show("Send Error: " + e.Message, "UDP Client", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        public void SendAsyncLoopData(IAsyncResult asyncResult)
        {
            try
            {
                loopBackSocket.EndSend(asyncResult);
            }
            catch (Exception)
            {
                //MessageBox.Show("SendData Error: " + ex.Message, "UDP Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //initialize loopback and udp network
        public void LoadUDPNetwork()
        {
            new Thread(() =>
            {
                using (udpServer = new UdpClient(port))
                {
                    while (true)
                    {
                        ListenForMessages();
                    }
                }
            })
            { IsBackground = true }.Start();

            //        udpServer.Close(); // where would I put this?

        }
        #region Receive UDP 
        //from AgIO v6.3.3

        private void ListenForMessages()
        {
            try
            {
                // Listen for UDP packets on the given port
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Loopback, 0);
                byte[] data = udpServer.Receive(ref endPoint);
                string receivedData = Encoding.UTF8.GetString(data);

                if (data[0] == 0x80 && data[1] == 0x81)
                {
                    //module return via udp sent to AOG
                    //SendToLoopBackMessageAOG(data);

                    //check for Scan and Hello
                    if (data[3] == 126 && data.Length == 11)
                    {
                        /*
                        traffic.helloFromAutoSteer = 0;
                        if (isViewAdvanced)
                        {
                            lblPing.Text = (((DateTime.Now - Process.GetCurrentProcess().StartTime).TotalSeconds - pingSecondsStart) * 1000).ToString("N0");
                            double actualSteerAngle = (Int16)((data[6] << 8) + data[5]);
                            lblSteerAngle.Text = (actualSteerAngle * 0.01).ToString("N1");
                            lblWASCounts.Text = ((Int16)((data[8] << 8) + data[7])).ToString();

                            lblSwitchStatus.Text = ((data[9] & 2) == 2).ToString();
                            lblWorkSwitchStatus.Text = ((data[9] & 1) == 1).ToString();
                        }
                        */
                    }

                    else if (data[3] == 123 && data.Length == 11)
                    {
                        /*
                        traffic.helloFromMachine = 0;

                        if (isViewAdvanced)
                        {
                            lblPingMachine.Text = (((DateTime.Now - Process.GetCurrentProcess().StartTime).TotalSeconds - pingSecondsStart) * 1000).ToString("N0");
                            lbl1To8.Text = Convert.ToString(data[5], 2).PadLeft(8, '0');
                            lbl9To16.Text = Convert.ToString(data[6], 2).PadLeft(8, '0');
                        }
                        */
                    }

                    else if (data[3] == 121 && data.Length == 11)
                    {
                        //traffic.helloFromIMU = 0;
                    }


                    //scan Reply
                    else if (data[3] == 203 && data.Length == 13) //
                    {
                        /*
                        if (data[2] == 126)  //steer module
                        {
                            scanReply.steerIP = data[5].ToString() + "." + data[6].ToString() + "." + data[7].ToString() + "." + data[8].ToString();

                            scanReply.subnet[0] = data[09];
                            scanReply.subnet[1] = data[10];
                            scanReply.subnet[2] = data[11];

                            scanReply.subnetStr = data[9].ToString() + "." + data[10].ToString() + "." + data[11].ToString();

                            scanReply.isNewData = true;
                            scanReply.isNewSteer = true;
                        }
                        //
                        else if (data[2] == 123)   //machine module
                        {
                            scanReply.machineIP = data[5].ToString() + "." + data[6].ToString() + "." + data[7].ToString() + "." + data[8].ToString();

                            scanReply.subnet[0] = data[09];
                            scanReply.subnet[1] = data[10];
                            scanReply.subnet[2] = data[11];

                            scanReply.subnetStr = data[9].ToString() + "." + data[10].ToString() + "." + data[11].ToString();

                            scanReply.isNewData = true;
                            scanReply.isNewMachine = true;

                        }
                        else if (data[2] == 121)   //IMU Module
                        {
                            scanReply.IMU_IP = data[5].ToString() + "." + data[6].ToString() + "." + data[7].ToString() + "." + data[8].ToString();

                            scanReply.subnet[0] = data[09];
                            scanReply.subnet[1] = data[10];
                            scanReply.subnet[2] = data[11];

                            scanReply.subnetStr = data[9].ToString() + "." + data[10].ToString() + "." + data[11].ToString();

                            scanReply.isNewData = true;
                            scanReply.isNewIMU = true;
                        }

                        else if (data[2] == 120)    //GPS module
                        {
                            scanReply.GPS_IP = data[5].ToString() + "." + data[6].ToString() + "." + data[7].ToString() + "." + data[8].ToString();

                            scanReply.subnet[0] = data[09];
                            scanReply.subnet[1] = data[10];
                            scanReply.subnet[2] = data[11];

                            scanReply.subnetStr = data[9].ToString() + "." + data[10].ToString() + "." + data[11].ToString();

                            scanReply.isNewData = true;
                            scanReply.isNewGPS = true;
                        }
                        */
                    }
                    /*
                    if (isUDPMonitorOn)
                    {
                        logUDPSentence.Append(DateTime.Now.ToString("ss.fff\t") + endPointUDP.ToString() + "\t" + " < " + data[3].ToString() + "\r\n");
                    }
                    */

                } // end of pgns

                else if (data[0] == 36 && (data[1] == 71 || data[1] == 80 || data[1] == 75))
                {
                    //traffic.cntrGPSOut += data.Length;
                    pn.rawBuffer += Encoding.ASCII.GetString(data);
                    //ParseNMEA(ref rawBuffer);
                    /*
                    if (isUDPMonitorOn && isGPSLogOn)
                    {
                        logUDPSentence.Append(DateTime.Now.ToString("ss.fff\t") + System.Text.Encoding.ASCII.GetString(data));
                    }
                    */
                }
            }
            catch
            {

            }
        }

        #endregion
    
}
}
